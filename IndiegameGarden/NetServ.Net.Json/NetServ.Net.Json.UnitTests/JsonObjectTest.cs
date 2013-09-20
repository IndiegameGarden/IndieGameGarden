using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NetServ.Net.Json;
using Vts.UnitTest;

namespace NetServ.Net.Json.UnitTests
{    
    [TestFixture()]
    public class JsonObjectTest
    {
        /// <summary>
        ///A test for Add (string, bool)
        ///</summary>
        [Test()]
        public void AddTest() {
            JsonObject obj = new JsonObject();

            string key = "testkey";

            bool item = false;

            obj.Add(key, item);
            Assertion.IsTrue(obj.Count == 1);
            Assertion.IsTrue(obj.ContainsKey(key));
            Assertion.IsTrue(obj.ContainsValue(JsonBoolean.Get(item)));
            Assertion.IsTrue(((JsonBoolean)obj[key]).Equals(item));
        }

        /// <summary>
        ///A test for Add (string, byte)
        ///</summary>
        [Test()]
        public void AddTest1() {
            JsonObject obj = new JsonObject();

            string key = "testkey";

            byte item = 0;

            obj.Add(key, item);
            Assertion.IsTrue(obj.Count == 1);
            Assertion.IsTrue(obj.ContainsKey(key));
            Assertion.IsTrue(obj.ContainsValue(new JsonNumber(item)));
            Assertion.IsTrue(((JsonNumber)obj[key]).Equals(item));
        }

        /// <summary>
        ///A test for Add (string, double)
        ///</summary>
        [Test()]
        public void AddTest2() {
            JsonObject obj = new JsonObject();

            string key = "testkey";

            double item = 0;

            obj.Add(key, item);
            Assertion.IsTrue(obj.Count == 1);
            Assertion.IsTrue(obj.ContainsKey(key));
            Assertion.IsTrue(obj.ContainsValue(new JsonNumber(item)));
            Assertion.IsTrue(((JsonNumber)obj[key]).Equals(item));            
        }

        /// <summary>
        ///A test for Add (string, int)
        ///</summary>
        [Test()]
        public void AddTest3() {
            JsonObject obj = new JsonObject();

            string key = "testkey";

            int item = 0;

            obj.Add(key, item);
            Assertion.IsTrue(obj.Count == 1);
            Assertion.IsTrue(obj.ContainsKey(key));
            Assertion.IsTrue(obj.ContainsValue(new JsonNumber(item)));
            Assertion.IsTrue(((JsonNumber)obj[key]).Equals(item));            
        }

        /// <summary>
        ///A test for Add (string, long)
        ///</summary>
        [Test()]
        public void AddTest4() {
            JsonObject obj = new JsonObject();

            string key = "testkey";

            long item = 0;

            obj.Add(key, item);
            Assertion.IsTrue(obj.Count == 1);
            Assertion.IsTrue(obj.ContainsKey(key));
            Assertion.IsTrue(obj.ContainsValue(new JsonNumber(item)));
            Assertion.IsTrue(((JsonNumber)obj[key]).Equals(item));            
        }

        /// <summary>
        ///A test for Add (string, sbyte)
        ///</summary>
        [Test()]
        public void AddTest5() {
            JsonObject obj = new JsonObject();

            string key = "testkey";

            sbyte item = 0;

            obj.Add(key, item);
            Assertion.IsTrue(obj.Count == 1);
            Assertion.IsTrue(obj.ContainsKey(key));
            Assertion.IsTrue(obj.ContainsValue(new JsonNumber(item)));
            Assertion.IsTrue(((JsonNumber)obj[key]).Equals(item));            
        }

        /// <summary>
        ///A test for Add (string, short)
        ///</summary>
        [Test()]
        public void AddTest6() {
            JsonObject obj = new JsonObject();

            string key = "testkey";

            short item = 0;

            obj.Add(key, item);
            Assertion.IsTrue(obj.Count == 1);
            Assertion.IsTrue(obj.ContainsKey(key));
            Assertion.IsTrue(obj.ContainsValue(new JsonNumber(item)));
            Assertion.IsTrue(((JsonNumber)obj[key]).Equals(item));            
        }

        /// <summary>
        ///A test for Add (string, string)
        ///</summary>
        [Test()]
        public void AddTest7() {
            JsonObject obj = new JsonObject();

            string key = "testkey";

            string item = "string";

            obj.Add(key, item);
            Assertion.IsTrue(obj.Count == 1);
            Assertion.IsTrue(obj.ContainsKey(key));
            Assertion.IsTrue(obj.ContainsValue(new JsonString(item)));
            Assertion.IsTrue(((JsonString)obj[key]).Equals(item));
        }

