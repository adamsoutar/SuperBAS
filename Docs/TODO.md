# TODO in SuperBAS

*It's like a consortium... of me... the only contributor*

Ideally, transpiling to new languages could be done with a config ie. JSON file.
Like, "this is what a definition looks like in C#", "this is the skeleton for JS"

Compilation targets I'd like to have via new config system:
 - JavaScript
 - C#
 - go
 - Ruby
 - C++/Rust?
 - Python?

Embed language configs into the executable with Embedded Resources

Working PINTAT

Combined operators like `+=`

Post-increment operators?

Ternary operator? `num = 3 IF x > y ELSE 2`

`;` should prevent `\n` in `PRINT`

Capitalisation for variables should be a config option.

Should be able to assign with `LIST` and `DIM` statements

Can we make the transpiler croak tell you what line failed? It gives you absolutely 0 context rn.

Make transpilers inherit from an abstract class that implements croak + Parser AST loading

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
