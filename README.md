# 👾 SuperBAS

A compiler for turning a BASIC superset into native executables and/or web code.

It's based on .NET Core 3 and supports Windows, macOS & Linux

## What and why?

SuperBAS (Superset of BASIC) is based on ZX BASIC, designed to simplify complex parts and add commands for interfacing with modern operating systems.

```
1 name$ = "SuperBAS!"
2 PRINT "Hello, " + name$
3 GOTO 1
```

So why?

**People who played around with home computers in the 80s like the Spectrum or Amstrad can feel alienated by programming for modern PCs**, and emulators or interpreters for old languages are plagued by low speed or dated sugar-less syntax.

SuperBAS fixes that! It is designed to match your rose-tinted view of older BASIC languages, not the frustration you felt with certain features, and it compiles right down to native binaries or runs right in a webpage with C# and JS transpilation. SuperBAS is not emulated or interpreted.

## Targets

SuperBAS' compiler is designed to target almost any language you can think of. Adding support for a new target is near-trivial for someone well antiquated with their language.

#### Supported right now

 - C#
 - JavaScript

#### Support is planned

 - Go
 - Ruby
 - Your favourite language? PRs are welcome

## Usage

Download [a release for your platform](https://github.com/adamsoutar/SuperBAS/releases), run it like so:

```bash
./SuperBAS test.sbas Program.cs CSharp
```

You can compile the resulting file on Windows with a command like this (assuming you have VS Build Tools installed):

```bash
csc Output.cs /out:MyProgram.exe /optimize
```

or

```bash
./SuperBAS test.sbas Script.js JavaScript
```

## So where do I start?

Using [the spec](https://github.com/adamsoutar/SuperBAS/blob/master/Docs/LanguageSpec.md) to learn SuperBAS is easy. Every command, operator and standard library function is documented with examples.

If you're looking for full examples like FizzBuzz, see [/BasicCode](https://github.com/adamsoutar/SuperBAS/blob/master/BasicCode/FizzBuzz.sbas).

Watch out though, it's a little full of test scripts in there.

## Live life in colour!

Syntax highlighting is available for [Atom](https://github.com/adamsoutar/atom-language-superbas) and [VS Code](https://github.com/adamsoutar/vscode-language-superbas)
