using System.Text.RegularExpressions;

public partial class IfExpression : ExpressionTemplate
{
    public override int importance => 1;
    public override object infos { get; set; } = new IfExpressionInfos();
    public IfExpression(string line) : base(line)
    {
        if (!IsExpression(line))
        {
            Typer.InternalError("Tried to parse a non-function-invocation expression as function invocation: " + line, 1002, true);
        }
        Match match = ExpressionRegex().Match(line);
        if (match.Success && this.infos is IfExpressionInfos infos)
        {
            infos.condition = new ConditionExpression(match.Groups[1].Value).infos;
            if (FunctionInvocationExpression.IsExpression(match.Groups[2].Value))
            {
                infos.functionInvocation = new FunctionInvocationExpression(match.Groups[2].Value);
            }
            else
            {
                Typer.CodeError("SyntaxError: Invalid function invocation syntax in if expression: " + match.Groups[2].Value, 24);
            }
        }
        else
        {
            Typer.InternalError("Tried to parse a non-if expression as if: " + line, 1002, true);
        }
    }
    public static new bool IsExpression(string line)
    {
        return ExpressionRegex().IsMatch(line);
    }

    public class IfExpressionInfos : ExpressionTemplateInfos
    {
        public ConditionExpression.ConditionInfos? condition { get; set; } = null;
        public FunctionInvocationExpression? functionInvocation { get; set; } = null;
    }

    [GeneratedRegex("^If\\s+\\((.*)\\):\\s+(.*)$")]
    public static new partial Regex ExpressionRegex();
}
