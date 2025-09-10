using System.Diagnostics;
using System.Text.RegularExpressions;

public partial class FunctionInvocationExpression : ExpressionTemplate
{
    public override int importance => 1;
    public override object infos { get; set; } = new FunctionInvocationExpressionInfos();
    public FunctionInvocationExpression(string line) : base(line)
    {
        if (!IsExpression(line))
        {
            Typer.InternalError("Tried to parse a non-function-invocation expression as function invocation: " + line, 1002, true);
        }
        Match match = ExpressionRegex().Match(line);
        if (match.Success && this.infos is FunctionInvocationExpressionInfos infos)
        {
            infos.functionName = match.Groups[1].Value;
            infos.argumentsRaw = match.Groups[2].Value;
            if (infos.argumentsRaw != "")
            {
                Match argsMatch = ArgsRegex().Match(infos.argumentsRaw);
                if (!argsMatch.Success)
                {
                    Typer.CodeError("SyntaxError: Invalid function invocation arguments: " + infos.argumentsRaw, 12);
                }
                foreach (Group group in argsMatch.Groups)
                {
                    infos.argumentsVariables.Add(group.Value.Trim().TrimEnd(';').Trim());
                    infos.argumentsValues.Add(new Value(group.Value.Trim().TrimEnd(';').Trim()));
                }
            }
        }
    }
    public static new bool IsExpression(string line)
    {
        return ExpressionRegex().IsMatch(line);
    }
    public class FunctionInvocationExpressionInfos : ExpressionTemplateInfos
    {
        public string functionName = "Undefined function name";
        public string argumentsRaw = "Undefined arguments raw";
        public List<string> argumentsVariables = new List<string>();
        public List<Value> argumentsValues = new List<Value>();
    }

    [GeneratedRegex("^([a-zA-Z_]+[a-zA-Z0-9_]*)\\(\\)<(.*)>$")]
    public static new partial Regex ExpressionRegex();

    [GeneratedRegex(".*\\s?;*")]
    public static partial Regex ArgsRegex();
}
