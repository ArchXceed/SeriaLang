using System.Text.RegularExpressions;

public partial class CustomDefine
{
    public List<FunctionArg> customDefineFunctionArguments = new List<FunctionArg>();
    public string customDefineFunctionName = "";
    public string customDefineFunctionExpression = ""; // Example: <summon ?entity? ?x? ?y? ?z?> -> C# equivalent: $"summon {entity} {x} {y} {z}"
    public CustomDefineInfos infos = new CustomDefineInfos();
    public class CustomDefineInfos
    {
        public List<FunctionArg.FunctionArgInfos>? functionArguments { get; set; } = null;
        public string? functionName { get; set; } = null;
        public string? functionExpression { get; set; } = null;
    }

    public static bool IsCustomDefineStatement(string line)
    {
        return CustomDefineRegex().IsMatch(line);
    }

    public CustomDefine(string line)
    {
        Match match = CustomDefineRegex().Match(line);
        if (!match.Success)
        {
            Typer.CodeError("Invalid custom define statement", 1001);
        }
        customDefineFunctionArguments = FunctionArg.ParseFunctionArg(match.Groups[2].Value);

        customDefineFunctionName = match.Groups[1].Value;
        customDefineFunctionExpression = match.Groups[3].Value;
        List<string> alreadyUsedNames = new List<string>();
        foreach (CustomDefine cd in Typer.customDefines)
        {
            alreadyUsedNames.Add(cd.customDefineFunctionName);
        }
        foreach (Function func in Typer.functions)
        {
            alreadyUsedNames.Add(func.name);
        }
        if (alreadyUsedNames.Contains(customDefineFunctionName))
        {
            Typer.CodeError($"CustomDefineError: Custom define name '{customDefineFunctionName}' is already used by another custom define or function", 52);
        }
        List<FunctionArg.FunctionArgInfos> functionArgInfos = new List<FunctionArg.FunctionArgInfos>();
        foreach (FunctionArg arg in customDefineFunctionArguments)
        {
            functionArgInfos.Add(arg.infos);
        }
        infos = new CustomDefineInfos
        {
            functionArguments = functionArgInfos,
            functionName = customDefineFunctionName,
            functionExpression = customDefineFunctionExpression
        };
    }

    [GeneratedRegex("^\\[(.*)\\((.*)\\)\\]\\s*=>\\s*<(.*)>$")]
    private static partial Regex CustomDefineRegex();
}