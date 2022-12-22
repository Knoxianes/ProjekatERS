using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace Hydroelectric_power_plant
{
    class DataBase
    {
        private SqlConnection connection;

        public DataBase()
        {
            StartDB();
        }

        public SqlConnection Connection { get => connection; set => connection = value; }

        private void StartDB()
        {
            var relativePath = Path.GetFullPath(".").Split("\\");
            var tmp = new string[relativePath.Length-4];
            for(int i =0; i < relativePath.Length - 4; i++)
            {
                tmp[i] = relativePath[i];
            }
            string absolutePath = Path.Combine(tmp) + "\\Solar panels and  wind generators\\bin\\Database1.mdf";

            string connectionString = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={0};Integrated Security=True", absolutePath);
            connection = new SqlConnection(connectionString);
            SendCommand("DROP TABLE Powerplant");
            string createTableSql = "CREATE TABLE Powerplant ( Snaga_elektrane decimal(13,2), Procenat_rada decimal(13,2), Timestamp varchar(255));";
            SendCommand(createTableSql);

        }
        public void SendCommand(string command)
        {
            try
            {
                connection.Open();
                SqlCommand tmp = new SqlCommand(command, connection);
                tmp.ExecuteNonQuery();
                connection.Close();
            }catch (Exception e)
            {
                Console.WriteLine("Komanda neuspesno poslata u bazu podataka! "  + e);
            }
        }
    }
}
