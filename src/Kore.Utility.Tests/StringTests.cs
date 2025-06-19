using NUnit.Framework;
using System;
using Kore.Utility;

namespace Kore.Utility.Tests
{
    // Test enum with StringValue attributes for testing
    public enum TestInstructionType
    {
        [StringValue("add")]
        Add,
        
        [StringValue("sub")]
        Subtract,
        
        [StringValue("mul")]
        Multiply,
        
        [StringValue("div")]
        Divide,
        
        // Enum member without StringValue attribute
        Unknown
    }

    [TestFixture]
    public class StringValueTests
    {
        [Test]
        public void StringValue_Constructor_ShouldSetValue()
        {
            // Arrange
            string expectedValue = "test-value";
            
            // Act
            var stringValue = new StringValue(expectedValue);
            
            // Assert
            Assert.AreEqual(expectedValue, stringValue.Value);
        }

        [Test]
        public void GetStringValue_WithStringValueAttribute_ShouldReturnCorrectString()
        {
            // Arrange
            var enumValue = TestInstructionType.Add;
            
            // Act
            string result = enumValue.GetStringValue();
            
            // Assert
            Assert.AreEqual("add", result);
        }

        [Test]
        public void GetStringValue_WithAllAttributedEnumValues_ShouldReturnCorrectStrings()
        {
            // Test all enum values with StringValue attributes
            Assert.AreEqual("add", TestInstructionType.Add.GetStringValue());
            Assert.AreEqual("sub", TestInstructionType.Subtract.GetStringValue());
            Assert.AreEqual("mul", TestInstructionType.Multiply.GetStringValue());
            Assert.AreEqual("div", TestInstructionType.Divide.GetStringValue());
        }

        [Test]
        public void GetStringValue_WithoutStringValueAttribute_ShouldReturnNull()
        {
            // Arrange
            var enumValue = TestInstructionType.Unknown;
            
            // Act
            string result = enumValue.GetStringValue();
            
            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void TryParseString_WithValidStringValue_ShouldReturnTrueAndCorrectEnum()
        {
            // Arrange
            string inputString = "add";
            
            // Act
            bool success = StringValueHelper.TryParseString<TestInstructionType>(inputString, out TestInstructionType result);
            
            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(TestInstructionType.Add, result);
        }

        [Test]
        public void TryParseString_WithAllValidStringValues_ShouldReturnCorrectEnums()
        {
            // Test all valid string values
            Assert.IsTrue(StringValueHelper.TryParseString<TestInstructionType>("add", out var add));
            Assert.AreEqual(TestInstructionType.Add, add);
            
            Assert.IsTrue(StringValueHelper.TryParseString<TestInstructionType>("sub", out var sub));
            Assert.AreEqual(TestInstructionType.Subtract, sub);
            
            Assert.IsTrue(StringValueHelper.TryParseString<TestInstructionType>("mul", out var mul));
            Assert.AreEqual(TestInstructionType.Multiply, mul);
            
            Assert.IsTrue(StringValueHelper.TryParseString<TestInstructionType>("div", out var div));
            Assert.AreEqual(TestInstructionType.Divide, div);
        }

        [Test]
        public void TryParseString_WithInvalidStringValue_ShouldReturnFalseAndDefaultEnum()
        {
            // Arrange
            string inputString = "invalid";
            
            // Act
            bool success = StringValueHelper.TryParseString<TestInstructionType>(inputString, out TestInstructionType result);
            
            // Assert
            Assert.IsFalse(success);
            Assert.AreEqual(default(TestInstructionType), result);
        }

        [Test]
        public void TryParseString_WithNullStringValue_ShouldReturnTrueAndMatchUnknownEnum()
        {
            // Arrange
            string inputString = null;
            
            // Act
            bool success = StringValueHelper.TryParseString<TestInstructionType>(inputString, out TestInstructionType result);
            
            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(TestInstructionType.Unknown, result);
        }

        [Test]
        public void TryParseString_WithEmptyStringValue_ShouldReturnFalseAndDefaultEnum()
        {
            // Arrange
            string inputString = "";
            
            // Act
            bool success = StringValueHelper.TryParseString<TestInstructionType>(inputString, out TestInstructionType result);
            
            // Assert
            Assert.IsFalse(success);
            Assert.AreEqual(default(TestInstructionType), result);
        }

        [Test]
        public void TryParseString_CaseSensitive_ShouldReturnFalseForWrongCase()
        {
            // Arrange
            string inputString = "ADD"; // Wrong case
            
            // Act
            bool success = StringValueHelper.TryParseString<TestInstructionType>(inputString, out TestInstructionType result);
            
            // Assert
            Assert.IsFalse(success);
            Assert.AreEqual(default(TestInstructionType), result);
        }

        [Test]
        public void StringValue_RoundTrip_ShouldPreserveValues()
        {
            // Test that converting enum -> string -> enum preserves the original value
            var originalEnum = TestInstructionType.Multiply;
            
            // Enum -> String
            string stringValue = originalEnum.GetStringValue();
            
            // String -> Enum
            bool success = StringValueHelper.TryParseString<TestInstructionType>(stringValue, out TestInstructionType roundTripEnum);
            
            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(originalEnum, roundTripEnum);
        }
    }
} 