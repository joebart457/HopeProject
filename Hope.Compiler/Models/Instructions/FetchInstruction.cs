using Hope.Compiler.Extensions;
using Hope.Compiler.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope.Compiler.Models.Instructions
{
    internal class FetchInstruction : InstructionBase
    {
        public string LocalIdentifier { get; set; } = "";
        public FetchInstruction(string prefix) : base(prefix)
        {
        }

        public override void Compile(CompilationState state)
        {
            state.WriteStream(UniqueIdentifier.ToBytes());
            var identifier = state.GetSymbolIdentifier(LocalIdentifier);
            state.WriteStream(identifier.ToBytes());
        }

        public override int GetCompiledSize()
        {
            return InstructionUtilities.InstructionPrefixSize + InstructionUtilities.IdentifierKeySize;
        }

        public override void RegisterSymbolsAndConstants(CompilationState state)
        {
            state.RegisterSymbol(LocalIdentifier);
        }
    }
}
