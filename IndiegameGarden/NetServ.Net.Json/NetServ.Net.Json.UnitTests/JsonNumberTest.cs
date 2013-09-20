using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NetServ.Net.Json;
using Vts.UnitTest;

namespace NetServ.Net.Json.UnitTests
{    
    [TestFixture()]
    public class JsonNumberTest
    {
        /// <summary>
        ///A test for JsonNumber (byte)
        ///</summary>
        [Test()]
        [Sequence(0)]
        public void ConstructorTest() {

            byte value = 0;
            JsonNumber n = new JsonNumber(value);

            Assertion.IsTrue(n.Value == value);
            value = byte.MinValue;
            n = new JsonNumber(value);
            Assertion.IsTrue(n.Value == value);
            value = byte.MaxValue;
            n = new JsonNumber(value);
            Assertion.IsTrue(n.Value == value);
        }

        /// <summary>
        ///A test for JsonNumber (double)
        ///</summary>
        [Test()]
        [Sequence(1)]
        public void ConstructorTest1() {

            double value = 0D;
            JsonNumber n = new JsonNumber(value);

            Assertion.IsTrue(n.Value == value);
            value = double.MinValue;
            n = new JsonNumber(value);
            Assertion.IsTrue(n.Value == value);
            value = double.MaxValue;
            n = new JsonNumber(value);
            Assertion.IsTrue(n.Value == value);
        }

        /// <summary>
        ///A test for JsonNumber (int)
        ///</summary>
        [Test()]
        [Sequence(2)]
        public void ConstructorTest2() {

            int value = 0;
            JsonNumber n = new JsonNumber(value);

            Assertion.IsTrue(n.Value == value);
            value = int.MinValue;
            n = new JsonNumber(value);
            Assertion.IsTrue(n.Value == value);
            value = int.MaxValue;
            n = new JsonNumber(value);
            Assertion.IsTrue(n.Value == value);
        }

        /// <summary>
        ///A test for JsonNumber (long)
        ///</summary>
        [Test()]
        [Sequence(3)]
        public void ConstructorTest3() {

            long value = 0;
            JsonNumber n = new JsonNumber(value);

            Assertion.IsTrue(n.Value == value);
            value = long.MinValue;
            n = new JsonNumber(value);
            Assertion.IsTrue(n.Value == value);
            value = long.MaxValue;
            n = new JsonNumber(value);
            Assertion.IsTrue(n.Value == value);
        }

        /// <summary>
        ///A test for JsonNumber (sbyte)
        ///</summary>
        [Test()]
        [Sequence(4)]
        public void ConstructorTest4() {

            sbyte value = 0;
            JsonNumber n = new JsonNumber(value);

            Assertion.IsTrue(n.Value == value);
            value = sbyte.MinValue;
            n = new JsonNumber(value);
            Assertion.IsTrue(n.Value == value);
            value = sbyte.MaxValue;
            n = new JsonNumber(value);
            Assertion.IsTrue(n.Value == value);
        }

        /// <summary>
        ///A test for JsonNumber (short)
        ///</summary>
        [Test()]
        [Sequence(5)]
        public void ConstructorTest5() {

            short value = 0;
            JsonNumber n = new JsonNumber(value);

            Assertion.IsTrue(n.Value == value);
            value = short.MinValue;
            n = new JsonNumber(value);
            Assertion.IsTrue(n.Value == value);
            value = short.MaxValue;
            n = new JsonNumber(value);
            Assertion.IsTrue(n.Value == value);
        }

        /// <summary>
        ///A test for JsonNumber (uint)
        ///</summary>
        [Test()]
        [Sequence(6)]
        public void ConstructorTest6() {

            uint value = 0;
            JsonNumber n = new JsonNumber(value);

            Assertion.IsTrue(n.Value == value);
            value = uint.MinValue;
            n = new JsonNumber(value);
            Assertion.IsTrue(n.Value == value);
            value = uint.MaxValue;
            n = new JsonNumber(value);
            Assertion.IsTrue(n.Value == value);
        }

