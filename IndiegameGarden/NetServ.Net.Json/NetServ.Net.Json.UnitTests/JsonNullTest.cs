using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NetServ.Net.Json;
using Vts.UnitTest;

namespace NetServ.Net.Json.UnitTests
{
    [TestFixture()]
    public class JsonNullTest
    {
        /// <summary>
        ///A test for Equals (IJsonNull)
        ///</summary>
        [Test()]
        public void EqualsTest() {

            JsonNull n = JsonNull.Null;

            Assertion.IsFalse(n.Equals((IJsonNull)null));
            Assertion.IsTrue(n.Equals((IJsonNull)n));
        }

        /// <summary>
        ///A test for Equals (JsonNull)
        ///</summary>
        [Test()]
        public void EqualsTest1() {

            JsonNull n = JsonNull.Null;

            Assertion.IsFalse(n.Equals((JsonNull)null));
            Assertion.IsTrue(n.Equals(n));
        }

        /// <summary>
        ///A test for Equals (object)
        ///</summary>
        [Test()]
        public void EqualsTest2() {

            JsonNull n = JsonNull.Null;

            Assertion.IsFalse(n.Equals((object)null));
            Assertion.IsTrue(n.Equals((object)n));
        }

        /// <summary>
        ///A test for GetHashCode ()
        ///</summary>
        [Test()]
        public void GetHashCodeTest() {

            JsonNull n = JsonNull.Null;

            Assertion.IsTrue(n.GetHashCode() == n.GetHashCode());
        }        

        /// <summary>
        ///A test for ToString ()
        ///</summary>
        [Test()]
        public void ToStringTest() {

            JsonNull n = JsonNull.Null;

            Assertion.IsNotNull(n.ToString());
            Assertion.IsTrue(n.ToString() == JsonNull.NullString);
        }

        /// <summary>
        ///A test for Write (IJsonWriter)
        ///</summary>
        [Test()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteTestNull() {

            JsonNull n = JsonNull.Null;

            n.Write(null);
        }

        [Test()]
        public void JsonTypeCodeTest() {

            JsonNull n = JsonNull.Null;

            Assertion.IsTrue(n.JsonTypeCode == JsonTypeCode.Null);
        }

        [Test()]
        public void SeializationTest() {

            JsonNull n = JsonNull.Null;
            BinaryFormatter fmt = new BinaryFormatter();

            using(MemoryStream ms = new MemoryStream()) {
                fmt.Serialize(ms, n);
                ms.Position = 0;

                JsonNull dn = (JsonNull)fmt.Deserialize(ms);

                Assertion.IsNotNull(dn);
                Assertion.IsTrue(n.Equals(dn));
                // Ensure IObjectReference.
                Assertion.IsTrue(object.ReferenceEquals(n, dn));
            }
        }
    }
}
