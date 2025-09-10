using System.Text.RegularExpressions;

public partial class FunctionArg
{
    public string name = "";
    public string type = "";
    public FunctionArgInfos infos = new FunctionArgInfos();

    public FunctionArg(string type = "", string name = "")
    {
        this.type = type;
        this.name = name;
        infos = new FunctionArgInfos
        {
            name = name,
            type = type
        };
    }

    public class FunctionArgInfos
    {
        public string? name { get; set; } = null;
        public string? type { get; set; } = null;
    }

    public static List<FunctionArg> ParseFunctionArg(string functionArgsRaw)
    {
        if (string.IsNullOrEmpty(functionArgsRaw))
        {
            return [];
        }

        List<FunctionArg> functionArgs = new List<FunctionArg>();
        MatchCollection argMatch = ArgsRegex().Matches(functionArgsRaw);
        foreach (Group group in argMatch)
        {
            Match match = ArgRegex().Match(group.Value.Trim());
            if (!match.Success)
            {
                Typer.CodeError("SyntaxError: Invalid function argument: " + group.Value.Trim(), 11);
            }
            string argType = match.Groups[1].Value;
            string argName = match.Groups[2].Value;
            functionArgs.Add(new FunctionArg(argType, argName));
        }
        return functionArgs;
    }

    [GeneratedRegex("^([A-Za-z0-9_]+)\\s+([A-Za-z0-9_]+)\\s*;\\s*$")]
    private static partial Regex ArgRegex();
    [GeneratedRegex("[A-Za-z0-9_]+\\s+[A-Za-z0-9_]+;\\s*")]
    private static partial Regex ArgsRegex();

}