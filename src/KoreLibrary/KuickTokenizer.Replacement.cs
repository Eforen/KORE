namespace Kore
{
    public partial class KuickTokenizer
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
