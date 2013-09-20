// JsonParser.cs
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
using System.Globalization;

namespace NetServ.Net.Json
{
    /// <summary>
    /// Provided support for parsing JavaScript Object Notation data types from
    /// an underlying <see cref="System.IO.TextReader"/>.
    /// </summary>    
    public class JsonParser : Disposable
    {
        #region Private Fields.

        private int _depth;
        private int _maxDepth = JsonParser.DEFAULT_MAX_DEPTH;        
        private readonly bool _ownsReader;        
        private TextReader _rdr;        

        private const int DEFAULT_MAX_DEPTH = 20;

        #endregion

        #region Public Interface.

        /// <summary>
        /// 
        /// </summary>
        [Serializable()]
        public enum TokenType
        {
            /// <summary>
            /// Begin array token.
            /// </summary>
            BeginArray,
            /// <summary>
            /// End array token.
            /// </summary>
            EndArray,
            /// <summary>
            /// Begin object token.
            /// </summary>
            BeginObject,
            /// <summary>
            /// End object token.
            /// </summary>
            EndObject,
            /// <summary>
            /// Member seperator token
            /// </summary>
            ValueSeperator,
            /// <summary>
            /// Object property name / value seperator token.
            /// </summary>
            NameSeperator,
            /// <summary>
            /// Start of string token.
            /// </summary>
            String,
            /// <summary>
            /// Start of number token.
            /// </summary>
            Number,
            /// <summary>
            /// Start of boolean token.
            /// </summary>
            Boolean,
            /// <summary>
            /// Start of null token.
            /// </summary>
            Null,
            /// <summary>
            /// End of input token.
            /// </summary>
            EOF,
        }

        /// <summary>
        /// Initialises a new instance of the JsonParser class and specifies the source
        /// <see cref="System.IO.TextReader"/> and a value indicating if the instance
        /// owns the specified TextReader.
        /// </summary>
        /// <param name="rdr">The underlying TextReader.</param>
        /// <param name="ownsReader">True if this instance owns the TextReader, otherwise; 
        /// false.</param>
        public JsonParser(TextReader rdr, bool ownsReader) {

            if(rdr == null)
                throw new ArgumentNullException("rdr");

            _rdr = rdr;
            _ownsReader = ownsReader;
        }

        /// <summary>
        /// Classifies the next token on the underlying stream.
        /// </summary>
        /// <returns>The classification of the next token from the underlying stream.</returns>
        public TokenType NextToken() {

            CheckDisposed();

            int ch = Peek(true);

            if(ch < 0)
                return TokenType.EOF;
            switch(ch) {
                case JsonWriter.BeginArray:
                    return TokenType.BeginArray;
                case JsonWriter.EndArray:
                    return TokenType.EndArray;
                case JsonWriter.BeginObject:
                    return TokenType.BeginObject;
                case JsonWriter.EndObject:
                    return TokenType.EndObject;
                case JsonWriter.NameSeperator:
                    return TokenType.NameSeperator;
                case JsonWriter.ValueSeperator:
                    return TokenType.ValueSeperator;
                case '"':
                    return TokenType.String;
                case 'n':
                case 'N':
                    return TokenType.Null;
                case 'f':
                case 'F':
                case 't':
                case 'T':
                    return TokenType.Boolean;
                case '.':
                case '-':
                case '+':
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return TokenType.Number;                
                default:
                    throw new FormatException(string.Format(
                        "The character '{0}' does not start any valid token.",
                        ((char)ch).ToString()));
            }
        }

        /// <summary>
        /// Parses a <see cref="NetServ.Net.Json.JsonBoolean"/> from the underlying
        /// stream.
        /// </summary>
        /// <returns>The parsed JsonBoolean.</returns>
        public JsonBoolean ParseBoolean() {
            
            AssertNext(TokenType.Boolean);

            int ch = Peek();

            if(ch == 'f' || ch == 'F') {
                if(Match(JsonBoolean.FalseString))
                    return JsonBoolean.False;
            } else if(Match(JsonBoolean.TrueString))
                return JsonBoolean.True;

            throw new FormatException("The input contains a malformed Json-Boolean.");
        }

