using System.Text.RegularExpressions;

public partial class Literals
{
    public static Dictionary<string, Regex> literals = new Dictionary<string, Regex>
    {
        { "Int", new Regex("^[0-9]+$") },
        { "Float", new Regex("^[0-9]+\\.[0-9]+$") },
        { "String", new Regex("^\"([^\"]*)\"|'([^']*)'$") },
        { "Bool", new Regex("^true|false$") },
        { "Void", new Regex("^null$") }
    };
}
