********
Glossary
********

.. glossary::
   :sorted:

   compilation unit
      A single source file and all the header files that are included in it, either directly or indirectly. In assembly, this typically refers to a single `.s` or `.asm` file that gets processed by the assembler.

   external linkage
      A property of symbols that makes them accessible from other compilation units. Symbols with external linkage can be referenced across different source files during the linking process.

   export symbols
      The process of making symbols available to other compilation units by placing them in the global symbol table. This is done using directives like `.global` or `.globl`.

   file scope
      The visibility of a symbol is limited to the current source file only. Symbols with file scope cannot be accessed from other compilation units.

   global scope
      The broadest level of symbol visibility where symbols are accessible across all compilation units in a program. Symbols in global scope have external linkage.

   internal linkage
      A property of symbols that restricts their visibility to the current compilation unit only. Symbols with internal linkage cannot be accessed from other source files.

   linker
      A program that combines multiple object files (compiled from different compilation units) into a single executable program. The linker resolves symbol references between different compilation units.

   local linkage
      Same as internal linkage. A property that restricts symbol visibility to the current compilation unit.

   private symbols
      Symbols that are not exposed outside their compilation unit, typically marked with `.local` directive. These symbols are only accessible within the file where they are defined.

   public API
      The set of functions, variables, and other symbols that are intentionally exposed for use by other parts of the program or by external programs. These symbols typically have external linkage.

   symbol
      A name that represents a memory location, function, or constant in assembly code. Symbols include labels, function names, variable names, and constants.

   symbol collision
      An error that occurs when two or more symbols with the same name are defined in the same scope, causing ambiguity for the linker about which symbol to use.

   symbol names
      The identifiers used to refer to memory locations, functions, or data in assembly code. Symbol names are resolved to actual memory addresses during assembly and linking.

   symbol table
      A data structure maintained by the assembler and linker that maps symbol names to their memory addresses, types, and scope information. The symbol table is used to resolve references between different parts of the program. 