        /// <summary>
        ///A test for JsonNumber (ulong)
        ///</summary>
        [Test()]
        [Sequence(7)]
        public void ConstructorTest7() {

            ulong value = 0;
            JsonNumber n = new JsonNumber(value);

            Assertion.IsTrue(n.Value == value);
            value = ulong.MinValue;
            n = new JsonNumber(value);
            Assertion.IsTrue(n.Value == value);
            value = ulong.MaxValue;
            n = new JsonNumber(value);
            Assertion.IsTrue(n.Value == value);
        }

        /// <summary>
        ///A test for JsonNumber (ushort)
        ///</summary>
        [Test()]
        [Sequence(8)]
        public void ConstructorTest8() {

            ushort value = 0;
            JsonNumber n = new JsonNumber(value);

            Assertion.IsTrue(n.Value == value);
            value = ushort.MinValue;
            n = new JsonNumber(value);
            Assertion.IsTrue(n.Value == value);
            value = ushort.MaxValue;
            n = new JsonNumber(value);
            Assertion.IsTrue(n.Value == value);
        }

        /// <summary>
        ///A test for JsonNumber (double.NaN)
        ///</summary>
        [Test()]
        [Sequence(8)]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConstructorTest9() {

            JsonNumber a = new JsonNumber(double.NaN);
        }

        /// <summary>
        ///A test for JsonNumber (double.PositiveInfinity)
        ///</summary>
        [Test()]
        [Sequence(9)]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConstructorTest10() {

            JsonNumber a = new JsonNumber(double.PositiveInfinity);
        }

        /// <summary>
        ///A test for JsonNumber (double.NegativeInfinity)
        ///</summary>
        [Test()]
        [Sequence(11)]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConstructorTest11() {

            JsonNumber a = new JsonNumber(double.NegativeInfinity);
        }
        /// <summary>
        ///A test for Value
        ///</summary>
        [Test()]
        [Sequence(12)]
        public void ValueTest() {

            double value = 10D;
            JsonNumber a = new JsonNumber(value);

            Assertion.IsTrue(a.Value == value);
        }

        ///A test for CompareTo (double)
        ///</summary>
        [Test()]
        public void CompareToTest() {
            
            JsonNumber n = new JsonNumber(10);

            Assertion.IsTrue(n.CompareTo(n.Value) == 0);
            Assertion.IsTrue(n.CompareTo(10) == 0);
            Assertion.IsTrue(n.CompareTo(20) < 0);
            Assertion.IsTrue(n.CompareTo(5) > 0); 
        }

        /// <summary>
        ///A test for CompareTo (IJsonNumber)
        ///</summary>
        [Test()]
        public void CompareToTest1() {

            JsonNumber n5 = new JsonNumber(5);
            JsonNumber n10 = new JsonNumber(10);
            JsonNumber n15 = new JsonNumber(15);

            Assertion.IsTrue(n10.CompareTo((IJsonNumber)n10) == 0);
            Assertion.IsTrue(n10.CompareTo((IJsonNumber)n15) < 0);
            Assertion.IsTrue(n10.CompareTo((IJsonNumber)n5) > 0);
        }

        /// <summary>
        ///A test for CompareTo (JsonNumber)
        ///</summary>
        [Test()]
        public void CompareToTest2() {

            JsonNumber n5 = new JsonNumber(5);
            JsonNumber n10 = new JsonNumber(10);
            JsonNumber n15 = new JsonNumber(15);

            Assertion.IsTrue(n10.CompareTo(n10) == 0);
            Assertion.IsTrue(n10.CompareTo(n15) < 0);
            Assertion.IsTrue(n10.CompareTo(n5) > 0);
        }

        /// <summary>
        ///A test for Equals (double)
        ///</summary>
        [Test()]
        public void EqualsTest() {
            
            JsonNumber n10 = new JsonNumber(10D);
            JsonNumber n20 = new JsonNumber(20D);

            Assertion.IsTrue(n10.Equals(10D));
            Assertion.IsFalse(n20.Equals(n10));
        }

        /// <summary>
        ///A test for Equals (IJsonNumber)
        ///</summary>
        [Test()]
        public void EqualsTest1() {

            double value = 10D;
            JsonNumber n = new JsonNumber(value);
            IJsonNumber on = new JsonNumber(value);

            Assertion.IsFalse(n.Equals((IJsonNumber)null));
            Assertion.IsTrue(n.Equals(on));
            on = new JsonNumber(value + 10);
            Assertion.IsFalse(n.Equals(on));
        }

