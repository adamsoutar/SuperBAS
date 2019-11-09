# SuperBAS

A compiler for turning a BASIC superset into native executables and/or web code.

## What and why?

SuperBAS is based on ZX BASIC, designed to simplify complex parts and add commands for interfacing with modern operating systems.

```
1 LET name$ = "SuperBAS!"
2 WRITEFILE "./hi.txt", "Hello, I'm " + name$
```

So why?

People who played around with home computers in the 80s like the Spectrum or Amstrad can feel alienated by modern languages, and running emulators or interpreters for old languages are plagued by low speed or dated sugar-less syntax.

SuperBAS fixes that! It is designed to match your nostalgia for older BASIC languages, not your frustration, and it compiles right down to a native binary via C# (or runs in a webpage with experimental Javascript transpilation). SuperBAS is not emulated or interpreted.

## Usage

Download a release for your platform, run it like so:

```bash
./SuperBas test.bas Program.cs native
```

You can compile the resulting file on Windows with

```bash
csc -out:MyProgram.exe Output.cs
```

or

```bash
./SuperBAS test.bas Script.js web
```

*Note: JS transpilation is at a pre-alpha stage*

## So where do I start?

Using [the spec](https://github.com/adamsoutar/SuperBAS/blob/master/Docs/LanguageSpec.md) to learn SuperBAS is easy. Every command, operator and standard library function is documented with examples.

If you're looking for full examples like FizzBuzz, see [/BasicCode](https://github.com/adamsoutar/SuperBAS/blob/master/BasicCode/FizzBuzz.bas) watch out though, it's a little full of test scripts in there.
