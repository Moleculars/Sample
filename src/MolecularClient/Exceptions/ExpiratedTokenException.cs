using System;

namespace MolecularClient
{
    [Serializable]
    public class ExpiratedTokenException : Exception
    {
        public ExpiratedTokenException() { }
        public ExpiratedTokenException(string message) : base(message) { }
        public ExpiratedTokenException(string message, Exception inner) : base(message, inner) { }
        protected ExpiratedTokenException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

}
