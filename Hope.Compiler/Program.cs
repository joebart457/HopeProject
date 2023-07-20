
using CliParser;
using Hope.Compiler;
using Hope.Compiler._Parser;
using Hope.Compiler._Parser.Constants;
using Hope.Compiler.Models.Enums;
using Hope.Compiler.Models.Instructions;
using Logger;

[Entry("ch")]
class Startup
{
    [Command]
    public void Compile(string filename, string outputFileName)
    {
        var builder = new RuleBuilder();
        builder.Register<JmpInstruction>(TokenTypes.jmp)
            .WithArgument<JmpInstruction>(ArgumentType.CodeLabel,x => x.GoToLabel);
        builder.Register<JmpInstruction>(TokenTypes.jz)
            .WithArgument<JmpInstruction>(ArgumentType.CodeLabel, x => x.GoToLabel);
        builder.Register<JmpInstruction>(TokenTypes.jnz)
            .WithArgument<JmpInstruction>(ArgumentType.CodeLabel, x => x.GoToLabel);

        builder.Register<ConstInstruction>(TokenTypes.push_const)
            .WithArgument<ConstInstruction>(ArgumentType.ConstantValue, x => x.Value);

        builder.Register<FetchInstruction>(TokenTypes.fetch)
            .WithArgument<FetchInstruction>(ArgumentType.IdentifierLocation, x => x.LocalIdentifier);
        builder.Register<FetchInstruction>(TokenTypes.pop)
            .WithArgument<FetchInstruction>(ArgumentType.IdentifierLocation, x => x.LocalIdentifier);

        builder.Register<LabelInstruction>(TokenTypes.label)
            .WithArgument<LabelInstruction>(ArgumentType.CodeLabel, x => x.Label);

        builder.Register<CallInstruction>(TokenTypes.call)
            .WithArgument<CallInstruction>(ArgumentType.FunctionIdentifier, x => x.FunctionName);

        builder.Register<SingularInstruction>(TokenTypes.mul);
        builder.Register<SingularInstruction>(TokenTypes.div);
        builder.Register<SingularInstruction>(TokenTypes.add);
        builder.Register<SingularInstruction>(TokenTypes.sub);
        builder.Register<SingularInstruction>(TokenTypes.gt );
        builder.Register<SingularInstruction>(TokenTypes.gte);
        builder.Register<SingularInstruction>(TokenTypes.lt );
        builder.Register<SingularInstruction>(TokenTypes.lte);
        builder.Register<SingularInstruction>(TokenTypes.eq );
        builder.Register<SingularInstruction>(TokenTypes.neq);
        builder.Register<SingularInstruction>(TokenTypes.end);

        builder.Register<LabelInstruction>(TokenTypes.label)
            .WithArgument<LabelInstruction>(ArgumentType.CodeLabel, x => x.Label);


        var settings = new CompilerSettings(filename, outputFileName, builder.GetRules());
        var compiler = new Compiler();
        compiler.Compile(settings);
        CliLogger.LogSuccess($"{filename} -> {outputFileName}");
    }
}

class Program
{
    static void Main(string[] args)
    {
#if DEBUG
        args = new string[] { "test.txt", "out" };
#endif
        args.ResolveWithTryCatch(new Startup(), (ex) =>  CliLogger.LogError($"err: {ex.Message} {(ex.InnerException?.Message ?? "")}"));
    }
}