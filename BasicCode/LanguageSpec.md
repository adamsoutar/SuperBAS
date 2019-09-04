# SuperBAS Language Spec

A compiled BASIC for Windows, macOS & Linux.

## Commands

#### PRINT

Logs the operand (string or number) to STDOUT

`PRINT "Hello, world!"`

#### CLS

Clears the console (no operand)

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

Assign a variable. If used on an array/list call, assign to the variable at that index.

`LET x = 10`

`LET name$ = "Adam"`

`LET myMap(10, 5) = x`

#### =

Perform an assignment to an **already defined** variable - not technically a command, but treated as such.

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

## Standard library

The standard library are built in functions such as ABS and SIN

**As yet, none of these are implemented**

## Keywords

All commands and standard library function names are keywords, and as such are not relisted here.

All keywords are reserved, and are illegal names for user variables and functions.

#### THEN

Mandatory with IF

#### ELSE

Optional with IF

## Binary operators

These are operators that take two arguments such as + or /

#### +

Precedence: 15

#### -

Precedence: 10

#### *

Precedence: 20

#### /

Precedence: 25

#### MOD

Precedence: 20, get the remainder if the left side were to be divided by the right. ie. 10 MOD 2 is 0, 10 MOD 3 is 1

#### NOT

Precedence: 7, inverts a boolean. Ie. `IF NOT x = 5 THEN`

#### !=

Precedence: 7, short for NOT something = something

#### OR

Precedence: 7, x OR y is true if either are true

#### AND

Precedence: 7

#### =

Precedence: 1

Used as part of an if statement, compares two objects. Else, causes an assignment

#### <

Precedence: 7

#### >

Precedence: 7

#### <=

Precedence: 7

#### >=

Precedence: 7
