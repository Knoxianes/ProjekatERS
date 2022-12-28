using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Hydroelectric_power_plant
{
    class Client
    {
        private NetworkStream stream; //stream za slanje i primanje poruka

        public Client()
        {
            Start();
        }

        public NetworkStream Stream { get => stream; set => stream = value; }

        // Funkcija pokrece tcp clienta 
        private void Start()
        {
            
            TcpClient client = new TcpClient("127.0.0.1", 8001);
            Console.WriteLine("Distribtuion server connected");
            stream = client.GetStream();
        }
        // Funkcija sluzi za slanje poruka preko streama
        public bool Send(string msg, string code)
        {
            try
            {
                if (code != "")
                {
                    code += ";";
                }
                byte[] data = Encoding.ASCII.GetBytes(code + msg);
                stream.Write(data, 0, data.Length);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Doslo je do greske prilikom slanja poruke: " + e);
                return false;
            }
        }

        // Funkcija sluzi za primanje poruka preko strima
        public string Recieve()
        {
            try
            {
                byte[] buffer = new byte[1024];
                stream.Read(buffer,0,1024);
                return Encoding.ASCII.GetString(buffer, 0, buffer.Length);
            } catch (Exception e)
            {
                Console.WriteLine("Doslo je do greske prilikom primanja poruke: " + e);
                return "";
            }
        }
        //Funkcija pise u fajl sa odredjenim pathom
        public bool WriteToFile(string msg,string path)
        {
            try
            {
                StreamWriter w = new StreamWriter(path, append: true) ;
                w.WriteLine(msg);
                w.Close();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Doslo je do greske prilikom pisanja u fajl! " + e);
                return false;
            }
        }
        
    }
}


