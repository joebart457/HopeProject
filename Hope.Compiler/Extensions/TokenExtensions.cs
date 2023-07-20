using Hope.Compiler._Parser.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenizerCore.Model;

namespace Hope.Compiler.Extensions
{
    internal static class TokenExtensions
    {
        public static object ToLiteral(this Token token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));
            if (token.Type == TokenTypes.String) return token.Lexeme;
            if (token.Type == TokenTypes.Integer) return int.Parse(token.Lexeme);
            if (token.Type == TokenTypes.Double) return double.Parse(token.Lexeme);
            throw new Exception($"unable to convert token of type {token.Type} to literal value");
        }
    }
}
