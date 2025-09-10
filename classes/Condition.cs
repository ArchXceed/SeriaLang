using System.Text.RegularExpressions;

public partial class ConditionExpression
{
    public string? conditionSymbol = null;
    public Value? firstValue = null;
    public Value? secondValue = null;
    public ConditionInfos infos { get; set; } = new ConditionInfos();
    public ConditionExpression(string condition)
    {
        Match match = ConditionRegex().Match(condition);
        if (match.Success)
        {
            firstValue = new Value(match.Groups[1].Value);
            conditionSymbol = match.Groups[2].Value;
            secondValue = new Value(match.Groups[3].Value);
            infos = new ConditionInfos
            {
                firstValue = firstValue.infos,
                conditionSymbol = conditionSymbol,
                secondValue = secondValue.infos
            };
        }
        else
        {
            Typer.CodeError("SyntaxError: Invalid condition syntax: " + condition, 23);
        }
    }

    public class ConditionInfos
    {
        public Value.ValueInfos? firstValue { get; set; } = null;
        public string? conditionSymbol { get; set; } = null;
        public Value.ValueInfos? secondValue { get; set; } = null;
    }

    [GeneratedRegex("^(.*)\\s+(.*)\\s+(.*)$")]
    public static partial Regex ConditionRegex();
}