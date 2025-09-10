using System.Text.RegularExpressions;

public partial class FunctionArg(string name = "", string type = "")
{
    public string name = name;
    public string type = type;

    public static List<FunctionArg> ParseFunctionArg(string functionArgsRaw)
    {
        if (string.IsNullOrEmpty(functionArgsRaw))
        {
            return [];
        }

        List<FunctionArg> functionArgs = new List<FunctionArg>();
        Match argMatch = ArgsRegex().Match(functionArgsRaw);
        foreach (Group group in argMatch.Groups)
        {
            Match match = ArgRegex().Match(group.Value.Trim());
            if (!match.Success)
            {
                Typer.CodeError("SyntaxError: Invalid function argument: " + group.Value.Trim(), 11);
            }
            string argType = match.Groups[1].Value;
            string argName = match.Groups[2].Value;
            // Console.WriteLine($"Type:{argType}, {argName}");
            functionArgs.Add(new FunctionArg(argType, argName));
        }
        return functionArgs;
    }

    [GeneratedRegex("^([A-Za-z0-9_]+)\\s+([A-Za-z0-9_]+)\\s*;\\s*$")]
    private static partial Regex ArgRegex();
    [GeneratedRegex("[A-Za-z0-9_]+\\s*[A-Za-z0-9_]+;\\s*")]
    private static partial Regex ArgsRegex();

}