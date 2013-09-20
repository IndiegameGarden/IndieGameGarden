// JsonTypeSkeleton.cs
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
    /// Defines a base class for most Json types. This class is abstract.
    /// </summary>
    [Serializable()]
    public abstract class JsonTypeSkeleton : IJsonType
    {
        #region Private Fields.

        private readonly JsonTypeCode _typeCode;

        #endregion

        #region Protected Interface.

        /// <summary>
        /// Initialises a new instance of the JsonTypeSkeleton class and specifies the 
        /// type code.
        /// </summary>
        /// <param name="typeCode">The type code.</param>
        protected JsonTypeSkeleton(JsonTypeCode typeCode) {

            _typeCode = typeCode;
        }

        #endregion

        #region Public Interface.

        /// <summary>
        /// When overriden in a derived class; writes the contents of the Json type 
        /// to the specified <see cref="NetServ.Net.Json.IJsonWriter"/>.
        /// </summary>
        /// <param name="writer">The Json writer.</param>
        public abstract void Write(IJsonWriter writer);

        /// <summary>
        /// Gets the type code of this Json type.
        /// </summary>
        public JsonTypeCode JsonTypeCode {

            get { return _typeCode; }
        }

        #endregion
    }
}
