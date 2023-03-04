using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kore.Utility {

    // Helper attribute for string values of enum members
    public class StringValue : Attribute {
        public string Value { get; }

        public StringValue(string value) {
            Value = value;
        }
    }
    public static class StringValueHelper {

        /// <summary>
        /// Converts an enum value to its corresponding string value.
        /// </summary>
        public static string GetStringValue(this Enum value) {
            Type type = value.GetType();
            FieldInfo fieldInfo = type.GetField(value.ToString());
            StringValue[] attributes = fieldInfo.GetCustomAttributes(typeof(StringValue), false) as StringValue[];
            return attributes.Length > 0 ? attributes[0].Value : null;
        }

        /// <summary>
        /// Converts a string value to its corresponding enum value.
        /// </summary>
        public static bool TryParseString<T>(string typeString, out T enumObj) where T : Enum {
            enumObj = default(T);
            foreach(T value in Enum.GetValues(typeof(T))) {
                if(value.GetStringValue() == typeString) {
                    enumObj = value;
                    return true;
                }
            }
            return false;
        }
    }
}
