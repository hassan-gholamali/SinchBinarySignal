using SinchBinarySignal.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SinchBinarySignal.Validators
{
    public static class MessageValidationHelper
    {
        public const int MaxHeaderSize = 1023;
        public const int MaxHeaders = 63;
        public const int MaxPayloadSize = 256 * 1024; // 256 KiB

        public static void ValidateMessage(Message message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (message.payload == null || message.payload.Length == 0)
                throw new ArgumentException("Invalid message format. Missing payload.");

            if (message.headers != null && message.headers.Count > MaxHeaders)
                throw new ArgumentException($"Invalid message format. Maximum {MaxHeaders} headers allowed.");

            if (message.payload != null && message.payload.Length > MaxPayloadSize)
                throw new ArgumentException($"Invalid message format. Maximum payload size exceeded: {MaxPayloadSize} bytes.");

            if (message.headers != null)
            {
                foreach (var header in message.headers)
                {
                    ValidateHeaderSize(header.Key);
                    ValidateHeaderSize(header.Value);
                }
            }
        }

        public static void ValidateEncodedMessage(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("Encoded value is Null");

            if(data.Length == 0)
                throw new ArgumentException("Encoded value is Empty");
        }

        private static void ValidateHeaderSize(string header)
        {
            if (header != null)
            {
                foreach (char c in header)
                {
                    if (!char.IsAscii(c))
                        throw new ArgumentException($"Invalid character in {header}. Non-ASCII characters are not allowed: {c}");
                }
                if (Encoding.ASCII.GetByteCount(header) > MaxHeaderSize)
                    throw new ArgumentException($"Invalid header size. Maximum size allowed: {MaxHeaderSize} bytes.");
            }
        }
    }
}