        /// <summary>
        ///A test for Equals (JsonNumber)
        ///</summary>
        [Test()]
        public void EqualsTest2() {

            double value = 10D;
            JsonNumber n = new JsonNumber(value);
            JsonNumber on = new JsonNumber(value);

            Assertion.IsTrue(n.Equals(on));
            on = new JsonNumber(value + 10);
            Assertion.IsFalse(n.Equals(on));	
        }

        /// <summary>
        ///A test for Equals (JsonNumber, JsonNumber)
        ///</summary>
        [Test()]
        public void EqualsTest3() {

            JsonNumber a = null;
            JsonNumber b = null;

            Assertion.IsTrue(JsonNumber.Equals(a, b));
            a = new JsonNumber(10);
            Assertion.IsFalse(JsonNumber.Equals(a, b));
            Assertion.IsTrue(JsonNumber.Equals(a, a));
            b = new JsonNumber(10);
            Assertion.IsTrue(JsonNumber.Equals(a, b));
            b = new JsonNumber(20);
            Assertion.IsFalse(JsonNumber.Equals(a, b));
        }

        /// <summary>
        ///A test for Equals (object)
        ///</summary>
        [Test()]
        public void EqualsTest4() {

            JsonNumber a = new JsonNumber(10);
            object b = new JsonNumber(10);

            Assertion.IsFalse(a.Equals((object)null));
            Assertion.IsFalse(a.Equals(""));
            Assertion.IsTrue(a.Equals(b));
            Assertion.IsTrue(b.Equals(a));
        }

