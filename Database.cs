using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketsProject
{
    public static class Database
    {
        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection("Server=localhost;UserId=root;Password=1234;Database=Tickets_db");
        }
    }
}
