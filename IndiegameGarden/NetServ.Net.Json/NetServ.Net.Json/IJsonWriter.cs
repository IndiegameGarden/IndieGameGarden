// IJsonEncoder.cs
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

namespace NetServ.Net.Json
{
    /// <summary>
    /// Defines a JavaScript Object Notation writer.
    /// </summary>
    public interface IJsonWriter
    {
        /// <summary>
        /// Writes the start of an array to the underlying data stream.
        /// </summary>
        void WriteBeginArray();        

        /// <summary>
        /// Writes the end of an array to the underlying data stream.
        /// </summary>
        void WriteEndArray();

        /// <summary>
        /// Writes the start of an object to the underlying data stream.
        /// </summary>
        void WriteBeginObject();        

        /// <summary>
        /// Writes the end of an object to the underlying data stream.
        /// </summary>
        void WriteEndObject();

        /// <summary>
        /// Writes a object property name to the underlying data stream.
        /// </summary>
        /// <param name="value">The property name.</param>
        void WriteName(string value);

        /// <summary>
        /// Writes a raw string value to the underlying data stream.
        /// </summary>
        /// <param name="value">The string to write.</param>
        void WriteValue(string value);        
    }
}
