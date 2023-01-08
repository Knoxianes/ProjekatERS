﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace Hydroelectric_power_plant
{
    public class DataBase
    {
        private SqlConnection connection;

        public DataBase()
        {
            StartDB();
        }

        public SqlConnection Connection { get => connection; set => connection = value; }

        //Funkcija uspostavlja konekciju sa bazom podataka
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
            connection = new SqlConnection(connectionString); // Uspotevljanje konekcije
            SendCommand("DROP TABLE Powerplant"); // Slanje komande za brisnaje stare tabele u bazi podataka
            string createTableSql = "CREATE TABLE Powerplant ( Snaga_elektrane decimal(13,2), Procenat_rada decimal(13,2), Timestamp varchar(255));";
            SendCommand(createTableSql); // Slanje komande za pravljenje nove tabele u bazi podataka

        }
        // Funkcija salje komandu u bazu sa kojom smo uspotavili vezu
        public bool SendCommand(string command)
        {
            try
            {
                connection.Open(); // Otvaranje konekcije
                SqlCommand tmp = new SqlCommand(command, connection);
                tmp.ExecuteNonQuery(); // Slanje komande
                connection.Close(); // Zatvaranje konekcije
                return true;
            }catch (Exception e)
            {
                Console.WriteLine("Komanda neuspesno poslata u bazu podataka! "  + e);
                return false;
            }
        }
    }
}
