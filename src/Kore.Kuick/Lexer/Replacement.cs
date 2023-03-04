namespace Kore.Kuick {
    public partial class Lexer
    {
        public struct Replacement
        {
            public Replacement(string test, string target)
            {
                this.test = test;
                this.target = target;
            }
            public string test;
            public string target;
        }
    }
}
