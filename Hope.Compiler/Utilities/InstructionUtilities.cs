using Hope.Compiler.Constants;
using TokenizerCore.Model;

namespace Hope.Compiler.Utilities
{
    internal static class InstructionUtilities
    {
        public static int TranslateInstruction(string instruction)
        {
            if (!CompilationConstants.Instructions.TryGetValue(instruction, out var result)) throw new Exception($"instruction {instruction} does not have a valid translation");
            return result;
        }


        public static int TranslateConstantType(Token constantToken)
        {
            if (!CompilationConstants.DataTypes.TryGetValue(constantToken.Type, out var result)) throw new Exception($"datatype {constantToken.Type} does not have a valid translation");
            return result;
        }


        public static int TranslateFunction(string functionName)
        {
            if (!CompilationConstants.FunctionSymbols.TryGetValue(functionName, out var result)) throw new Exception($"function {functionName} is not defined in the standard library");
            return result;
        }

        public static int InstructionPrefixSize = 4;
        public static int IdentifierKeySize = 4;
        public static int LabelLocationSize = sizeof(long);
    }
}
