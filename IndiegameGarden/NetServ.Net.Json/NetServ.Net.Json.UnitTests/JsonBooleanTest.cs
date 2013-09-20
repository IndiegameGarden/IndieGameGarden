using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NetServ.Net.Json;
using Vts.UnitTest;

namespace NetServ.Net.Json.UnitTests
{    
    [TestFixture()]
    public class JsonBooleanTest
    {                
        [Test()]
        [Sequence(0)]
        public void GetTest() {

            JsonBoolean f = JsonBoolean.False;            

            Assertion.IsNotNull(f);
            Assertion.IsFalse(f.Value);

            JsonBoolean t = JsonBoolean.True;

            Assertion.IsNotNull(t);
            Assertion.IsTrue(t.Value);            
        }

        [Test()]
        [Sequence(1)]
        public void EqualsTest() {

            JsonBoolean f = JsonBoolean.False;            

            Assertion.IsTrue(f.Equals(false));
            Assertion.IsFalse(f.Equals(true));

            JsonBoolean t = JsonBoolean.True;

            Assertion.IsTrue(t.Equals(true));
            Assertion.IsFalse(t.Equals(false));
        }

        /// <summary>
        ///A test for Equals (IJsonBoolean)
        ///</summary>
        [Test()]
        [Sequence(2)]
        public void EqualsTest1() {

            JsonBoolean f = JsonBoolean.False;
            JsonBoolean t = JsonBoolean.True;

            Assertion.IsFalse(f.Equals((IJsonBoolean)null));

            Assertion.IsTrue(f.Equals((IJsonBoolean)f));
            Assertion.IsFalse(f.Equals((IJsonBoolean)t));

            Assertion.IsTrue(t.Equals((IJsonBoolean)t));
            Assertion.IsFalse(t.Equals((IJsonBoolean)f));
        }

        /// <summary>
        ///A test for Equals (JsonBoolean)
        ///</summary>
        [Test()]
        [Sequence(3)]
        public void EqualsTest2() {

            JsonBoolean f = JsonBoolean.False;
            JsonBoolean t = JsonBoolean.True;

            Assertion.IsFalse(f.Equals(null));

            Assertion.IsTrue(f.Equals(f));
            Assertion.IsFalse(f.Equals(t));

            Assertion.IsTrue(t.Equals(t));
            Assertion.IsFalse(t.Equals(f));
        }

        /// <summary>
        ///A test for Equals (JsonBoolean, JsonBoolean)
        ///</summary>
        [Test()]
        [Sequence(4)]
        public void EqualsTest3() {

            JsonBoolean t = null;
            JsonBoolean f = null;

            Assertion.IsTrue(JsonBoolean.Equals(t, f));

            t = JsonBoolean.True;

            Assertion.IsFalse(JsonBoolean.Equals(t, f));
            Assertion.IsFalse(JsonBoolean.Equals(f, t));

            f = JsonBoolean.False;

            Assertion.IsFalse(JsonBoolean.Equals(t, f));
            Assertion.IsFalse(JsonBoolean.Equals(f, t));

            Assertion.IsTrue(JsonBoolean.Equals(t, t));
            Assertion.IsTrue(JsonBoolean.Equals(f, f));
        }


