using System.Text.RegularExpressions;

public partial class FunctionModificationExpression : ExpressionTemplate
{
    public override int importance => 1;
    public override object infos { get; set; } = new FunctionModificationExpressionInfos();
    public FunctionModificationExpression(string line) : base(line)
    {
        if (!IsExpression(line))
        {
            Typer.InternalError("Tried to parse a non-function-modification expression as function modification: " + line, 1002, true);
        }
        Match match = ExpressionRegex().Match(line);
        if (match.Success && this.infos is FunctionModificationExpressionInfos infos)
        {
            infos.variableName = match.Groups[1].Value;
            infos.functionName = match.Groups[2].Value;
            Value argCountVal = new Value(match.Groups[3].Value);
            if (argCountVal.type == "Int")
            {
                if (argCountVal.preProcessedValue != null && argCountVal.preProcessedValue != "")
                {
                    infos.argumentCount = int.Parse(argCountVal.preProcessedValue.ToString());
                }
                else
                {
                    Typer.InternalError("Argument count value is null in function modification expression: " + match.Groups[3].Value, 1005, true);
                }
            }
            else
            {
                Typer.CodeError($"TypeError: Argument Count in function {infos.functionName} must be an Int, got: {argCountVal.type}", 25);
            }
        }
        else
        {
            Typer.InternalError("Failed to parse function modification expression: " + line, 1004, true);
        }
    }
    public static new bool IsExpression(string line)
    {
        return ExpressionRegex().IsMatch(line);
    }

    public class FunctionModificationExpressionInfos : ExpressionTemplateInfos
    {
        public string? variableName { get; set; } = null;
        public string? functionName { get; set; } = null;
        public int argumentCount { get; set; } = 0;
    }

    [GeneratedRegex("^#::(.*)::(.*)\\(\\)<([0-9]+)>$")]
    public static new partial Regex ExpressionRegex();
}
