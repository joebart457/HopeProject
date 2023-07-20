using Hope.Compiler._Parser;
using Hope.Compiler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Hope.Compiler
{

    internal class CompilerSettings
    {
        public string InputFilePath { get; set; }
        public string OutputFilePath { get; set; }
        public List<ParsingRule> Rules { get; set; }
        public CompilerSettings(string inputFilePath, string outputFilePath, List<ParsingRule> rules)
        {
            InputFilePath = inputFilePath;
            OutputFilePath = outputFilePath;
            Rules = rules;
        }
    }
    internal class Compiler
    {
        private readonly Parser _parser = new Parser();
        public void Compile(CompilerSettings settings)
        {
            var instructions = _parser.Parse(settings.InputFilePath, settings.Rules);
            var state = new CompilationState();
            state.Compile(instructions, settings.OutputFilePath);
        }
    }
}
