// JsonObject.cs
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
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NetServ.Net.Json
{
    /// <summary>
    /// Represents a JavaScript Object Notation Object data type which contains a 
    /// collection of <see cref="NetServ.Net.Json.IJsonType"/>s accessed by key.
    /// </summary>
    [Serializable()]
    public class JsonObject : Dictionary<string, IJsonType>, IJsonObject
    {
        #region Protected Interface.

        /// <summary>
        /// Deserialisation constructor.
        /// </summary>
        /// <param name="info">The object containing the data needed to deserialise an instance.</param>
        /// <param name="context">The boejct which specifies the source of the deserialisation.</param>
        protected JsonObject(SerializationInfo info, StreamingContext context)
            : base(info, context) {
        }

        #endregion

        #region Public Interface.

        /// <summary>
        /// Inialises a new instance of the JsonObject class.
        /// </summary>
        public JsonObject()
            : base(StringComparer.Ordinal) {
        }

        /// <summary>
        /// Writes the contents of this Json type using the specified
        /// <see cref="NetServ.Net.Json.IJsonWriter"/>.
        /// </summary>
        /// <param name="writer">The Json writer.</param>
        public void Write(IJsonWriter writer) {

            if(writer == null)
                throw new ArgumentNullException("writer");

            writer.WriteBeginObject();
            foreach(KeyValuePair<string, IJsonType> pair in this) {
                writer.WriteName(pair.Key);
                pair.Value.Write(writer);
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Adds the specified key and item to this dictionary.
        /// </summary>
        /// <param name="key">The key of the item</param>
        /// <param name="item">The value of the item.</param>
        public void Add(string key, string item) {

            if(string.IsNullOrEmpty(item))
                base.Add(key, JsonString.Empty);
            else
                base.Add(key, new JsonString(item));
        }

        /// <summary>
        /// Adds the specified key and item to this dictionary.
        /// </summary>
        /// <param name="key">The key of the item</param>
        /// <param name="item">The value of the item.</param>
        public void Add(string key, bool item) {

            base.Add(key, JsonBoolean.Get(item));
        }

        /// <summary>
        /// Adds the specified key and item to this dictionary.
        /// </summary>
        /// <param name="key">The key of the item</param>
        /// <param name="item">The value of the item.</param>
        public void Add(string key, byte item) {

            base.Add(key, new JsonNumber(item));
        }

        /// <summary>
        /// Adds the specified key and item to this dictionary.
        /// </summary>
        /// <param name="key">The key of the item</param>
        /// <param name="item">The value of the item.</param>
        [CLSCompliant(false)]
        public void Add(string key, sbyte item) {

            base.Add(key, new JsonNumber(item));
        }

        /// <summary>
        /// Adds the specified key and item to this dictionary.
        /// </summary>
        /// <param name="key">The key of the item</param>
        /// <param name="item">The value of the item.</param>
        public void Add(string key, short item) {

            base.Add(key, new JsonNumber(item));
        }

        /// <summary>
        /// Adds the specified key and item to this dictionary.
        /// </summary>
        /// <param name="key">The key of the item</param>
        /// <param name="item">The value of the item.</param>
        [CLSCompliant(false)]
        public void Add(string key, ushort item) {

            base.Add(key, new JsonNumber(item));
        }

        /// <summary>
        /// Adds the specified key and item to this dictionary.
        /// </summary>
        /// <param name="key">The key of the item</param>
        /// <param name="item">The value of the item.</param>
        [CLSCompliant(false)]
        public void Add(string key, int item) {

            base.Add(key, new JsonNumber(item));
        }

        /// <summary>
        /// Adds the specified key and item to this dictionary.
        /// </summary>
        /// <param name="key">The key of the item</param>
        /// <param name="item">The value of the item.</param>
        [CLSCompliant(false)]
        public void Add(string key, uint item) {

            base.Add(key, new JsonNumber(item));
        }

        /// <summary>
        /// Adds the specified key and item to this dictionary.
        /// </summary>
        /// <param name="key">The key of the item</param>
        /// <param name="item">The value of the item.</param>
        public void Add(string key, long item) {

            base.Add(key, new JsonNumber(item));
        }

        /// <summary>
        /// Adds the specified key and item to this dictionary.
        /// </summary>
        /// <param name="key">The key of the item</param>
        /// <param name="item">The value of the item.</param>
        [CLSCompliant(false)]
        public void Add(string key, ulong item) {

            base.Add(key, new JsonNumber(item));
        }

        /// <summary>
        /// Adds the specified key and item to this dictionary.
        /// </summary>
        /// <param name="key">The key of the item</param>
        /// <param name="item">The value of the item.</param>
        public void Add(string key, double item) {

            base.Add(key, new JsonNumber(item));
        }        

        /// <summary>
        /// Returns the JsonTypeCode for this instance.
        /// </summary>
        public JsonTypeCode JsonTypeCode {

            get { return JsonTypeCode.Object; }
        }

        /// <summary>
        /// Implicit conversion operator.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>This method always returns null.</returns>
        public static implicit operator JsonObject(JsonNull value) {

            return null;
        }

        #endregion
    }
}
