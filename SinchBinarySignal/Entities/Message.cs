using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SinchBinarySignal.Entities
{
    public class Message
    {
        public Dictionary<String, String> headers;
        public byte[] payload;
    }
}
