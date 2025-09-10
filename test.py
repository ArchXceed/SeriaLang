import re
prog = re.compile(r"[fun ]+[a-zA-Z0-9]+\(+([A-Za-z]+.+[A-Za-z]+ +[a-zA-Z0-9]+[;])+\)+[:]")
print(prog.match("fun test(S.s m;I.i m;):"))