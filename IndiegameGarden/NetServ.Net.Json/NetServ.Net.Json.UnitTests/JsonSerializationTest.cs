using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using NetServ.Net.Json;
using Vts.UnitTest;

namespace NetServ.Net.Json.UnitTests
{
    [TestFixture()]
    public class JsonSerializationTest
    {
        [Test()]        
        public void ComplexTest() {

            MemoryStream ms = new MemoryStream();
            IFormatter fmt = new BinaryFormatter();
            JsonArray methods;
            JsonArray filters;
            JsonObject filter;
            JsonObject service;
            JsonObject attrs;
            JsonObject daccount;
            JsonObject account = new JsonObject();
            JsonArray services = new JsonArray();
            JsonObject credentials = new JsonObject();

            account.Add("id", "BogusAccount");
            account.Add("enabled", true);
            credentials.Add("username", "BogusAccount");
            credentials.Add("password", "guvfvfabgbarbszlcnffjbeqf");
            account.Add("credentials", credentials);
            account.Add("services", services);

            service = new JsonObject();
            services.Add(service);

            service.Add("id", "pop3");
            service.Add("enabled", true);

            filters = new JsonArray();
            service.Add("transport-filters", filters);
            filter = new JsonObject();
            filter.Add("type", "cidr");
            filter.Add("pattern", "127.0.0.0/8");
            filter.Add("action", "accept");
            filters.Add(filter);
            filter = new JsonObject();
            filter.Add("type", "cidr");
            filter.Add("pattern", "192.168.1.0/8");
            filter.Add("action", "accept");
            filters.Add(filter);

            methods = new JsonArray();
            service.Add("authentication-methods", methods);
            methods.Add("CRAM-MD5");
            methods.Add("APOP");

            attrs = new JsonObject();
            service.Add("named-attributes", attrs);
            attrs.Add("debug-session", true);
            attrs.Add("max-connections", 1);
            attrs.Add("force-secure", false);

            service = new JsonObject();
            services.Add(service);

            service.Add("id", "smtp");
            service.Add("enabled", false);

            filters = new JsonArray();
            service.Add("transport-filters", filters);
            filter = new JsonObject();
            filter.Add("type", "literal");
            filter.Add("pattern", "10.36.1.1");
            filter.Add("action", "accept");
            filters.Add(filter);

            methods = new JsonArray();
            service.Add("authentication-methods", methods);
            methods.Add("CRAM-MD5");
            methods.Add("LOGIN");

            attrs = new JsonObject();
            service.Add("named-attributes", attrs);
            attrs.Add("debug-session", true);
            attrs.Add("allow-relay", true);
            attrs.Add("max-connections", 1);
            attrs.Add("routing-priority-adjust", -10);

            fmt.Serialize(ms, account);
            ms.Position = 0;
            daccount = (JsonObject)fmt.Deserialize(ms);
            Assertion.IsNotNull(daccount);
            CompareObjects(account, daccount);
        }

        private void CompareObjects(JsonObject obj1, JsonObject obj2) {

            IJsonType type;

            Assertion.IsTrue(obj1.Count == obj1.Count);
            foreach(KeyValuePair<string, IJsonType> pair in obj1) {
                Assertion.IsTrue(obj2.ContainsKey(pair.Key));
                type = obj2[pair.Key];
                Assertion.IsTrue(pair.Value.JsonTypeCode == type.JsonTypeCode);
                switch(type.JsonTypeCode) {                    
                    case JsonTypeCode.Object:
                        CompareObjects((JsonObject)pair.Value, (JsonObject)type);
                        break;
                    case JsonTypeCode.Array:
                        CompareArrays((JsonArray)pair.Value, (JsonArray)type);
                        break;
                    default:
                        Assertion.IsTrue(pair.Value.Equals(type));
                        break;
                }
            }
        }

        private void CompareArrays(JsonArray arr1, JsonArray arr2) {

            // This relies on the arrays being in the same order. Serializers
            // should (must really) maintain the order of arrays otherwise they 
            // couldn't be used for stacks, queues etc.

            Assertion.IsTrue(arr1.Count == arr2.Count);
            for(int i = 0; i < arr1.Count; ++i) {
                Assertion.IsTrue(arr1[i].JsonTypeCode == arr2[i].JsonTypeCode);
                switch(arr1[i].JsonTypeCode) {
                    case JsonTypeCode.Object:
                        CompareObjects((JsonObject)arr1[i], (JsonObject)arr2[i]);
                        break;
                    case JsonTypeCode.Array:
                        CompareArrays((JsonArray)arr1[i], (JsonArray)arr2[i]);
                        break;
                    default:
                        Assertion.IsTrue(arr1[i].Equals(arr2[i]));
                        break;
                }
            }
        }
    }
}
