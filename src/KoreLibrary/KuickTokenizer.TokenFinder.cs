using System.Text.RegularExpressions;

namespace Kore
{
    public partial class KuickTokenizer
    {
        public struct TokenFinder
        {
            public TokenFinder(string test, Token token)
            {
                this.test = new Regex(test, RegexOptions.Compiled);
                this.token = token;
            }
            public Regex test;
            public Token token;
        }
    }
}
