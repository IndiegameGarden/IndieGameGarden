// JsonTypeFactory.cs
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

// See comment in IJsonTypeFactory.cs.

#if NOT_USED

using System;

namespace NetServ.Net.Json
{
    /// <summary>
    /// Represents the default type factory for the <see cref="NetServ.Net.Json"/>
    /// namespace.
    /// </summary>
    public sealed class JsonTypeFactory : IJsonTypeFactory
    {
        #region Public Interface.

        /// <summary>
        /// Gets the default instance of the factory.
        /// </summary>
        public static readonly JsonTypeFactory Instance = new JsonTypeFactory();        

        /// <summary>
        /// Creates a <see cref="NetServ.Net.Json.JsonObject"/>.
        /// </summary>
        /// <returns>A <see cref="NetServ.Net.Json.JsonObject"/>.</returns>
        public JsonObject CreateObject() {

            return new JsonObject();
        }

        /// <summary>
        /// Creates a <see cref="NetServ.Net.Json.JsonArray"/>.
        /// </summary>
        /// <returns>A <see cref="NetServ.Net.Json.JsonArray"/>.</returns>
        public JsonArray CreateArray() {

            return new JsonArray();
        }

        /// <summary>
        /// Creates a <see cref="NetServ.Net.Json.JsonString"/> representing
        /// the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>a <see cref="NetServ.Net.Json.JsonString"/> representing
        /// the specified <paramref name="value"/></returns>
        public JsonString CreateString(string value) {

            if(value == null)
                return null;
            if(value.Equals(string.Empty))
                return JsonString.Empty;
            return new JsonString(value);
        }

        /// <summary>
        /// Creates a <see cref="NetServ.Net.Json.JsonNumber"/> representing
        /// the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>a <see cref="NetServ.Net.Json.JsonNumber"/> representing
        /// the specified <paramref name="value"/></returns>
        public JsonNumber CreateNumber(double value) {

            return new JsonNumber(value);
        }

        /// <summary>
        /// Creates a <see cref="NetServ.Net.Json.JsonBoolean"/> representing
        /// the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>a <see cref="NetServ.Net.Json.JsonBoolean"/> representing
        /// the specified <paramref name="value"/></returns>
        public JsonBoolean CreateBoolean(bool value) {

            return JsonBoolean.Get(value);
        }

        /// <summary>
        /// Creates a <see cref="NetServ.Net.Json.JsonNull"/>.
        /// </summary>
        /// <returns>A <see cref="NetServ.Net.Json.JsonNull"/>.</returns>
        public JsonNull CreateNull() {

            return JsonNull.Null;
        }

        #endregion

        #region Explict Interface.

        IJsonObject IJsonTypeFactory.CreateObject() {

            return CreateObject();
        }

        IJsonArray IJsonTypeFactory.CreateArray() {

            return CreateArray();
        }

        IJsonString IJsonTypeFactory.CreateString(string value) {

            return CreateString(value);
        }

        IJsonNumber IJsonTypeFactory.CreateNumber(double value) {

            return CreateNumber(value);
        }

        IJsonBoolean IJsonTypeFactory.CreateBoolean(bool value) {

            return CreateBoolean(value);
        }

        IJsonNull IJsonTypeFactory.CreateNull() {

            return CreateNull();
        }

        #endregion

        #region Private Impl.

        private JsonTypeFactory() {
        }

        #endregion
    }  
}

#endif