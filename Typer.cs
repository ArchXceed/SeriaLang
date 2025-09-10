using System.Text.RegularExpressions;

public partial class Typer
{
    public List<Type> expressionTypes = new List<Type>
    {
        typeof(FunctionInvocationExpression)
    };
    public List<string> lines = new List<string>();
    public static List<GlobalReplaceVar> globalVars = new List<GlobalReplaceVar>();
    public List<Function> functions = new List<Function>();
    public static void CodeWarning(string warning, int warningCode = 0)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[WARNING] {warning} {(warningCode != 0 ? $"(Code {warningCode})" : "")}\n --> {currentContext.lineCode} \n in function {currentContext.function}: {currentContext.file}:{currentContext.line}");
        Console.ResetColor();
    }

    public static void CodeError(string error, int errorCode = 0)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[ERROR] {error} {(errorCode != 0 ? $"(Code {errorCode})" : "")}\n --> {currentContext.lineCode} \n in function {currentContext.function}: {currentContext.file}:{currentContext.line}");
        Environment.Exit(errorCode != 0 ? errorCode : 1);
    }

    public static void InternalError(string error, int errorCode = 0, bool fatal = false)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[INTERNAL ERROR] {error} {(errorCode != 0 ? $"(Code {errorCode})" : "")} **THIS IS NOT SUPPOSED TO HAPPEN, AND IS NOT YOUR FAULT**\n --> {currentContext.lineCode} \n in function {currentContext.function}: {currentContext.file}:{currentContext.line}");
        Console.ResetColor();
        if (fatal)
        {
            Environment.Exit(errorCode != 0 ? errorCode : 1);
        }
    }

    public static Context currentContext = new Context();

    public void TyperMain(string[] args)
    {
        if (args.Count() < 2)
        {
            Console.WriteLine("Usage: <executable> source output");
            return;
        }
        string source = args[0];
        currentContext.file = source;
        if (!File.Exists(source))
        {
            Console.WriteLine("source needs to exist");
        }
        using StreamReader reader = new(source);
        string code = reader.ReadToEnd();

        string[] linesRaw = code.Split("\n");

        foreach (string line in linesRaw)
        {
            lines.Add(line);
        }
        Compile();
    }

    private void Compile()
    {
        bool hasFirstFunctionBeenFound = false;
        for (int currentLine = 0; currentLine < lines.Count; currentLine++)
        {
            string line = lines[currentLine];
            currentContext.line = currentLine;
            currentContext.lineCode = line;
            string lineTrim = line.Trim();

            // Skip empty lines and comments
            if (lineTrim == "" || lineTrim.StartsWith("//"))
            {
                continue;
            }
            // Skip inline comments
            lineTrim = lineTrim.Split("//")[0].Trim();
            // Allow escaped comments
            lineTrim = lineTrim.Replace("\\/\\/", "//");
            if (!hasFirstFunctionBeenFound)
            {
                if (GlobalReplaceVar.IsGlobalReplaceVar(lineTrim))
                {
                    globalVars.Add(new GlobalReplaceVar(lineTrim));
                    continue;
                }
            }
            Match regexMatch = FunRegex().Match(lineTrim);
            if (regexMatch.Success)
            {
                // Debug
                // foreach (Group group in regexMatch.Groups)
                // {
                //     Console.WriteLine(group.Value);
                // }
                string funName = regexMatch.Groups[1].Value;
                string argsRaw = regexMatch.Groups[2].Value;
                List<FunctionArg> args = FunctionArg.ParseFunctionArg(argsRaw);
                List<string> funBody = new List<string>();
                bool endOfFunction = false;
                for (int functionLineIndex = currentLine + 1; functionLineIndex < lines.Count; functionLineIndex++)
                {
                    if (lines[functionLineIndex].Trim().StartsWith(funName))
                    {
                        currentLine = functionLineIndex;
                        endOfFunction = true;
                        break;
                    }
                    string funLine = lines[functionLineIndex].Trim();
                    if (funLine == "" || funLine.StartsWith("//"))
                    {
                        continue;
                    }
                    funLine = funLine.Split("//")[0].Trim();
                    funLine = funLine.Replace("\\/\\/", "//");
                    funBody.Add(funLine);
                }
                if (!endOfFunction)
                {
                    CodeError("Function " + funName + " not closed", 22);
                }
                currentContext.function = funName;
                Function function = new Function(funName, args);
                function.insideCode = funBody;
                function.lineStart = currentLine - funBody.Count + 1;
                function.Compile(expressionTypes);
                functions.Add(function);
                hasFirstFunctionBeenFound = true;
            }
        }
    }



    [GeneratedRegex("^fun\\s+([A-Za-z0-9_]+)\\(([^\n]*)\\):$")]
    private static partial Regex FunRegex();


}