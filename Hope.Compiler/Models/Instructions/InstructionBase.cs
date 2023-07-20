using Hope.Compiler.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope.Compiler.Models.Instructions
{
    internal abstract class InstructionBase
    {
        public string Prefix { get; set; }
        public int UniqueIdentifier { get; set; }

        public InstructionBase(string prefix)
        {
            Prefix = prefix;
            UniqueIdentifier = InstructionUtilities.TranslateInstruction(prefix);
        }

        public abstract int GetCompiledSize();
        public abstract void RegisterSymbolsAndConstants(CompilationState state);
        public virtual void RegisterLabels(CompilationState state) { }
        public abstract void Compile(CompilationState state);
    }
}
