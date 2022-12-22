using System;

namespace Hydroelectric_power_plant
{
    class Program
    {
        static void Main(string[] args)
        {
            var powerplant = new Powerplant(50000);
            if (int.Parse(powerplant.Client.Recieve()) != 1)
            {
                Console.WriteLine("Doslo je do greske prilikom pokretanja!");
                Environment.Exit(1);
            }
            Console.WriteLine("Elektrana krece sa radom!");

            powerplant.Client.Send(powerplant.Snaga.ToString(),"1");
            while (true)
            {
                var msg = powerplant.Client.Recieve().Split(";");
                if(int.Parse(msg[0]) == 2)
                {
                    if (double.Parse(msg[1]) <= 0)
                    {
                        PowerPlant_Update(0, powerplant);
                        
                    }
                    else
                    {
                        PowerPlant_Update(double.Parse(msg[1]), powerplant);
                    }
                }else if(int.Parse(msg[0]) == 7)
                {
                    Console.WriteLine("\n*****PROGRAM SE GASI!!!*****");
                    Environment.Exit(0);
                }
               

            }
        }
        static void PowerPlant_Update(double nova_snaga,Powerplant powerplant)
        {
            Console.WriteLine("Potrebno je: " + Math.Round(nova_snaga,2) + " energije!");
            powerplant.Trenutna_proizvodnja = nova_snaga;
            powerplant.UpdateProcenat();
            Console.WriteLine("Elektrana sada radi na: " + Math.Round(powerplant.Procenat_rada,1) + "%");
            string vreme = DateTime.Now.ToString("HH:mm:ss tt");
            var command = "insert into Powerplant (Snaga_elektrane,Procenat_rada,Timestamp) values ('" + Math.Round(powerplant.Trenutna_proizvodnja, 2) + "', '" + Math.Round(powerplant.Procenat_rada, 1) + "', '" + vreme + "')";
            powerplant.Db.SendCommand(command);
            powerplant.Client.Send(powerplant.Procenat_rada.ToString(), "2");
        }
    }
}
