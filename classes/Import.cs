using System.Text.RegularExpressions;

public partial class ImportExpression
{
    public string filePath = "";
    public static bool IsImportStatement(string line)
    {
        return ImportRegex().IsMatch(line);
    }

    public ImportExpression(string line)
    {
        Match match = ImportRegex().Match(line);
        if (!match.Success)
        {
            Typer.CodeError("Invalid import statement", 1001);
        }
        filePath = match.Groups[1].Value;
        if (!File.Exists(filePath))
        {
            Typer.CodeError($"ImportError: Import file '{filePath}' does not exist", 41);
        }
        string lastFilePath = Typer.currentContext.file;
        Typer fileTyper = new Typer();
        fileTyper.TyperMain([filePath, "N/A"]);
        Typer.currentContext.file = lastFilePath;
    }

    [GeneratedRegex("^\\%import\\s+([^\\s]+[\\.ucl|\\.uchc])$")]
    public static partial Regex ImportRegex();
}