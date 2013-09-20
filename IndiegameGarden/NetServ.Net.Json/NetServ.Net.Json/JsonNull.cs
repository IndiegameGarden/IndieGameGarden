// JsonNull.cs
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
using System.IO;
using System.Diagnostics;
using System.Security.Permissions;
using System.Runtime.Serialization;

namespace NetServ.Net.Json
{
    /// <summary>
    /// Represents a Json null value. This class cannot be inherited.
    /// </summary>
    [Serializable()]    
    [DebuggerDisplay("JsonNull")]
    public sealed class JsonNull : JsonTypeSkeleton, IJsonNull, IObjectReference
    {
        #region Public Interface.

        /// <summary>
        /// Defines the JsonNull string. This field is constant.
        /// </summary>
        public const string NullString = "null";

        /// <summary>
        /// Defines a JsonNull instance. This field is readonly.
        /// </summary>
        public static readonly JsonNull Null = new JsonNull();        

        /// <summary>
        /// Writes the contents of this Json type using the specified
        /// <see cref="NetServ.Net.Json.IJsonWriter"/>.
        /// </summary>
        /// <param name="writer">The Json writer.</param>
        public override void Write(IJsonWriter writer) {

            if(writer == null)
                throw new ArgumentNullException("writer");

            writer.WriteValue(JsonNull.NullString);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> representation of this JsonNull instance.
        /// </summary>
        /// <returns> <see cref="System.String"/> representation of this JsonNull instance.</returns>
        public override string ToString() {

            return JsonNull.NullString;
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

            return Equals((JsonNull)obj);
        }

        /// <summary>
        /// Returns a indicating whether this instance is equal to the specified
        /// JsonNull.
        /// </summary>
        /// <param name="other">The value to compare.</param>
        /// <returns>True if the specified instance is equal to this instance, otherwise;
        /// false.</returns>
        public bool Equals(JsonNull other) {

            // Should I make a null JsonNull equal to this regardless?
            return other != null;
        }

        /// <summary>
        /// Returns a indicating whether this instance is equal to the specified
        /// <see cref="NetServ.Net.Json.IJsonNull"/>.
        /// </summary>
        /// <param name="other">The value to compare.</param>
        /// <returns>True if the specified instance is equal to this instance, otherwise;
        /// false.</returns>
        public bool Equals(IJsonNull other) {

            // Should I make a null IJsonNull equal to this regardless?
            return other != null;
        }   

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance.</returns>
        public override int GetHashCode() {            

            // We do not want to return zero as that would make this equal
            // with a false JsonBoolean.
            return 0x3641694F;            
        }             

        /// <summary>
        /// Maps the specified value to the type of the type paramater.
        /// </summary>
        /// <typeparam name="T">The type to map to.</typeparam>
        /// <param name="value">The value to map.</param>
        /// <returns>The mapped value if not logically null, otherwise the default value of 
        /// <typeparamref name="T"/>.</returns>
        public static T Map<T>(IJsonType value) where T : IJsonType {

            if(value == null || value.JsonTypeCode == JsonTypeCode.Null)
                return default(T);
            return (T)value;
        }

        #endregion

        #region Explicit Interface.

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        object IObjectReference.GetRealObject(StreamingContext context) {

            return JsonNull.Null;
        }

        #endregion

        #region Private Impl.

        /// <summary>
        /// Initialises a new instance of the JsonNull class.
        /// </summary>
        private JsonNull()
            : base(JsonTypeCode.Null) {
        }

        #endregion
    }
}
