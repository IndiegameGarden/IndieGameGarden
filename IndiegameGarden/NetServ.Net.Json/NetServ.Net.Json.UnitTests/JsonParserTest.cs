using System;
using System.IO;
using NetServ.Net.Json;
using Vts.UnitTest;

namespace NetServ.Net.Json.UnitTests
{
    [TestFixture()]
    public class JsonParserTest
    {
        /// <summary>
        ///A test for JsonParser (TextReader, bool)
        ///</summary>
        [Test()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorTest() {

            JsonParser parser = new JsonParser(null, false);
        }

        /// <summary>
        ///A test for JsonParser (TextReader, bool)
        ///</summary>
        [Test()]
        public void ConstructorTest1() {

            JsonParser parser = new JsonParser(TextReader.Null, false);
        }

        /// <summary>
        ///A test for Close ()
        ///</summary>
        [Test()]
        public void CloseTest() {

            StringReader rdr = new StringReader("test");
            JsonParser parser = new JsonParser(rdr, false);

            parser.Close();
            string s = rdr.ReadLine();
        }

        /// <summary>
        ///A test for Close ()
        ///</summary>
        [Test()]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CloseTest1() {

            StringReader rdr = new StringReader("test");
            JsonParser parser = new JsonParser(rdr, true);

            parser.Close();
            string s = rdr.ReadLine();
        }

        /// <summary>
        ///A test for MaximumDepth
        ///</summary>
        [Test()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MaximumDepthTest() {

            JsonParser parser = new JsonParser(TextReader.Null, false);

            parser.MaximumDepth = -1;
        }

        /// <summary>
        ///A test for MaximumDepth
        ///</summary>
        [Test()]
        public void MaximumDepthTest2() {

            JsonParser parser = new JsonParser(TextReader.Null, false);

            parser.MaximumDepth = int.MaxValue;
        }

        /// <summary>
        ///A test for MaximumDepth
        ///</summary>
        [Test()]
        [ExpectedException(typeof(FormatException))]
        public void MaximumDepthTest3() {

            JsonParser parser = new JsonParser(new StringReader("[[[[[[[[[[10]]]]]]]]]]"), false);

            parser.MaximumDepth = 9;
            JsonArray arr = parser.ParseArray();
        }

        /// <summary>
        ///A test for MaximumDepth
        ///</summary>
        [Test()]        
        public void MaximumDepthTest4() {

            JsonParser parser = new JsonParser(new StringReader("[[[[[[[[[[10]]]]]]]]]]"), false);

            parser.MaximumDepth = 10;
            JsonArray arr = parser.ParseArray();
        }

        /// <summary>
        ///A test for NextToken ()
        ///</summary>
        [Test()]
        public void NextTokenTest() {

            JsonParser parser = new JsonParser(TextReader.Null, false);

            Assertion.IsTrue(parser.NextToken() == JsonParser.TokenType.EOF);
        }

        /// <summary>
        ///A test for ParseArray ()
        ///</summary>
        [Test()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ParseArrayTest() {

            JsonParser parser = new JsonParser(TextReader.Null, false);

            JsonArray arr = parser.ParseArray();
        }

        /// <summary>
        ///A test for ParseArray ()
        ///</summary>
        [Test()]
        public void ParseArrayTest1() {

            JsonParser parser = new JsonParser(new StringReader("[]"), true);

            JsonArray arr = parser.ParseArray();
            Assertion.IsNotNull(arr);
            Assertion.IsTrue(arr.Count == 0);
        }

        /// <summary>
        ///A test for ParseArray ()
        ///</summary>
        [Test()]
        public void ParseArrayTest2() {

            JsonParser parser = new JsonParser(new StringReader("[1,2,3,4,5,6,7,8,9,10]"), true);

            JsonArray arr = parser.ParseArray();
            Assertion.IsNotNull(arr);
            Assertion.IsTrue(arr.Count == 10);
        }

        /// <summary>
        ///A test for ParseArray ()
        ///</summary>
        [Test()]
        [ExpectedException(typeof(FormatException))]
        public void ParseArrayTest3() {

            JsonParser parser = new JsonParser(new StringReader("[1,]"), true);

            JsonArray arr = parser.ParseArray();            
        }

        /// <summary>
        ///A test for ParseArray ()
        ///</summary>
        [Test()]
        [ExpectedException(typeof(FormatException))]
        public void ParseArrayTest4() {

            JsonParser parser = new JsonParser(new StringReader("[1,,]"), true);

            JsonArray arr = parser.ParseArray();
        }

        /// <summary>
        ///A test for ParseArray ()
        ///</summary>
        [Test()]
        [ExpectedException(typeof(FormatException))]
        public void ParseArrayTest5() {

            JsonParser parser = new JsonParser(new StringReader("["), true);

            JsonArray arr = parser.ParseArray();
        }

        /// <summary>
        ///A test for ParseArray ()
        ///</summary>
        [Test()]
        [ExpectedException(typeof(FormatException))]
        public void ParseArrayTest6() {

            JsonParser parser = new JsonParser(new StringReader("[1"), true);

            JsonArray arr = parser.ParseArray();
        }

        /// <summary>
        ///A test for ParseBoolean ()
        ///</summary>
        [Test()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ParseBooleanTest() {

            JsonParser parser = new JsonParser(TextReader.Null, false);

            JsonBoolean b = parser.ParseBoolean();
        }

        /// <summary>
        ///A test for ParseBoolean ()
        ///</summary>
        [Test()]
        public void ParseBooleanTest1() {

            JsonParser parser = new JsonParser(new StringReader("truefalse"), false);

            JsonBoolean b = parser.ParseBoolean();
            Assertion.IsNotNull(b);
            Assertion.IsTrue(b.Value);
            b = parser.ParseBoolean();
            Assertion.IsNotNull(b);
            Assertion.IsFalse(b.Value);
        }

        /// <summary>
        ///A test for ParseBoolean ()
        ///</summary>
        [Test()]
        [ExpectedException(typeof(FormatException))]
        public void ParseBooleanTest2() {

            JsonParser parser = new JsonParser(new StringReader("tru1e"), false);

            JsonBoolean b = parser.ParseBoolean();            
        }

        /// <summary>
        ///A test for ParseBoolean ()
        ///</summary>
        [Test()]
        [ExpectedException(typeof(FormatException))]
        public void ParseBooleanTest3() {

            JsonParser parser = new JsonParser(new StringReader("fal3e"), false);

            JsonBoolean b = parser.ParseBoolean();
        }

        /// <summary>
        ///A test for ParseNull ()
        ///</summary>
        [Test()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ParseNullTest() {

            JsonParser parser = new JsonParser(TextReader.Null, false);

            JsonNull n = parser.ParseNull();
        }

        /// <summary>
        ///A test for ParseNull ()
        ///</summary>
        [Test()]        
        public void ParseNullTest1() {

            JsonParser parser = new JsonParser(new StringReader("null"), false);

            JsonNull n = parser.ParseNull();

            Assertion.IsNotNull(n);
        }

        /// <summary>
        ///A test for ParseNull ()
        ///</summary>
        [Test()]
        [ExpectedException(typeof(FormatException))]
        public void ParseNullTest2() {

            JsonParser parser = new JsonParser(new StringReader("nu4ll"), false);

            JsonNull n = parser.ParseNull();            
        }

        /// <summary>
        ///A test for ParseNumber ()
        ///</summary>
        [Test()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ParseNumberTest() {

            JsonParser parser = new JsonParser(TextReader.Null, false);

            JsonNumber arr = parser.ParseNumber();
        }

        /// <summary>
        ///A test for ParseNumber ()
        ///</summary>
        [Test()]        
        public void ParseNumberTest1() {

            JsonParser parser = new JsonParser(new StringReader("123e-6"), false);

            JsonNumber n = parser.ParseNumber();

            Assertion.IsNotNull(n);
            Assertion.IsTrue(n.Equals(123e-6));            
        }

        /// <summary>
        ///A test for ParseNumber ()
        ///</summary>
        [Test()]
        public void ParseNumberTest2() {

            JsonParser parser = new JsonParser(new StringReader("-123"), false);

            JsonNumber n = parser.ParseNumber();

            Assertion.IsNotNull(n);
            Assertion.IsTrue(n.Equals(-123));
        }

        /// <summary>
        ///A test for ParseNumber ()
        ///</summary>
        [Test()]
        public void ParseNumberTest3() {

            JsonParser parser = new JsonParser(new StringReader("0"), false);

            JsonNumber n = parser.ParseNumber();

            Assertion.IsNotNull(n);
            Assertion.IsTrue(n.Equals(0));
        }

        /// <summary>
        ///A test for ParseNumber ()
        ///</summary>
        [Test()]
        [ExpectedException(typeof(FormatException))]
        public void ParseNumberTest4() {

            JsonParser parser = new JsonParser(new StringReader("12e*6"), false);

            JsonNumber n = parser.ParseNumber();            
        }

        /// <summary>
        ///A test for ParseNumber ()
        ///</summary>
        [Test()]
        [ExpectedException(typeof(FormatException))]
        public void ParseNumberTest5() {

            JsonParser parser = new JsonParser(new StringReader("*123"), false);

            JsonNumber n = parser.ParseNumber();
        }

        /// <summary>
        ///A test for ParseObject ()
        ///</summary>
        [Test()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ParseObjectTest() {

            JsonParser parser = new JsonParser(TextReader.Null, false);

            JsonObject arr = parser.ParseObject();
        }

        /// <summary>
        ///A test for ParseObject ()
        ///</summary>
        [Test()]
        public void ParseObjectTest1() {

            JsonParser parser = new JsonParser(new StringReader("{}"), true);

            JsonObject obj = parser.ParseObject();

            Assertion.IsTrue(obj.Count == 0);
        }

        /// <summary>
        ///A test for ParseObject ()
        ///</summary>
        [Test()]
        public void ParseObjectTest2() {

            JsonParser parser = new JsonParser(new StringReader("{\"1\":1,\"true\":true,\"text\":\"text\"}"), true);

            JsonObject obj = parser.ParseObject();

            Assertion.IsTrue(obj.Count == 3);
            Assertion.IsTrue(obj["1"].JsonTypeCode == JsonTypeCode.Number);
            Assertion.IsTrue(((JsonNumber)obj["1"]).Equals(1D));
            Assertion.IsTrue(obj["true"].JsonTypeCode == JsonTypeCode.Boolean);
            Assertion.IsTrue(((JsonBoolean)obj["true"]).Equals(true));
            Assertion.IsTrue(obj["text"].JsonTypeCode == JsonTypeCode.String);
            Assertion.IsTrue(((JsonString)obj["text"]).Equals("text"));
        }

        /// <summary>
        ///A test for ParseObject ()
        ///</summary>
        [Test()]
        [ExpectedException(typeof(FormatException))]
        public void ParseObjectTest3() {

            JsonParser parser = new JsonParser(new StringReader("{,}"), true);

            JsonObject obj = parser.ParseObject(); 
        }

        /// <summary>
        ///A test for ParseObject ()
        ///</summary>
        [Test()]
        [ExpectedException(typeof(FormatException))]
        public void ParseObjectTest4() {

            JsonParser parser = new JsonParser(new StringReader("{key}"), true);

            JsonObject obj = parser.ParseObject();
        }

        /// <summary>
        ///A test for ParseObject ()
        ///</summary>
        [Test()]
        [ExpectedException(typeof(FormatException))]
        public void ParseObjectTest5() {

            JsonParser parser = new JsonParser(new StringReader("{\"key\"}"), true);

            JsonObject obj = parser.ParseObject();
        }

        /// <summary>
        ///A test for ParseObject ()
        ///</summary>
        [Test()]
        [ExpectedException(typeof(FormatException))]
        public void ParseObjectTest6() {

            JsonParser parser = new JsonParser(new StringReader("{\"key\":,}"), true);

            JsonObject obj = parser.ParseObject();
        }

        /// <summary>
        ///A test for ParseObject ()
        ///</summary>
        [Test()]
        [ExpectedException(typeof(FormatException))]
        public void ParseObjectTest7() {

            JsonParser parser = new JsonParser(new StringReader("{\"key\":\"value\",}"), true);

            JsonObject obj = parser.ParseObject();
        }

        /// <summary>
        ///A test for ParseString ()
        ///</summary>
        [Test()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ParseStringTest() {

            JsonParser parser = new JsonParser(TextReader.Null, false);

            JsonString arr = parser.ParseString();
        }

        /// <summary>
        ///A test for ParseString ()
        ///</summary>
        [Test()]
        public void ParseStringTest1() {

            JsonParser parser = new JsonParser(new StringReader("\"\"\"test\"\"\\r\\n\\t\\b\\\\\\/\""), true);

            JsonString s = parser.ParseString();

            Assertion.IsTrue(s.Equals(""));
            s = parser.ParseString();
            Assertion.IsTrue(s.Equals("test"));
            s = parser.ParseString();
            Assertion.IsTrue(s.Equals("\r\n\t\b\\/"));
        }

        /// <summary>
        ///A test for ParseString ()
        ///</summary>
        [Test()]
        [ExpectedException(typeof(FormatException))]
        public void ParseStringTest2() {

            JsonParser parser = new JsonParser(new StringReader("\""), true);

            JsonString s = parser.ParseString();
        }

        /// <summary>
        ///A test for ParseString ()
        ///</summary>
        [Test()]
        [ExpectedException(typeof(FormatException))]
        public void ParseStringTest3() {

            JsonParser parser = new JsonParser(new StringReader("\"\\w\""), true);

            JsonString s = parser.ParseString();
        }

        /// <summary>
        ///A test for ParseString ()
        ///</summary>
        [Test()]
        [ExpectedException(typeof(FormatException))]
        public void ParseStringTest4() {

            JsonParser parser = new JsonParser(new StringReader("\"\\uZSAW\""), true);

            JsonString s = parser.ParseString();
        }

        /// <summary>
        ///A test for ParseString ()
        ///</summary>
        [Test()]
        [ExpectedException(typeof(FormatException))]
        public void ParseStringTest5() {

            JsonParser parser = new JsonParser(new StringReader("\"\\u\""), true);

            JsonString s = parser.ParseString();
        }
    }
}
