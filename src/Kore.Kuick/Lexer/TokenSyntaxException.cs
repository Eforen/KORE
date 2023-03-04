using System;

namespace Kore.Kuick {
    public partial class Lexer
    {
        public class TokenSyntaxException : Exception
        {
            public TokenSyntaxException(string message) : base(message) { }
            public TokenSyntaxException(string message, Exception innerException) : base(message, innerException) { }
        }
    }
}
