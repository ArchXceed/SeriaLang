using System.Text.RegularExpressions;

public partial class Value
{
    public string type = "Void";
    public string? preProcessedValue = null; // If it's a literal or if the value is a global var (constant), we don't need to wait runtime to get its value
    public string? localVarName = null; // If it's a local var, we store its name here
    public ValueInfos infos = new ValueInfos();
    public class ValueInfos
    {
        public string type { get; set; } = "Void";
        public string? preProcessedValue { get; set; } = null;
        public string? localVarName { get; set; } = null;
    }

    public Value(string valueRaw)
    {
        if (GlobalVarRegex().IsMatch(valueRaw))
        {
            preProcessedValue = Typer.globalVars.Find(v => v.name == GlobalVarRegex().Match(valueRaw).Groups[1].Value)?.value;
            type = "GlobalVar";
            GlobalReplaceVar? var = Typer.globalVars.Find(v => v.name == GlobalVarRegex().Match(valueRaw).Groups[1].Value);
            if (var == null)
            {
                Typer.CodeError($"- NameError: Global constant '{GlobalVarRegex().Match(valueRaw).Groups[1].Value}' not found", 32);
            }
        }
        else if (LocalVarRegex().IsMatch(valueRaw))
        {
            type = "LocalVar";
            localVarName = LocalVarRegex().Match(valueRaw).Groups[1].Value;
        }
        else
        {
            bool foundLiteral = false;
            foreach (KeyValuePair<string, Regex> literal in Literals.literals)
            {
                Match literalMatch = literal.Value.Match(valueRaw);
                if (literalMatch.Success)
                {
                    preProcessedValue = literalMatch.Groups[1].Value;
                    type = literal.Key;
                    foundLiteral = true;
                    break;
                }
            }
            if (!foundLiteral)
            {
                Typer.CodeError($"- TypeError: Value {valueRaw} isn't a known literal/global constant/local variable", 33);
            }
        }
        infos = new ValueInfos
        {
            type = type,
            preProcessedValue = preProcessedValue,
            localVarName = localVarName
        };
    }

    [GeneratedRegex("^\\$::([a-zA-Z_]+[a-zA-Z0-9_]*)$")]
    private static partial Regex GlobalVarRegex();

    [GeneratedRegex("^#::([a-zA-Z_]+[a-zA-Z0-9_]*)$")]
    private static partial Regex LocalVarRegex();
}