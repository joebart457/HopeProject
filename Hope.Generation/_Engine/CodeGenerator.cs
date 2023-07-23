using Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Hope.Generation._Engine
{
    internal class SupportedTypeInfo
    {
        public int UniqueIdentifier { get; set; } = 0;
        public string Name { get; set; } = "";
        public string CSharpType { get; set; } = "";
        public string CType { get; set; } = "";
    }
    internal class SupportedTypeConfiguration
    {
        public List<SupportedTypeInfo> SupportedTypes { get; set; } = new();
    }
    internal class InstructionInfo
    {
        public string Prefix { get; set; } = "";
        public int InstructionCode { get; set; } = 0;

    }
    internal class InstructionConfiguration
    {
        public List<InstructionInfo> Instructions { get; set; } = new();
    }


    internal class StdLibConfiguration
    {
        public string StdLibHeaderPath { get; set; } = "";
        public string StdLibContextBuilderPath { get; set; } = "";
    }
    internal class GenerationManifest 
    {
        public string DefinitionsFileLocation { get; set; } = "";
        public InstructionConfiguration InstructionConfiguration { get; set; } = new();

        public SupportedTypeConfiguration SupportedTypeConfiguration { get; set; } = new();
        public string CompilationConstantsFileLocation { get; set; } = "";
        public StdLibConfiguration StdLibConfiguration { get; set; } = new();
    }

    internal static class CodeGenerator
    {
        public static void GenerateDefinitionsFile(GenerationManifest manifest)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// (Auto generated from {Assembly.GetExecutingAssembly().GetName()})");
            sb.Append("\n// Supported type defintions\n");
            foreach(var type in manifest.SupportedTypeConfiguration.SupportedTypes)
            {
                sb.Append($"#define TY_{type.Name.ToUpper()} {type.UniqueIdentifier}");
            }
            sb.Append("\n// Instruction definitions\n");
            foreach(var ins in manifest.InstructionConfiguration.Instructions)
            {
                sb.Append($"#define INS_{ins.Prefix.ToUpper()} {ins.InstructionCode}");
            }
            
            if (!File.Exists(manifest.DefinitionsFileLocation)) throw new FileNotFoundException($"{manifest.DefinitionsFileLocation} does not exist");
            File.WriteAllText(manifest.DefinitionsFileLocation, sb.ToString());
            CliLogger.LogSuccess($"-> {manifest.DefinitionsFileLocation}");
        }

        public static void GenerateConstantsFile(GenerationManifest manifest)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// (Auto generated from {Assembly.GetExecutingAssembly().GetName()})");

            sb.AppendLine("using Hope.Compiler._Parser.Constants;");
            sb.AppendLine("namespace Hope.Compiler.Constants;");
            sb.AppendLine("internal static class CompilationConstants");
            sb.AppendLine("{");
            sb.AppendLine("\tpublic static Dictionary<string, int> Instructions => new()");
            sb.AppendLine("\t{");
            sb.AppendLine("\t{\"label\", -1},");
            foreach(var ins in manifest.InstructionConfiguration.Instructions)
            {
                sb.AppendLine($"\t{{\"{ins.Prefix}\",{ins.InstructionCode}}},");
            }
            sb.AppendLine("\t};");

            sb.AppendLine("\tpublic static Dictionary<string, int> DataTypes => new()");
            sb.AppendLine("\t{");
            foreach (var type in manifest.SupportedTypeConfiguration.SupportedTypes)
            {
                sb.AppendLine($"\t{{\"{type.Name}\",{type.UniqueIdentifier}}},");
            }
            sb.AppendLine("\t};");

            sb.AppendLine("\tpublic static Dictionary<string, int> FunctionSymbols => new()");
            sb.AppendLine("\t{");
            int counter = 0;
            foreach (var fn in GetStdLibFunctionsAsList(manifest.StdLibConfiguration.StdLibHeaderPath))
            {
                sb.AppendLine($"\t{{\"{fn}\",{counter++}}},");
            }
            sb.AppendLine("\t};");

            sb.AppendLine("}");
            if (!File.Exists(manifest.CompilationConstantsFileLocation)) throw new FileNotFoundException($"{manifest.CompilationConstantsFileLocation} does not exist");
            File.WriteAllText(manifest.CompilationConstantsFileLocation, sb.ToString());
            CliLogger.LogSuccess($"-> {manifest.CompilationConstantsFileLocation}");
        }

        private static List<string> GetStdLibFunctionsAsList(string filepath)
        {
            if (!File.Exists(filepath)) throw new FileNotFoundException($"{filepath} does not exist");
            var text = File.ReadAllText(filepath);
            var functionNames = text
                .Replace("(_args args);", "")
                .Replace("std::any ", "")
                .Replace("#ifndef _INCLUDE_STDLIB_H", "")
                .Replace("#define _INCLUDE_STDLIB_H", "")
                .Replace("#endif", "")
                .Split('\n')
                .OrderBy(x => x);
            return functionNames.Where(fn => !string.IsNullOrWhiteSpace(fn)).ToList();
        }

        public static void CleanStdLibHeader(GenerationManifest manifest)
        {
            var symbols = GetStdLibFunctionsAsList(manifest.StdLibConfiguration.StdLibHeaderPath);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// (Auto generated from {Assembly.GetExecutingAssembly().GetName()})");
            sb.AppendLine("#pragma once");  
            sb.AppendLine("#ifndef Std_Lib_h");
            sb.AppendLine("#define Std_Lib_h");
            sb.AppendLine("#include <any>");
            sb.AppendLine("#include \"args.hpp\"");

            foreach(var fn in symbols)
            {
                sb.AppendLine($"std::any {fn}(_args args);");
            }

            sb.AppendLine("#endif // end define _INCLUDE_STDLIB_H");
            if (!File.Exists(manifest.StdLibConfiguration.StdLibHeaderPath)) throw new FileNotFoundException($"{manifest.StdLibConfiguration.StdLibHeaderPath} does not exist");
            File.WriteAllText(manifest.StdLibConfiguration.StdLibHeaderPath, sb.ToString());
            CliLogger.LogSuccess($"-> {manifest.StdLibConfiguration.StdLibHeaderPath}");
        }

        public static void CreateContextBuilder(GenerationManifest manifest)
        {
            CliLogger.LogWarning("The following action will overwrite the standard context file.\nThis action is IRREVERSIBLE. Proceed? (y/n)");
            var input = Console.ReadKey();
            if ($"{input}".ToLower() != "y")
            {
                CliLogger.LogError("aborted");
                return;
            }
            var symbols = GetStdLibFunctionsAsList(manifest.StdLibConfiguration.StdLibHeaderPath);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// (Auto generated from {Assembly.GetExecutingAssembly().GetName()})");

            sb.AppendLine("#pragma once");
            sb.AppendLine("");
            sb.AppendLine("#include <memory>");
            sb.AppendLine("#include <map>");
            sb.AppendLine("#include <cstdlib>");
            sb.AppendLine("#include \"scope.hpp\"");
            sb.AppendLine("#include \"native_fn.hpp\"");
            sb.AppendLine("#include \"stdlib.hpp\"");
            sb.AppendLine("");
            sb.AppendLine("class ContextBuilder");
            sb.AppendLine("{");
            sb.AppendLine("public:");
            sb.AppendLine("	static std::shared_ptr<std::map<int32_t, std::shared_ptr<native_fn>>> create_std_lib()");
            sb.AppendLine("	{");
            sb.AppendLine("		auto stdlib = std::make_shared<std::map<int32_t, std::shared_ptr<native_fn>>>();");
            sb.AppendLine();
            int counter = 0;
            foreach (var fn in symbols)
            {
                sb.AppendLine($"		(*stdlib)[{counter++}] = std::make_shared<native_fn>(\"{fn}\", {fn})");
                sb.AppendLine("			->registerParameter<int32_t>(\"x\");");
            }
            sb.AppendLine();
            sb.AppendLine("		return stdlib;");
            sb.AppendLine("	}");
            sb.AppendLine();
            sb.AppendLine("};");


            if (!File.Exists(manifest.StdLibConfiguration.StdLibContextBuilderPath)) throw new FileNotFoundException($"{manifest.StdLibConfiguration.StdLibContextBuilderPath} does not exist");
            File.WriteAllText(manifest.StdLibConfiguration.StdLibContextBuilderPath, sb.ToString());
            CliLogger.LogSuccess($"-> {manifest.StdLibConfiguration.StdLibContextBuilderPath}");
        }

    }
}
