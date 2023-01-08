using System;

namespace Hydroelectric_power_plant
{
    class Program
    {
        static void Main(string[] args)
        {
            var powerplant = new Powerplant(50000); // Pravljenje elektrane sa 50000w snagom
            if (int.Parse(powerplant.Client.Recieve()) != 1) // Cekanje da se primi kod 1 od servera da moze elektrana da krene sa radom
            {
                Console.WriteLine("Doslo je do greske prilikom pokretanja!");
                Environment.Exit(1);
            }
            Console.WriteLine("Elektrana krece sa radom!");

            powerplant.Client.Send(powerplant.Snaga.ToString(),"1"); //Salje se serveru snaga elektrane sa kod 1 sto oznacava prvo pokretanje
            while (true) // While petlja koja ce da prima poruke od servera i da updejtuje procenat rada elektrane
            {
                var msg = powerplant.Client.Recieve().Split(";"); // primanje poruke od servera i delje iste na delove
                try
                {
                    if (int.Parse(msg[0]) == 2)
                    {
                        if (double.Parse(msg[1]) <= 0) // ako je snaga manje od nule koja je potrebna postavljamo procenat rada na 0
                        {
                            PowerPlant_Update(0, powerplant); // updejtuje eletranu

                        }
                        else
                        {
                            PowerPlant_Update(double.Parse(msg[1]), powerplant); //updektuje elektranu
                        }
                    }
                    else if (int.Parse(msg[0]) == 7) // ako je poslat kod broj 7 gasi se elektrana
                    {
                        Console.WriteLine("\n*****PROGRAM SE GASI!!!*****");
                        Environment.Exit(0);
                    }
                }
                catch
                {

                }
                
               

            }
        }
        //Funkcija koja updejtuje procenat rada elektrane i upisuje podatke u bazu podataka
        static void PowerPlant_Update(double nova_snaga,Powerplant powerplant)
        {
            Console.WriteLine("Potrebno je: " + Math.Round(nova_snaga,2) + " energije!");
            powerplant.Trenutna_proizvodnja = nova_snaga;
            powerplant.UpdateProcenat(); // Update procenta rada elektrane
            Console.WriteLine("Elektrana sada radi na: " + Math.Round(powerplant.Procenat_rada,1) + "%");
            string vreme = DateTime.Now.ToString("HH:mm:ss tt");
            var command = "insert into Powerplant (Snaga_elektrane,Procenat_rada,Timestamp) values ('" + Math.Round(powerplant.Trenutna_proizvodnja, 2) + "', '" + Math.Round(powerplant.Procenat_rada, 1) + "', '" + vreme + "')";
            powerplant.Db.SendCommand(command); // Slanje komande bazi podataka da upise podatke vezane za update rada elektrane
            powerplant.Client.Send(powerplant.Procenat_rada.ToString(), "2"); // Slanje serveru procenat rada elektrane
        }
    }
}
