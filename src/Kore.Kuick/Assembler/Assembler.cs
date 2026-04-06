using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kore.AST;
using Kore.RiscMeta;

namespace Kore.Kuick.Assembler {
    public class Assembler
    {

    }

    public struct AssemblerContext {
        /// <summary>Relocation records to fix up after section layout (PC-relative hi/lo pairs, etc.).</summary>
        public List<Relocation> Relocations { get; set; }
        public List<Symbol> Symbols { get; set; }
        public List<AssembledSectionObject> Sections { get; set; }

        // Pointer to the current Section using the index of the Sections list
        public int CurrentSectionIndex { get; set; }

        public AssemblerContext(List<Symbol> symbols, List<AssembledSectionObject> sections, int currentSectionIndex) {
            // Always valid for consumers; avoid null checks when appending relocations.
            Relocations = new List<Relocation>();
            Symbols = symbols;
            Sections = sections;
            CurrentSectionIndex = currentSectionIndex;
        }

        public static AssemblerContext CreateBlank() {
            return new AssemblerContext {
                // Relocations starts empty; populated as instructions with symbolic operands are assembled.
                Relocations = new List<Relocation>(),
                Symbols = new List<Symbol>(),
                Sections = new List<AssembledSectionObject>(),
                CurrentSectionIndex = 0
            };
        }
    }

    public class AssembledSectionObject {
        public string Name { get; set; }
        public List<byte> MachineCode { get; set; }
        public List<Relocation> Relocations { get; set; }
    }
}