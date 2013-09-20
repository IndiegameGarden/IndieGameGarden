// JsonBoolean.cs
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

namespace NetServ.Net.Json
{
    /// <summary>
    /// Represents a JavaScript Object Notation Boolean data type. This class 
    /// cannot be inherited.
    /// </summary>
    [Serializable()]    
    [DebuggerDisplay("{_value}")]
    public sealed class JsonBoolean : JsonTypeSkeleton, IJsonBoolean
    {
        #region Private Fields.

        private readonly bool _value;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Defines the Json true string. This field is constant.
        /// </summary>
        public const string TrueString = "true";

        /// <summary>
        /// Defines the Json false string. This field is constant.
        /// </summary>
        public const string FalseString = "false";

        /// <summary>
        /// Defines a true JsonBoolean instance. This field is readonly.
        /// </summary>
        public static readonly JsonBoolean True = new JsonBoolean(true);

        /// <summary>
        /// Defines a false JsonBoolean instance. This field is readonly.
        /// </summary>
        public static readonly JsonBoolean False = new JsonBoolean(false);        

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
        /// Returns a <see cref="System.String"/> representation of this JsonBoolean instance.
        /// </summary>
        /// <returns> <see cref="System.String"/> representation of this JsonBoolean instance</returns>
        public override string ToString() {

            return this.Value ? JsonBoolean.TrueString : JsonBoolean.FalseString;
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

            return Equals((JsonBoolean)obj);
        }

        /// <summary>
        /// Returns a indicating whether this instance is equal to the specified
        /// JsonBoolean.
        /// </summary>
        /// <param name="other">The value to compare.</param>
        /// <returns>True if the specified instance is equal to this instance, otherwise;
        /// false.</returns>
        public bool Equals(JsonBoolean other) {

            return other != null && this.Value == other.Value;
        }

        /// <summary>
        /// Returns a indicating whether this instance is equal to the specified
        /// <see cref="NetServ.Net.Json.IJsonBoolean"/>.
        /// </summary>
        /// <param name="other">The value to compare.</param>
        /// <returns>True if the specified instance is equal to this instance, otherwise;
        /// false.</returns>
        public bool Equals(IJsonBoolean other) {

            return other != null && this.Value == other.Value;
        }

        /// <summary>
        /// Returns a indicating whether this instance is equal to the specified
        /// <see cref="System.Boolean"/>.
        /// </summary>
        /// <param name="other">The value to compare.</param>
        /// <returns>True if the specified bool is equal to this instance, otherwise;
        /// false.</returns>
        public bool Equals(bool other) {

            return this.Value == other;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance.</returns>
        public override int GetHashCode() {

            return this.Value.GetHashCode();
        }

        /// <summary>
        /// Gets the value of this JsonBoolean.
        /// </summary>
        public bool Value {

            get { return _value; }
        }

        /// <summary>
        /// Returns a static JsonBoolean instance representing <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A static JsonBoolean instance representing <paramref name="value"/>.
        /// </returns>
        public static JsonBoolean Get(bool value) {

            return value ? JsonBoolean.True : JsonBoolean.False;
        }

        /// <summary>
        /// Determines if the two <see cref="NetServ.Net.Json.JsonBoolean"/>s are
        /// equal.
        /// </summary>
        /// <param name="a">The first JsonBoolean.</param>
        /// <param name="b">The second JsonBoolean.</param>
        /// <returns>True if the JsonBooleans are equal, otherwise; false.</returns>
        public static bool Equals(JsonBoolean a, JsonBoolean b) {

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
        /// <param name="a">The first JsonBoolean.</param>
        /// <param name="b">The second JsonBoolean.</param>
        /// <returns>True if the JsonBooleans are equal, otherwise; false.</returns>
        public static bool operator ==(JsonBoolean a, JsonBoolean b) {

            return JsonBoolean.Equals(a, b);
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="a">The first JsonBoolean.</param>
        /// <param name="b">The second JsonBoolean.</param>
        /// <returns>True if the JsonBooleans are not equal, otherwise; false.</returns>
        public static bool operator !=(JsonBoolean a, JsonBoolean b) {

            return !JsonBoolean.Equals(a, b);
        }

        /// <summary>
        /// Implicit <see cref="System.Boolean"/> conversion operator.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator JsonBoolean(bool value) {

            return JsonBoolean.Get(value);
        }

        /// <summary>
        /// Implicit <see cref="NetServ.Net.Json.JsonNull"/> conversion operator.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>This method always returns null.</returns>
        public static implicit operator JsonBoolean(JsonNull value) {

            return null;
        }

        /// <summary>
        /// Explicit <see cref="NetServ.Net.Json.JsonBoolean"/> conversion operator.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator bool(JsonBoolean value) {

            if(value == null)
                throw new ArgumentNullException();

            return value.Value;
        }

        #endregion 

        #region Private Impl.

        /// <summary>
        /// Initialises a new instance of the JsonBoolean class and specifies the 
        /// value.
        /// </summary>
        /// <param name="value"></param>
        private JsonBoolean(bool value)
            : base(JsonTypeCode.Boolean) {

            _value = value;
        }

        #endregion
    }
}
