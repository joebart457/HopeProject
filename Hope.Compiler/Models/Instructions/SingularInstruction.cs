using Hope.Compiler.Extensions;
using Hope.Compiler.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope.Compiler.Models.Instructions
{
    internal class SingularInstruction : InstructionBase
    {
        public SingularInstruction(string prefix) : base(prefix)
        {
        }

        public override void Compile(CompilationState state)
        {
            var uniqueIdentifier = InstructionUtilities.TranslateInstruction(Prefix);
            state.WriteStream(uniqueIdentifier.ToBytes());
        }

        public override int GetCompiledSize()
        {
            return InstructionUtilities.InstructionPrefixSize;
        }

        public override void RegisterSymbolsAndConstants(CompilationState state)
        {
            return;
        }
    }
}
