1 LIST names$
2 LISTADD names$, "Adam"
3 LISTADD names$, "Rob"
4 FOR i = 0 TO LEN(names$) - 1
5 PRINT names$(i)
6 NEXT i
7 PRINT LEN("Hello" + "World")
