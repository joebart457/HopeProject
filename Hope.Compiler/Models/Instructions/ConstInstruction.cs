using Hope.Compiler.Extensions;
using Hope.Compiler.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenizerCore.Model;

namespace Hope.Compiler.Models.Instructions
{
    internal class ConstInstruction : InstructionBase
    {
        public Token Value { get; set; } = new Token("", "", 0, 0);
        public ConstInstruction(string prefix) : base(prefix)
        {
        }

        public override void Compile(CompilationState state)
        {
            state.WriteStream(UniqueIdentifier.ToBytes());
            var identifier = state.GetConstantsIdentifier(Value);
            state.WriteStream(identifier.ToBytes());
        }

        public override int GetCompiledSize()
        {
            return InstructionUtilities.InstructionPrefixSize + InstructionUtilities.IdentifierKeySize;
        }

        public override void RegisterSymbolsAndConstants(CompilationState state)
        {
            state.RegisterConstant(Value);
        }
    }
}