        /// <summary>
        /// Parses a <see cref="NetServ.Net.Json.JsonNull"/> from the underlying
        /// stream.
        /// </summary>
        /// <returns>The parsed JsonNull.</returns>
        public JsonNull ParseNull() {
            
            AssertNext(TokenType.Null);
            if(Match(JsonNull.NullString))
                return JsonNull.Null;
            throw new FormatException("The input contains a malformed Json-Null.");
        }

        /// <summary>
        /// Parses a <see cref="NetServ.Net.Json.JsonNumber"/> from the underlying
        /// stream.
        /// </summary>
        /// <returns>The parsed JsonNumber.</returns>
        public JsonNumber ParseNumber() {
            
            AssertNext(TokenType.Number);

            int ch;
            double value;
            StringBuilder sb = new StringBuilder();

            while((ch = Peek()) > -1 && JsonParser.IsNumberComponent(ch))
                sb.Append((char)Read());

            if(double.TryParse(sb.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture,
                out value))
                return new JsonNumber(value);

            throw new FormatException("The input contains a malformed Json-Number.");
        }

        /// <summary>
        /// Parses a <see cref="NetServ.Net.Json.JsonObject"/> and all contained types 
        /// from the underlying stream.
        /// </summary>
        /// <returns>The parsed JsonObject.</returns>
        public virtual JsonObject ParseObject() {

            AssertNext(TokenType.BeginObject);
            AssertDepth(++this.Depth);

            string key;
            TokenType type;
            SpState state = SpState.Initial;
            JsonObject obj = new JsonObject();
            // Move into the object.            
            for(Read(); ; ) {
                switch(NextToken()) {
                    case TokenType.String:
                        key = ParseStringImpl();
                        if(NextToken() != TokenType.NameSeperator)
                            goto error;
                        Read();
                        break;
                    case TokenType.ValueSeperator:
                        if(state != SpState.SepValid)
                            goto error;
                        Read();
                        // Empty members are illegal.
                        state = SpState.ReqValue;
                        continue;
                    case TokenType.EndObject:
                        if(state == SpState.ReqValue)
                            goto error;
                        Read();
                        --this.Depth;
                        return obj;
                    default:
                        goto error;
                }
                switch(type = NextToken()) {
                    case TokenType.EndArray:
                    case TokenType.EndObject:
                    case TokenType.NameSeperator:
                    case TokenType.EOF:
                    case TokenType.ValueSeperator:
                        goto error;
                    default:
                        obj.Add(key, ParseNext(type));
                        state = SpState.SepValid;
                        break;
                }
            }
error:
            throw new FormatException("The input contains a malformed Json-Object.");
        }

        /// <summary>
        /// Parses a <see cref="NetServ.Net.Json.JsonArray"/> and all contained types 
        /// from the underlying stream.
        /// </summary>
        /// <returns>The parsed JsonArray.</returns>
        public virtual JsonArray ParseArray() {

            AssertNext(TokenType.BeginArray);
            AssertDepth(++this.Depth);

            TokenType type;
            SpState state = SpState.Initial;
            JsonArray arr = new JsonArray();
            // Move into the array.            
            for(Read(); ; ) {
                switch(type = NextToken()) {
                    case TokenType.EndArray:
                        if(state == SpState.ReqValue)
                            goto error;
                        Read();
                        --this.Depth;
                        return arr;
                    case TokenType.ValueSeperator:
                        if(state != SpState.SepValid)
                            goto error;
                        Read();
                        // Empty elements are illegal.
                        state = SpState.ReqValue;
                        break;
                    case TokenType.EndObject:
                    case TokenType.EOF:
                    case TokenType.NameSeperator:
                        goto error;
                    default:
                        arr.Add(ParseNext(type));
                        state = SpState.SepValid;
                        break;
                }
            }
error:
            throw new FormatException("The input contains a malformed Json-Array.");
        }

        /// <summary>
        /// Parses a <see cref="NetServ.Net.Json.JsonString"/> from the underlying stream.
        /// </summary>
        /// <returns>The parsed JsonString.</returns>
        public JsonString ParseString() {

            return new JsonString(ParseStringImpl());
        }

