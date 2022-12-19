using Moq;
using NUnit.Framework;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;


namespace Power_distribution_system_Test
{
    [TestFixture]
    public class Tests
    {

        [Test]
        public void ServerSend_Success()
        {
            var mockStream = new Moq.Mock<NetworkStream>();
            object p = mockStream.Setup(s => s.Write(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Throws(() => { return; });
            var message = "test message";
            var code = "TEST";

            // Act
            var result = Server_Send(mockStream.Object, message, code);

            // Assert
            Assert.IsTrue(true, result.ToString());
        }

        [Test]
        public void ServerSend_Exception() 
        { 
        // Arrange
            var mockStream = new Mock<NetworkStream>();
            mockStream.Setup(s => s.Write(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Throws(new Exception());
            var message = "test message";
            var code = "TEST";

            // Act
            var result = Server_Send(mockStream.Object, message, code);

            // Assert
            Assert.IsFalse(false, result.ToString());
        }

        private object Server_Send(NetworkStream @object, string message, string code)
        {
            throw new NotImplementedException();
        }
    }
}

