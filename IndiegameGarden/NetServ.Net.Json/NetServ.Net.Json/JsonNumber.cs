// JsonNumber.cs
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
using System.Diagnostics;
using System.Globalization;

namespace NetServ.Net.Json
{
    /// <summary>
    /// Represents a JavaScript Object Notation Number data type. This class cannot 
    /// be inherited.
    /// </summary>
    [Serializable()]
    [DebuggerDisplay("{_value}")]
    public sealed class JsonNumber : JsonTypeSkeleton, IJsonNumber
    {
        #region Private Fields.

        private double _value;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Defines the smallest Json number. This field is readonly.
        /// </summary>
        public static readonly JsonNumber MinValue = new JsonNumber(double.MinValue);

        /// <summary>
        /// Defines a Json number with a value of zero. This field is readonly.
        /// </summary>
        public static readonly JsonNumber Zero = new JsonNumber(0D);

        /// <summary>
        /// Defines the lasrgest Json number. This field is readonly.
        /// </summary>
        public static readonly JsonNumber MaxValue = new JsonNumber(double.MaxValue);        

        /// <summary>
        /// Initialises a new instance of the JsonNumber class and specifies the 
        /// value.
        /// </summary>
        /// <param name="value">The value of the new instance.</param>
        public JsonNumber(byte value)
            : this((double)value) {
        }

        /// <summary>
        /// Initialises a new instance of the JsonNumber class and specifies the 
        /// value.
        /// </summary>
        /// <param name="value">The value of the new instance.</param>
        [CLSCompliant(false)]
        public JsonNumber(sbyte value)
            : this((double)value) {
        }

        /// <summary>
        /// Initialises a new instance of the JsonNumber class and specifies the 
        /// value.
        /// </summary>
        /// <param name="value">The value of the new instance.</param>
        public JsonNumber(short value)
            : this((double)value) {
        }

        /// <summary>
        /// Initialises a new instance of the JsonNumber class and specifies the 
        /// value.
        /// </summary>
        /// <param name="value">The value of the new instance.</param>
        [CLSCompliant(false)]
        public JsonNumber(ushort value)
            : this((double)value) {
        }

        /// <summary>
        /// Initialises a new instance of the JsonNumber class and specifies the 
        /// value.
        /// </summary>
        /// <param name="value">The value of the new instance.</param>
        public JsonNumber(int value)
            : this((double)value) {
        }

        /// <summary>
        /// Initialises a new instance of the JsonNumber class and specifies the 
        /// value.
        /// </summary>
        /// <param name="value">The value of the new instance.</param>
        [CLSCompliant(false)]
        public JsonNumber(uint value)
            : this((double)value) {
        }

        /// <summary>
        /// Initialises a new instance of the JsonNumber class and specifies the 
        /// value.
        /// </summary>
        /// <param name="value">The value of the new instance.</param>
        public JsonNumber(long value)
            : this((double)value) {
        }

        /// <summary>
        /// Initialises a new instance of the JsonNumber class and specifies the 
        /// value.
        /// </summary>
        /// <param name="value">The value of the new instance.</param>
        [CLSCompliant(false)]
        public JsonNumber(ulong value)
            : this((double)value) {
        }

