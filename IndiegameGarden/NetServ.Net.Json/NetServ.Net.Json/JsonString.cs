// JsonString.cs
//
// Copyright (C) 2006 Andy Kernahan
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.Text;
using System.Diagnostics;
using System.Globalization;

namespace NetServ.Net.Json
{
    /// <summary>
    /// Represents a JavaScript Object Notation String data type. This class cannot 
    /// be inherited.
    /// </summary>
    [Serializable()]
    [DebuggerDisplay("{_value}")]
    public sealed class JsonString : JsonTypeSkeleton, IJsonString
    {
        #region Private Fields.

        private string _encodedValue;
        private readonly string _value;

        private static readonly char[] QUOTE_CHARS = { '"', '/', '\\', '\b', '\f', '\n', '\r', '\t' };        

        #endregion

        #region Public Interface.

        /// <summary>
        /// Defines an empty JsonString. This field is readonly.
        /// </summary>
        public static readonly JsonString Empty = new JsonString(string.Empty);

        /// <summary>
        /// Initialises a new instance of the JsonString class and specifies the
        /// value.
        /// </summary>
        /// <param name="value">The value of the instance.</param>
        public JsonString(string value)
            : base(JsonTypeCode.String) {

            if(value == null)
                throw new ArgumentNullException("value");

            _value = value;
        }

        /// <summary>
        /// Writes the contents of this Json type using the specified
        /// <see cref="NetServ.Net.Json.IJsonWriter"/>.
        /// </summary>
        /// <param name="writer">The Json writer.</param>
        public override void Write(IJsonWriter writer) {

            if(writer == null)
                throw new ArgumentNullException("writer");

            writer.WriteValue(this.EncodedValue);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> representation of this JsonString 
        /// instance.
        /// </summary>
        /// <returns> <see cref="System.String"/> representation of this JsonString 
        /// instance.</returns>
        public override string ToString() {

            return this.Value;
        }

        /// <summary>
        /// Returns a indicating whether this instance is equal to the specified
        /// <see cref="System.Object"/>.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if the specified object is equal to this instance, otherwise;
        /// false.</returns>
        public override bool Equals(object obj) {

            if(obj == null)
                return false;
            if(obj.GetType() != GetType())
                return false;

            return Equals((JsonString)obj);
        }

        /// <summary>
        /// Returns a indicating whether this instance is equal to the specified
        /// JsonString.
        /// </summary>
        /// <param name="other">The value to compare.</param>
        /// <returns>True if the specified instance is equal to this instance, otherwise;
        /// false.</returns>
        public bool Equals(JsonString other) {

            return other != null && Equals(other.Value);
        }

        /// <summary>
        /// Returns a indicating whether this instance is equal to the specified
        /// <see cref="NetServ.Net.Json.IJsonString"/>.
        /// </summary>
        /// <param name="other">The value to compare.</param>
        /// <returns>True if the specified instance is equal to this instance, otherwise;
        /// false.</returns>
        public bool Equals(IJsonString other) {

            return other != null && Equals(other.Value);
        }  

        /// <summary>
        /// Returns a indicating whether this instance is equal to the specified
        /// <see cref="System.String"/>.
        /// </summary>
        /// <param name="other">The value to compare.</param>
        /// <returns>True if the specified instance is equal to this instance, otherwise;
        /// false.</returns>
        public bool Equals(string other) {

            return this.Value.Equals(other);
        }

        /// <summary>
        /// Returns a value indicating equality with the specified instance.
        /// </summary>
        /// <param name="other">The JsonNumber to compare.</param>
        /// <returns></returns>
        public int CompareTo(JsonString other) {

            return other != null ? this.Value.CompareTo(other.Value) : -1;
        }

        /// <summary>
        /// Returns a value indicating equality with the specified
        /// <see cref="NetServ.Net.Json.IJsonString"/>.
        /// </summary>
        /// <param name="other">The <see cref="NetServ.Net.Json.IJsonString"/> to 
        /// compare.</param>
        /// <returns></returns>
        public int CompareTo(IJsonString other) {

            return other != null ? this.Value.CompareTo(other.Value) : -1;
        }

        /// <summary>
        /// Returns a value indicating equality with the specified <see cref="System.String"/>.
        /// </summary>
        /// <param name="other">The String to compare.</param>
        /// <returns></returns>
        public int CompareTo(string other) {

            return this.Value.CompareTo(other);
        }

        /// <summary>
        /// Returns a hash code for this JsonString.
        /// </summary>
        /// <returns>A hash code for this JsonString.</returns>
        public override int GetHashCode() {

            return this.Value.GetHashCode();
        }

        /// <summary>
        /// Gets the un-encoded value of the this JsonString.
        /// </summary>
        public string Value {

            get { return _value; }
        }

        /// <summary>
        /// Gets the encoded value of this JsonString.
        /// </summary>
        public string EncodedValue {

            get {
                if(_encodedValue == null)
                    _encodedValue = JsonString.Encode(this.Value);
                return _encodedValue;
            }
        }

        /// <summary>
        /// Determines if the two <see cref="NetServ.Net.Json.JsonString"/>s are
        /// equal.
        /// </summary>
        /// <param name="a">The first JsonString.</param>
        /// <param name="b">The second JsonString.</param>
        /// <returns>True if the JsonStrings are equal, otherwise; false.</returns>
        public static bool Equals(JsonString a, JsonString b) {

            object ao = a;
            object bo = b;

            if(ao == bo)
                return true;
            if(ao == null || bo == null)
                return false;

            return a.Equals(b.Value);
        }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="a">The first JsonString.</param>
        /// <param name="b">The second JsonString.</param>
        /// <returns>True if the JsonStrings are equal, otherwise; false.</returns>
        public static bool operator ==(JsonString a, JsonString b) {

            return JsonString.Equals(a, b);
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="a">The first JsonString.</param>
        /// <param name="b">The second JsonString.</param>
        /// <returns>True if the JsonStrings are not equal, otherwise; false.</returns>
        public static bool operator !=(JsonString a, JsonString b) {

            return !JsonString.Equals(a, b);
        }

        /// <summary>
        /// Implicit conversion operator.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static implicit operator JsonString(string value) {            

            if(value == null)
                return null;
            if(value.Equals(string.Empty))
                return JsonString.Empty;
            return new JsonString(value);
        }

        /// <summary>
        /// Implicit conversion operator.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>This method always returns null.</returns>
        public static implicit operator JsonString(JsonNull value) {

            return null;
        }

        /// <summary>
        /// Explicit conversion operator.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static explicit operator string(JsonString value) {

            return value != null ? value.Value : null;
        }        

