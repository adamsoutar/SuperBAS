# SuperBAS Language Spec

A compiled BASIC for Windows, macOS & Linux.

## Commands

#### PRINT

Logs the operand (string or number) to STDOUT

`PRINT "Hello, world!"`

#### CLS

Clears the console (no operand)

#### WAITKEY

Wait for a keypress before resuming the program

#### EXIT

Quit the program immediately

#### STOP

Quit the program after waiting for a keypress

Equivalent of:

```
1 WAITKEY
2 EXIT
```

#### GOTO

Jumps program to the operand as a line number. Operand can be an expression based on user input and does not have to be a compile-time constant!

`GOTO 10`

`GOTO x + 4 * 2`

#### GOSUB

Usage is the same as GOTO, but control returns to this line when a 'RETURN' command is encountered.

`GOSUB 10`

`GOSUB x + y`

#### RETURN

Return the program to the line after the latest GOSUB - If no GOSUB has been called, this terminates the program. No operand.

#### LET

Assign a variable.

`LET x = 10`

`LET name$ = "Adam"`

#### DIM

Create an n-dimensional array. Call parameters indicate how many items there are in that dimension of the array.

`DIM names$(3)`

*Note:* names$ will have 3 items: 0, 1 and 2.

`DIM map(10, 10)`

`DIM fourDVector(20, 10, 30, 15)`

#### LIST

Creates a list. Lists are like arrays but do not have a fixed size. For manipulating them, see `LISTADD` and `LISTRM`. List indices begin at 0 and end at the length - 1. Eg, a list that is 3 items long has indices 0, 1 and 2.

`LIST myList`

`LIST names$`

#### LISTADD

Add an item to the end of the specified list.

`LISTADD names$, "Adam"`

`LISTADD myList, 3.5`

#### LISTRM

Remove the item at the specified index.

`LISTRM names$, 0` - Removes the first (left-most) item of names$

`LISTRM myList, LEN(myList) - 1` - Removes the last (right-most) item of myList

#### INK

Change the colour of the console's foreground text. See "Console colours appendix" for the colour values.

`INK 7`

`INK x`

#### PAPER

Usage is the same as INK, but changes the background colour. Initially only change the colour for printing "from now on". To change the entire background, call PAPER, then CLS.

#### INPUT

Takes input from the console and assigns it to a variable. Optionally presents a prompt.

`INPUT myString$`

`INPUT "What's your name?", name$`

#### =

Perform an assignment to an **already defined** variable - not technically a command, but treated as such. Use with array index calls to set the variable at that index.

`x = 5`

`myMap(2, 3) = 4`

#### FOR

Initiate a counting loop.

`FOR i = 1 TO 5`

Starts a loop where i (not previously defined) will be 1, 2, 3, 4 then 5. Almost all loops should have a NEXT statement at the end. There's an optional STEP argument to change the amount the loop increases by each time.

`FOR x = 0 TO 1 STEP 0.2`

#### NEXT

`NEXT i`

Increments that loop variable and, if it still satisfies the loop condition, jumps back to the start of the loop.

#### TOPOF

`TOPOF x`

Jumps to the start of the loop without performing a check or incrementing the counter

#### IF

If the condition is met, execute the command listed afer THEN. Optionally, if it isn't met, perform the command after ELSE.

`IF x > 5 THEN PRINT "Big number"`

`IF number > 10 THEN PRINT "Big" ELSE PRINT "Small"`

*Note 1*

When used in conjunction with `:`, this does **not** allow you to execute multiple statements on the satisfaction of an IF statement. They are separated as though they followed on multiple lines.

`IF x = 5 THEN PRINT "Hello" : PRINT "World"`

Prints "World" **always** no matter what the outcome of the if was.

*Note 2*

Conditionals are evaluated lazily, in the statement:

`IF x() = 10 AND y() = 20 THEN ...`

Where x and y are user functions, if x() is 10, then y is **not called** (likewise with OR if condition 1 is true).

*Note 3*

When used in IF statement conditions, `=` is not an assignment but a comparator.

#### SLEEP

Pause the program for the amount of milliseconds passed to this command

