using Distribution_centar;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerDistributionSystemTests
{
    [TestFixture]

    public class IzvestajTest
    {

        [Test]
        public void Add_AddItem() 
        {
            //Arrange
            Izvestaj izvestaj = new Izvestaj();
            int vati = 1;
            int cena = 1;


            //Act
            izvestaj.Add(vati, cena);

            //Assert
            CollectionAssert.IsNotEmpty(izvestaj.Potrosaci);
        
        }


        [Test]
        public void Remove_RemovesItem()
        {
            //Arrange
            Izvestaj izvestaj = new Izvestaj();
            int vati = 1;
            int cena = 1;
            Potrosac tmp = new Potrosac(vati, cena);
            int id = 1;
            tmp.Id = id;
            izvestaj.Potrosaci.Add(tmp);


            //Act
            izvestaj.Remove(id);

            //Assert
            CollectionAssert.IsEmpty(izvestaj.Potrosaci);
        }

        [Test]
        public void Izracunaj_Izvestaj_ReturnsTrue()
        {
            //Arrange
            Izvestaj i = new Izvestaj();

            int vati = 1;
            int cena = 1;
            Potrosac tmp = new Potrosac(vati, cena);
            tmp.Id = 1;
            i.Potrosaci.Add(tmp);
            i.izvestaj.Add(tmp.Id, 1);

            //Act
            bool result = i.Izracunaj_izvestaj();

            //Assert
            Assert.IsTrue(result);
        }

       

    }
}
