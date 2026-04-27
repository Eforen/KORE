using System.Text;
using Kore.RiscMeta;

namespace Kore.AST
{
    /// <summary>
    /// Wraps a concrete <see cref="InstructionNode"/> that will receive an ELF relocation (symbol + addend + r_type)
    /// when the assembler / linker fixes up the instruction word. This is the explicit AST form for sites that are
    /// not yet fully resolved (e.g. <c>auipc</c>/<c>addi</c> from <c>la</c>, or <c>%pcrel_hi</c>/<c>%pcrel_lo</c> operands).
    /// </summary>
    public abstract class RelocationInstructionNode : AstNode
    {
        /// <summary>ELF relocation type for the slot that patches <see cref="GetWrappedInstruction"/>.</summary>
        public RiscvRelocationType RelocationKind { get; set; }

        /// <summary>Symbol name this relocation refers to (<c>r_sym</c> / label string).</summary>
        public string SymbolName { get; set; }

        /// <summary>Stable id of <see cref="SymbolName"/> in the program symbol table (assigned when label references are resolved).</summary>
        public uint SymbolId { get; set; }

        /// <summary>
        /// For <see cref="RiscvRelocationType.R_RISCV_PCREL_LO12_I"/> / <c>LO12_S</c>, optional name of the paired
        /// <c>auipc</c> / HI20 site (<c>%pcrel_lo</c> of matching <c>%pcrel_hi</c>); often left unset when the linker pairs by section layout.
        /// </summary>
        public string RelatedSymbol { get; set; }

        /// <summary>Stable id of <see cref="RelatedSymbol"/> when set; otherwise 0.</summary>
        public uint RelatedSymbolId { get; set; }

        public long Addend { get; set; }

        public abstract InstructionNode GetWrappedInstruction();

        public override AstNode CallProcessor(ASTProcessor processor)
        {
            return processor.ProcessASTNode(this);
        }

        public override int GetOwnByteSize()
        {
            return GetWrappedInstruction().GetOwnByteSize();
        }

        public override int GetTotalByteSize()
        {
            return GetOwnByteSize();
        }

        protected bool BaseEquals(RelocationInstructionNode other)
        {
            return RelocationKind == other.RelocationKind
                && SymbolName == other.SymbolName
                && SymbolId == other.SymbolId
                && RelatedSymbol == other.RelatedSymbol
                && RelatedSymbolId == other.RelatedSymbolId
                && Addend == other.Addend;
        }

        protected int BaseHashCode()
        {
            unchecked
            {
                int hash = RelocationKind.GetHashCode();
                hash = (hash * 397) ^ (SymbolName != null ? SymbolName.GetHashCode() : 0);
                hash = (hash * 397) ^ SymbolId.GetHashCode();
                hash = (hash * 397) ^ (RelatedSymbol != null ? RelatedSymbol.GetHashCode() : 0);
                hash = (hash * 397) ^ RelatedSymbolId.GetHashCode();
                hash = (hash * 397) ^ Addend.GetHashCode();
                return hash;
            }
        }

        protected StringBuilder AppendRelocationHeader(int indentLevel, StringBuilder builder)
        {
            addDebugTextHeader(false, -1, indentLevel, builder)
                .Append("RELOC ")
                .Append(RelocationKind)
                .Append(" symbol[")
                .Append(SymbolId)
                .Append("]:")
                .Append(SymbolName);
            if (!string.IsNullOrEmpty(RelatedSymbol))
            {
                builder.Append(" related[").Append(RelatedSymbolId).Append("]:").Append(RelatedSymbol);
            }

            if (Addend != 0)
            {
                builder.Append(" addend:").Append(Addend);
            }

            return builder.AppendLine(" {");
        }

        protected StringBuilder AppendRelocationFooter(int indentLevel, StringBuilder builder)
        {
            return addDebugTextHeader(false, -1, indentLevel, builder).AppendLine("}");
        }
    }

    /// <summary>
    /// Strongly typed relocation wrapper around a single instruction AST node.
    /// </summary>
    public sealed class RelocationInstructionNode<T> : RelocationInstructionNode where T : InstructionNode
    {
        public T WrappedInstruction { get; set; }

        public override InstructionNode GetWrappedInstruction()
        {
            return WrappedInstruction;
        }

        public override bool Equals(object obj)
        {
            RelocationInstructionNode<T> other = obj as RelocationInstructionNode<T>;
            if (other == null)
            {
                return false;
            }

            return BaseEquals(other) && Equals(WrappedInstruction, other.WrappedInstruction);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (BaseHashCode() * 397) ^ (WrappedInstruction != null ? WrappedInstruction.GetHashCode() : 0);
            }
        }

        public override StringBuilder getDebugText(int indentLevel, StringBuilder builder)
        {
            AppendRelocationHeader(indentLevel, builder);
            WrappedInstruction.getDebugText(indentLevel + 1, builder);
            return AppendRelocationFooter(indentLevel, builder);
        }
    }
}
