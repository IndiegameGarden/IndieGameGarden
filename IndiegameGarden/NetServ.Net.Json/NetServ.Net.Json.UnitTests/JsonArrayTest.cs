using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NetServ.Net.Json;
using Vts.UnitTest;

namespace NetServ.Net.Json.UnitTests
{
    [TestFixture()]
    public class JsonArrayTest
    {
        /// <summary>
        ///A test for Add (bool)
        ///</summary>
        [Test()]
        public void AddTest() {
            JsonArray arr = new JsonArray();

            bool item = false;

            arr.Add(item);
            Assertion.IsTrue(arr.Count == 1);
            Assertion.IsTrue(arr.Contains(JsonBoolean.Get(item)));            
        }

        /// <summary>
        ///A test for Add (byte)
        ///</summary>
        [Test()]
        public void AddTest1() {
            JsonArray arr = new JsonArray();

            byte item = 123;

            arr.Add(item);
            Assertion.IsTrue(arr.Count == 1);
            Assertion.IsTrue(arr.Contains(new JsonNumber(item)));
        }

        /// <summary>
        ///A test for Add (double)
        ///</summary>
        [Test()]
        public void AddTest2() {
            JsonArray arr = new JsonArray();

            double item = 65433D;

            arr.Add(item);
            Assertion.IsTrue(arr.Count == 1);
            Assertion.IsTrue(arr.Contains(new JsonNumber(item)));            
        }

        /// <summary>
        ///A test for Add (int)
        ///</summary>
        [Test()]
        public void AddTest3() {
            JsonArray arr = new JsonArray();

            int item = 6643534;

            arr.Add(item);
            Assertion.IsTrue(arr.Count == 1);
            Assertion.IsTrue(arr.Contains(new JsonNumber(item)));            
        }

        /// <summary>
        ///A test for Add (long)
        ///</summary>
        [Test()]
        public void AddTest4() {
            JsonArray arr = new JsonArray();

            long item = 34575464523L;

            arr.Add(item);
            Assertion.IsTrue(arr.Count == 1);
            Assertion.IsTrue(arr.Contains(new JsonNumber(item)));            
        }

        /// <summary>
        ///A test for Add (sbyte)
        ///</summary>
        [Test()]
        public void AddTest5() {
            JsonArray arr = new JsonArray();

            sbyte item = 33;

            arr.Add(item);
            Assertion.IsTrue(arr.Count == 1);
            Assertion.IsTrue(arr.Contains(new JsonNumber(item)));            
        }

        /// <summary>
        ///A test for Add (short)
        ///</summary>
        [Test()]
        public void AddTest6() {
            JsonArray arr = new JsonArray();

            short item = 23432;

            arr.Add(item);
            Assertion.IsTrue(arr.Count == 1);
            Assertion.IsTrue(arr.Contains(new JsonNumber(item)));            
        }

        /// <summary>
        ///A test for Add (string)
        ///</summary>
        [Test()]
        public void AddTest7() {
            JsonArray arr = new JsonArray();

            string item = "test";

            arr.Add(item);
            Assertion.IsTrue(arr.Count == 1);
            Assertion.IsTrue(arr.Contains(new JsonString(item)));            
        }

        /// <summary>
        ///A test for Add (uint)
        ///</summary>
        [Test()]
        public void AddTest8() {
            JsonArray arr = new JsonArray();

            uint item = 23456453;

            arr.Add(item);
            Assertion.IsTrue(arr.Count == 1);
            Assertion.IsTrue(arr.Contains(new JsonNumber(item)));                        
        }

        /// <summary>
        ///A test for Add (ulong)
        ///</summary>
        [Test()]
        public void AddTest9() {
            JsonArray arr = new JsonArray();

            ulong item = 12321413451345UL;

            arr.Add(item);
            Assertion.IsTrue(arr.Count == 1);
            Assertion.IsTrue(arr.Contains(new JsonNumber(item)));            
        }

        /// <summary>
        ///A test for Add (ushort)
        ///</summary>
        [Test()]
        public void AddTest10() {
            JsonArray arr = new JsonArray();

            ushort item = 2423;

            arr.Add(item);
            Assertion.IsTrue(arr.Count == 1);
            Assertion.IsTrue(arr.Contains(new JsonNumber(item)));                        
        }


        /// <summary>
        ///A test for Add (null)
        ///</summary>
        [Test()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddTest11() {
            JsonArray arr = new JsonArray();

            arr.Add((IJsonType)null);
        }

        /// <summary>
        ///A test for Add (JsonArray)
        ///</summary>
        [Test()]        
        public void AddTest12() {

            JsonArray arr = new JsonArray();
            JsonArray item = new JsonArray();

            arr.Add(item);
            Assertion.IsTrue(arr.Count == 1);
            Assertion.IsTrue(arr.Contains(item));
        }

        /// <summary>
        ///A test for Add (JsonObject)
        ///</summary>
        [Test()]
        public void AddTest13() {

            JsonArray arr = new JsonArray();
            JsonObject item = new JsonObject();

            arr.Add(item);
            Assertion.IsTrue(arr.Count == 1);
            Assertion.IsTrue(arr.Contains(item));
        }
    
        [Test()]
        public void AddTestMulti() {

            JsonArray arr = new JsonArray();
            int add = 20;

            for(int i = 0; i < add; ++i)
                arr.Add(i);

            Assertion.IsTrue(arr.Count == add);

            for(int i = 0; i < add; ++i)
                Assertion.IsTrue(arr.Contains(new JsonNumber(i)));            
        }

        /// <summary>
        ///A test for implicit operator (JsonNull)
        ///</summary>
        [Test()]
        public void ConversionTest() {

            JsonArray arr = JsonNull.Null;

            Assertion.IsNull(arr);
        }

        /// <summary>
        ///A test for JsonArray ()
        ///</summary>
        [Test()]
        public void ConstructorTest() {

            JsonArray arr = new JsonArray();

            Assertion.IsTrue(arr.Count == 0);
        }

        /// <summary>
        ///A test for JsonTypeCode
        ///</summary>
        [Test()]
        public void JsonTypeCodeTest() {

            JsonArray arr = new JsonArray();

            Assertion.IsTrue(arr.JsonTypeCode == JsonTypeCode.Array);
        }

        [Test()]
        public void BasicSerializationTest() {

            JsonArray arr = new JsonArray();
            BinaryFormatter fmt = new BinaryFormatter();

            using(MemoryStream ms = new MemoryStream()) {
                fmt.Serialize(ms, arr);
                ms.Position = 0;

                JsonArray darr = (JsonArray)fmt.Deserialize(ms);

                Assertion.IsNotNull(darr);
                Assertion.IsTrue(arr.Count == darr.Count);
            }
        }
    }
}
