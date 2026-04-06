using System;
using System.IO;
using Kore.Kuick;

namespace Kore.Kuick.AstDump;

internal static class Program {
    private static int Main(string[] args) {
        if(args.Length == 0 || args[0] is "-h" or "--help") {
            Console.Error.WriteLine("Usage: Kore.Kuick.AstDump <file.S>");
            Console.Error.WriteLine("Reads RISC-V assembly, parses it, and prints AstNode.getDebugText() to stdout.");
            Console.Error.WriteLine("Example: dotnet run --project Kore.Kuick.AstDump -- path/to/foo.S");
            return args.Length == 0 ? 1 : 0;
        }

        var path = args[0];
        if(!File.Exists(path)) {
            Console.Error.WriteLine($"File not found: {path}");
            return 1;
        }

        try {
            var source = File.ReadAllText(path);
            var lexer = new Lexer();
            lexer.Load(source);
            var ast = Parser.Parse(lexer);
            Console.Write(ast.getDebugText());
        } catch(Exception ex) {
            Console.Error.WriteLine(ex.Message);
            return 1;
        }

        return 0;
    }
}
