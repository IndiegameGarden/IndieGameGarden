// JsonTokenType.cs
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
    /// Defines the high level Json tokens.
    /// </summary>
    [Serializable()]
    public enum JsonTokenType
    {
        /// <summary>
        /// No token.
        /// </summary>
        None = 0,
        /// <summary>
        /// The start of array token.
        /// </summary>
        BeginArray = 1,
        /// <summary>
        /// The end of array token.
        /// </summary>
        EndArray = 2,
        /// <summary>
        /// The start of object token.
        /// </summary>
        BeginObject = 3,
        /// <summary>
        /// The end of object token.
        /// </summary>
        EndObject = 4,
        /// <summary>
        /// An object property name token.
        /// </summary>
        Name = 5,
        /// <summary>
        /// A value token.
        /// </summary>
        Value = 6,
    }

}