`SLEEP 1000` - Sleeps for 1 sec

`SLEEP x`

#### WRITEFILE

Write a string expression to a file. If it already exists, it will be emptied and over-written.

`WRITEFILE "./hello.txt", x$`

#### APPENDFILE

Similar to `WRITEFILE` but if the file exists, the string is appended to the end.

#### PRINTAT

Print the passed string at a location on the screen

`PRINTAT 5, 5, "Hello"`

`PRINTAT x, y, string$`

### Console colours

The following colour IDs are used by `INK` and `PAPER`:

Also see [here](https://docs.microsoft.com/en-us/dotnet/api/system.consolecolor?view=netcore-2.2)

 0. Black
 1. Dark Blue
 2. Dark Green
 3. Dark Cyan
 4. Dark Red
 5. Dark Magenta
 6. Dark Yellow
 7. Grey
 8. Dark Grey
 9. Blue
 10. Green
 11. Cyan
 12. Red
 13. Magenta
 14. Yellow
 15. White

```
1 FOR i = 0 TO 15
2 INK i
3 PRINT "Multicolours!"
4 NEXT i
```

## Standard library

The standard library are built in functions such as ABS and SIN

#### STR$

Converts a number into a string

```
1 LET x = 10
2 PRINT "x is " + STR$(x)
```

#### VAL

Converts a string into a number

```
1 LET mystr$ = "2"
2 PRINT VAL(mystr$) * 2
```

#### SPLIT$

Splits a string by a separator & returns a list of substrings.

```
1 LET a$ = "Adam,George,James,Stephen"
2 LIST lst$
3 lst$ = SPLIT$(a$, ",")
```
*Note: As of now, you can't assign in a `LIST` command. You must initialise the list, then assign to it in a separate line.*

#### JOIN$

Join a string array into a single string separated by the operand.

```
1 DIM a$(2)
2 a$(0) = "Hello"
3 a$(1) = "World"
4 PRINT JOIN$(a$, ",")
```

#### LEN

Gets the length of a string, list or array.
If used on a multi-dimensional array, returns the area/volume of the grid. ie. the LEN of myMap(5, 3, 2) returns 30 (5 * 3 * 2)

```
1 DIM a(5)
2 LIST b
3 LET c$ = "Hello"
5 PRINT LEN(a) + LEN(b) + LEN(c$)
```

The program above prints 10

#### SIN

Returns the sine of the first parameter **in radians**

#### COS

Returns the cosine of the first parameter **in radians**

#### TAN

Returns the tangent of the first parameter **in radians**

#### FLOOR

Returns the lowest integer before the decimal passed as the parameter, ie. cuts off the decimal point.

`FLOOR(3.9) = 3`

#### CEIL

Returns the smallest integer after the number passed as the parameter. ie. Round up.

`CEIL(3.1) = 4`

#### ROUND

Rounds the number to the closest integer. If passed a second argument, that is the amount of decimal points to round to.

`ROUND(4.4) = 4`

`ROUND(4.5) = 5`

`ROUND(4.46, 1) = 4.5`

See [this question](https://stackoverflow.com/questions/311696/why-does-net-use-bankers-rounding-as-default) and [C#'s documentation](https://docs.microsoft.com/en-us/dotnet/api/system.math.round?view=netcore-2.2#System_Math_Round_System_Double_) for details like why `ROUND(4.45, 1) = 4.4`

## Keywords

All commands and standard library function names are keywords, and as such are not relisted here.

All keywords are reserved, and are illegal names for user variables and functions.

#### THEN

Mandatory with `IF`

#### ELSE

Optional with `IF`

#### TO

Mandatory in `FOR` loop definition

#### STEP

Optional in `FOR` definition

## Compiler Keywords

Any line beginning with `#` instead of a line number indicates a command at compile time, rather than part of program code.

#### INCLUDE

Use with a string indicating a path to another basic file. This file will essentially be 'pasted' into the program.

Calculate.bas
```
3 LET z = x + y
```

Main.bas
```
1 LET x = 1
2 LET y = 2
#INCLUDE "Calculate.bas"
4 PRINT z
```

Outputs 3
