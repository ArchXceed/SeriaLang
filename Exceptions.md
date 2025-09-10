# Compile errors

## Arguments (CodePrefix: 10)
- Invalid function argument (Code: 11), Type: SyntaxError

- Invalid function argument count (Code: 12), Type: SyntaxError

## General Syntax (CodePrefix: 20)
- Unknown expression found in function (Code 21), Type: SyntaxError. Basicly the syntax is not found. Can be set to warning by cli arg -c+=unknown-expression-warning-only

- Function xyz not closed (Code 22), Type: SyntaxError. Did you forget to put the function name to close it?

- SyntaxError: Invalid condition syntax (Code 23), Type: SyntaxError.

- SyntaxError: Invalid function invocation syntax in [LOGIN_EXPRESSION] expression (Code 24), Type: SyntaxError

- SyntaxError: Unknown expression (Code 25), Type: SyntaxError

## Typing (CodePrefix: 30)

- TypeError: Type xyz not found (Code: 31), Type: TypeError

- TypeError: Value abc is not of type xyz (Code: 32), Type: TypeError

- TypeError: Value abc isn't a known literal/global constant/local variable (Code: 33), Type: TypeError

- TypeError: Argument Count in function abc must be an abc, got: xyz (Code: 34), Type: TypeError

## Import Error (CodePrefix: 40)
- Import file abc does not exist (Code 41), Type: ImportError

- File already imported (Code 42), Type: ImportWarning

## .uchc Files error (UniversalCompilatorHeaderCompiler) (CodePrefix: 50)
- CustomDefineError: Custom define name abc is already used by another custom define or function (Code: 51), Type: CustomDefineError

## Internal (CodePrefix 1000)
- Expression is not an ExpressionTemplate (Code: 1001), Type: InternalError

- Tried to parse a xyz expression as xyz (Code: 1002), Type: InternalError

- Expression does not have IsExpression method (Code: 1003), Type: InternalError

- Argument count value is null in function modification expression (Code: 1004), Type: InternalError. This should never happen, please report this bug.