        /// <summary>
        /// Parses the next type from the underlying stream.
        /// </summary>
        /// <returns>The next type from the underlying stream.</returns>
        public IJsonType ParseNext() {

            return ParseNext(NextToken());
        }

        /// <summary>
        /// Parses the specified <paramref name="type"/> from the underlying stream.
        /// </summary>
        /// <param name="type">The type to parse.</param>
        /// <returns>The type parsed from the underlying stream.</returns>
        public virtual IJsonType ParseNext(TokenType type) {

            switch(type) {
                case TokenType.BeginArray:
                    return ParseArray();
                case TokenType.BeginObject:
                    return ParseObject();
                case TokenType.String:
                    return ParseString();
                case TokenType.Number:
                    return ParseNumber();
                case TokenType.Boolean:
                    return ParseBoolean();
                case TokenType.Null:
                    return ParseNull();
                default:
                    throw new System.ComponentModel.InvalidEnumArgumentException(
                        "type", type.GetHashCode(), typeof(TokenType));
            }
        }

        /// <summary>
        /// Closes this parser.
        /// </summary>
        public void Close() {

            Dispose(true);
        }

        /// <summary>
        /// Gets the current depth of the parser.
        /// </summary>
        public int Depth {

            get { return _depth; }
            protected set { _depth = value; }
        }

        /// <summary>
        /// Gets or sets the maximum depth of structures before a 
        /// <see cref="System.FormatException"/> is thrown.
        /// </summary>
        public int MaximumDepth {

            get { return _maxDepth; }
            set {
                if(value < 1)
                    throw new ArgumentOutOfRangeException("value");
                _maxDepth = value;
            }
        }

        #endregion

        #region Protected Interface.

        /// <summary>
        /// Peeks at and returns a single character from the underlying stream.
        /// </summary>
        /// <returns>The character, otherwise; -1 if the end of the stream has been reached.</returns>
        protected int Peek() {

            CheckDisposed();

            return this.Reader.Peek();
        }

        /// <summary>
        /// Peeks at the next character from the underlying stream and specifies a value
        /// which indicates whether white space characters should be advanced over.
        /// </summary>
        /// <param name="skipWhite">True to skip white space characters, otherwise; false.</param>
        /// <returns>The next character from the underlying stream, or -1 if the end
        /// has been reached.</returns>
        protected int Peek(bool skipWhite) {

            CheckDisposed();

            if(!skipWhite)
                return this.Reader.Peek();

            int ch;

            while((ch = this.Reader.Peek()) > 0) {
                if(!char.IsWhiteSpace((char)ch))
                    return ch;
                this.Reader.Read();
            }

            return -1;
        }

        /// <summary>
        /// Reads and returns a single character from the underlying stream.
        /// </summary>
        /// <returns>The character, otherwise; -1 if the end of the stream has been reached.</returns>
        protected int Read() {

            CheckDisposed();

            return this.Reader.Read();
        }

        /// <summary>
        /// Reads the next character from the underlying stream and specified a value
        /// which indicates whether white space characters should be skipped.
        /// </summary>
        /// <param name="skipWhite">True to skip white space characters, otherwise; false.</param>
        /// <returns>The next character from the underlying stream, or -1 if the end
        /// has been reached.</returns>
        protected int Read(bool skipWhite) {

            CheckDisposed();

            if(!skipWhite)
                return this.Reader.Read();

            int ch;

            while((ch = this.Reader.Read()) > 0) {
                if(!char.IsWhiteSpace((char)ch))
                    return ch;
            }

            return -1;
        }

        /// <summary>
        /// Asserts that the specified depth does not exceed 
        /// <see cref="P:NetServ.Net.Json.JsonParser.MaximumDepth"/>. If the depth has been
        /// exceeded, a <see cref="System.FormatException"/> is thrown.
        /// </summary>
        /// <param name="depth">The depth.</param>
        protected void AssertDepth(int depth) {

            if(depth > this.MaximumDepth)
                throw new FormatException(string.Format(
                    "The maximum depth of {0} has been exceeded.", this.MaximumDepth.ToString()));
        }

