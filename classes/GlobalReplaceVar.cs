using System.Text.RegularExpressions;

public partial class GlobalReplaceVar
{
    public string name = "";
    public string value = "";
    public GlobalReplaceVarInfos? infos = null;

    public class GlobalReplaceVarInfos
    {
        public string? type { get; set; } = null;
        public string? name { get; set; } = null;
        public string? value { get; set; } = null;
    }

    public static bool IsGlobalReplaceVar(string line)
    {
        return GetRegex().IsMatch(line);
    }

    public GlobalReplaceVar(string line)
    {
        Match match = GetRegex().Match(line);
        if (!match.Success)
        {
            Typer.InternalError("Tried to parse a non-global-replace-var line as global replace var: " + line, 1002, true);
        }
        name = match.Groups[2].Value;
        Regex? typeRegex = Literals.literals.ContainsKey(match.Groups[1].Value) ? Literals.literals[match.Groups[1].Value] : null;
        if (typeRegex == null)
        {
            Typer.CodeError($"TypeError: Type {match.Groups[1].Value} not found", 31);
            return; // To make the compiler happy
        }
        Match valueMatch = typeRegex.Match(match.Groups[3].Value);
        if (!valueMatch.Success)
        {
            Typer.CodeError($"TypeError: Value {match.Groups[3].Value} is not of type {match.Groups[1].Value}", 32);
            return; // To make the compiler happy
        }
        value = valueMatch.Groups[1].Value;
        infos = new GlobalReplaceVarInfos
        {
            type = match.Groups[1].Value,
            name = name,
            value = value
        };
    }

    [GeneratedRegex("^const (.*) \\$([a-zA-Z_]+[a-zA-Z0-9_]*) = (.+)$")]
    public static partial Regex GetRegex();
}