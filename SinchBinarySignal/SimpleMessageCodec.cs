using SinchBinarySignal.Entities;
using SinchBinarySignal.Interfaces;
using SinchBinarySignal.Validators;
using System.Text;

namespace SinchBinarySignal
{
    public class SimpleMessageCodec : IMessageCodec
    {
        /// <summary>
        /// Encodes a Message object into a binary representation.
        /// </summary>
        /// <param name="message">The Message object to encode.</param>
        /// <returns>A byte array representing the encoded message.</returns>
        public byte[] Encode(Message message)
        {
            // Implementation logic:
            // The first byte of the encoded message represents the header count. As the maximum header count
            // is 63, a single byte is sufficient.
            // Each header name and value is encoded sequentially, with a two-byte prefix indicating their size.
            // Since the maximum size for header names and values is 1023 bytes, two bytes are allocated for size representation.
            // The payload is appended to the end of the byte array without any modification.

            try
            {
                MessageValidationHelper.ValidateMessage(message);

                List<byte> encodedMessage = new List<byte>();
                // Null or empty headers are considered interchangeable 
                if (message.headers == null)
                    message.headers = new Dictionary<string, string>();

                // Number of headers would be needed for decoding
                byte headerCount = (byte)message.headers.Count;
                encodedMessage.Add(headerCount);

                // Encode headers
                foreach (var header in message.headers)
                {
                    EncodeString(encodedMessage, header.Key);
                    EncodeString(encodedMessage, header.Value);
                }

                // Encode payload
                encodedMessage.AddRange(message.payload);

                return encodedMessage.ToArray();
            }
            catch (Exception)
            {
                // If a logging system is available, we can utilize it here.
                throw;
            }

        }

        /// <summary>
        /// Decodes a binary representation into a Message object.
        /// </summary>
        /// <param name="data">The byte array representing the encoded message.</param>
        /// <returns>A Message object decoded from the binary data.</returns>
        public Message Decode(byte[] data)
        {
            try
            {
                MessageValidationHelper.ValidateEncodedMessage(data);

                int headerCount = (int)data[0];
                var decodedHeaders = new Dictionary<string, string>();

                // Current index is one, because the first byte is read for header count
                int currentIndex = 1;
                while (currentIndex < data.Length && decodedHeaders.Count < headerCount)
                {
                    string name = DecodeString(data, ref currentIndex);
                    string value = DecodeString(data, ref currentIndex);
                    if (!decodedHeaders.ContainsKey(name))
                        decodedHeaders.Add(name, value);
                    else
                        throw new ArgumentException($"Header error, duplicate name detected: {name}");
                }

                if (currentIndex == data.Length)
                    throw new ArgumentException("Invalid message format. Missing payload.");

                byte[] payload = new byte[data.Length - currentIndex];
                Array.Copy(data, currentIndex, payload, 0, payload.Length);

                return new Message { headers = decodedHeaders, payload = payload };
            }
            catch (Exception)
            {
                // If a logging system is available, we can utilize it here.
                throw;
            }
        }

        private void EncodeString(List<byte> encodedMessage, string value)
        {
            // ushort is used for size storage because it is a two-byte integer capable of
            // storing the maximum size of 1023 bytes.
            if (value == null)
            {
                encodedMessage.AddRange(BitConverter.GetBytes((ushort)0));
            }
            else
            {
                byte[] encodedValue = Encoding.ASCII.GetBytes(value);
                encodedMessage.AddRange(BitConverter.GetBytes((ushort)encodedValue.Length));
                encodedMessage.AddRange(encodedValue);
            }
        }

        private string DecodeString(byte[] data, ref int currentIndex)
        {
            // ushort is used for size storage because it is a two-byte integer capable of
            // storing the maximum size of 1023 bytes.
            if (currentIndex + sizeof(ushort) > data.Length)
                throw new ArgumentException("Invalid message format. Incomplete data for header.");

            ushort length = BitConverter.ToUInt16(data, currentIndex);
            currentIndex += sizeof(ushort);

            if (currentIndex + length > data.Length)
                throw new ArgumentException("Invalid message format. Incomplete data for header.");

            string value = Encoding.ASCII.GetString(data, currentIndex, length);
            currentIndex += length;

            return value;
        }
    }
}