        /// <summary>
        ///A test for explicit operator (JsonNumber)
        ///</summary>
        [Test()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConversionTestNullException() {

            JsonNumber value = null;

            double n = (double)value;            
        }

        /// <summary>
        ///A test for explicit operator (JsonNumber)
        ///</summary>
        [Test()]
        public void ConversionTest() {

            double value = 10D;
            JsonNumber n = new JsonNumber(value);

            Assertion.IsTrue((double)n == value);
        }

        /// <summary>
        ///A test for explicit operator (JsonNumber)
        ///</summary>
        [Test()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConversionTest1NullException() {

            JsonNumber value = null;

            uint n = (uint)value;            
        }

        /// <summary>
        ///A test for explicit operator (JsonNumber)
        ///</summary>
        [Test()]
        public void ConversionTest1() {

            uint value = 100;
            JsonNumber n = new JsonNumber(value);

            Assertion.IsTrue((uint)n == value);
        }

        /// <summary>
        ///A test for explicit operator (JsonNumber)
        ///</summary>
        [Test()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConversionTest2NullException() {

            JsonNumber value = null;

            ulong n = (ulong)value;            
        }

        /// <summary>
        ///A test for explicit operator (JsonNumber)
        ///</summary>
        [Test()]
        public void ConversionTest2() {

            ulong value = 100;
            JsonNumber n = new JsonNumber(value);

            Assertion.IsTrue((ulong)n == value);
        }

        /// <summary>
        ///A test for explicit operator (JsonNumber)
        ///</summary>
        [Test()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConversionTest3NullException() {

            JsonNumber value = null;

            long n = (long)value;            
        }

        /// <summary>
        ///A test for explicit operator (JsonNumber)
        ///</summary>
        [Test()]
        public void ConversionTest3() {

            long value = 100;
            JsonNumber n = new JsonNumber(value);

            Assertion.IsTrue((long)n == value);
        }

        /// <summary>
        ///A test for explicit operator (JsonNumber)
        ///</summary>
        [Test()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConversionTest4NullException() {

            JsonNumber value = null;

            ushort n = (ushort)value;            
        }

        /// <summary>
        ///A test for explicit operator (JsonNumber)
        ///</summary>
        [Test()]
        public void ConversionTest4() {

            ushort value = 100;
            JsonNumber n = new JsonNumber(value);

            Assertion.IsTrue((ushort)n == value);
        }

        /// <summary>
        ///A test for explicit operator (JsonNumber)
        ///</summary>
        [Test()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConversionTest5NullException() {

            JsonNumber value = null;

            int n = (int)value;            
        }

        /// <summary>
        ///A test for explicit operator (JsonNumber)
        ///</summary>
        [Test()]
        public void ConversionTest5() {

            int value = 100;
            JsonNumber n = new JsonNumber(value);

            Assertion.IsTrue((int)n == value);
        }

        /// <summary>
        ///A test for explicit operator (JsonNumber)
        ///</summary>
        [Test()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConversionTest6NullException() {

            JsonNumber value = null;

            byte n = (byte)value;            
        }

        /// <summary>
        ///A test for explicit operator (JsonNumber)
        ///</summary>
        [Test()]
        public void ConversionTest6() {

            byte value = 100;
            JsonNumber n = new JsonNumber(value);

            Assertion.IsTrue((byte)n == value);
        }

        /// <summary>
        ///A test for explicit operator (JsonNumber)
        ///</summary>
        [Test()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConversionTest7NullException() {

            JsonNumber value = null;

            sbyte n = (sbyte)value;            
        }

        /// <summary>
        ///A test for explicit operator (JsonNumber)
        ///</summary>
        [Test()]
        public void ConversionTest7() {

            sbyte value = 100;
            JsonNumber n = new JsonNumber(value);

            Assertion.IsTrue((sbyte)n == value);
        }

        /// <summary>
        ///A test for explicit operator (JsonNumber)
        ///</summary>
        [Test()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConversionTest8NullException() {

            JsonNumber value = null;

            short n = (short)value;            
        }

        /// <summary>
        ///A test for explicit operator (JsonNumber)
        ///</summary>
        [Test()]
        public void ConversionTest8() {

            short value = 100;
            JsonNumber n = new JsonNumber(value);

            Assertion.IsTrue((short)n == value);
        }

        /// <summary>
        ///A test for GetHashCode ()
        ///</summary>
        [Test()]
        public void GetHashCodeTest() {

            JsonNumber n1 = new JsonNumber(10);
            JsonNumber n2 = new JsonNumber(10);

            Assertion.IsTrue(n1.GetHashCode() == n1.GetHashCode());
            Assertion.IsTrue(n1.GetHashCode() == n2.GetHashCode());
            n2 = new JsonNumber(20);
            Assertion.IsFalse(n1.GetHashCode() == n2.GetHashCode());	
        }

        /// <summary>
        ///A test for implicit operator (byte)
        ///</summary>
        [Test()]
        public void ConversionTest9() {

            byte value = 10;
            JsonNumber n = new JsonNumber(value);

            Assertion.IsTrue(n.Value == value);
        }

        /// <summary>
        ///A test for implicit operator (double)
        ///</summary>
        [Test()]
        public void ConversionTest10() {

            double value = 0D;
            JsonNumber n = value;

            Assertion.IsNotNull(n);
            Assertion.IsTrue(n.Value == value);
            value = double.MinValue;
            n = value;
            Assertion.IsTrue(n.Value == value);
            value = double.MaxValue;
            n = value;
            Assertion.IsTrue(n.Value == value);
        }

        /// <summary>
        ///A test for implicit operator (int)
        ///</summary>
        [Test()]
        public void ConversionTest11() {

            int value = 0;
            JsonNumber n = value;

            Assertion.IsNotNull(n);
            Assertion.IsTrue(n.Value == value);
            value = int.MinValue;
            n = value;
            Assertion.IsTrue(n.Value == value);
            value = int.MaxValue;
            n = value;
            Assertion.IsTrue(n.Value == value);
        }

        /// <summary>
        ///A test for implicit operator (JsonNull)
        ///</summary>
        [Test()]
        public void ConversionTest12() {

            JsonNumber n = JsonNull.Null;

            Assertion.IsNull(n);
        }

        /// <summary>
        ///A test for implicit operator (long)
        ///</summary>
        [Test()]
        public void ConversionTest13() {

            long value = 0;
            JsonNumber n = value;

            Assertion.IsNotNull(n);
            Assertion.IsTrue(n.Value == value);
            value = long.MinValue;
            n = value;
            Assertion.IsTrue(n.Value == value);
            value = long.MaxValue;
            n = value;
            Assertion.IsTrue(n.Value == value);
        }

        /// <summary>
        ///A test for implicit operator (sbyte)
        ///</summary>
        [Test()]
        public void ConversionTest14() {

            sbyte value = 0;
            JsonNumber n = value;

            Assertion.IsNotNull(n);
            Assertion.IsTrue(n.Value == value);
            value = sbyte.MinValue;
            n = value;
            Assertion.IsTrue(n.Value == value);
            value = sbyte.MaxValue;
            n = value;
            Assertion.IsTrue(n.Value == value);
        }

        /// <summary>
        ///A test for implicit operator (short)
        ///</summary>
        [Test()]
        public void ConversionTest15() {

            short value = 0;
            JsonNumber n = value;

            Assertion.IsNotNull(n);
            Assertion.IsTrue(n.Value == value);
            value = short.MinValue;
            n = value;
            Assertion.IsTrue(n.Value == value);
            value = short.MaxValue;
            n = value;
            Assertion.IsTrue(n.Value == value);
        }

        /// <summary>
        ///A test for implicit operator (uint)
        ///</summary>
        [Test()]
        public void ConversionTest16() {

            uint value = 0;
            JsonNumber n = value;

            Assertion.IsNotNull(n);
            Assertion.IsTrue(n.Value == value);
            value = uint.MaxValue;
            n = value;
            Assertion.IsTrue(n.Value == value);        
        }

        /// <summary>
        ///A test for implicit operator (ulong)
        ///</summary>
        [Test()]
        public void ConversionTest17() {

            ulong value = 0;
            JsonNumber n = value;

            Assertion.IsNotNull(n);
            Assertion.IsTrue(n.Value == value);
            value = ulong.MaxValue;
            n = value;
            Assertion.IsTrue(n.Value == value);
        }

        /// <summary>
        ///A test for implicit operator (ushort)
        ///</summary>
        [Test()]
        public void ConversionTest18() {

           ushort value = 0;
            JsonNumber n = value;

            Assertion.IsNotNull(n);
            Assertion.IsTrue(n.Value == value);
            value = ushort.MaxValue;
            n = value;
            Assertion.IsTrue(n.Value == value);
        }

        /// <summary>
        ///A test for operator != (JsonNumber, JsonNumber)
        ///</summary>
        [Test()]
        public void InequalityTest() {

            JsonNumber a = null;
            JsonNumber b = null;

            Assertion.IsFalse(a != b);
            a = new JsonNumber(10);
            Assertion.IsTrue(a != b);
            b = new JsonNumber(10);
            Assertion.IsFalse(a != b);
            b = new JsonNumber(20);
            Assertion.IsTrue(a != b);
        }

        /// <summary>
        ///A test for operator == (JsonNumber, JsonNumber)
        ///</summary>
        [Test()]
        public void EqualityTest() {

            JsonNumber a = null;
            JsonNumber b = null;

            Assertion.IsTrue(a == b);
            a = new JsonNumber(10);
            Assertion.IsFalse(a == b);
            Assertion.IsFalse(b == a);
            b = new JsonNumber(10);
            Assertion.IsTrue(a == b);
            b = new JsonNumber(20);
            Assertion.IsFalse(a == b);
        }

        /// <summary>
        ///A test for ToString ()
        ///</summary>
        [Test()]
        public void ToStringTest() {

            double value = 1.67834e-6;

            JsonNumber n = new JsonNumber(value);

            Assertion.IsTrue(n.ToString() == n.ToString());
            Assertion.IsTrue(n.ToString() == value.ToString());
        }

        /// <summary>
        ///A test for Write (IJsonWriter)
        ///</summary>
        [Test()]        
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteTestNull() {

            JsonNumber n = new JsonNumber(10);

            n.Write(null);
        }

        [Test()]        
        public void JsonTypeCodeTest() {

            JsonNumber n = new JsonNumber(25);

            Assertion.IsTrue(n.JsonTypeCode == JsonTypeCode.Number);
        }

        [Test()]        
        public void SeializationTest() {

            JsonNumber n = new JsonNumber(254);
            BinaryFormatter fmt = new BinaryFormatter();

            using(MemoryStream ms = new MemoryStream()) {
                fmt.Serialize(ms, n);
                ms.Position = 0;

                JsonNumber dn = (JsonNumber)fmt.Deserialize(ms);

                Assertion.IsNotNull(dn);
                Assertion.IsTrue(n.Value == dn.Value);
            }
        }
    }
}
