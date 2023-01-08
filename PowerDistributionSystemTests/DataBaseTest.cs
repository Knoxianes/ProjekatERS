using Moq;
using NUnit.Framework;
using Solar_panels_and__wind_generators;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerDistributionSystemTests
{
    [TestFixture]
    public class DataBaseTest
    {

        [Test]
        public void SendCommand_ReturnsFalse() 
        {
            //Arrange
            DataBase db = new DataBase();

            var mock = new Mock<IMySqlConnection>();
            mock.Setup(x => x.Open()).Throws(new Exception());
            db.MyConnection = mock.Object;


            //Act

            bool result = db.SendCommand("test");
            //Assert
            Assert.IsFalse(result);

        }

        [Test]
        public void SendCommand_ReturnsTrue()
        {
            //Arrange
            DataBase db = new DataBase();

            var mock = new Mock<IMySqlConnection>();
            mock.Setup(x => x.Open()).Verifiable();
            mock.Setup(x => x.Close()).Verifiable();
            db.MyConnection = mock.Object;


            //Act

            bool result = db.SendCommand("test");
            //Assert
            Assert.IsFalse(result);
        }
    }
}
