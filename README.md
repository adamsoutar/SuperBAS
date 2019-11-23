# SuperBAS

A compiler for turning a BASIC superset into native executables and/or web code.

It's based on .NET Core 3 and supports Windows, macOS & Linux

## What and why?

SuperBAS is based on ZX BASIC, designed to simplify complex parts and add commands for interfacing with modern operating systems.

```
1 LET name$ = "SuperBAS!"
2 WRITEFILE "./hi.txt", "Hello, I'm " + name$
```

So why?

**People who played around with home computers in the 80s like the Spectrum or Amstrad can feel alienated by programming for modern PCs**, and emulators or interpreters for old languages are plagued by low speed or dated sugar-less syntax.

SuperBAS fixes that! It is designed to match your nostalgia for older BASIC languages, not your frustration, and it compiles right down to a native binary via C# (or runs in a webpage with experimental Javascript transpilation). SuperBAS is not emulated or interpreted.

## Usage

Download [a release for your platform](https://github.com/adamsoutar/SuperBAS/releases), run it like so:

```bash
./SuperBas test.sbas Program.cs native
```

You can compile the resulting file on Windows with a command like this (assuming you have VS Build Tools installed):

```bash
csc Output.cs /out:MyProgram.exe /optimize
```

or

```bash
./SuperBAS test.sbas Script.js web
```

*Note: JS transpilation is at a pre-alpha stage*

## So where do I start?

Using [the spec](https://github.com/adamsoutar/SuperBAS/blob/master/Docs/LanguageSpec.md) to learn SuperBAS is easy. Every command, operator and standard library function is documented with examples.

If you're looking for full examples like FizzBuzz, see [/BasicCode](https://github.com/adamsoutar/SuperBAS/blob/master/BasicCode/FizzBuzz.sbas).

Watch out though, it's a little full of test scripts in there.
