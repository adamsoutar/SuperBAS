# TODO in SuperBAS

*It's like a consortium... of me... the only contributor*

~~`!=` should also be aliased with `<>`~~

`;` should prevent `\n` in `PRINT`

~~Capitalisation should not be enforced on keywords or variables~~, capitalisation for variables should be a config option.

~~`:` should allow multi-line `IF`~~

~~First reference to var should initialise it.~~

Should be able to assign with `LIST` and `DIM` statements

~~Arrays of string and uninitialised string (`LET a$`) vars should be `""` NOT `null`.~~

~~`INT` should be an alias for `FLOOR`~~

Can we make the transpiler croak tell you what line failed? It gives you absolutely 0 context rn.

Make transpilers inherit from an abstract class that implements croak + Parser AST loading

~~User functions~~

~~REM/Comments~~

When jumping to a line that is out of bounds, we should continue execution from the next valid line

Compiler config.json file or something - configure some opinionated switches, like decimal accuracy (BigInt for JS)

Publish the spec in the releases so that its information doesn't go out of sync with the version you download.

In the code, I'd like to change the definition of stdlib to strings like `STR$`, not
```csharp
new ASTVariable {
  Name = "STR",
  IsString = true
}
```
Seems over the top.

Add `redefinedLoops` option
```basic
1 FOR i = 0 TO 5
2 PRINT i
3 NEXT i

4 FOR i = 10 TO 1 STEP -1
5 PRINT i
6 NEXT i
```
Users might expect this to work in SuperBAS, which it won't (due to ambiguous `NEXT i`). Redefined loops would increase runtime overhead with the benefit of making this work (two loops with the same counter variable). Maybe we could auto-enable it if we spot this pattern.

Make executable compilation easier - maybe automate the `dotnet publish` commands. This makes the `.NET Core SDK` a dependency. This will greatly reduce the size of the SuperBAS executable though, since it wouldn't have to be framework-independent.