using System.Text.RegularExpressions;

public partial class ExpressionTemplate
{
    public virtual int importance => 0; // Higher means if two expressions are in conflict, the one with higher importance wins (e.g. function invoke is a less important as complex expression)

    public string line;
    public virtual object infos { get; set; } = new ExpressionTemplateInfos();

    public ExpressionTemplate(string line)
    {
        this.line = line;
        // infos = Parse(line);
    }

    public static bool IsExpression(string line)
    {
        return ExpressionRegex().IsMatch(line);
    }

    public ExpressionTemplateInfos Parse(string line)
    {
        // if (!IsExpression(line))
        // {
        //     throw new Exception("Not an expression: " + line);
        // }
        // Disabled for performance reasons, re-enable for debugging
        Match match = ExpressionRegex().Match(line);
        ExpressionTemplateInfos infos = new ExpressionTemplateInfos();
        if (match.Success)
        {
            infos.message = match.Groups[1].Value;
        }
        return infos;
    }

    [GeneratedRegex("$You need to modify this message: ([a-zA-Z]*)^")]
    public static partial Regex ExpressionRegex();
    public partial class ExpressionTemplateInfos
    {
        public string message = "Undifined message";
        public string? expressionType { get; set; } = null;
    }
}

