1 LET count = 0
2 count = count + 1
3 LET out$ = ""
4 IF count MOD 3 = 0 THEN out$ = out$ + "Fizz"
5 IF count MOD 5 = 0 THEN out$ = out$ + "Buzz"
6 IF out$ = "" THEN out$ = STR$(count)
7 PRINT out$
8 GOTO 2