        /// <summary>
        /// Disposed of this instance.
        /// </summary>
        /// <param name="disposing">True if being called explicitly, otherwise; false
        /// to indicate being called implicitly by the GC.</param>
        protected override void Dispose(bool disposing) {

            if(!base.IsDisposed) {
                try {
                    if(this.OwnsReader && disposing)
                        ((IDisposable)this.Reader).Dispose();
                } catch {
                } finally {
                    this.Reader = null;                    
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets a value indicating if this instance owns it's 
        /// <see cref="P:NetServ.Net.Json.TextParser.Reader"/>.
        /// </summary>
        protected bool OwnsReader {

            get { return _ownsReader; }
        }

        /// <summary>
        /// Gets the underlying <see cref="System.IO.TextReader"/>.
        /// </summary>
        protected TextReader Reader {

            get { return _rdr; }
            private set { _rdr = value; }
        }

        #endregion     

        #region Private Impl.

        /// <summary>
        /// Structure parse state.
        /// </summary>
        private enum SpState
        {
            /// <summary>
            /// Initial.
            /// </summary>
            Initial,
            /// <summary>
            /// A value is required.
            /// </summary>
            ReqValue,
            /// <summary>
            /// A seperator is currently valid.
            /// </summary>
            SepValid,
        }

        private bool Match(string value) {

            // This method assumes that value is in lower case.

            int ch;

            for(int i = 0; i < value.Length; ++i) {
                if((ch = Read()) < 0 || char.ToLowerInvariant((char)ch) != value[i])
                    return false;
            }

            return true;
        }

        private string ParseStringImpl() {

            AssertNext(TokenType.String);

            int ch;
            StringBuilder sb = new StringBuilder();
            // Move into the string.
            Read();
            while((ch = Read()) > -1) {
                if(ch == '"')
                    return sb.ToString();
                if(ch != '\\')
                    sb.Append((char)ch);
                else {
                    if((ch = Read()) < 0)
                        goto error;
                    switch(ch) {
                        case '"':
                            sb.Append('"');
                            break;
                        case '/':
                            sb.Append('/');
                            break;
                        case '\\':
                            sb.Append('\\');
                            break;
                        case 'b':
                            sb.Append('\b');
                            break;
                        case 'f':
                            sb.Append('\f');
                            break;
                        case 'n':
                            sb.Append('\n');
                            break;
                        case 'r':
                            sb.Append('\r');
                            break;
                        case 't':
                            sb.Append('\t');
                            break;
                        case 'u':
                            sb.Append(ParseUnicode());
                            break;
                        default:
                            goto error;
                    }
                }
            }
error:
            throw new FormatException("The input contains a malformed Json-String.");
        }

        private char ParseUnicode() {

            int ch1 = Read();
            int ch2 = Read();
            int ch3 = Read();
            int ch4 = Read();

            if(ch1 > -1 && ch2 > -1 && ch3 > -1 && ch4 > -1)
                return (char)(JsonParser.FromHex(ch1) << 12 | JsonParser.FromHex(ch2) << 8 |
                    JsonParser.FromHex(ch3) << 4 | JsonParser.FromHex((ch4)));

            throw new FormatException("The input contains a malformed character escape.");
        }

        private static int FromHex(int ch) {

            if(ch >= '0' && ch <= '9')
                return ch - '0';
            if(ch >= 'a' && ch <= 'f')
                return (ch - 'a') + 10;
            if(ch >= 'A' && ch <= 'F')
                return (ch - 'A') + 10;

            throw new FormatException("The input contains a malformed hexadecimal character escape.");
        }

        private static bool IsNumberComponent(int ch) {

            return (ch >= '0' && ch <= '9') || ch == '-' || ch == '+' || ch == '.' ||
                ch == 'e' || ch == 'E';
        }

        private void AssertNext(TokenType type) {

            if(NextToken() != type)
                throw new InvalidOperationException(
                    string.Format("Method must only be called when {0} is the next token.",
                        type.ToString()));
        }

        #endregion
    }
}
