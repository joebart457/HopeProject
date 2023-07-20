using Hope.Compiler._Parser.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenizerCore.Model;

namespace Hope.Compiler.Utilities
{
    internal static class InstructionUtilities
    {
        private static Dictionary<string, int> _instructions = new()
        {
            {"label", -1}, // unused
            {"add", 1 },
            {"sub", 2 },
            {"mul", 3 },
            {"div", 4 },
            {"gte", 5 },
            {"gt", 6 },
            {"lte", 7 },
            {"lt", 8 },
            {"neq", 9 },
            {"eq", 10},
            {"jmp", 11},
            {"jnz", 12},
            {"jz", 13},
            {"pop", 14},
            {"call", 15},
            {"const", 16},
            {"fetch", 17},
            {"end", 18},
        };

        private static Dictionary<string, int> _dataTypes = new()
        {
            {TokenTypes.Integer, 3},
            {TokenTypes.String, 8},
            {TokenTypes.Double, 9},
        };

        private static Dictionary<string, int> _functionSymbols = new()
        {
            {"Console.Log", 1},
            {"Console.LogInt", 2},
        };
        public static int TranslateInstruction(string instruction)
        {
            if (!_instructions.TryGetValue(instruction, out var result)) throw new Exception($"instruction {instruction} does not have a valid translation");
            return result;
        }


        public static int TranslateConstantType(Token constantToken)
        {
            if (!_dataTypes.TryGetValue(constantToken.Type, out var result)) throw new Exception($"datatype {constantToken.Type} does not have a valid translation");
            return result;
        }


        public static int TranslateFunction(string functionName)
        {
            if (!_functionSymbols.TryGetValue(functionName, out var result)) throw new Exception($"function {functionName} is not defined in the standard library");
            return result;
        }

        public static int InstructionPrefixSize = 4;
        public static int IdentifierKeySize = 4;
        public static int LabelLocationSize = sizeof(long);
    }
}
