using System.Text.RegularExpressions;

public partial class LocalVariableExpression : ExpressionTemplate
{
    public override int importance => 1;
    public override object infos { get; set; } = new LocalVariableExpressionInfos();
    public LocalVariableExpression(string line) : base(line)
    {
        if (!IsExpression(line))
        {
            Typer.InternalError("Tried to parse a non-local-variable expression as local variable: " + line, 1002, true);
        }
        Match match = ExpressionRegex().Match(line);
        if (match.Success && this.infos is LocalVariableExpressionInfos infos)
        {
            infos.type = match.Groups[1].Value;
            infos.name = match.Groups[2].Value;
            infos.initialValue = new Value(match.Groups[3].Value).infos;
            Typer.variables.Add(infos.name);
        }
    }
    public static new bool IsExpression(string line)
    {
        return ExpressionRegex().IsMatch(line);
    }

    public class LocalVariableExpressionInfos : ExpressionTemplateInfos
    {
        public Value.ValueInfos? initialValue { get; set; } = null;
        public string? type { get; set; } = null;
        public string? name { get; set; } = null;
    }

    [GeneratedRegex("^(.*)\\s+(.*)\\s*=\\s*(.*)$")]
    public static new partial Regex ExpressionRegex();
}
