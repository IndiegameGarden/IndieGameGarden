using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using NetServ.Net.Json;
using Vts.UnitTest;

namespace NetServ.Net.Json.UnitTests
{
    [TestFixture()]
    public class JsonStringTest
    {
        /// <summary>
        ///A test for JsonString (string)
        ///</summary>
        [Test()]
        [Sequence(0)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorTest() {

            JsonString s = new JsonString(null);
        }

        /// <summary>
        ///A test for JsonString (string)
        ///</summary>
        [Test()]
        [Sequence(1)]
        public void ConstructorTest2() {

            string value = "hello test";
            JsonString s = new JsonString(value);

            Assertion.IsTrue(s.Value == value);
        }

        /// <summary>
        ///A test for Value
        ///</summary>
        [Test()]
        [Sequence(2)]
        public void ValueTest() {

            string value = "hello test";
            JsonString s = new JsonString(value);

            Assertion.IsTrue(s.Value == value);
        }

        /// <summary>
        ///A test for CompareTo (IJsonString)
        ///</summary>
        [Test()]
        public void CompareToTest() {            

            JsonString s = new JsonString("abc");
            IJsonString other = null;

            Assertion.IsTrue(s.CompareTo(other) != 0);
            other = new JsonString("zxy");
            Assertion.IsTrue(s.CompareTo(other) < 0);
            s = new JsonString("zzzz");
            Assertion.IsTrue(s.CompareTo(other) > 0);
        }

        /// <summary>
        ///A test for CompareTo (JsonString)
        ///</summary>
        [Test()]
        public void CompareToTest1() {

            JsonString s = new JsonString("abc");
            JsonString other = null;

            Assertion.IsTrue(s.CompareTo(other) != 0);
            other = new JsonString("zxy");
            Assertion.IsTrue(s.CompareTo(other) < 0);
            s = new JsonString("zzzz");
            Assertion.IsTrue(s.CompareTo(other) > 0);
        }

        /// <summary>
        ///A test for CompareTo (string)
        ///</summary>
        [Test()]
        public void CompareToTest2() {

            JsonString s = new JsonString("abc");
            string other = null;

            Assertion.IsTrue(s.CompareTo(other) != 0);
            other = "zxy";
            Assertion.IsTrue(s.CompareTo(other) < 0);
            s = "zzzz";
            Assertion.IsTrue(s.CompareTo(other) > 0);
        }

        /// <summary>
        ///A test for Encode (string)
        ///</summary>
        [Test()]
        public void EncodeTest() {

            string value = "\u0123\u4567\u89AB\uCDEF\uabcd\uef4A\r\n\b\t\\/";
            string result = @"""\u0123\u4567\u89AB\uCDEF\uABCD\uEF4A\r\n\b\t\\\/""";

            Assertion.IsTrue(JsonString.Encode(value) == result);
        }

        /// <summary>
        ///A test for Encode (string)
        ///</summary>
        [Test()]
        public void EncodeTest1() {

            string value = "shouldn't be encoded.";
            string result = @"""shouldn't be encoded.""";

            Assertion.IsTrue(JsonString.Encode(value) == result);
        }

        /// <summary>
        ///A test for EncodedValue
        ///</summary>
        [Test()]
        public void EncodedValueTest() {

            string value = "\u0123\u4567\u89AB\uCDEF\uabcd\uef4A\r\n\b\t\\/";
            string result = @"""\u0123\u4567\u89AB\uCDEF\uABCD\uEF4A\r\n\b\t\\\/""";

            JsonString s = new JsonString(value);

            Assertion.IsTrue(s.EncodedValue == result);
        }

        /// <summary>
        ///A test for EncodedValue
        ///</summary>
        [Test()]
        public void EncodedValueTest2() {

            string value = "shouldn't be encoded.";
            string result = @"""shouldn't be encoded.""";

            JsonString s = new JsonString(value);

            Assertion.IsTrue(s.EncodedValue == result);
        }

        /// <summary>
        ///A test for Equals (IJsonString)
        ///</summary>
        [Test()]
        public void EqualsTest() {

            string value = "test text";

            JsonString s = new JsonString(value);
            IJsonString other = null;

            Assertion.IsFalse(s.Equals(other));
            other = new JsonString(value + " and more");
            Assertion.IsFalse(s.Equals(other));
            other = new JsonString(value);
            Assertion.IsTrue(s.Equals(other));
        }

        /// <summary>
        ///A test for Equals (JsonString)
        ///</summary>
        [Test()]
        public void EqualsTest1() {

            string value = "test text";

            JsonString s = new JsonString(value);
            JsonString other = null;

            Assertion.IsFalse(s.Equals(other));
            other = new JsonString(value + " and more");
            Assertion.IsFalse(s.Equals(other));
            other = new JsonString(value);
            Assertion.IsTrue(s.Equals(other));
        }

        /// <summary>
        ///A test for Equals (JsonString, JsonString)
        ///</summary>
        [Test()]
        public void EqualsTest2() {

            JsonString a = null;
            JsonString b = null;

            Assertion.IsTrue(JsonString.Equals(a, b));
            a = new JsonString("test");
            Assertion.IsFalse(JsonString.Equals(a, b));
            Assertion.IsFalse(JsonString.Equals(b, a));
            Assertion.IsTrue(JsonString.Equals(a, a));
            b = new JsonString("test");
            Assertion.IsTrue(JsonString.Equals(a, b));
            b = new JsonString("testing");
            Assertion.IsFalse(JsonString.Equals(a, b));
        }

        /// <summary>
        ///A test for Equals (object)
        ///</summary>
        [Test()]
        public void EqualsTest3() {

            JsonString s = new JsonString("test");
            object os = null;

            Assertion.IsFalse(s.Equals((object)null));
            Assertion.IsFalse(s.Equals(23));
            os = new JsonString("test");
            Assertion.IsTrue(s.Equals(os));
            os = new JsonString("testing");
            Assertion.IsFalse(s.Equals(os));
        }

        /// <summary>
        ///A test for Equals (string)
        ///</summary>
        [Test()]
        public void EqualsTest4() {

            JsonString s = new JsonString("test");
            string os = null;

            Assertion.IsFalse(s.Equals((string)null));
            Assertion.IsFalse(s.Equals(23));
            os = "test";
            Assertion.IsTrue(s.Equals(os));
            os = "testing";
            Assertion.IsFalse(s.Equals(os));
        }        

        /// <summary>
        ///A test for explicit operator (JsonString)
        ///</summary>
        [Test()]        
        public void ConversionTest1() {

            JsonString s = new JsonString("test");
            string value = (string)s;

            Assertion.IsNotNull(value);
            Assertion.IsTrue(s.Value == value);
        } 

        /// <summary>
        ///A test for implicit operator (JsonNull)
        ///</summary>
        [Test()]
        public void ConversionTest2() {

            JsonString s = JsonNull.Null;

            Assertion.IsNull(s);
        }

        /// <summary>
        ///A test for implicit operator (string)
        ///</summary>
        [Test()]
        public void ConversionTest3() {

            string value = "test";
            JsonString s = (string)null;

            Assertion.IsNull(s);
            s = value;
            Assertion.IsTrue(s.Value == value);
        }

        /// <summary>
        ///A test for GetHashCode ()
        ///</summary>
        [Test()]
        public void GetHashCodeTest() {

            string value = "testing";
            JsonString s1 = new JsonString(value);
            JsonString s2 = new JsonString(value + " some more");

            Assertion.IsTrue(s1.GetHashCode() == s1.GetHashCode());
            Assertion.IsFalse(s1.GetHashCode() == s2.GetHashCode());
            s2 = new JsonString(s1.Value);
            Assertion.IsTrue(s1.GetHashCode() == s2.GetHashCode());
        }

        /// <summary>
        ///A test for operator != (JsonString, JsonString)
        ///</summary>
        [Test()]
        public void InequalityTest() {

            JsonString a = null;
            JsonString b = null;

            Assertion.IsFalse(a != b);
            a = new JsonString("test");
            Assertion.IsTrue(a != b);
            b = new JsonString("test");
            Assertion.IsFalse(a != b);
            b = new JsonString("tssting");
            Assertion.IsTrue(a != b);
        }

        /// <summary>
        ///A test for operator == (JsonString, JsonString)
        ///</summary>
        [Test()]
        public void EqualityTest() {

            JsonString a = null;
            JsonString b = null;

            Assertion.IsTrue(a == b);
            a = new JsonString("test");
            Assertion.IsFalse(a == b);
            Assertion.IsFalse(b == a);
            b = new JsonString("test");
            Assertion.IsTrue(a == b);
            b = new JsonString("testing");
            Assertion.IsFalse(a == b);
        }

        /// <summary>
        ///A test for ToString ()
        ///</summary>
        [Test()]
        public void ToStringTest() {

            JsonString a = new JsonString("test");
            JsonString b = new JsonString("test");

            Assertion.IsNotNull(a.ToString());
            Assertion.IsNotNull(b.ToString());
            Assertion.IsTrue(a.ToString() == a.ToString());
            Assertion.IsTrue(a.ToString() == b.ToString());
        } 

        /// <summary>
        ///A test for Write (IJsonWriter)
        ///</summary>
        [Test()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteTestNull() {

            JsonString s = new JsonString("text");

            s.Write(null);
        }

        [Test()]
        public void JsonTypeCodeTest() {

            JsonString s = new JsonString("text");

            Assertion.IsTrue(s.JsonTypeCode == JsonTypeCode.String);
        }

        [Test()]
        public void SeializationTest() {

            JsonString s = new JsonString("this is some sez text.");
            BinaryFormatter fmt = new BinaryFormatter();

            using(MemoryStream ms = new MemoryStream()) {
                fmt.Serialize(ms, s);
                ms.Position = 0;

                JsonString ds = (JsonString)fmt.Deserialize(ms);

                Assertion.IsNotNull(ds);
                Assertion.IsTrue(s.Value == ds.Value);
            }
        }
    }
}
