using System;
using System.Data.SqlClient;
using System.IO;
using System.Diagnostics;


namespace Solar_panels_and__wind_generators
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
            string relativePath = @"..\..\Database1.mdf";
            string absolutePath = Path.GetFullPath(relativePath);
            string connectionString = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={0};Integrated Security=True", absolutePath);
            connection = new SqlConnection(connectionString);
            var broj_instanci = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length;
            
            if(broj_instanci <= 1)
            {
                SendCommand("DROP TABLE Solar_Wind");
                string createTableSql = "CREATE TABLE Solar_Wind ( Energija_Sunca decimal(13,2),Energija_Vetra decimal(13,2),Panel decimal(13,2),Generator decimal(13,2), Timestamp varchar(255));";
                SendCommand(createTableSql);
            }
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
                connection.Close();
            }
        }
    }
}