        /// <summary>
        ///A test for explicit operator (JsonNumber)
        ///</summary>
        [Test()]
        [Sequence(5)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConversionTestNullException() {

            JsonBoolean value = null;

            bool b = (bool)value;            
        }

        /// <summary>
        ///A test for explicit operator (JsonBoolean)
        ///</summary>
        [Test()]
        [Sequence(5)]
        public void ConversionTest() {

            JsonBoolean t = JsonBoolean.True;
            JsonBoolean f = JsonBoolean.False;

            Assertion.IsTrue((bool)t);
            Assertion.IsFalse((bool)f);
        }

        [Test()]
        [Sequence(6)]
        public void GetHashCodeTest() {

            JsonBoolean t = JsonBoolean.True;
            JsonBoolean f = JsonBoolean.False;

            Assertion.IsTrue(t.GetHashCode() == t.GetHashCode());
            Assertion.IsTrue(f.GetHashCode() == f.GetHashCode());

            Assertion.IsFalse(t.GetHashCode() == f.GetHashCode());
        }

        /// <summary>
        ///A test for implicit operator (bool)
        ///</summary>
        [Test()]
        [Sequence(7)]
        public void ConversionTest1() {

            JsonBoolean t = true;
            JsonBoolean f = false;

            Assertion.IsNotNull(t);
            Assertion.IsTrue(t.Value);

            Assertion.IsNotNull(f);
            Assertion.IsFalse(f.Value);
        }

        /// <summary>
        ///A test for implicit operator (JsonNull)
        ///</summary>
        [Test()]
        [Sequence(8)]
        public void ConversionTest2() {

            JsonBoolean value = JsonNull.Null;

            Assertion.IsNull(value);
        }

        /// <summary>
        ///A test for operator != (JsonBoolean, JsonBoolean)
        ///</summary>
        [Test()]
        [Sequence(9)]
        public void InequalityTest() {

            JsonBoolean t = JsonBoolean.True;
            JsonBoolean f = JsonBoolean.False;

            Assertion.IsTrue(t != null);            
            Assertion.IsTrue(t != f);
            Assertion.IsFalse(t != t);

            Assertion.IsTrue(f != null);
            Assertion.IsTrue(f != t);
            Assertion.IsFalse(f != f);
        }

        /// <summary>
        ///A test for operator == (JsonBoolean, JsonBoolean)
        ///</summary>
        [Test()]
        [Sequence(10)]
        public void EqualityTest() {

            JsonBoolean t = JsonBoolean.True;
            JsonBoolean f = JsonBoolean.False;
            
            Assertion.IsFalse(t == null);
            Assertion.IsTrue(t == t);
            Assertion.IsFalse(t == f);

            Assertion.IsFalse(f == null);
            Assertion.IsTrue(f == f);
            Assertion.IsFalse(f == t);
        }

        /// <summary>
        ///A test for ToString ()
        ///</summary>
        [Test()]
        [Sequence(11)]
        public void ToStringTest() {

            JsonBoolean t = JsonBoolean.True;
            JsonBoolean f = JsonBoolean.False;

            Assertion.IsNotNull(t.ToString());
            Assertion.IsFalse(t.ToString().Trim().Length == 0);
            Assertion.IsTrue(t.ToString() == t.ToString());
            Assertion.IsTrue(t.ToString() == JsonBoolean.TrueString);

            Assertion.IsNotNull(f.ToString());
            Assertion.IsFalse(f.ToString().Trim().Length == 0);
            Assertion.IsTrue(f.ToString() == f.ToString());
            Assertion.IsTrue(f.ToString() == JsonBoolean.FalseString);

            Assertion.IsTrue(f.ToString() != t.ToString());
        }

        /// <summary>
        ///A test for Value
        ///</summary>
        [Test()]
        [Sequence(12)]
        public void ValueTest() {

            JsonBoolean t = JsonBoolean.True;
            JsonBoolean f = JsonBoolean.False;

            Assertion.IsTrue(t.Value);
            Assertion.IsFalse(f.Value);
        }

        /// <summary>
        ///A test for Write (IJsonWriter)
        ///</summary>
        [Test()]
        [Sequence(13)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteTestNull() {

            JsonBoolean t = JsonBoolean.True;

            t.Write(null);          
        }

        [Test()]
        [Sequence(14)]
        public void JsonTypeCodeTest() {

            JsonBoolean t = JsonBoolean.True;

            Assertion.IsTrue(t.JsonTypeCode == JsonTypeCode.Boolean);
        }

        [Test()]
        [Sequence(15)]
        public void SeializationTest() {

            JsonBoolean t = JsonBoolean.True;
            BinaryFormatter fmt = new BinaryFormatter();

            using(MemoryStream ms = new MemoryStream()) {
                fmt.Serialize(ms, t);
                ms.Position = 0;

                JsonBoolean dt = (JsonBoolean)fmt.Deserialize(ms);

                Assertion.IsNotNull(dt);
                Assertion.IsTrue(t.Value == dt.Value);
            }            
        }
    }
}
