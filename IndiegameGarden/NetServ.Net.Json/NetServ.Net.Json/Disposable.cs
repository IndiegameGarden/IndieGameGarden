// DisposableSkeleton.cs
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
    /// Defines a base for a class which implements the Disposable pattern. This class
    /// is abstract.
    /// </summary>    
    public abstract class Disposable : MarshalByRefObject, IDisposable
    {
        #region Private Fields.

        private volatile bool _isDisposed;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Gets a value indicating if this instance has been disposed of.
        /// </summary>
        public bool IsDisposed {

            get { return _isDisposed; }
            private set { _isDisposed = value; }
        }

        #endregion

        #region Protected Interface.

        /// <summary>
        /// Disposes of this instance.
        /// </summary>
        /// <param name="disposing">True if being called explicitly, otherwise; false
        /// to indicate being called implicitly by the GC.</param>
        protected virtual void Dispose(bool disposing) {

            if(!this.IsDisposed) {
                this.IsDisposed = true;
                // No point calling SuppressFinalize if were are being called from
                // the finalizer.
                if(disposing)
                    GC.SuppressFinalize(this);
            }            
        }

        /// <summary>
        /// Helper method to throw a <see cref="System.ObjectDisposedException"/>
        /// if this instance has been disposed of.
        /// </summary>
        protected void CheckDisposed() {

            if(this.IsDisposed)
                throw new ObjectDisposedException(GetType().FullName);
        }

        #endregion

        #region Explicit Interface.

        void IDisposable.Dispose() {

            Dispose(true);
        }

        #endregion
    }
}