        /// <summary>
        /// Encodes the specified <see cref="System.String"/>.
        /// </summary>
        /// <param name="s">The string to encode.</param>
        /// <returns>The encoded string.</returns>
        public static string Encode(string s) {

            if(s == null)
                throw new ArgumentNullException("s");

            if(s.Equals(string.Empty) || !JsonString.ShouldEncode(s))
                return string.Concat("\"", s, "\"");

            char ch;
            StringBuilder sb = new StringBuilder(s.Length);

            sb.Append('"');
            for(int i = 0; i < s.Length; ++i) {
                ch = s[i];
                switch(ch) {
                    case '"':
                        sb.Append(@"\""");
                        break;
                    case '/':
                        sb.Append(@"\/");
                        break;
                    case '\\':
                        sb.Append(@"\\");
                        break;
                    case '\b':
                        sb.Append(@"\b");
                        break;
                    case '\f':
                        sb.Append(@"\f");
                        break;
                    case '\n':
                        sb.Append(@"\n");
                        break;
                    case '\r':
                        sb.Append(@"\r");
                        break;
                    case '\t':
                        sb.Append(@"\t");
                        break;
                    default:
                        if(ch > 0x7F)
                            // TODO: MUST add support for UTF-16.
                            sb.AppendFormat(@"\u{0}", ((int)ch).ToString("X4"));
                        else
                            sb.Append(ch);
                        break;
                }
            }
            sb.Append('"');

            return sb.ToString();
        }        

        #endregion

        #region Private Impl.

        private static bool ShouldEncode(string s) {

            for(int i = 0; i < s.Length; ++i) {
                if(s[i] > 0x7F || Array.IndexOf(JsonString.QUOTE_CHARS, s[i]) > -1)
                    return true;
            }

            return false;
        }

        #endregion
    }
}
