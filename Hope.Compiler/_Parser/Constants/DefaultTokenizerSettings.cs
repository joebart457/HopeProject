using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenizerCore;
using TokenizerCore.Model;

namespace Hope.Compiler._Parser.Constants
{
    internal class Tokenizers
    {
        public static Tokenizer Default { get
            {
                var settings = TokenizerSettings.Default;
                settings.WordIncluded = "@`.$";
                var rules = new List<TokenizerRule>()
                {
                    new TokenizerRule(TokenTypes.pop, "pop", ignoreCase: true),
                    new TokenizerRule(TokenTypes.mul, "mul", ignoreCase: true),
                    new TokenizerRule(TokenTypes.div, "div", ignoreCase: true),
                    new TokenizerRule(TokenTypes.add, "add", ignoreCase: true),
                    new TokenizerRule(TokenTypes.sub, "sub", ignoreCase: true),
                    new TokenizerRule(TokenTypes.gt , "gt", ignoreCase: true),
                    new TokenizerRule(TokenTypes.gte, "gte", ignoreCase: true),
                    new TokenizerRule(TokenTypes.lt , "lt", ignoreCase: true),
                    new TokenizerRule(TokenTypes.lte, "lte", ignoreCase: true),
                    new TokenizerRule(TokenTypes.eq , "eq", ignoreCase: true),
                    new TokenizerRule(TokenTypes.neq, "neq", ignoreCase: true),
                    new TokenizerRule(TokenTypes.jmp, "jmp", ignoreCase: true),
                    new TokenizerRule(TokenTypes.jz , "jz", ignoreCase: true),
                    new TokenizerRule(TokenTypes.jnz, "jnz", ignoreCase: true),
                    new TokenizerRule(TokenTypes.call, "call", ignoreCase: true),
                    new TokenizerRule(TokenTypes.fetch, "fetch", ignoreCase: true),
                    new TokenizerRule(TokenTypes.label, "label", ignoreCase: true),
                    new TokenizerRule(TokenTypes.push_const, "const", ignoreCase: true),
                    new TokenizerRule(TokenTypes.end, "end", ignoreCase: true),
                    new TokenizerRule(TokenTypes.push_scope, "push_scope", ignoreCase: true),
                    new TokenizerRule(TokenTypes.pop_scope, "pop_scope", ignoreCase: true),
                    new TokenizerRule(TokenTypes.Comma, ","),
                    new TokenizerRule(TokenTypes.SemiColon, ";"),
                    new TokenizerRule(TokenTypes.EndOfLineComment, ";"),
                    new TokenizerRule(TokenTypes.String, "\"", enclosingLeft: "\"", enclosingRight: "\""),
                };
                settings.AllowNegatives = true;
                settings.NegativeChar = '-';
                return new Tokenizer(rules, settings);
            } 
        }
    }
}
