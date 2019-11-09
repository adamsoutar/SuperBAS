1 LIST a$
2 LISTADD a$, "Hello"
3 LISTADD a$, " World"
4 WRITEFILE "hello.txt", JOIN$(a$, ",") + "!"
5 PRINT "It says: " + READFILE$("hello.txt")
