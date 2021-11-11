using System;

namespace Kore
{
    public partial class KuickTokenizer
    {
        public class TokenSyntaxException : Exception
        {
            public TokenSyntaxException(string message) : base(message) { }
            public TokenSyntaxException(string message, Exception innerException) : base(message, innerException) { }
        }
    }
}
