using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NetBLogger
{
    public class NoNetworkInterfacesException : Exception
    {
        public NoNetworkInterfacesException() : base() { }

        public NoNetworkInterfacesException(string message) : base(message)
        {
        }

        public NoNetworkInterfacesException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoNetworkInterfacesException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
