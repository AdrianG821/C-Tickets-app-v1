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
using static TicketsProject.ClientsPage;

namespace TicketsProject
{
    /// <summary>
    /// Interaction logic for TestersPage.xaml
    /// </summary>
    public partial class TestersPage : Page
    {
        public ObservableCollection<TestersClass> TestersClass { get; set; }
        public Boolean testers;

        public TestersPage(string x)
        {
            InitializeComponent();

            TestersClass = new ObservableCollection<TestersClass>();

            if(x == "T")
            {
                testers = true;
                LoadTestersFromDb();
                TextContentBlock.Text = "Add a tester";

                

            }
            else if(x == "D")
            {

                testers = false;
                LoadDevelopersFromDb();
                TextContentBlock.Text = "Add a developer";

            }



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


        public void AddFunction(object sender, RoutedEventArgs e)
        {
            string nameE = NameETextBox.Text.Trim();
            string table = testers ? "testers" : "developers";

            if (nameE == "Name" || nameE == "" || nameE.Length < 4)
            {
                MessageBox.Show("You have to put a valid name");
                return;
            }

            try
            {
                using var connection = Database.GetConnection();
                connection.Open();

                string insert = $@"insert into {table} (name) values (@name)";

                using var command = new MySqlCommand(insert, connection);
                command.Parameters.AddWithValue("@name", nameE);

                int rowsAffected = command.ExecuteNonQuery();

                if(rowsAffected == 0)
                {
                    MessageBox.Show("Changes not comitted");
                }

                if(table == "testers")
                {
                    LoadTestersFromDb();

                }
                else if(table == "developers")
                {
                    LoadDevelopersFromDb();
                }

                NameETextBox.Text = "";

            } 
            catch(Exception er)
            {
                MessageBox.Show(er.Message);
            }
        }




        public void LoadTestersFromDb()
        {
            try
            {
                TestersClass.Clear();
                using var connection = Database.GetConnection();

                connection.Open();

                string query = "select id, name, active , created_at from testers";

                using var command = new MySqlCommand(query, connection);

                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    TestersClass.Add(new TestersClass
                    {
                        TestersId = reader["id"].ToString(),
                        TestersName = reader["name"].ToString(),
                        TestersActive = reader["active"].ToString(),
                        TestersCreatedAt = ((DateTime)reader["created_at"]).ToString("dd.MM.yyyy HH:mm")
                    });
                }
            }
            catch (Exception e)
            {

            }
        }

        public void LoadDevelopersFromDb()
        {
            try
            {
                TestersClass.Clear();

                using var connection = Database.GetConnection();

                connection.Open();

                string query = "select id, name, active , created_at from developers";

                using var command = new MySqlCommand(query, connection);

                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    TestersClass.Add(new TestersClass
                    {
                        TestersId = reader["id"].ToString(),
                        TestersName = reader["name"].ToString(),
                        TestersActive = reader["active"].ToString(),
                        TestersCreatedAt = ((DateTime)reader["created_at"]).ToString("dd.MM.yyyy HH:mm")
                    });
                }
            }
            catch (Exception e)
            {

            }
        }

    }


    public class TestersClass
    {
        public string TestersId { get; set; }
        public string TestersName { get; set; }
        public string TestersActive { get; set; }
        public string TestersCreatedAt { get; set; }
    }



}



