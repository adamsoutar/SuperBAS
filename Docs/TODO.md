# TODO in SuperBAS

*It's like a consortium... of me... the only contributor*

## File IO

Since we don't have any types besides strings and floats, we can't keep a file stream object or anything. As a result, it looks like we might be limited to reading a file in its entirety.

```
1 LET x$ = READFILE$("./hello.txt")
```

We can do slightly better with writing

```
Write the string, if the file exists, empty and overwrite it
1 WRITEFILE "./hello.txt", x$

Add to the end of the file
2 APPENDFILE "./hello.txt", x$
```

In order to effectively read back data, we will probably need string split functions and, while we're at it, we should probably add join too.

```
1 LIST a$ = SPLIT$(x$, ",")
2 LET x$ = JOIN$(a$, ",")
```
