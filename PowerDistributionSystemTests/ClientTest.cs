using Consumer;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PowerDistributionSystemTests
{
    [TestFixture]
    public class ClientTest
    {

        [Test]
        public void Send_ReturnsFalse() 
        {
            //Arrange
            string code = "test";
            string msg = "test";
            byte[] data = Encoding.ASCII.GetBytes(code + msg);
            Client client = new Client();
            var stream = new Mock<IMyNetworkStream>();
            stream.Setup(x => x.Write(It.IsAny<byte[]>(),It.IsAny<int>(),It.IsAny<int>())).Throws(new Exception());
            client.MyStream = stream.Object;


            //Act
            bool result = client.Send(code, msg);


            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Send_ReturnsTrue()
        {
            //Arrange
            string code = "test";
            string msg = "test";
            byte[] data = Encoding.ASCII.GetBytes(code + msg);
            Client client = new Client();
            var stream = new Mock<IMyNetworkStream>();
            stream.Setup(x => x.Write(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Verifiable();
            client.MyStream = stream.Object;


            //Act
            bool result = client.Send(code, msg);


            //Assert
            Assert.IsTrue(result);
        }


        [Test]
        public void Receive_ReturnsString() 
        {

            Client client = new Client();
            var stream = new Mock<IMyNetworkStream>();
            stream.Setup(x => x.Read(It.IsAny<byte[]>())).Callback((byte[] buffer) => Array.Copy(Encoding.ASCII.GetBytes("test"), buffer, Encoding.ASCII.GetBytes("test").Length));
            client.MyStream = stream.Object;

            //Act

            string result = client.Recieve();
            //Assert
            Assert.AreEqual("test", result.Substring(0,4));


        }

        [Test]
        public void WriteToFile_ReturnsTrue() 
        {
            //Arrange
            Client client = new Client();
            string msg = "test";
            string path = "test";


            //Act
            bool result = client.WriteToFile(msg, path);


            //Assert
            Assert.IsTrue(result);
        }
    }
}
