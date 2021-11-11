using System;

namespace Kore
{
    public partial class KuickParser
    {
        public struct ParseData
        {
            public ParseData(StatementType type, object value)
            {
                this.type = type;
                this.value = value;
            }
            public StatementType type;
            public object value;
            public override string ToString()
            {
                if(value != null && value.GetType().IsArray)
                {
                    string str = "";
                    bool first = true;
                    if (value!=null && typeof(ParseData[]) == value.GetType())
                    {
                        foreach (ParseData item in (ParseData[])value)
                        {
                            if (first) first = false;
                            else str += "|";
                            str += item.ToString();
                        }
                    }
                    else
                    {
                        foreach (object item in (object[])value)
                        {
                            if (item == null) continue;
                            if (first) first = false;
                            else str += "|";
                            str += item.ToString();
                        }
                    }
                    // str += "]";
                    return "[" + Enum.GetName(typeof(StatementType), type) + "|" + str + "]";
                }
                return "[" + Enum.GetName(typeof(StatementType), type) + (value == null ? "" : "|" + value.ToString()) +"]";
            }
        }
    }
}
