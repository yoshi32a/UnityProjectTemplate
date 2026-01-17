using System;
using Master.Exceptions;
using Master.TypeParsers;
using NUnit.Framework;
using Unity.Mathematics;

namespace Master.Tests
{
    public class TypeParserTests
    {
        TypeParserRegistry registry;
        ParseContext context;

        [SetUp]
        public void Setup()
        {
            registry = new TypeParserRegistry();
            context = new ParseContext("test_table", 1, "test_column");
        }

        [Test]
        public void Parse_Int_ValidInput()
        {
            var result = registry.Parse(typeof(int), "42", context);
            Assert.AreEqual(42, result);
        }

        [Test]
        public void Parse_Int_NegativeValue()
        {
            var result = registry.Parse(typeof(int), "-123", context);
            Assert.AreEqual(-123, result);
        }

        [Test]
        public void Parse_Int_InvalidInput_ThrowsException()
        {
            Assert.Throws<MasterParseException>(() =>
                registry.Parse(typeof(int), "abc", context));
        }

        [Test]
        public void Parse_Float_ValidInput()
        {
            var result = registry.Parse(typeof(float), "3.14", context);
            Assert.AreEqual(3.14f, result);
        }

        [Test]
        public void Parse_Float_IntegerFormat()
        {
            var result = registry.Parse(typeof(float), "42", context);
            Assert.AreEqual(42f, result);
        }

        [Test]
        public void Parse_Bool_True()
        {
            var result = registry.Parse(typeof(bool), "true", context);
            Assert.AreEqual(true, result);
        }

        [Test]
        public void Parse_Bool_One()
        {
            var result = registry.Parse(typeof(bool), "1", context);
            Assert.AreEqual(true, result);
        }

        [Test]
        public void Parse_Bool_Zero()
        {
            var result = registry.Parse(typeof(bool), "0", context);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void Parse_String_ReturnsAsIs()
        {
            var result = registry.Parse(typeof(string), "hello world", context);
            Assert.AreEqual("hello world", result);
        }

        [Test]
        public void Parse_IntArray_ValidInput()
        {
            var result = registry.Parse(typeof(int[]), "1,2,3", context);
            Assert.AreEqual(new[] { 1, 2, 3 }, result);
        }

        [Test]
        public void Parse_IntArray_EmptyInput_ReturnsEmptyArray()
        {
            var result = registry.Parse(typeof(int[]), "", context);
            Assert.AreEqual(Array.Empty<int>(), result);
        }

        [Test]
        public void Parse_IntArray_SingleElement()
        {
            var result = registry.Parse(typeof(int[]), "42", context);
            Assert.AreEqual(new[] { 42 }, result);
        }

        [Test]
        public void Parse_IntArray_InvalidElement_ThrowsException()
        {
            Assert.Throws<MasterParseException>(() =>
                registry.Parse(typeof(int[]), "1,abc,3", context));
        }

        [Test]
        public void Parse_FloatArray_ValidInput()
        {
            var result = registry.Parse(typeof(float[]), "1.5,2.5,3.5", context);
            Assert.AreEqual(new[] { 1.5f, 2.5f, 3.5f }, result);
        }

        [Test]
        public void Parse_FloatArray_EmptyInput_ReturnsEmptyArray()
        {
            var result = registry.Parse(typeof(float[]), "", context);
            Assert.AreEqual(Array.Empty<float>(), result);
        }

        [Test]
        public void Parse_Float3_ValidInput()
        {
            var result = registry.Parse(typeof(float3), "1.0,2.0,3.0", context);
            Assert.AreEqual(new float3(1f, 2f, 3f), result);
        }

        [Test]
        public void Parse_Float3_WithSpaces()
        {
            var result = registry.Parse(typeof(float3), "1.0, 2.0, 3.0", context);
            Assert.AreEqual(new float3(1f, 2f, 3f), result);
        }

        [Test]
        public void Parse_Float3_WithQuotes()
        {
            var result = registry.Parse(typeof(float3), "\"1.0,2.0,3.0\"", context);
            Assert.AreEqual(new float3(1f, 2f, 3f), result);
        }

        [Test]
        public void Parse_Float3_EmptyInput_ThrowsException()
        {
            Assert.Throws<MasterParseException>(() =>
                registry.Parse(typeof(float3), "", context));
        }

        [Test]
        public void Parse_Float3_TwoComponents_ThrowsException()
        {
            Assert.Throws<MasterParseException>(() =>
                registry.Parse(typeof(float3), "1.0,2.0", context));
        }

        [Test]
        public void Parse_Float3_InvalidComponent_ThrowsException()
        {
            Assert.Throws<MasterParseException>(() =>
                registry.Parse(typeof(float3), "1.0,abc,3.0", context));
        }

        [Test]
        public void Parse_Enum_ByName()
        {
            var result = registry.Parse(typeof(TestEnum), "Value2", context);
            Assert.AreEqual(TestEnum.Value2, result);
        }

        [Test]
        public void Parse_Enum_ByNumber()
        {
            var result = registry.Parse(typeof(TestEnum), "1", context);
            Assert.AreEqual(TestEnum.Value2, result);
        }

        [Test]
        public void Parse_Enum_CaseInsensitive()
        {
            var result = registry.Parse(typeof(TestEnum), "VALUE2", context);
            Assert.AreEqual(TestEnum.Value2, result);
        }

        [Test]
        public void Parse_Enum_InvalidValue_ThrowsException()
        {
            var ex = Assert.Throws<MasterParseException>(() =>
                registry.Parse(typeof(TestEnum), "InvalidValue", context));

            // エラーメッセージに有効な値が含まれていることを確認
            Assert.That(ex.Message, Does.Contain("Value1"));
            Assert.That(ex.Message, Does.Contain("Value2"));
        }

        [Test]
        public void Parse_NullableInt_ValidInput()
        {
            var result = registry.Parse(typeof(int?), "42", context);
            Assert.AreEqual(42, result);
        }

        [Test]
        public void Parse_NullableInt_EmptyInput_ReturnsNull()
        {
            var result = registry.Parse(typeof(int?), "", context);
            Assert.IsNull(result);
        }

        [Test]
        public void Parse_NullableInt_WhitespaceInput_ReturnsNull()
        {
            var result = registry.Parse(typeof(int?), "   ", context);
            Assert.IsNull(result);
        }

        [Test]
        public void Parse_Error_ContainsTableName()
        {
            var ctx = new ParseContext("my_table", 5, "my_column");
            var ex = Assert.Throws<MasterParseException>(() =>
                registry.Parse(typeof(int), "invalid", ctx));

            Assert.AreEqual("my_table", ex.TableName);
            Assert.AreEqual(5, ex.RowNumber);
            Assert.AreEqual("my_column", ex.ColumnName);
        }

        enum TestEnum
        {
            Value1 = 0,
            Value2 = 1,
            Value3 = 2
        }
    }
}
