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

User functions

~~REM/Comments~~

When jumping to a line that is out of bounds, we should continue execution from the next valid line

Compiler config.json file or something - configure some opinionated switches, like decimal accuracy (BigInt for JS)

Publish the spec in the releases so that its information doesn't go out of sync with the version you download.