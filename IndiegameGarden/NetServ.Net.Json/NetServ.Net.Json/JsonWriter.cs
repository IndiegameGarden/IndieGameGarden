// JsonWriter.cs
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
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace NetServ.Net.Json
{
    /// <summary>
    /// Provided support for writing JavaScript Object Notation data types to an
    /// underlying <see cref="System.IO.TextWriter"/>.
    /// </summary>    
    public class JsonWriter : Disposable, IJsonWriter
    {
        #region Private Fields.

        private readonly bool _ownsWriter;
        private TextWriter _writer;
        private JsonTokenType _token;
        private Stack<JsonStructType> _structStack;

        // Table is indexed by JsonTokenType. See PreWrite.
        // Note: jagged arrays are more efficient than multi-dimensional arrays.
        private static readonly ST[][] TRANSITION_TABLE = 
            {
                new ST[] { ST.ERR, ST.SOK, ST.ERR, ST.SOK, ST.ERR, ST.ERR, ST.ERR },
                new ST[] { ST.ERR, ST.SOK, ST.AIA, ST.SOK, ST.ERR, ST.ERR, ST.SOK },
                new ST[] { ST.ERR, ST.AIS, ST.AIA, ST.AIS, ST.AIO, ST.AIO, ST.AIS },
                new ST[] { ST.ERR, ST.ERR, ST.ERR, ST.ERR, ST.AIO, ST.SOK, ST.ERR },
                new ST[] { ST.ERR, ST.AIS, ST.AIA, ST.AIS, ST.AIO, ST.AIO, ST.AIS },
                new ST[] { ST.ERR, ST.SOK, ST.ERR, ST.SOK, ST.ERR, ST.ERR, ST.SOK },
                new ST[] { ST.ERR, ST.SOK, ST.AIA, ST.SOK, ST.AIO, ST.AIO, ST.SOK }
            };

        #endregion

        #region Public Interface.

        /// <summary>
        /// Defines the start of an array. This field is constant.
        /// </summary>
        public const char BeginArray = '[';

        /// <summary>
        /// Defines the end of an array. This field is constant.
        /// </summary>
        public const char EndArray = ']';

        /// <summary>
        /// Defines the start of an object. This field is constant.
        /// </summary>
        public const char BeginObject = '{';

        /// <summary>
        /// Defines the end of an object. This field is constant.
        /// </summary>
        public const char EndObject = '}';

        /// <summary>
        /// Defines the value seperator. This field is constant.
        /// </summary>
        public const char ValueSeperator = ',';

        /// <summary>
        /// Defines the object property name and value seperator. This field is constant.
        /// </summary>       
        public const char NameSeperator = ':';        

        /// <summary>
        /// Initialises a new instance of then JsonWriter class using a
        /// <see cref="System.IO.StringWriter"/> as the underlying 
        /// <see cref="System.IO.TextWriter"/>.
        /// </summary>
        public JsonWriter()
            : this(new StringWriter(), true) {
        }

        /// <summary>
        /// Initialises a new instance of the JsonWriter class and specifies
        /// the underlying <see cref="System.IO.TextWriter"/> and a value indicating
        /// if the instance owns the specified TextWriter.
        /// </summary>
        /// <param name="writer">The underlying text writer.</param>
        /// <param name="ownsWriter">True if this instance owns the specified TextWriter,
        /// otherwise; false.</param>
        public JsonWriter(TextWriter writer, bool ownsWriter) {

            if(writer == null)
                throw new ArgumentNullException("writer");

            _writer = writer;
            _ownsWriter = ownsWriter;
            _token = JsonTokenType.None;            
            _structStack = new Stack<JsonStructType>();
        }        

        /// <summary>
        /// Writes the start of an array to the underlying data stream.
        /// </summary>
        public void WriteBeginArray() {

            CheckDisposed();
            PreWrite(JsonTokenType.BeginArray);
            this.Writer.Write(JsonWriter.BeginArray);
            PostWrite(JsonTokenType.BeginArray);
        }       

        /// <summary>
        /// Writes the end of an array to the underlying data stream.
        /// </summary>
        public void WriteEndArray() {

            CheckDisposed();
            PreWrite(JsonTokenType.EndArray);
            this.Writer.Write(JsonWriter.EndArray);
            PostWrite(JsonTokenType.EndArray);
        }

        /// <summary>
        /// Writes the start of an object to the underlying data stream.
        /// </summary>
        public void WriteBeginObject() {

            CheckDisposed();
            PreWrite(JsonTokenType.BeginObject);
            this.Writer.Write(JsonWriter.BeginObject);
            PostWrite(JsonTokenType.BeginObject);
        }

        /// <summary>
        /// Writes the end of an object to the underlying data stream.
        /// </summary>
        public void WriteEndObject() {

            CheckDisposed();
            PreWrite(JsonTokenType.EndObject);
            this.Writer.Write(JsonWriter.EndObject);
            PostWrite(JsonTokenType.EndObject);
        }

        /// <summary>
        /// Writes a object property name to the underlying data stream.
        /// </summary>
        /// <param name="value">The property name.</param>
        public void WriteName(string value) {

            if(value == null)
                throw new ArgumentNullException("value");

            CheckDisposed();
            PreWrite(JsonTokenType.Name);
            this.Writer.Write(JsonString.Encode(value));
            this.Writer.Write(JsonWriter.NameSeperator);
            PostWrite(JsonTokenType.Name);
        }

        /// <summary>
        /// Writes a raw string value to the underlying data stream.
        /// </summary>
        /// <param name="value">The string to write.</param>
        public void WriteValue(string value) {

            if(value == null)
                throw new ArgumentNullException("value");

            CheckDisposed();
            PreWrite(JsonTokenType.Value);
            this.Writer.Write(value);
            PostWrite(JsonTokenType.Value);
        }

        /// <summary>
        /// Closes this writer.
        /// </summary>
        public void Close() {

            Dispose(true);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> representation of this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> representation of this instance.</returns>
        public override string ToString() {

            CheckDisposed();

            return this.Writer.ToString();
        }

        #endregion

        #region Protected Interface.

        /// <summary>
        /// Performs any assertions and / or write operations needed before the specified
        /// token is written to the underlying stream.
        /// </summary>
        /// <param name="token">The next token to be written.</param>
        protected virtual void PreWrite(JsonTokenType token) {            

            switch(JsonWriter.TRANSITION_TABLE[(int)this.CurrentToken][(int)token]) {
                case ST.SOK:
                    // void.
                    break;
                case ST.ERR:
                    Assert(false, token);
                    break;
                case ST.AIO:
                    Assert(this.CurrentStruct == JsonStructType.Object, token);
                    break;
                case ST.AIA:
                    Assert(this.CurrentStruct == JsonStructType.Array, token);
                    break;
                case ST.AIS:
                    Assert(this.CurrentStruct != JsonStructType.None, token);
                    break;
                default:
                    Debug.Fail("JsonWriter::PreWrite - Unknown token.");
                    break;
            }
            // This is horrible but without increasing the complexity of the state
            // table it is needed.
            Assert(!(this.CurrentStruct == JsonStructType.Object && token != JsonTokenType.EndObject &&
                this.CurrentToken != JsonTokenType.Name && token != JsonTokenType.Name), token);
            // See if we should write a seperator.
            if(!JsonWriter.IsStructEnd(token) && (JsonWriter.IsStructEnd(this.CurrentToken) ||
                this.CurrentToken == JsonTokenType.Value))
                this.Writer.Write(JsonWriter.ValueSeperator);
        }

        /// <summary>
        /// Performs any post write operations needed after the specified
        /// token has been written to the underlying stream.
        /// </summary>
        /// <param name="token">The token written.</param>
        protected virtual void PostWrite(JsonTokenType token) {

            this.CurrentToken = token;
            switch(token) {
                case JsonTokenType.BeginArray:
                    this.StructStack.Push(JsonStructType.Array);
                    break;
                case JsonTokenType.BeginObject:
                    this.StructStack.Push(JsonStructType.Object);
                    break;
                case JsonTokenType.EndArray:
                    Debug.Assert(this.CurrentStruct == JsonStructType.Array);
                    this.StructStack.Pop();
                    break;
                case JsonTokenType.EndObject:
                    Debug.Assert(this.CurrentStruct == JsonStructType.Object);
                    this.StructStack.Pop();
                    break;
                default:
                    break;
            }

        }

        /// <summary>
        /// Disposed of this instance.
        /// </summary>
        /// <param name="disposing">True if being called explicitly, otherwise; false
        /// to indicate being called implicitly by the GC.</param>
        protected override void Dispose(bool disposing) {

            if(!base.IsDisposed) {
                try {
                    if(this.OwnsWriter && disposing)
                        ((IDisposable)this.Writer).Dispose();
                } catch {
                } finally {
                    this.Writer = null;                    
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets or sets the current lexical JsonToken.
        /// </summary>
        protected JsonTokenType CurrentToken {

            get { return _token; }
            set { _token = value; }
        }

        /// <summary>
        /// Peeks at the top structure on the 
        /// <see cref="P:NetServ.Net.Json.JsonWriter.StructStack"/>.
        /// </summary>
        protected JsonStructType CurrentStruct {

            get { 
                return this.StructStack.Count > 0 ? this.StructStack.Peek() : 
                    JsonStructType.None; 
            }
        }

        /// <summary>
        /// Gets the stack of structure types. DO NOT push 
        /// <see cref="NetServ.Net.Json.JsonStructType.None"/> onto the stack.
        /// </summary>
        protected Stack<JsonStructType> StructStack {

            get { return _structStack; }
        }

        /// <summary>
        /// Gets a value indicating if this instance owns it's 
        /// <see cref="P:NetServ.Net.Json.JsonWriter.Writer"/>.
        /// </summary>
        protected bool OwnsWriter {

            get { return _ownsWriter; }
        }

        /// <summary>
        /// Gets the underlying <see cref="System.IO.TextWriter"/>.
        /// </summary>
        protected TextWriter Writer {

            get { return _writer; }
            private set { _writer = value; }
        }  

        #endregion

        #region Internal Interface.

        internal static bool IsStructEnd(JsonTokenType token) {

            return token == JsonTokenType.EndArray || token == JsonTokenType.EndObject;
        }

        internal static bool IsStructStart(JsonTokenType token) {

            return token == JsonTokenType.BeginArray || token == JsonTokenType.BeginObject;
        }  

        #endregion

        #region Private Impl.

        [Serializable()]
        private enum ST
        {
            /// <summary>
            /// State OK.
            /// </summary>
            SOK,
            /// <summary>
            /// Error.
            /// </summary>
            ERR,
            /// <summary>
            /// Assert in object.
            /// </summary>
            AIO,
            /// <summary>
            /// Assert in array.
            /// </summary>
            AIA,
            /// <summary>
            /// Assert in struct.
            /// </summary>
            AIS
        }

        private void Assert(bool cond, JsonTokenType nextToken) {

            if(!cond) {
                StringBuilder sb = new StringBuilder(150);

                sb.Append("Attempted state transition would lead to an invalid JSON output.");
                sb.Append(Environment.NewLine);
                sb.Append("Current Token:\t").Append(this.CurrentToken.ToString());
                sb.Append(Environment.NewLine);
                sb.Append("Attempted Token:\t").Append(nextToken.ToString());
                sb.Append(Environment.NewLine);
                sb.Append("Current Struct:\t").Append(this.CurrentStruct.ToString());
                throw new InvalidOperationException(sb.ToString());
            }
        }

        #endregion
    }
}
