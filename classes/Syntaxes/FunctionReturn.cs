using System.Text.RegularExpressions;

public partial class FunctionReturn : ExpressionTemplate
{
    public override int importance => 1;
    public override object infos { get; set; } = new FunctionReturnInfos();
    public FunctionReturn(string line) : base(line)
    {
        if (!IsExpression(line))
        {
            Typer.InternalError("Tried to parse a non-function-return expression as function return: " + line, 1002, true);
        }
        Match match = ExpressionRegex().Match(line);
        if (match.Success && this.infos is FunctionReturnInfos infos)
        {
            infos.returnValue = new Value(match.Groups[1].Value).infos;
        }
    }
    public static new bool IsExpression(string line)
    {
        return ExpressionRegex().IsMatch(line);
    }

    public class FunctionReturnInfos : ExpressionTemplateInfos
    {
        public Value.ValueInfos? returnValue { get; set; } = null;
    }

    [GeneratedRegex("^Return\\s+(.*)$")]
    public static new partial Regex ExpressionRegex();
}
