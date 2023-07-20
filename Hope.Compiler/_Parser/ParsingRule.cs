using Hope.Compiler._Parser.Constants;
using Hope.Compiler.Models.Enums;
using Hope.Compiler.Models.Instructions;
using ParserLite;
using System.Linq.Expressions;
using System.Reflection;

namespace Hope.Compiler._Parser
{
    internal class ParsingRule
    {
        private readonly Type _type;
        private List<ArgumentRule> _argumentRules = new();

        private string _prefix;
        public ParsingRule(Type type, string prefix)
        {
            _type = type;
            _prefix = prefix;
        }

        public void AddArgument(ArgumentType type, PropertyInfo property)
        {
            _argumentRules.Add(new ArgumentRule(type, property));
        }

        public bool CanParse(TokenParser parser)
        {
            return parser.PeekMatch(0, _prefix);
        }
        public InstructionBase Parse(TokenParser parser)
        {
            parser.Consume(_prefix, $"expect {_prefix}");
            var ins = (InstructionBase?)Activator.CreateInstance(_type, _prefix);
            if (ins == null) throw new Exception("error creating instruction object");
            foreach(var rule in _argumentRules)
            {
                if (rule.Type == ArgumentType.CodeLabel)
                {
                    var gotoLabel = parser.Consume(TokenTypes.Word, "expect a label").Lexeme;
                    rule.FillProperty(ins, gotoLabel);
                }
                else if (rule.Type == ArgumentType.IdentifierLocation)
                {
                    var identifier = parser.Consume(TokenTypes.Word, "expect an identifier").Lexeme;
                    rule.FillProperty(ins, identifier);
                }
                else if (rule.Type == ArgumentType.FunctionIdentifier)
                {
                    var funcIdentifier = parser.Consume(TokenTypes.String, "expect a function identifier").Lexeme;
                    rule.FillProperty(ins, funcIdentifier);
                }
                else if (rule.Type == ArgumentType.ConstantValue)
                {
                    var token = parser.Current();
                    parser.Advance();
                    rule.FillProperty(ins, token);
                }
                else throw new Exception($"unsupported argument type {rule.Type}");
            }
            return ins;
        }
    }
}
