using Hope.Compiler._Parser.Constants;
using Hope.Compiler.Models.Instructions;
using TokenizerCore;
using TokenizerCore.Model;

namespace Hope.Compiler._Parser
{
    internal class Parser: ParserLite.TokenParser
    {
        private readonly Tokenizer _tokenizer;

        public Parser()
        {
            _tokenizer = Tokenizers.Default;
        }

        private void Init(string fileName)
        {
            string data = File.ReadAllText(fileName);
            var tokens = _tokenizer.Tokenize(data).Where(x => x.Type != TokenTypes.Comma && x.Type != TokenTypes.EndOfFile);
            Initialize(tokens.ToList());
        }

        public List<InstructionBase> Parse(string fileName, List<ParsingRule> rules)
        {

            var instructions = new List<InstructionBase>();
            Init(fileName);
            
            while (!AtEnd())
            {
                bool matched = false;

                foreach (var rule in rules)
                {
                    if (rule.CanParse(this))
                    {
                        instructions.Add(rule.Parse(this));
                        matched = true;
                    }
                }
                if (matched) continue;
                if (!AtEnd()) throw new Exception($"unexpected token {Current()}");
            }
            return instructions;
        }
    }
}
