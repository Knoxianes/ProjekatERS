using System;

namespace Hydroelectric_power_plant
{
    class Program
    {
        static void Main(string[] args)
        {
            DataBase db = new DataBase();
            var client = new Client();
            if(int.Parse(client.Recieve()) != 1)
            {
                Console.WriteLine("Doslo je do greske prilikom pokretanja!");
                Environment.Exit(1);
            }
            Console.WriteLine("Elektrana krece sa radom!");
            var powerplant = new Powerplant(50000);
            client.Send(powerplant.Snaga.ToString(),"1");
            while (true)
            {
                var msg = client.Recieve().Split(";");
                if(int.Parse(msg[0]) == 2)
                {
                    Console.WriteLine("Potrebno je: " + msg[1] + " energije!");
                    powerplant.Trenutna_proizvodnja = double.Parse(msg[1]);
                    powerplant.UpdateProcenat();
                    Console.WriteLine("Elektrana sada radi na: " + powerplant.Procenat_rada + "%");
                    string vreme = DateTime.Now.ToString("HH:mm:ss tt");
                    var command = "insert into Powerplant (Snaga_elektrane,Procenat_rada,Timestamp) values ('" + Math.Round(powerplant.Trenutna_proizvodnja, 2) + "', '" + Math.Round(powerplant.Procenat_rada, 2) + "', '"  + vreme + "')";
                    db.SendCommand(command);
                    client.Send(powerplant.Procenat_rada.ToString(), "2");
                }else if(int.Parse(msg[0]) == 7)
                {
                    Console.WriteLine("\n*****PROGRAM SE GASI!!!*****");
                    Environment.Exit(0);
                }
               

            }
        }
    }
}