        /// <summary>
        ///A test for Add (string, uint)
        ///</summary>
        [Test()]
        public void AddTest8() {
            JsonObject obj = new JsonObject();

            string key = "testkey";

            uint item = 0;

            obj.Add(key, item);
            Assertion.IsTrue(obj.Count == 1);
            Assertion.IsTrue(obj.ContainsKey(key));
            Assertion.IsTrue(obj.ContainsValue(new JsonNumber(item)));
            Assertion.IsTrue(((JsonNumber)obj[key]).Equals(item));            
        }

        /// <summary>
        ///A test for Add (string, ulong)
        ///</summary>
        [Test()]
        public void AddTest9() {
            JsonObject obj = new JsonObject();

            string key = "testkey";

            ulong item = 0;

            obj.Add(key, item);
            Assertion.IsTrue(obj.Count == 1);
            Assertion.IsTrue(obj.ContainsKey(key));
            Assertion.IsTrue(obj.ContainsValue(new JsonNumber(item)));
            Assertion.IsTrue(((JsonNumber)obj[key]).Equals(item));            
        }

        /// <summary>
        ///A test for Add (string, ushort)
        ///</summary>
        [Test()]
        public void AddTest10() {
            JsonObject obj = new JsonObject();

            string key = "testkey";

            ushort item = 0;

            obj.Add(key, item);
            Assertion.IsTrue(obj.Count == 1);
            Assertion.IsTrue(obj.ContainsKey(key));
            Assertion.IsTrue(obj.ContainsValue(new JsonNumber(item)));
            Assertion.IsTrue(((JsonNumber)obj[key]).Equals(item));            
        }

        /// <summary>
        ///A test for Add (string, JsonObject)
        ///</summary>
        [Test()]
        public void AddTest11() {
            JsonObject obj = new JsonObject();

            string key = "testkey";

            JsonObject item = new JsonObject();

            obj.Add(key, item);
            Assertion.IsTrue(obj.Count == 1);
            Assertion.IsTrue(obj.ContainsKey(key));
            Assertion.IsTrue(obj.ContainsValue(item));
            Assertion.IsTrue(obj[key] == item);
        }

        /// <summary>
        ///A test for Add (string, JsonArray)
        ///</summary>
        [Test()]
        public void AddTest12() {
            JsonObject obj = new JsonObject();

            string key = "testkey";

            JsonArray item = new JsonArray();

            obj.Add(key, item);
            Assertion.IsTrue(obj.Count == 1);
            Assertion.IsTrue(obj.ContainsKey(key));
            Assertion.IsTrue(obj.ContainsValue(item));
            Assertion.IsTrue(obj[key] == item);
        }

        [Test()]
        public void AddTestMulti() {

            JsonObject obj = new JsonObject();
            int add = 20;

            for(int i = 0; i < add; ++i)
                obj.Add(i.ToString(), new JsonNumber(i));
            Assertion.IsTrue(obj.Count == add);
            for(int i = 0; i < add; ++i) {
                Assertion.IsTrue(obj.ContainsKey(i.ToString()));
                Assertion.IsTrue(obj.ContainsValue(new JsonNumber(i)));
                Assertion.IsTrue(obj[i.ToString()].Equals(new JsonNumber(i)));
            }
        }

        /// <summary>
        ///A test for implicit operator (JsonNull)
        ///</summary>
        [Test()]
        public void ConversionTest() {

            JsonObject obj = JsonNull.Null;

            Assertion.IsNull(obj);
        }

        /// <summary>
        ///A test for JsonObject ()
        ///</summary>
        [Test()]
        public void ConstructorTest() {

            JsonObject obj = new JsonObject();

            Assertion.IsTrue(obj.Count == 0);
        }

        /// <summary>
        ///A test for JsonTypeCode
        ///</summary>
        [Test()]
        public void JsonTypeCodeTest() {
            JsonObject obj = new JsonObject();

            Assertion.IsTrue(obj.JsonTypeCode == JsonTypeCode.Object);
        }

        [Test()]
        public void BasicSerializationTest() {

            JsonObject obj = new JsonObject();
            BinaryFormatter fmt = new BinaryFormatter();

            using(MemoryStream ms = new MemoryStream()) {
                fmt.Serialize(ms, obj);
                ms.Position = 0;

                JsonObject dobj = (JsonObject)fmt.Deserialize(ms);

                Assertion.IsNotNull(dobj);
                Assertion.IsTrue(obj.Count == dobj.Count);
            }
        }
    }
}
