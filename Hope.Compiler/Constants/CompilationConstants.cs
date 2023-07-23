using Hope.Compiler._Parser.Constants;

namespace Hope.Compiler.Constants
{
    internal static class CompilationConstants
    {
        public static Dictionary<string, int> Instructions => new()
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
            {"push_scope", 20},
            {"pop_scope", 21},
        };

        public static Dictionary<string, int> DataTypes => new()
        {
            {TokenTypes.Integer, 3},
            {TokenTypes.String, 8},
            {TokenTypes.Double, 9},
        };

        public static Dictionary<string, int> FunctionSymbols => new()
        {
            {"Console.Log", 1},
            {"Console.LogInt", 2},
            {"Fs.Exists", 3},
        };
    }
}
