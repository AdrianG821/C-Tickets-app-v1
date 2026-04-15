using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace TicketsProject
{
    /// <summary>
    /// Interaction logic for TicketsDetails.xaml
    /// </summary>
    public partial class TicketsDetails : Page
    {
        public ObservableCollection<Comments> Comments { get; set; }
        public ObservableCollection<Testers> Testers { get; set; }
        public ObservableCollection<Clients> Clients { get; set; }
        public ObservableCollection<Developers> Developers { get; set; }
        public Ticket Ticket { get; set; }
        public List<string> StatusList { get; set; }
        public List<string> PriorityList { get; set; }

        int TicketId = 0;

        public TicketsDetails(string id)
        {
            InitializeComponent();
            Testers = new ObservableCollection<Testers>();
            Developers = new ObservableCollection<Developers>();
            Clients = new ObservableCollection<Clients>();
            Comments = new ObservableCollection<Comments>();


            StatusList = new List<string>
            {
                "NEW",
                "SPECIFICATIONS",
                "DEVELOPMENT",
                "FEEDBACK",
                "CLOSED"
            };

            PriorityList = new List<string>
            {
                "LOW",
                "MEDIUM",
                "HIGH",
                "URGENT"
            };



            bool loaded = true;
            LoadDevelopersFromDb();
            LoadTestersFromDb();
            LoadClientsFromDb();

            if (int.TryParse(id, out TicketId))
            {
                LoadTicketFromDb(TicketId);
                LoadCommentsFromDb(TicketId);
                InsertTicketBtn.Visibility = Visibility.Collapsed;

            }
            else
            {
                loaded = false;
                GridCommentsSection.Visibility = Visibility.Collapsed;
                GridCommentsInput.Visibility = Visibility.Collapsed;
                StatusDropDown.Visibility = Visibility.Hidden;
                UpdateTicketBtn.Visibility = Visibility.Collapsed;
            }

            
            Loaded += TicketsDetails_Loaded;

            this.DataContext = this;
        }


        public void BackButton(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TicketsList());
        }


        public void TicketsDetails_Loaded(object sender, RoutedEventArgs e)
        {
            if(Comments != null && Comments.Count > 0)
            {
                var lastItem = Comments.Last();
                ListBoxComments.ScrollIntoView(lastItem);
            }
        }

        private void RemovePlaceholder(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;

            if (tb.Text == tb.Tag.ToString())
            {
                tb.Text = "";
            }
        }

        private void AddPlaceholder(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;

            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = tb.Tag.ToString();
            }
        }


        public void LoadTicketFromDb(int TicketId)
        {
            try
            {
                using var connection = Database.GetConnection();
                connection.Open();

                string query = @"select id, name, tester_id ,developer_id, client_id, priority , status, created_at 
                                             from tickets where id = @id";

                using var command = new MySqlCommand(query, connection);

                command.Parameters.AddWithValue("@id", TicketId);

                using var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    Ticket = new Ticket
                    {
                        TicketsId = reader["id"].ToString(),
                        TicketsName = reader["name"].ToString(),
                        TicketsPriority = reader["priority"].ToString(),
                        TicketsTester = reader["tester_id"].ToString(),
                        TicketsDeveloper = reader["developer_id"].ToString(),
                        TicketsClient = reader["client_id"].ToString(),
                        TicketsStatus = reader["status"].ToString()
                    };

                }


            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void LoadTestersFromDb()
        {
            try
            {
                using var connection = Database.GetConnection();
                connection.Open();

                string query = @"select id, name from testers where active = true";

                using var command = new MySqlCommand(query, connection);

                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Testers.Add(new Testers
                    {
                        TestersId = reader["id"].ToString(),
                        TestersName = reader["name"].ToString()
                    });
                }

            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void LoadDevelopersFromDb()
        {
            try
            {
                using var connection = Database.GetConnection();
                connection.Open();

                string query = @"select id, name from developers where active = true";

                using var command = new MySqlCommand(query, connection);

                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Developers.Add(new Developers
                    {
                        DevelopersId = reader["id"].ToString(),
                        DevelopersName = reader["name"].ToString()
                    });
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void LoadClientsFromDb()
        {
            try
            {
                using var connection = Database.GetConnection();
                connection.Open();

                string query = @"select id, name from clients where active = true";

                using var command = new MySqlCommand(query, connection);

                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Clients.Add(new Clients
                    {
                        ClientsId = reader["id"].ToString(),
                        ClientsName = reader["name"].ToString()
                    });
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void LoadCommentsFromDb(int TicketId)
        {
            try
            {
                Comments.Clear();
                using var connection = Database.GetConnection();
                connection.Open();

                string query = @"select 
                                senders_name, message, created_at
                                from comments where tickets_id = @id
                                order by created_at asc;";

                using var command = new MySqlCommand(query, connection);

                command.Parameters.AddWithValue("@id", TicketId);
                using var reader = command.ExecuteReader();


                while (reader.Read())
                {
                    Comments.Add(new Comments
                    {
                        CommentName = reader["senders_name"].ToString(),
                        Comment = reader["message"].ToString(),
                        CommentDate = ((DateTime)reader["created_at"]).ToString("yyyy-MM-dd HH:mm")
                    }); 
                }


            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }


        public void InsertComments(object sender, RoutedEventArgs e)
        {
            string name = NameChatTextBox.Text.Trim();
            string message = CommentChatTextBox.Text.Trim();
            int id = TicketId;

            if (id == 0)
            {
                NavigationService.Navigate(new TicketsList());
                return;
            }
            if(name == "" || name == "Name")
            {
                MessageBox.Show("You have to put a valid name");
                return;
            }
            if (message == "" || message == "Comment")
            {
                MessageBox.Show("You have to put a valid comment"); return;
            }

            try
            {
                using var connection = Database.GetConnection();
                connection.Open();

                string insert = @"insert into comments (tickets_id, senders_name, message, created_at)
                                                values (@tickets_id, @senders_name, @message, now())";

                using var command = new MySqlCommand(insert, connection);

                command.Parameters.AddWithValue("@tickets_id", id);
                command.Parameters.AddWithValue("@senders_name", name);
                command.Parameters.AddWithValue("@message", message);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    MessageBox.Show("Comment was not commited");
                }
                LoadCommentsFromDb(id);

                NameChatTextBox.Text = "";
                CommentChatTextBox.Text = "";

            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
            }
        }


        public void InsertTicket(object sender, RoutedEventArgs e)
        {
            bool nameChecked = CheckCorrect(TitleTicketsName);
            string name = TitleTicketsName.Text;
            if(!nameChecked)
            {
                MessageBox.Show("Please type a correct title");
                return;
            }


            if(PriorityDropDown.SelectedItem == null)
            {
                MessageBox.Show("Please select a priority");
                return;
            }
            string priority = PriorityDropDown.SelectedItem.ToString();



            string status = "NEW";



            if (TesterDropDown.SelectedValue == null)
            {
                MessageBox.Show("Please choose a tester");
                return;
            }
            var selectedTester = TesterDropDown.SelectedItem as Testers;

            int? tester_id = Convert.ToInt32(selectedTester.TestersId);


            if (DeveloperDropDown.SelectedValue == null)
            {
                MessageBox.Show("Please choose a developer");
                return;
            }
            var selectedDeveloper = DeveloperDropDown.SelectedItem as Developers;

            int? developer_id = Convert.ToInt32(selectedDeveloper.DevelopersId);


            if (ClientsDropDown.SelectedValue == null)
            {
                MessageBox.Show("Please select a client!");
                return;
            }
            var selectedClient = ClientsDropDown.SelectedItem as Clients;

            int? client_id = Convert.ToInt32(selectedClient?.ClientsId);

            //MessageBox.Show(developer_id.ToString());

            try
            {
                using var connection = Database.GetConnection();
                connection.Open();

                string insert = @"insert into tickets 
                                        (name, tester_id, developer_id, client_id, priority, status) 
                                        values (@name, @tester_id, @developer_id, @client_id, @priority, @status);";

                using var command = new MySqlCommand(insert, connection);

                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@tester_id", tester_id);
                command.Parameters.AddWithValue("@developer_id", developer_id);
                command.Parameters.AddWithValue("@client_id", client_id);
                command.Parameters.AddWithValue("@priority", priority);
                command.Parameters.AddWithValue("@status", status);


                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    MessageBox.Show("Error inserting the ticket!");
                }

                string idInserted = command.LastInsertedId.ToString();

                NavigationService.Navigate(new TicketsDetails(idInserted));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public bool CheckCorrect(TextBox tb)
        {
            string textTb = tb.Text.ToLower();
            if (string.IsNullOrWhiteSpace(textTb)) return false;
            if (textTb == tb.Tag?.ToString().ToLower()) return false;
            if (textTb.Length < 5) return false;

            return true;
        }

        public void UpdateCurrentTicket(object sender, RoutedEventArgs e)
        {
            string name = Ticket.TicketsName;

            if(name == "" || name.Length <= 5)
            {
                MessageBox.Show("Please put a valid name");
                return;
            }

            string priority = Ticket.TicketsPriority;
            string status = Ticket.TicketsStatus;
            int tester_id = Convert.ToInt32(Ticket.TicketsTester);
            int developer_id = Convert.ToInt32(Ticket.TicketsDeveloper);
            int client_id = Convert.ToInt32(Ticket.TicketsClient);

            if(TicketId == 0)
            {
                MessageBox.Show("Failed to update");
                return;
            } 

            try
            {
                using var connection = Database.GetConnection();
                connection.Open();

                string update = @"update tickets 
                                    set name = @name, tester_id = @tester_id, developer_id = @developer_id, client_id = @client_id, priority = @priority, status = @status 
                                    where id = @id";

                using var command = new MySqlCommand(update, connection);

                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@tester_id", tester_id);
                command.Parameters.AddWithValue("@developer_id", developer_id);
                command.Parameters.AddWithValue("@client_id", client_id);
                command.Parameters.AddWithValue("@priority", priority);
                command.Parameters.AddWithValue("@status", status);
                command.Parameters.AddWithValue("@id", TicketId);

                int rowsAffected = command.ExecuteNonQuery();

                if(rowsAffected == 0)
                {
                    MessageBox.Show("Update has failed.");
                }


            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
            }

        }

    }


    public class Ticket
    {
        public string TicketsId { get; set; }
        public string TicketsName { get; set; }
        public string TicketsPriority { get; set; }
        public string TicketsTester { get; set; }
        public string TicketsDeveloper { get; set; }
        public string TicketsClient { get; set; }
        public string TicketsStatus { get; set; }

    }

    public class Comments
    {
        public string CommentName { get; set; }
        public string Comment { get; set; }
        public string CommentDate { get; set; }

    }

    public class Testers
    {
        public string TestersId { get; set; }
        public string TestersName { get; set; }
    }

    public class Developers
    {
        public string DevelopersId { get; set; }
        public string DevelopersName { get; set; }
    }


    public class Clients
    {
        public string ClientsId { get; set; }
        public string ClientsName { get; set; }
    }

}
