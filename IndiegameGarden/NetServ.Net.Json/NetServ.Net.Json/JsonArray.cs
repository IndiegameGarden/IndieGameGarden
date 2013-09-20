// JsonArray.cs
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
using System.Collections.ObjectModel;

namespace NetServ.Net.Json
{
    /// <summary>
    /// Represents a JavaScript Object Notation Array data type which contains a 
    /// collection of <see cref="NetServ.Net.Json.IJsonType"/>s.
    /// </summary>
    [Serializable()]
    public class JsonArray : Collection<IJsonType>, IJsonArray
    {
        #region Protected Interface.

        /// <summary>
        /// Inserts the specified <paramref name="item"/> into this collection at the
        /// specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index at which to insert the item.</param>
        /// <param name="item">The item to insert.</param>
        protected override sealed void InsertItem(int index, IJsonType item) {

            ValidateItem(item);
            base.InsertItem(index, item);
        }

        /// <summary>
        /// Replaces an item at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index of the item to replace.</param>
        /// <param name="item">The replacement item.</param>
        protected override sealed void SetItem(int index, IJsonType item) {

            ValidateItem(item);
            base.SetItem(index, item);
        }

        /// <summary>
        /// Validates the specified <paramref name="item"/> before insertion or 
        /// setting.
        /// </summary>
        /// <param name="item">The item to validate.</param>
        protected virtual void ValidateItem(IJsonType item) {

            // If the user wants to add a null member they should use JsonNull.
            if(item == null)
                throw new ArgumentNullException("item");
        }

        #endregion

        #region Public Interface.
     
        /// <summary>
        /// Inialises a new instance of the JsonArray class.
        /// </summary>
        public JsonArray() {
        }

        /// <summary>
        /// Writes the contents of this Json type using the specified
        /// <see cref="NetServ.Net.Json.IJsonWriter"/>.
        /// </summary>
        /// <param name="writer">The Json writer.</param>
        public void Write(IJsonWriter writer) {

            if(writer == null)
                throw new ArgumentNullException("writer");

            writer.WriteBeginArray();
            foreach(IJsonType jt in this)
                jt.Write(writer);
            writer.WriteEndArray();
        }

        /// <summary>
        /// Adds the specified item to this collection.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void Add(string item) {

            if(string.IsNullOrEmpty(item))
                base.Add(JsonString.Empty);
            else
                base.Add(new JsonString(item));
        }

        /// <summary>
        /// Adds the specified item to this collection.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void Add(bool item) {

            base.Add(JsonBoolean.Get(item));
        }

        /// <summary>
        /// Adds the specified item to this collection.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void Add(byte item) {

            base.Add(new JsonNumber(item));
        }

        /// <summary>
        /// Adds the specified item to this collection.
        /// </summary>
        /// <param name="item">The item to add.</param>
        [CLSCompliant(false)]
        public void Add(sbyte item) {

            base.Add(new JsonNumber(item));
        }

        /// <summary>
        /// Adds the specified item to this collection.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void Add(short item) {

            base.Add(new JsonNumber(item));
        }

        /// <summary>
        /// Adds the specified item to this collection.
        /// </summary>
        /// <param name="item">The item to add.</param>
        [CLSCompliant(false)]
        public void Add(ushort item) {

            base.Add(new JsonNumber(item));
        }

        /// <summary>
        /// Adds the specified item to this collection.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void Add(int item) {

            base.Add(new JsonNumber(item));
        }

        /// <summary>
        /// Adds the specified item to this collection.
        /// </summary>
        /// <param name="item">The item to add.</param>
        [CLSCompliant(false)]
        public void Add(uint item) {

            base.Add(new JsonNumber(item));
        }

        /// <summary>
        /// Adds the specified item to this collection.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void Add(long item) {

            base.Add(new JsonNumber(item));
        }

        /// <summary>
        /// Adds the specified item to this collection.
        /// </summary>
        /// <param name="item">The item to add.</param>
        [CLSCompliant(false)]
        public void Add(ulong item) {

            base.Add(new JsonNumber(item));
        }

        /// <summary>
        /// Adds the specified item to this collection.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void Add(double item) {

            base.Add(new JsonNumber(item));
        }        

        /// <summary>
        /// Returns the JsonTypeCode for this instance.
        /// </summary>
        public JsonTypeCode JsonTypeCode {

            get { return JsonTypeCode.Array; }
        }

        /// <summary>
        /// Implicit conversion operator.
        /// </summary>
        /// <param name="value">JsonNull value.</param>
        /// <returns>This method always returns null.</returns>
        public static implicit operator JsonArray(JsonNull value) {

            return null;
        }

        #endregion
    }
}
