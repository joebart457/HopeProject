using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope.Compiler._Parser.Constants
{
    public static class TokenTypes
    {
       
        public const string Comma = "Comma";    
        public const string SemiColon = "SemiColon";

        public const string pop   = "pop";
        public const string mul   = "mul";
        public const string div   = "div";
        public const string add   = "add";
        public const string sub   = "sub";
        public const string gt    = "gt";
        public const string gte   = "gte";
        public const string lt    = "lt";
        public const string lte   = "lte";
        public const string eq    = "eq";
        public const string neq   = "neq";
        public const string jmp   = "jmp";
        public const string jz    = "jz";
        public const string jnz   = "jnz";
        public const string call  = "call";
        public const string fetch = "fetch";
        public const string push_const = "const";
        public const string end = "end";

        public const string label = "label";

        // Built-in Types
        public const string Space = "Space";
        public const string Tab = "Tab";
        public const string CarriageReturn = "CarriageReturn";
        public const string LineFeed = "LineFeed";

        public const string Word = "TTWord";
        public const string String = "TTString";
        public const string Integer = "TTInteger";
        public const string UnsignedInteger = "TTUnsignedInteger";
        public const string Double = "TTDouble";
        public const string Float = "TTFloat";

        public const string EndOfLineComment = "EndOfLineComment";
        public const string MultiLineComment = "MultiLineComment";

        public const string EndOfFile = "EndOfFile";
    }
}
