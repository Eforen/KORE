using System;
using Kore.RiscMeta;

namespace Kore.AST
{
    /// <summary>
    /// Maps assembler <see cref="InlineDirectiveNode.InlineDirectiveType"/> tokens (e.g. <c>%pcrel_hi</c>) plus the
    /// concrete <see cref="InstructionNode"/> shape to the ELF <see cref="RiscvRelocationType"/> used at link time.
    /// </summary>
    public static class RelocationSiteMapping
    {
        /// <summary>
        /// Resolves ELF relocation kind from the inline directive and the wrapped instruction (I/S/B/J/U determine LO12_I vs LO12_S, etc.).
        /// </summary>
        public static RiscvRelocationType ToElfRelocation(InlineDirectiveNode.InlineDirectiveType directive, InstructionNode wrapped)
        {
            if (wrapped == null)
            {
                throw new ArgumentNullException(nameof(wrapped));
            }

            switch (directive)
            {
                case InlineDirectiveNode.InlineDirectiveType.PCREL_HI:
                    return RiscvRelocationType.R_RISCV_PCREL_HI20;

                case InlineDirectiveNode.InlineDirectiveType.PCREL_LO:
                    if (wrapped is InstructionNodeTypeS)
                    {
                        return RiscvRelocationType.R_RISCV_PCREL_LO12_S;
                    }

                    if (wrapped is InstructionNodeTypeBImmediate)
                    {
                        return RiscvRelocationType.R_RISCV_BRANCH;
                    }

                    if (wrapped is InstructionNodeTypeJImmediate)
                    {
                        return RiscvRelocationType.R_RISCV_JAL;
                    }

                    return RiscvRelocationType.R_RISCV_PCREL_LO12_I;

                case InlineDirectiveNode.InlineDirectiveType.HI:
                    return RiscvRelocationType.R_RISCV_HI20;

                case InlineDirectiveNode.InlineDirectiveType.LO:
                    if (wrapped is InstructionNodeTypeS)
                    {
                        return RiscvRelocationType.R_RISCV_LO12_S;
                    }

                    return RiscvRelocationType.R_RISCV_LO12_I;

                case InlineDirectiveNode.InlineDirectiveType.ADDR:
                    return RiscvRelocationType.R_RISCV_32;

                default:
                    throw new ArgumentOutOfRangeException(nameof(directive), directive, "Unknown inline directive for relocation.");
            }
        }
    }
}
