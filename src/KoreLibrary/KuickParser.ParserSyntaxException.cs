using System;

namespace Kore
{
    public partial class KuickParser
    {
        public class ParserSyntaxException : Exception
        {
            public ParserSyntaxException(string message) : base(message) { }
            public ParserSyntaxException(string message, Exception innerException) : base(message, innerException) { }
        }
    }
}
