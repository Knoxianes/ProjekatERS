using System;
using System.Data.SqlClient;
using System.IO;
using System.Diagnostics;


namespace Solar_panels_and__wind_generators
{
    public class DataBase
    {

        public IMySqlConnection MyConnection { get; set; }

        public DataBase()
        {
            MyConnection = new MySqlConnection();
            StartDB();
        }



        //Funkcija se povezuje sa bazom podataka i pravi novu tabelu
        private void StartDB()
        {
            string relativePath = @"..\..\Database1.mdf";
            string absolutePath = Path.GetFullPath(relativePath);
            string connectionString = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={0};Integrated Security=True", absolutePath);
            MyConnection.Connection = new SqlConnection(connectionString);
            var broj_instanci = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length; // Nalazi broj instanci programa
            
            if(broj_instanci <= 1) // Ako je broj instanci 1 ili manji od 1 pokrece brisanje i pravljenje nove tabele u bazi
            {
                SendCommand("DROP TABLE Solar_Wind");
                string createTableSql = "CREATE TABLE Solar_Wind ( Energija_Sunca decimal(13,2),Energija_Vetra decimal(13,2),Panel decimal(13,2),Generator decimal(13,2), Timestamp varchar(255));";
                SendCommand(createTableSql);
            }
        }

        // Funkcija salje komandu u bazu
        public bool SendCommand(string command)
        {
            try
            {
                MyConnection.Open();
                SqlCommand tmp = new SqlCommand(command, MyConnection.Connection);
                tmp.ExecuteNonQuery();
                MyConnection.Close();
                return true;
            }catch (Exception e)
            {
                Console.WriteLine("Komanda neuspesno poslata u bazu podataka! "  + e);
                MyConnection.Close();
                return false;
            }
        }
    }
}
