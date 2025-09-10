
using System.Reflection;

public class Function
{
    public Function(string name, List<FunctionArg> functionArgs)
    {
        this.name = name;
        this.functionArgs = functionArgs;
    }

    public void Compile(List<Type> expressions)
    {
        for (int lineNumber = 0; lineNumber < insideCode.Count; lineNumber++)
        {
            string line = insideCode[lineNumber];
            Typer.currentContext.lineCode = line;
            Typer.currentContext.line = lineNumber + lineStart;
            bool expressionFound = false;
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
                            lines.Add(expressionTemplate);
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
    }

    public List<string> insideCode = new List<string>();
    public string name;
    public List<ExpressionTemplate> lines = new List<ExpressionTemplate>();
    public int lineStart = 0;
    public List<FunctionArg> functionArgs = new List<FunctionArg>();
}