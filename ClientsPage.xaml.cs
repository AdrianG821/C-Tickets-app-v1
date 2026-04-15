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


namespace TicketsProject
{
    /// <summary>
    /// Interaction logic for ClientsPage.xaml
    /// </summary>
    public partial class ClientsPage : Page
    {
        public ObservableCollection<ClientsClass> ClientsClass { get; set; }


        public ClientsPage()
        {
            InitializeComponent();

            ClientsClass = new ObservableCollection<ClientsClass>();

            //ListViewClients.Visibility = Visibility.Collapsed;
            LoadClientsFromDb();
            this.DataContext = this;
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


        public void BackButton(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TicketsList());
        }

        public void InsertClient(object sender, RoutedEventArgs e)
        {
            bool nameCheck = CheckCorrect(ClientsNameBox);
            string name = nameCheck ? ClientsNameBox.Text : null;

            if (!nameCheck)
            {
                MessageBox.Show("You have to put a valid name!");
                return;
            }

            bool addressCheck = CheckCorrect(ClientsAddressBox);
            string address = addressCheck ? ClientsAddressBox.Text : null;

            bool phoneCheck = CheckCorrect(ClientsPhoneBox);
            string phone = phoneCheck ? ClientsPhoneBox.Text : null;

            bool emailCheck = CheckCorrect(ClientsEmailBox);
            string email = emailCheck ? ClientsEmailBox.Text : null;

            //MessageBox.Show(name+ address+ phone+ email);

            try
            {
                using var connection = Database.GetConnection();
                connection.Open();

                string insert = "insert into clients (name, address , phone_number, email) values (@name, @address, @phone_number, @email)";

                using var command = new MySqlCommand(insert, connection);

                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@address", address);
                command.Parameters.AddWithValue("@phone_number", phone);
                command.Parameters.AddWithValue("@email", email);

                int insertCount = command.ExecuteNonQuery();

                if(insertCount == 0)
                {
                    MessageBox.Show("Insert failed!");
                }
                LoadClientsFromDb();


            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        public bool CheckCorrect(TextBox tb)
        {
            string textTb = tb.Text.ToLower();
            if(string.IsNullOrWhiteSpace(textTb)) return false;
            if(textTb == tb.Tag?.ToString().ToLower()) return false;
            if(textTb.Length < 5) return false;

            return true;
        }

        public void LoadClientsFromDb()
        {
            try
            {
                ClientsClass.Clear();

                using var connection = Database.GetConnection();

                connection.Open();

                string query = "select id, name, address, phone_number, email, active, created_at from clients;";

                using var command = new MySqlCommand(query, connection);

                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    ClientsClass.Add(new ClientsClass
                    {
                        ClientsId = reader["id"].ToString(),
                        ClientsName = reader["name"].ToString(),
                        ClientsAddress = reader["address"].ToString(),
                        ClientsPhone = reader["phone_number"].ToString(),
                        ClientsEmail = reader["email"].ToString(),
                        ClientsActive = reader["active"].ToString(),
                        ClientsCreatedAt = ((DateTime)reader["created_at"]).ToString("dd.MM.yyyy HH:mm")
                    });
                }
            }
            catch (Exception e)
            {

            }
        }

    }

        public class ClientsClass
        {
            public string ClientsId { get; set; }
            public string ClientsName { get; set; }
            public string ClientsPhone { get; set; }
            public string ClientsAddress { get; set; }
            public string ClientsEmail { get; set; }
            public string ClientsActive { get; set; }
            public string ClientsCreatedAt { get; set; }
        }

}
