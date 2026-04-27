using Kore.AST;

namespace Kore.Kuick {
    /// <summary>
    /// Mutable state for <see cref="Parser.Parse"/> (program tree, current section index). Passed through static parse methods.
    /// </summary>
    public sealed class ParserContext {
        public ProgramNode Program { get; }

        /// <summary>
        /// Index into <see cref="ProgramNode.Sections"/> for the section currently being parsed, or <c>-1</c> before any section.
        /// </summary>
        public int CurrentSectionIndex { get; set; } = -1;

        /// <summary>Once true, file-level <see cref="ProgramNode.Preamble"/> is no longer collected (first section has been opened).</summary>
        public bool HaveOpenedAnySection { get; set; }

        public ParserContext() {
            Program = new ProgramNode();
        }
    }
}
