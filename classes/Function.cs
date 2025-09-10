
using System.Reflection;

public class Function
{

    public List<string> insideCode = new List<string>();
    public string name;
    public List<ExpressionTemplate> lines = new List<ExpressionTemplate>();
    public int lineStart = 0;
    public List<FunctionArg> functionArgs = new List<FunctionArg>();
    public FunctionInfos? infos = null;
    public Function(string name, List<FunctionArg> functionArgs)
    {
        this.name = name;
        this.functionArgs = functionArgs;
    }

    public List<object> GetLinesSerializable()
    {
        List<object> serializedLines = new List<object>();
        foreach (ExpressionTemplate line in lines)
        {
            if (line.infos is not null)
            {
                if (line.infos is ExpressionTemplate.ExpressionTemplateInfos infos)
                {
                    infos.expressionType = line.GetType().Name;
                }
            }
            serializedLines.Add(line.infos!);
        }
        return serializedLines;
    }

    public class FunctionInfos
    {
        public string? name { get; set; } = null;
        public List<FunctionArg.FunctionArgInfos>? functionArgs { get; set; } = null;
        public List<object>? lines { get; set; } = null;
        public int lineStart = 0;
    }

    public void Compile(List<Type> expressions)
    {
        for (int lineNumber = 0; lineNumber < insideCode.Count; lineNumber++)
        {
            string line = insideCode[lineNumber];
            Typer.currentContext.lineCode = line;
            Typer.currentContext.line = lineNumber + lineStart;
            bool expressionFound = false;
            List<ExpressionTemplate> expressionFounds = new List<ExpressionTemplate>();
            foreach (Type expression in expressions)
            {
                if (typeof(ExpressionTemplate).IsAssignableFrom(expression))
                {

                    MethodInfo? method = expression.GetMethod("IsExpression", BindingFlags.Public | BindingFlags.Static);
                    if (method == null)
                    {
                        Typer.InternalError("Expression does not have IsExpression method: " + expression.Name, 1003, true);
                        return; // To please the compiler
                    }
                    bool isExpression = (bool)method.Invoke(null, [line])!;
                    if (isExpression)
                    {
                        object? instance = Activator.CreateInstance(expression, line);
                        if (instance is ExpressionTemplate expressionTemplate)
                        {
                            expressionFounds.Add(expressionTemplate);
                            expressionFound = true;
                        }
                        else
                        {
                            Typer.InternalError("Failed to create instance of ExpressionTemplate: " + expression.Name, 1002);
                        }
                    }
                }
                else
                {
                    Typer.InternalError("Expression is not an ExpressionTemplate: " + expression.Name, 1001);
                }
            }
            if (expressionFounds.Count > 1)
            {
                int highestImportance = -1;
                ExpressionTemplate? bestExpression = null;
                foreach (ExpressionTemplate expr in expressionFounds)
                {
                    if (expr.importance > highestImportance)
                    {
                        highestImportance = expr.importance;
                        bestExpression = expr;
                    }
                }
                if (bestExpression != null)
                {
                    lines.Add(bestExpression);
                }
            }
            else
            {
                if (expressionFounds.Count == 1)
                {
                    lines.Add(expressionFounds[0]);
                }
            }
            if (!expressionFound)
            {
                if (Program.argsGlobal.Contains("-c+=unknown-expression-warning-only"))
                {
                    Typer.CodeWarning("Unknown expression found in function (No match, please refer to the documentation)", 21);
                }
                else
                {
                    Typer.CodeError("Unknown expression found in function (No match, please refer to the documentation)", 21);
                }
            }
        }
        List<FunctionArg.FunctionArgInfos> functionArgInfos = new List<FunctionArg.FunctionArgInfos>();
        foreach (FunctionArg arg in functionArgs)
        {
            functionArgInfos.Add(arg.infos);
        }
        infos = new FunctionInfos
        {
            name = name,
            functionArgs = functionArgInfos,
            lines = GetLinesSerializable(),
            lineStart = lineStart
        };
    }
}