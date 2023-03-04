namespace Kore.Kuick {
    public partial class Lexer
    {
        public struct TokenData
        {
            public TokenData(Token token, string value, int lineNumber, int columnNumber)
            {
                this.token = token;
                this.value = value;
                this.lineNumber = lineNumber;
                this.columnNumber = columnNumber;
            }
            public Token token;
            public string value;
            public int lineNumber;
            public int columnNumber;
            public override bool Equals(object obj)
            {
                // if its a token just compair it to our token type
                if (obj.GetType() == typeof(Token)) return (Token)obj == token;
                // if its a string just compair it to our value
                if (obj.GetType() == typeof(string)) return ((string)obj).Equals(obj);
                // if its another TokenData then compair everything
                if (obj.GetType() == typeof(TokenData)) return ((TokenData)obj).value.Equals(obj) && ((TokenData)obj).token == token;
                // if its none of these then just dump to false
                return false;
            }

            public static bool operator ==(TokenData tokenData, Token token)
            {
                return tokenData.Equals(token);
            }

            public static bool operator !=(TokenData tokenData, Token token)
            {
                return tokenData.Equals(token) == false;
            }

            public static bool operator ==(TokenData tokenData, string value)
            {
                return tokenData.Equals(value);
            }

            public static bool operator !=(TokenData tokenData, string value)
            {
                return tokenData.Equals(value) == false;
            }

            public static bool operator ==(TokenData tokenData, TokenData tokenData2)
            {
                return tokenData.Equals(tokenData2);
            }

            public static bool operator !=(TokenData tokenData, TokenData tokenData2)
            {
                return tokenData.Equals(tokenData2) == false;
            }
        }
    }
}
