// IJsonTypeFactory.cs
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

// The purpose of this interface and the JsonTypeFactory was to allow consumers to
// utilise the JsonParser and have it construct their own types. The reason that it
// is not used is that it made the use of the parser rather messy if one wanted to
// take advantage of the operator overloads defined in most of the NetServ types.
// Also, as I have tried to make the classes as consumer friendly as possible, I 
// couldn't think of any other way that they may implemented. These factors negated 
// the usefulness of this interface and therefore it is not included.

#if NOT_USED

using System;

namespace NetServ.Net.Json
{
    /// <summary>
    /// Defines a factory for JavaScript Object Notation data types.
    /// </summary>
    public interface IJsonTypeFactory
    {
        /// <summary>
        /// Creates a <see cref="NetServ.Net.Json.IJsonObject"/>.
        /// </summary>
        /// <returns>A <see cref="NetServ.Net.Json.IJsonObject"/>.</returns>
        IJsonObject CreateObject();

        /// <summary>
        /// Creates a <see cref="NetServ.Net.Json.IJsonArray"/>.
        /// </summary>
        /// <returns>A <see cref="NetServ.Net.Json.IJsonArray"/>.</returns>
        IJsonArray CreateArray();

        /// <summary>
        /// Creates a <see cref="NetServ.Net.Json.IJsonString"/> representing
        /// the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>a <see cref="NetServ.Net.Json.IJsonString"/> representing
        /// the specified <paramref name="value"/></returns>
        IJsonString CreateString(string value);

        /// <summary>
        /// Creates a <see cref="NetServ.Net.Json.IJsonNumber"/> representing
        /// the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>a <see cref="NetServ.Net.Json.IJsonNumber"/> representing
        /// the specified <paramref name="value"/></returns>
        IJsonNumber CreateNumber(double value);

        /// <summary>
        /// Creates a <see cref="NetServ.Net.Json.IJsonBoolean"/> representing
        /// the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>a <see cref="NetServ.Net.Json.IJsonBoolean"/> representing
        /// the specified <paramref name="value"/></returns>
        IJsonBoolean CreateBoolean(bool value);

        /// <summary>
        /// Creates a <see cref="NetServ.Net.Json.IJsonNull"/>.
        /// </summary>
        /// <returns>A <see cref="NetServ.Net.Json.IJsonNull"/>.</returns>
        IJsonNull CreateNull();
    }
}

#endif