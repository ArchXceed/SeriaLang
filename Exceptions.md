# Compile errors

## Arguments (CodePrefix: 10)
- Invalid function argument (Code: 11), Type: SyntaxError

- Invalid function argument count (Code: 12), Type: SyntaxError

## General Syntax (CodePrefix: 20)
- Unknown expression found in function (Code 21), Type: SyntaxError. Basicly the syntax is not found. Can be set to warning by cli arg -c+=unknown-expression-warning-only

- Function xyz not closed (Code 22), Type: SyntaxError. Did you forget to put the function name to close it?

## Typing (CodePrefix: 30)

- TypeError: Type xyz not found (Code: 31), Type: TypeError

- TypeError: Value abc is not of type xyz (Code: 32), Type: TypeError

- TypeError: Value abc isn't a known literal/global constant/local variable (Code: 33), Type: TypeError

## Internal (CodePrefix 1000)
- Expression is not an ExpressionTemplate (Code: 1001), Type: InternalError

- Tried to parse a xyz expression as xyz (Code: 1002), Type: InternalError

- Expression does not have IsExpression method (Code: 1003), Type: InternalError