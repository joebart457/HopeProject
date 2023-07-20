using Hope.Compiler.Extensions;
using Hope.Compiler.Utilities;

namespace Hope.Compiler.Models.Instructions
{
    internal class JmpInstruction : InstructionBase
    {
        public string GoToLabel { get; set; } = "";
        public JmpInstruction(string prefix) : base(prefix)
        {
        }

        public override void Compile(CompilationState state)
        {
            state.WriteStream(UniqueIdentifier.ToBytes());
            var location = state.GetLabelLocation(GoToLabel);
            state.WriteStream(location.ToBytes());    
        }

        public override int GetCompiledSize()
        {
            return InstructionUtilities.InstructionPrefixSize + InstructionUtilities.LabelLocationSize;
        }

        public override void RegisterSymbolsAndConstants(CompilationState state)
        {
            return;
        }
    }
}
