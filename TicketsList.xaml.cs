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
using MySqlConnector;

namespace TicketsProject
{
    /// <summary>
    /// Interaction logic for TicketsList.xaml
    /// </summary>
    enum Priority
    {
        ALL,
        LOW,
        MEDIUM,
        HIGH,
        URGENT
    }

    enum Status
    {
        ALL,
        NEW,
        SPECIFICATIONS,
        DEVELOPMENT,
        FEEDBACK,
        CLOSED
    }

    public partial class TicketsList : Page
    {
        public ObservableCollection<Tickets> Tickets { get; set; }

        public TicketsList()
        {
            InitializeComponent();

            StatusDropDown.ItemsSource = Enum.GetValues(typeof(Status));
            PriorityDropDown.ItemsSource = Enum.GetValues(typeof(Priority));

            StatusDropDown.SelectedItem = Status.ALL;
            PriorityDropDown.SelectedItem = Priority.ALL;


            //Tickets = new ObservableCollection<Tickets>
            //{
            //    new Tickets{TicketsId="1000", TicketsName="Solicitari Moreni", TicketsPriority="HIGH", TicketsResponsable="Adrian Gheorghe", TicketsClient="Spitalul Municipal Moreni", TicketsStatus="NEW"},
            //    new Tickets{TicketsId="1001", TicketsName="Solicitari Mures 1", TicketsPriority="LOW" , TicketsResponsable="Alexandra Damian", TicketsClient="Spitalul Judetean de Urgenta Targu Mures", TicketsStatus="SPECIFICATIONS"},
            //    new Tickets{TicketsId="1002", TicketsName="Solicitari Coltea", TicketsPriority="MEDIUM" , TicketsResponsable="Alexandru Vizauer", TicketsClient="Spitalul de Urgenta Coltea", TicketsStatus="DEVELOPMENT"},
            //    new Tickets{TicketsId="1003", TicketsName="Solicitari Constanta", TicketsPriority="URGENT" , TicketsResponsable="Adrian Gheorghe", TicketsClient="Spitalul Judetean de Urgenta Constanta", TicketsStatus="CLOSED"},

            //};
            Tickets = new ObservableCollection<Tickets>();


            LoadTicketsFromDatabase(StatusDropDown.SelectedItem.ToString(), PriorityDropDown.SelectedItem.ToString());

            //ConnectionTest();

            this.DataContext = this;

        }

        public void AddNewTicket(object sender, RoutedEventArgs e)
        {
            string id = "Y";

            NavigationService.Navigate(new TicketsDetails(id));

        }

        public void AddNewClient(object sender, RoutedEventArgs e)
        {
            string id = "Y";

            NavigationService.Navigate(new ClientsPage());

        }
        public void AddNewTester(object sender, RoutedEventArgs e)
        {

            NavigationService.Navigate(new TestersPage("T"));

        }

        public void AddNewDeveloper(object sender, RoutedEventArgs e)
        {

            NavigationService.Navigate(new TestersPage("D"));

        }



        public void FiltersSearch(object sender, RoutedEventArgs e)
        {
            int id;
            string name = IdSearchBox.Text;


            if (int.TryParse(name, out id))
            {
                NavigationService.Navigate(new TicketsDetails(id.ToString()));
            }
            else
            {
                MessageBox.Show("\"" + name + "\" " + "IS NOT A VALID NUMBER");
            }

        }

        public void FilterTickets(object sender, SelectionChangedEventArgs e)
        {
            if (StatusDropDown.SelectedItem is Status selectedStatus)
            {
                   
            }
        }



        public void SelectTicket(object sender, MouseButtonEventArgs e)
        {
            if (ListViewTicket.SelectedItem is Tickets selectedTicket)
            {
                string id = selectedTicket.TicketsId;

                NavigationService.Navigate(new TicketsDetails(id));

            }
        }

        //public void ConnectionTest()
        //{
        //    try
        //    {
        //        using var connection = Database.GetConnection();
        //        connection.Open();

        //        MessageBox.Show("Conectat!!");
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        public void RefreshTickets(object sender, RoutedEventArgs e)
        {
            LoadTicketsFromDatabase(StatusDropDown.SelectedItem.ToString(), PriorityDropDown.SelectedItem.ToString());
        }



        public void LoadTicketsFromDatabase(string s, string p)
        {
            string statusSelected = s;
            string prioritySelected = p;

            try
            {
                
                using var connection = Database.GetConnection();
                connection.Open();

                string query = @"select t.id , t.name, test.name as tester, d.name as developer, c.name as client, priority, status 
	                                from tickets as t
	                                join testers as test on test.id= t.tester_id
                                    join developers as d on d.id= t.developer_id
                                    join clients as c on c.id= t.client_id";

                query = statusSelected == "ALL" ? query : query + " where status =\"" + statusSelected + "\"";

                switch (statusSelected)
                {
                    case "ALL":
                        query = prioritySelected == "ALL" ? query : query + " where priority = \"" + prioritySelected + "\"";
                        break;
                    default:
                        query = prioritySelected == "ALL" ? query : query + " AND priority = \"" + prioritySelected + "\"";
                        break;
                }

                //MessageBox.Show(statusSelected + "  " + prioritySelected);

                using var command = new MySqlCommand(query, connection);

                using var reader = command.ExecuteReader();

                Tickets.Clear();

                while (reader.Read())
                {
                    Tickets.Add(new Tickets
                    {
                        TicketsId = reader["id"].ToString(),
                        TicketsName = reader["name"].ToString(),
                        TicketsPriority = reader["priority"].ToString(),
                        TicketsTester = reader["tester"].ToString(),
                        TicketsDeveloper = reader["developer"].ToString(),
                        TicketsClient = reader["client"].ToString(),
                        TicketsStatus = reader["status"].ToString()
                    });
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }


}


        public class Tickets
    {
        public string TicketsId { get; set; }
        public string TicketsName { get; set; }
        public string TicketsPriority { get; set; }
        public string TicketsTester { get; set; }
        public string TicketsDeveloper { get; set; }
        public string TicketsClient { get; set; }
        public string TicketsStatus { get; set; }

    }

    public class SearchForId
    {
        public int TicketId { get; set; }


    }

    public class FiltersObject
    {
        //public int 
    }
}
