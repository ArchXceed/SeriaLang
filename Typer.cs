using System.Diagnostics.Tracing;
using System.Text.Json;
using System.Text.RegularExpressions;

public partial class Typer
{
    public List<Type> expressionTypes = new List<Type>
    {
        typeof(FunctionInvocationExpression),
        typeof(FunctionReturn),
        typeof(IfExpression),
        typeof(ForExpression),
        typeof(LocalVariableExpression),
        typeof(FunctionModificationExpression)
    };
    public List<string> lines = new List<string>();
    public static List<GlobalReplaceVar> globalVars = new List<GlobalReplaceVar>();
    public static List<string> variables = new List<string>(); // All variables are GLOBAL, why? It's because MinLang will only support globals
    public static List<Function> functions = new List<Function>();
    public static List<ImportExpression> imports = new List<ImportExpression>();
    public static List<CustomDefine> customDefines = new List<CustomDefine>();
    public bool isCompilatorHeaderDefinitionFile = false; // This is a file used to define functions for the compiler, not to be compiled itself
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
        if (source.EndsWith(".uchc"))
        {
            isCompilatorHeaderDefinitionFile = true;
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
            bool hasFoundExpression = false;

            // Compilator header definition file
            if (isCompilatorHeaderDefinitionFile)
            {
                if (lineTrim != "" && !lineTrim.StartsWith("//"))
                {
                    lineTrim = lineTrim.Split("//")[0].Trim();
                    lineTrim = lineTrim.Replace("\\/\\/", "//");
                    if (CustomDefine.IsCustomDefineStatement(lineTrim))
                    {
                        customDefines.Add(new CustomDefine(lineTrim));
                        hasFoundExpression = true;
                    }
                }
            }
            // Code file
            else
            {
                // Skip empty lines and comments
                if (lineTrim == "" || lineTrim.StartsWith("//"))
                {
                    continue;
                }
                // Skip inline comments
                lineTrim = lineTrim.Split("//")[0].Trim();
                // Allow escaped comments
                lineTrim = lineTrim.Replace("\\/\\/", "//");

                // ---------: Global vars
                if (!hasFirstFunctionBeenFound)
                {
                    if (GlobalReplaceVar.IsGlobalReplaceVar(lineTrim))
                    {
                        globalVars.Add(new GlobalReplaceVar(lineTrim));
                        continue;
                    }
                }

                // ---------: Imports
                if (!hasFirstFunctionBeenFound)
                {
                    if (ImportExpression.IsImportStatement(lineTrim))
                    {
                        ImportExpression importExpression = new ImportExpression(lineTrim);
                        List<string> importParts = [];
                        foreach (ImportExpression import in imports)
                        {
                            importParts.Add(import.filePath);
                        }
                        if (importParts.Contains(importExpression.filePath))
                        {
                            CodeWarning("File already imported: " + importExpression.filePath, 42);
                            continue;
                        }
                        imports.Add(importExpression);
                        continue;
                    }
                }

                // ---------: Functions
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
                        if (lines[functionLineIndex].Trim() == funName)
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
                        currentContext.line = currentLine + 1;
                        currentContext.lineCode = lines[currentLine + 1];
                        CodeError("Function " + funName + " not closed", 22);
                    }
                    currentContext.function = funName;
                    Function function = new Function(funName, args);
                    hasFoundExpression = true;
                    function.insideCode = funBody;
                    function.lineStart = currentLine - funBody.Count + 1;
                    function.Compile(expressionTypes);
                    functions.Add(function);
                    hasFirstFunctionBeenFound = true;
                }
            }
            if (!hasFoundExpression)
            {
                CodeError("SyntaxError: Unknown expression", 25);
            }
        }
    }

    public bool Serialize(string filePath)
    {
        if (!filePath.EndsWith(".ucobject"))
        {
            filePath += ".ucobject";
        }
        try
        {
            UCObject uCObject = new UCObject();
            foreach (GlobalReplaceVar globalVar in globalVars)
            {
                if (globalVar.infos != null)
                {
                    uCObject.globalVars.Add(globalVar.infos);
                }
                else
                {
                    InternalError("Somethings wrong...", 1000, false);
                }
            }
            foreach (CustomDefine customDefine in customDefines)
            {
                if (customDefine.infos != null)
                {
                    uCObject.customDefines.Add(customDefine.infos);
                }
                else
                {
                    InternalError("Somethings wrong...", 1000, false);
                }
            }
            foreach (Function function in functions)
            {
                if (function.infos != null)
                {
                    uCObject.functions.Add(function.infos);
                }
                else
                {
                    InternalError("Somethings wrong...", 1000, false);
                }
            }
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.Write(JsonSerializer.Serialize(uCObject, new JsonSerializerOptions { WriteIndented = true }));
            }
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error writing to file: " + e.Message);
            return false;
        }
    }




    [GeneratedRegex("^fun\\s+([A-Za-z0-9_]+)\\(([^\n]*)\\):$")]
    private static partial Regex FunRegex();


}