        /// <summary>
        /// Initialises a new instance of the JsonNumber class and specifies the 
        /// value.
        /// </summary>
        /// <param name="value">The value of the new instance.</param>
        public JsonNumber(double value)
            : base(JsonTypeCode.Number) {

            if(double.IsInfinity(value) || double.IsNegativeInfinity(value) || 
                double.IsNaN(value))
                throw new ArgumentOutOfRangeException("value");

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

            writer.WriteValue(ToString());
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> representation of this JsonNumber instance.
        /// </summary>
        /// <returns> <see cref="System.String"/> representation of this JsonNumber instance</returns>
        public override string ToString() {

            return this.Value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> representation of this JsonNumber instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The culture specific format provider.</param>
        /// <returns> <see cref="System.String"/> representation of this JsonNumber instance</returns>
        public string ToString(string format, IFormatProvider formatProvider) {

            return this.Value.ToString(format, formatProvider);
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

            return Equals((JsonNumber)obj);
        }

        /// <summary>
        /// Returns a indicating whether this instance is equal to the specified
        /// JsonNumber.
        /// </summary>
        /// <param name="other">The value to compare.</param>
        /// <returns>True if the specified instance is equal to this instance, otherwise;
        /// false.</returns>
        public bool Equals(JsonNumber other) {

            return other != null && this.Value == other.Value;
        }

        /// <summary>
        /// Returns a indicating whether this instance is equal to the specified
        /// <see cref="NetServ.Net.Json.IJsonNumber"/>.
        /// </summary>
        /// <param name="other">The value to compare.</param>
        /// <returns>True if the specified instance is equal to this instance, otherwise;
        /// false.</returns>
        public bool Equals(IJsonNumber other) {

            return other != null && this.Value == other.Value;
        }

        /// <summary>
        /// Returns a indicating whether this instance is equal to the specified
        /// <see cref="System.Double"/>.
        /// </summary>
        /// <param name="other">The value to compare.</param>
        /// <returns>True if the specified double is equal to this instance, otherwise;
        /// false.</returns>
        public bool Equals(double other) {

            return this.Value == other;
        }

        /// <summary>
        /// Returns a value indicating equality with the specified instance.
        /// </summary>
        /// <param name="other">The JsonNumber to compare.</param>
        /// <returns></returns>
        public int CompareTo(JsonNumber other) {

            return other != null ? this.Value.CompareTo(other.Value) : -1;
        }

        /// <summary>
        /// Returns a value indicating equality with the specified instance.
        /// </summary>
        /// <param name="other">The <see cref="NetServ.Net.Json.IJsonNumber"/> to compare.</param>
        /// <returns></returns>
        public int CompareTo(IJsonNumber other) {

            return other != null ? this.Value.CompareTo(other.Value) : -1;
        }

        /// <summary>
        /// Returns a value indicating equality with the specified <see cref="System.Double"/>.
        /// </summary>
        /// <param name="other">The Double to compare.</param>
        /// <returns></returns>
        public int CompareTo(double other) {

            return this.Value.CompareTo(other);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance.</returns>
        public override int GetHashCode() {

            return this.Value.GetHashCode();
        }

        /// <summary>
        /// Gets the value of this JsonNumber.
        /// </summary>
        public double Value {

            get { return _value; }
        }

        /// <summary>
        /// Determines if the two <see cref="NetServ.Net.Json.JsonNumber"/>s are
        /// equal.
        /// </summary>
        /// <param name="a">The first JsonNumber.</param>
        /// <param name="b">The second JsonNumber.</param>
        /// <returns>True if the JsonNumbers are equal, otherwise; false.</returns>
        public static bool Equals(JsonNumber a, JsonNumber b) {

            object ao = a;
            object bo = b;

            if(ao == bo)
                return true;
            if(ao == null || bo == null)
                return false;

            return a.Value == b.Value;
        }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="a">The first JsonNumber.</param>
        /// <param name="b">The second JsonNumber.</param>
        /// <returns>True if the JsonNumbers are equal, otherwise; false.</returns>
        public static bool operator ==(JsonNumber a, JsonNumber b) {

            return JsonNumber.Equals(a, b);
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="a">The first JsonNumber.</param>
        /// <param name="b">The second JsonNumber.</param>
        /// <returns>True if the JsonNumbers are not equal, otherwise; false.</returns>
        public static bool operator !=(JsonNumber a, JsonNumber b) {

            return !JsonNumber.Equals(a, b);
        }

        /// <summary>
        /// Implict conversion operator.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator JsonNumber(byte value) {

            return new JsonNumber(value);
        }

        /// <summary>
        /// Implict conversion operator.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [CLSCompliant(false)]
        public static implicit operator JsonNumber(sbyte value) {

            return new JsonNumber(value);
        }

        /// <summary>
        /// Implict conversion operator.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator JsonNumber(short value) {

            return new JsonNumber(value);
        }

        /// <summary>
        /// Implict conversion operator.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [CLSCompliant(false)]
        public static implicit operator JsonNumber(ushort value) {

            return new JsonNumber(value);
        }

        /// <summary>
        /// Implict conversion operator.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator JsonNumber(int value) {

            return new JsonNumber(value);
        }

        /// <summary>
        /// Implict conversion operator.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [CLSCompliant(false)]
        public static implicit operator JsonNumber(uint value) {

            return new JsonNumber(value);
        }

        /// <summary>
        /// Implict conversion operator.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator JsonNumber(long value) {

            return new JsonNumber(value);
        }

        /// <summary>
        /// Implict conversion operator.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [CLSCompliant(false)]
        public static implicit operator JsonNumber(ulong value) {

            return new JsonNumber(value);
        }

        /// <summary>
        /// Implict conversion operator.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator JsonNumber(double value) {

            return new JsonNumber(value);
        }

        /// <summary>
        /// Implicit conversion operator.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>This method always returns null.</returns>
        public static implicit operator JsonNumber(JsonNull value) {

            return null;
        }

        /// <summary>
        /// Explicit conversion operator.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator double(JsonNumber value) {

            if(value == null)
                throw new ArgumentNullException();

            return value.Value;
        }

        /// <summary>
        /// Explicit conversion operator.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator byte(JsonNumber value) {

            if(value == null)
                throw new ArgumentNullException();

            return (byte)value.Value;
        }

        /// <summary>
        /// Explicit conversion operator.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [CLSCompliant(false)]
        public static explicit operator sbyte(JsonNumber value) {

            if(value == null)
                throw new ArgumentNullException();

            return (sbyte)value.Value;
        }

        /// <summary>
        /// Explicit conversion operator.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator short(JsonNumber value) {

            if(value == null)
                throw new ArgumentNullException();

            return (short)value.Value;
        }

        /// <summary>
        /// Explicit conversion operator.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [CLSCompliant(false)]
        public static explicit operator ushort(JsonNumber value) {

            if(value == null)
                throw new ArgumentNullException();

            return (ushort)value.Value;
        }

        /// <summary>
        /// Explicit conversion operator.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator int(JsonNumber value) {

            if(value == null)
                throw new ArgumentNullException();

            return (int)value.Value;
        }

        /// <summary>
        /// Explicit conversion operator.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [CLSCompliant(false)]
        public static explicit operator uint(JsonNumber value) {

            if(value == null)
                throw new ArgumentNullException();

            return (uint)value.Value;
        }

        /// <summary>
        /// Explicit conversion operator.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator long(JsonNumber value) {

            if(value == null)
                throw new ArgumentNullException();

            return (long)value.Value;
        }

        /// <summary>
        /// Explicit conversion operator.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [CLSCompliant(false)]
        public static explicit operator ulong(JsonNumber value) {

            if(value == null)
                throw new ArgumentNullException();

            return (ulong)value.Value;
        }

        #endregion 
    }
}
