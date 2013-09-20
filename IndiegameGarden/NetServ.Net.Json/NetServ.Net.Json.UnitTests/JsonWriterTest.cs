using System;
using System.IO;
using NetServ.Net.Json;
using Vts.UnitTest;

namespace NetServ.Net.Json.UnitTests
{   
    [TestFixture()]
    public class JsonWriterTest
    {
        /// <summary>
        ///A test for JsonWriter ()
        ///</summary>
        [Test()]
        public void ConstructorTest() {

            JsonWriter writer = new JsonWriter();
        }

        /// <summary>
        ///A test for JsonWriter (TextWriter, bool)
        ///</summary>
        [Test()]        
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorTest1() {

            JsonWriter writer = new JsonWriter(null, true);
        }

        /// <summary>
        ///A test for JsonWriter (TextWriter, bool)
        ///</summary>
        [Test()]
        public void ConstructorTest2() {

            JsonWriter writer = new JsonWriter(TextWriter.Null, true);
        }

        /// <summary>
        ///A test for Close ()
        ///</summary>
        [Test()]
        public void CloseTest() {

            StringWriter sw = new StringWriter();
            JsonWriter writer = new JsonWriter(sw, false);

            writer.Close();
            sw.Write("");
        }

        /// <summary>
        ///A test for Close ()
        ///</summary>
        [Test()]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CloseTest1() {

            StringWriter sw = new StringWriter();
            JsonWriter writer = new JsonWriter(sw, true);

            writer.Close();
            sw.Write("");
        }

        /// <summary>
        ///A test for WriteBeginArray ()
        ///</summary>
        [Test()]
        public void WriteBeginArrayTest() {

            JsonWriter writer = new JsonWriter();

            writer.WriteBeginArray();
            writer.ToString().Equals(JsonWriter.BeginArray.ToString());
        }

        /// <summary>
        ///A test for WriteBeginObject ()
        ///</summary>
        [Test()]
        public void WriteBeginObjectTest() {

            JsonWriter writer = new JsonWriter();

            writer.WriteBeginObject();
            writer.ToString().Equals(JsonWriter.BeginObject.ToString());
        }

        /// <summary>
        ///A test for WriteEndArray ()
        ///</summary>
        [Test()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WriteEndArrayTest() {

            JsonWriter writer = new JsonWriter();

            writer.WriteEndArray();            
        }

        /// <summary>
        ///A test for WriteEndArray ()
        ///</summary>
        [Test()]
        public void WriteEndArrayTest2() {

            JsonWriter writer = new JsonWriter();

            writer.WriteBeginArray();
            writer.WriteEndArray();
            writer.ToString().EndsWith(JsonWriter.EndArray.ToString());
        }

        /// <summary>
        ///A test for WriteEndObject ()
        ///</summary>
        [Test()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WriteEndObjectTest() {

            JsonWriter writer = new JsonWriter();
            
            writer.WriteEndObject();            
        }

        /// <summary>
        ///A test for WriteEndObject ()
        ///</summary>
        [Test()]
        public void WriteEndObjectTest2() {

            JsonWriter writer = new JsonWriter();

            writer.WriteBeginObject();
            writer.WriteEndObject();
            writer.ToString().EndsWith(JsonWriter.EndObject.ToString());
        }

        /// <summary>
        ///A test for WriteName (string)
        ///</summary>
        [Test()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WriteNameTest() {

            JsonWriter writer = new JsonWriter();
            
            writer.WriteName("");
        }

        /// <summary>
        ///A test for WriteName (string)
        ///</summary>
        [Test()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteNameTest1() {

            JsonWriter writer = new JsonWriter();

            writer.WriteBeginObject();
            writer.WriteName(null);
        }

        /// <summary>
        ///A test for WriteName (string)
        ///</summary>
        [Test()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WriteNameTest2() {

            JsonWriter writer = new JsonWriter();

            writer.WriteBeginObject();
            writer.WriteName("a");
            writer.WriteName("b");
        }

        /// <summary>
        ///A test for WriteName (string)
        ///</summary>
        [Test()]
        public void WriteNameTest4() {

            JsonWriter writer = new JsonWriter();

            writer.WriteBeginObject();
            writer.WriteName("a");
            Assertion.IsTrue(writer.ToString().EndsWith("\"a\":"));
        }

        /// <summary>
        ///A test for WriteValue (string)
        ///</summary>
        [Test()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WriteValueTest() {

            JsonWriter writer = new JsonWriter();

            writer.WriteValue("");
        }

        /// <summary>
        ///A test for WriteValue (string)
        ///</summary>
        [Test()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteValueTest1() {

            JsonWriter writer = new JsonWriter();

            writer.WriteBeginArray();
            writer.WriteValue(null);
        }

        /// <summary>
        ///A test for WriteValue (string)
        ///</summary>
        [Test()]
        public void WriteValueTest2() {

            JsonWriter writer = new JsonWriter();

            writer.WriteBeginArray();
            writer.WriteValue("");
            Assertion.IsTrue(writer.ToString().EndsWith(JsonWriter.BeginArray.ToString()));
            writer.WriteValue("test");
            Assertion.IsTrue(writer.ToString().EndsWith(",test"));
        }

        [Test()]
        public void FullTest() {

            JsonWriter writer = new JsonWriter();
            string result = "{\"Name\":\"Andrew Kernahan\",\"Tel.\":\"34534534\",\"Delivery\":true,\"Address\":{\"Street\":\"16 Bogus Ave\",\"Line1\":\"Bogus\",\"City\":\"Bogus\",\"County\":\"Bogus\",\"Postcode\":\"Bogus\"},\"Total\":9.86,\"Items\":[{\"ID\":\"Chicken & Chips\",\"Qty\":2,\"Price\":1.5,\"Req\":\"Plenty of salad.\"},{\"ID\":\"Pizza\",\"Qty\":-1,\"Price\":9.6,\"Size\":\"16\\\"\",\"Req\":\"\"}]}";
            JsonObject order = new JsonObject();
            JsonObject addr = new JsonObject();
            JsonArray items = new JsonArray();
            JsonObject item;

            order.Add("Name", "Andrew Kernahan");
            order.Add("Tel.", "34534534");
            order.Add("Delivery", true);
            order.Add("Address", addr);
            order.Add("Total", 9.86D);
            order.Add("Items", items);

            addr.Add("Street", "16 Bogus Ave");
            addr.Add("Line1", "Bogus");
            addr.Add("City", "Bogus");
            addr.Add("County", "Bogus");
            addr.Add("Postcode", "Bogus");

            item = new JsonObject();
            item.Add("ID", "Chicken & Chips");
            item.Add("Qty", 2);
            item.Add("Price", 1.5D);
            item.Add("Req", "Plenty of salad.");
            items.Add(item);

            item = new JsonObject();
            item.Add("ID", "Pizza");
            item.Add("Qty", -1D);
            item.Add("Price", +9.60D);
            item.Add("Size", "16\"");
            item.Add("Req", "");
            items.Add(item);

            order.Write(writer);
            Assertion.IsTrue(writer.ToString().Equals(result));
        }
    }
}
