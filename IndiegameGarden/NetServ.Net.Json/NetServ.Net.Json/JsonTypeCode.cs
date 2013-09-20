// JsonTypeCode.cs
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
    /// Defines the different types of Json structures and primitives.
    /// </summary>
    [Serializable()]
    public enum JsonTypeCode
    {
        /// <summary>
        /// A unicode encoded string.
        /// </summary>
        String,
        /// <summary>
        /// A number.
        /// </summary>
        Number,
        /// <summary>
        /// A boolean value represented by literal "true" and "false".
        /// </summary>
        Boolean,
        /// <summary>
        /// A null value.
        /// </summary>
        Null,
        /// <summary>
        /// A structured object containing zero or more name/value pairs, delimited by 
        /// curly brackets.
        /// </summary>
        Object,
        /// <summary>
        /// An unordered collection of values, delimted by square brackets.
        /// </summary>
        Array
    }
}
