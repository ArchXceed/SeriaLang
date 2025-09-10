using System.Text.RegularExpressions;

public partial class ForExpression : ExpressionTemplate
{
    public override int importance => 1;
    public override object infos { get; set; } = new ForExpressionInfos();
    public ForExpression(string line) : base(line)
    {
        if (!IsExpression(line))
        {
            Typer.InternalError("Tried to parse a non-for-loop expression as for-loop: " + line, 1002, true);
        }
        Match match = ExpressionRegex().Match(line);
        if (match.Success && this.infos is ForExpressionInfos infos)
        {
            infos.start = int.Parse(match.Groups[1].Value);
            infos.end = int.Parse(match.Groups[2].Value);
            infos.step = int.Parse(match.Groups[3].Value);
            string bodyCode = match.Groups[4].Value.Trim();
            if (FunctionInvocationExpression.IsExpression(bodyCode))
            {
                infos.body = (FunctionInvocationExpression.FunctionInvocationExpressionInfos)new FunctionInvocationExpression(bodyCode).infos;
            }
            else
            {
                Typer.CodeError("SyntaxError: Invalid function invocation syntax in for loop expression: " + bodyCode, 24);
            }
        }
        else
        {
            Typer.InternalError("Failed to parse for-loop expression: " + line, 1004, true);
        }
    }
    public static new bool IsExpression(string line)
    {
        return ExpressionRegex().IsMatch(line);
    }

    public class ForExpressionInfos : ExpressionTemplateInfos
    {
        public int start { get; set; } = 0;
        public int end { get; set; } = 0;
        public int step { get; set; } = 1;
        public FunctionInvocationExpression.FunctionInvocationExpressionInfos? body { get; set; } = null;
    }

    [GeneratedRegex("^For\\s*\\(([0-9]+);\\s*([0-9]+);\\s*([0-9]+)\\):\\s*(.*)$")]
    public static new partial Regex ExpressionRegex();
}
