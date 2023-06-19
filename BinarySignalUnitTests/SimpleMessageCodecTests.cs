using SinchBinarySignal;
using SinchBinarySignal.Entities;
using SinchBinarySignal.Validators;
using System.Text;

namespace BinarySignalUnitTests
{
    [TestClass]
    public class SimpleMessageCodecTests
    {
        private SimpleMessageCodec codec;

        [TestInitialize]
        public void Setup()
        {
            codec = new SimpleMessageCodec();
        }

        [TestMethod]
        public void EncodeAndDecode_MessageMatchesOriginal()
        {
            // Arrange
            var originalMessage = new Message
            {
                headers = new Dictionary<string, string>
            {
                { "Header1", "Value1" },
                { "Header2", "Value2" },
                { "Header3", "Value3" }
            },
                payload = Encoding.ASCII.GetBytes("PayloadData")
            };

            // Act
            var encodedData = codec.Encode(originalMessage);
            var decodedMessage = codec.Decode(encodedData);

            // Assert
            CollectionAssert.AreEqual(originalMessage.headers.Keys.ToList(), decodedMessage.headers.Keys.ToList());
            CollectionAssert.AreEqual(originalMessage.headers.Values.ToList(), decodedMessage.headers.Values.ToList());
            CollectionAssert.AreEqual(originalMessage.payload, decodedMessage.payload);
        }

        [TestMethod]
        public void EncodeAndDecode_MessageWithoutHeaderMatchesOriginal()
        {
            // Arrange
            var originalMessage = new Message
            {
                payload = Encoding.ASCII.GetBytes("PayloadData")
            };

            // Act
            var encodedData = codec.Encode(originalMessage);
            var decodedMessage = codec.Decode(encodedData);

            // Assert
            CollectionAssert.AreEqual(originalMessage.headers.Keys.ToList(), decodedMessage.headers.Keys.ToList());
            CollectionAssert.AreEqual(originalMessage.headers.Values.ToList(), decodedMessage.headers.Values.ToList());
            CollectionAssert.AreEqual(originalMessage.payload, decodedMessage.payload);
        }

        [TestMethod]
        public void EncodeAndDecode_MaximumSizeMessageMatchesOriginal()
        {
            // Arrange
            var originalMessage = new Message
            {
                headers = new Dictionary<string, string>()
            };

            for (int i = 1; i <= MessageValidationHelper.MaxHeaders; i++)
            {
                originalMessage.headers.Add(
                    $"{new string('N', MessageValidationHelper.MaxHeaderSize - i.ToString().Length)}{i}",
                    $"{new string('V', MessageValidationHelper.MaxHeaderSize - i.ToString().Length)}{i}");
            }

            originalMessage.payload = new byte[MessageValidationHelper.MaxPayloadSize];

            // Act
            var encodedData = codec.Encode(originalMessage);
            var decodedMessage = codec.Decode(encodedData);

            // Assert
            CollectionAssert.AreEqual(originalMessage.headers.Keys.ToList(), decodedMessage.headers.Keys.ToList());
            CollectionAssert.AreEqual(originalMessage.headers.Values.ToList(), decodedMessage.headers.Values.ToList());
            CollectionAssert.AreEqual(originalMessage.payload, decodedMessage.payload);
        }

        [TestMethod]
        public void Encode_NullOrEmptyPayload_ThrowsArgumentException()
        {
            // Arrange
            Message message = new Message();

            // Act and Assert
            Assert.ThrowsException<ArgumentException>(() => codec.Encode(message));
        }

        [TestMethod]
        public void Encode_NonASCIIHeaderCharacter_ThrowsArgumentException()
        {
            // Arrange
            var message = new Message
            {
                headers = new Dictionary<string, string>
                {
                    { "Ä", "Value1" }
                }
            };

            // Act and Assert
            Assert.ThrowsException<ArgumentException>(() => codec.Encode(message));
        }

        [TestMethod]
        public void Encode_MaxHeaderCountExceeded_ThrowsArgumentException()
        {
            // Arrange
            var message = new Message
            {
                headers = new Dictionary<string, string>()
            };

            for (int i = 1; i <= MessageValidationHelper.MaxHeaders + 1; i++)
            {
                message.headers.Add($"Header{i}", $"Value{i}");
            }

            // Act and Assert
            Assert.ThrowsException<ArgumentException>(() => codec.Encode(message));
        }

        [TestMethod]
        public void Encode_MaxPayloadSizeExceeded_ThrowsArgumentException()
        {
            // Arrange
            var message = new Message
            {
                headers = new Dictionary<string, string>(),
                payload = new byte[MessageValidationHelper.MaxPayloadSize + 1]
            };

            // Act and Assert
            Assert.ThrowsException<ArgumentException>(() => codec.Encode(message));
        }

        [TestMethod]
        public void Encode_MaxHeaderSizeExceeded_ThrowsArgumentException()
        {
            // Arrange
            var message = new Message
            {
                headers = new Dictionary<string, string>
            {
                { "Header1", new string('A', MessageValidationHelper.MaxHeaderSize + 1) }
            },
                payload = Encoding.ASCII.GetBytes("PayloadData")
            };

            // Act and Assert
            Assert.ThrowsException<ArgumentException>(() => codec.Encode(message));
        }

        [TestMethod]
        public void Decode_EmptyData_ThrowsArgumentException()
        {
            // Arrange
            byte[] data = new byte[0];

            // Act and Assert
            Assert.ThrowsException<ArgumentException>(() => codec.Decode(data));
        }

        [TestMethod]
        public void Decode_CorruptedData_ThrowsArgumentException()
        {
            // Arrange
            var originalMessage = new Message
            {
                headers = new Dictionary<string, string>
            {
                { "Header1", "Value1" },
                { "Header2", "Value2" },
                { "Header3", "Value3" }
            },
                payload = Encoding.ASCII.GetBytes("PayloadData")
            };

            // Act
            var encodedData = codec.Encode(originalMessage);
            encodedData[0] = 4;

            // Assert
            Assert.ThrowsException<ArgumentException>(() => codec.Decode(encodedData));
        }
    }
}