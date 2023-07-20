using Hope.Compiler.Extensions;
using Hope.Compiler.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Hope.Compiler.Models.Instructions
{
    
    internal class CallInstruction : InstructionBase
    {
        public string FunctionName { get; set; } = "";
        public CallInstruction(string prefix) : base(prefix)
        {
        }

        public override void Compile(CompilationState state)
        {
            state.WriteStream(UniqueIdentifier.ToBytes());
            var functionId = InstructionUtilities.TranslateFunction(FunctionName);
            state.WriteStream(functionId.ToBytes());      
        }

        public override int GetCompiledSize()
        {
            return InstructionUtilities.InstructionPrefixSize + 4; // int32 function identifier
        }

        public override void RegisterSymbolsAndConstants(CompilationState state)
        {
            return;
        }
    }
}
