using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope.Compiler.Models.Instructions
{
    internal class LabelInstruction : InstructionBase
    {
        public string Label { get; set; } = "";
        public LabelInstruction(string prefix) : base(prefix)
        {
        }

        public override void Compile(CompilationState state)
        {
            return;
        }

        public override int GetCompiledSize()
        {
            return 0;
        }

        public override void RegisterSymbolsAndConstants(CompilationState state)
        {
            return;
        }

        public override void RegisterLabels(CompilationState state)
        {
            state.RegisterLabel(Label);
        }
    }
}
