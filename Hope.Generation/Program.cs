// See https://aka.ms/new-console-template for more information



using Hope.Generation._Engine;

var manifest = new GenerationManifest
{
    DefinitionsFileLocation = "C:\\Users\\Jimmy Barnes\\Documents\\HopeProject\\Hope.Vm\\definitions.hpp",
    SupportedTypeConfiguration = new()
    {
        SupportedTypes = new()
        {
            new() { Name = "integer", CSharpType = "int", CType = "int32_t" },
            new() { Name = "double", CSharpType = "double", CType = "double" },
            new() { Name = "string", CSharpType = "string", CType = "std::string" },
        }
    },
    InstructionConfiguration = new()
    {
        Instructions = new()
        {
            new() { Prefix = "", InstructionCode = 3 },
        }
    },
    CompilationConstantsFileLocation = "C:\\Users\\Jimmy Barnes\\Documents\\HopeProject\\Hope.Compiler\\Constants\\CompilationConstants.cs",
    StdLibConfiguration = new()
    {
        StdLibContextBuilderPath = "C:\\Users\\Jimmy Barnes\\Documents\\HopeProject\\Hope.Vm\\ContextBuilder.hpp",
        StdLibHeaderPath = "C:\\Users\\Jimmy Barnes\\Documents\\HopeProject\\Hope.Vm\\definitions.hpp",
    }
};

CodeGenerator.GenerateDefinitionsFile(manifest);
CodeGenerator.GenerateConstantsFile(manifest);
CodeGenerator.CleanStdLibHeader(manifest);
CodeGenerator.CreateContextBuilder(manifest);
