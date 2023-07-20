using Hope.Compiler.Extensions;
using Hope.Compiler.Models.Instructions;
using Hope.Compiler.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using TokenizerCore.Model;

namespace Hope.Compiler.Models
{
    internal class CompilationState
    {
        class LabelReplacementManifest
        {
            public long Location { get; set; }
            public string Label { get; set; }

            public LabelReplacementManifest(long location, string label)
            {
                Location = location;
                Label = label;
            }
        }

        class TokenKey
        {
            public Token Token { get; set; }

            public TokenKey(Token token)
            {
                Token = token;
            }

            public override bool Equals(object? obj)
            {
                if (obj is TokenKey key)
                {
                    return Equals(key.Token.ToLiteral(), Token.ToLiteral()) && key.Token.Type == Token.Type;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Token.Type);
            }
        }

        private Stream _stream;
        private HashSet<string> _symbols = new();
        private HashSet<TokenKey> _constants = new();
        private Dictionary<string, uint> _symbolsManifest = new();
        private Dictionary<TokenKey, uint> _constantsManifest = new();
        private Dictionary<string, long> _labels = new();
        private long _predictedLocation = 0;
        private uint _symbolTableAddress = 0;
        private uint _symbolTableSize = 0;
        private uint _constantsTableAddress = 0;
        private uint _constantsTableSize = 0;

        public CompilationState()
        {
            _stream = new MemoryStream();
        }
        public void WriteStream(ReadOnlySpan<byte> bytes)
        {
            if (!_stream.CanWrite) throw new Exception("stream is closed or unavailable for writing");
            _stream.Write(bytes);
        }

        public long GetLabelLocation(string label)
        {
            if (_labels.TryGetValue(label, out var location)) return location;
            throw new Exception($"label {label} does not exist");
        }

        public uint GetSymbolIdentifier(string symbol)
        {
            if (_symbolsManifest.TryGetValue(symbol, out var identifier)) return identifier;
            throw new Exception($"symbol {symbol} does not exist");
        }

        public uint GetConstantsIdentifier(Token constant)
        {
            if (_constantsManifest.TryGetValue(new TokenKey(constant), out var identifier)) return identifier;
            throw new Exception($"constant {constant.Lexeme} does not exist in value table");
        }

        public void RegisterLabel(string label)
        {
            if (_labels.ContainsKey(label)) throw new Exception($"label {label} is already defined");
            _labels[label] = _predictedLocation;
        }

        public void RegisterSymbol(string symbol)
        {
            if (!_symbols.Contains(symbol))
            {
                _symbols.Add(symbol);
            }
        }

        public void RegisterConstant(Token token)
        {
            var key = new TokenKey(token);
            if (!_constants.Contains(key))
            {
                _constants.Add(key);
            }
        }

        public void Compile(List<InstructionBase> instructions, string outFile)
        {
            Reset();
            DoFirstPass(instructions);
            PopulateMagicNumber();
            CreateSymbolTable();
            CreateConstantsTable();
            PopulateMagicNumber(); // fill in actual address values
            DoSecondPass(instructions);
            DoThirdPass(instructions);
            Output(outFile);
        }

        private void Reset()
        {
            _labels = new();
            _constants = new();
            _symbols = new();
            _symbolsManifest = new();
            _constantsManifest = new();
            _predictedLocation = 0;
            _symbolTableAddress = 0;
            _symbolTableSize = 0;
            _constantsTableAddress = 0;
            _constantsTableSize = 0;
            _stream = new MemoryStream();
        }

        private void DoFirstPass(List<InstructionBase> instructions)
        {
            foreach(var instruction in instructions)
            {
                instruction.RegisterSymbolsAndConstants(this);
            }
        }

        private void DoSecondPass(List<InstructionBase> instructions)
        {
            foreach (var instruction in instructions)
            {
                instruction.RegisterLabels(this);
                _predictedLocation += instruction.GetCompiledSize();
            }
        }

        private void DoThirdPass(List<InstructionBase> instructions)
        {
            foreach(var instruction in instructions)
            {
                instruction.Compile(this);
            }
        }

        private void PopulateMagicNumber()
        {
            _stream.Seek(0, SeekOrigin.Begin);
            int magicNumber = 39;
            _stream.Write(magicNumber.ToBytes());
            _stream.Write(_symbolTableAddress.ToBytes());   
            _stream.Write(_symbolTableSize.ToBytes());   
            _stream.Write(_constantsTableAddress.ToBytes());   
            _stream.Write(_constantsTableSize.ToBytes()); 
            _stream.Write(magicNumber.ToBytes());
            _stream.Seek(0, SeekOrigin.End);
        }

        private void CreateSymbolTable()
        {
            _symbolTableAddress = (uint)_stream.Position; 
            var translatedSymbols = new Dictionary<string, uint>();
            uint offset = 0;
            foreach(var symbol in _symbols)
            {
                _stream.Write(symbol.Length.ToBytes());
                offset += 4;
                _stream.Write(symbol.ToBytes());
                if (offset + symbol.Length < offset) throw new Exception($"symbol table overflow");
                offset += (uint)symbol.Length;
            }
            _symbolsManifest = translatedSymbols;
            _symbolTableSize = offset;
        }

        private void CreateConstantsTable()
        {
            _constantsTableAddress = (uint)_stream.Position;
            var translatedConstants = new Dictionary<TokenKey, uint>();
            uint offset = 0;
            uint valueSize = 0;
            foreach(var key in _constants)
            {
                translatedConstants.Add(key, offset);
                var type = InstructionUtilities.TranslateConstantType(key.Token);
                if (type > 256) throw new Exception("constant type translation must be one byte in length");
                _stream.WriteByte((byte)type);
                offset += 1;
                var value = key.Token.ToLiteral();
                if (value is int intVal)
                {
                    _stream.Write(intVal.ToBytes());
                    valueSize = 4;
                }
                else if (value is double doubleVal)
                {
                    _stream.Write(doubleVal.ToBytes());
                    valueSize = sizeof(double);
                }
                else if (value is string strVal)
                {
                    _stream.Write(strVal.Length.ToBytes());
                    _stream.Write(strVal.ToBytes());
                    valueSize = sizeof(int);
                    valueSize += (uint)strVal.Length;
                }
                else throw new Exception($"constant with type {value.GetType()} is not supported");
                if (offset + valueSize < offset) throw new Exception("constants table overflow");
                offset += valueSize;
            }
            _constantsManifest = translatedConstants;
            _constantsTableSize = offset;
        }

        private void Output(string filename)
        {
            try
            {
                FileStream fs = new FileStream(filename, FileMode.Create);
                _stream.Seek(0, SeekOrigin.Begin);
                _stream.CopyTo(fs);
                _stream.Close();
                fs.Close();
            }catch(Exception ex)
            {
                throw new Exception($"cannot output to {filename}", ex);
            }
        }

    }
}
