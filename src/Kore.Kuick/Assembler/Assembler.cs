using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kore.AST;
using Kore.RiscMeta;

namespace Kore.Kuick.Assembler {
    public class AssemblerContext
    {
        public AssemblerContext()
        {
            Symbols = new List<Symbol>();
            Sections = new List<AssembledSectionObject>();
            // Initialize the text section
            CurrentSection = new AssembledSectionObject
            {
                Name = ".text",
                Alignment = 4,
                SectionRelativeOffset = 0,
                MachineCode = new List<byte>(),
                Relocations = new List<Relocation>()
            };
            Sections.Add(CurrentSection);
        }
        public AssemblerContext(List<Symbol> symbols, List<AssembledSectionObject> sections, int currentSectionIndex)
        {
            Symbols = symbols;
            Sections = sections;
            CurrentSection = sections[currentSectionIndex];
        }

        public List<Symbol> Symbols { get; set; }
        public List<AssembledSectionObject> Sections { get; set; }

        // Pointer to the current Section using the index of the Sections list
        public AssembledSectionObject CurrentSection { get; set; }
    }

    public class AssembledSectionObject
    {
        public string Name { get; set; }
        /// <summary>The current alignment of the section.</summary>
        public uint Alignment { get; set; }
        /// <summary>Cache of the current position in the section used mainly for alignment.</summary>
        public ulong SectionRelativeOffset { get; set; }
        /// <summary>The machine code for the section.</summary>
        public List<byte> MachineCode { get; set; } = new List<byte>();
        /// <summary>The relocations for the section.</summary>
        public List<Relocation> Relocations { get; set; } = new List<Relocation>();

        public void Align()
        {
            if (SectionRelativeOffset % (ulong)Alignment != 0)
            {
                ulong padding = Alignment - (SectionRelativeOffset % (ulong)Alignment);
                MachineCode.AddRange(new byte[padding]);
                SectionRelativeOffset += padding;
            }
        }
        public void AddAlignedCode(byte[] code)
        {
            Align();
            MachineCode.AddRange(code);
            SectionRelativeOffset += (ulong)code.Length;
        }
        public void AddUnalignedCode(byte[] code)
        {
            MachineCode.AddRange(code);
            SectionRelativeOffset += (ulong)code.Length;
        }
        public void AddAlignedCode(uint instruction)
        {
            AddAlignedCode(BitConverter.GetBytes(instruction));
        }
        public void AddUnalignedCode(uint instruction)
        {
            AddUnalignedCode(BitConverter.GetBytes(instruction));
        }
    }

    public class Assembler : ASTProcessor
    {
        /// <summary>The context of the assembler.</summary>
        protected AssemblerContext context = new AssemblerContext();
        /// <summary>The program to assemble.</summary>
        protected ProgramNode program;

        public Assembler(AssemblerContext context, ProgramNode program)
        {
            this.context = context;
            this.program = program;
        }

        public void Assemble()
        {
            program.CallProcessor(this);
        }

        public void WriteObjectToElf(string outputPath)
        {
            // TODO: Write the object to elf
        }

        public AstNode ProcessASTNode(ProgramNode node)
        {
            foreach(SectionNode n in node.Sections) {
                n.CallProcessor(this);
            }
            return node;
        }

        public AstNode ProcessASTNode(SectionNode node)
        {
            // If the section is not found, create a new one
            if (context.Sections.FirstOrDefault(s => s.Name == node.Name) == null) {
                context.CurrentSection = new AssembledSectionObject
                {
                    Name = node.Name,
                    Alignment = 4,
                    SectionRelativeOffset = 0,
                    MachineCode = new List<byte>(),
                    Relocations = new List<Relocation>()
                };
                context.Sections.Add(context.CurrentSection);
            }
            // If the section is not the current section, set the current section
            if (context.CurrentSection.Name != node.Name)
            {
                context.CurrentSection = context.Sections.FirstOrDefault(s => s.Name == node.Name);
            }
            // Process the contents of the section
            foreach(var content in node.Contents) {
                content.CallProcessor(this);
            }
            return node;
        }

        public AstNode ProcessASTNode(DirectiveNode node)
        {
            throw new NotImplementedException();
        }

        public AstNode ProcessASTNode(SymbolDirectiveNode node)
        {
            throw new NotImplementedException();
        }

        public AstNode ProcessASTNode(InlineDirectiveNode node)
        {
            throw new NotImplementedException();
        }

        public AstNode ProcessASTNode<T>(InstructionNode<T> node)
        {
            throw new NotImplementedException();
        }

        public AstNode ProcessASTNode(InstructionNodeTypeR node)
        {
            uint instruction = node.GetMachineCode();
            context.CurrentSection.AddAlignedCode(instruction);
            return node;
        }

        public AstNode ProcessASTNode(InstructionNodeTypeI node)
        {
            uint instruction = node.GetMachineCode();
            context.CurrentSection.AddAlignedCode(instruction);
            return node;
        }

        public AstNode ProcessASTNode(InstructionNodeTypeU node)
        {
            uint instruction = node.GetMachineCode();
            context.CurrentSection.AddAlignedCode(instruction);
            return node;
        }

        public AstNode ProcessASTNode(InstructionNodeTypeBImmediate node)
        {
            uint instruction = node.GetMachineCode();
            context.CurrentSection.AddAlignedCode(instruction);
            return node;
        }

        public AstNode ProcessASTNode(InstructionNodeTypeBLabel node)
        {
            uint instruction = node.GetMachineCode();
            context.CurrentSection.AddAlignedCode(instruction);
            return node;
        }

        public AstNode ProcessASTNode(InstructionNodeTypeBSymbol node)
        {
            uint instruction = node.GetMachineCode();
            context.CurrentSection.AddAlignedCode(instruction);
            return node;
        }

        public AstNode ProcessASTNode(InstructionNodeTypeJImmediate node)
        {
            uint instruction = node.GetMachineCode();
            context.CurrentSection.AddAlignedCode(instruction);
            return node;
        }

        public AstNode ProcessASTNode(InstructionNodeTypeJLabel node)
        {
            uint instruction = node.GetMachineCode();
            context.CurrentSection.AddAlignedCode(instruction);
            return node;
        }

        public AstNode ProcessASTNode(InstructionNodeTypeJSymbol node)
        {
            uint instruction = node.GetMachineCode();
            context.CurrentSection.AddAlignedCode(instruction);
            return node;
        }

        public AstNode ProcessASTNode(InstructionNodeTypeMisc node)
        {
            uint instruction = node.GetMachineCode();
            context.CurrentSection.AddAlignedCode(instruction);
            return node;
        }

        public AstNode ProcessASTNode(LabelNode node)
        {
            throw new NotImplementedException();
        }

        public AstNode ProcessASTNode(CommentNode node)
        {
            throw new NotImplementedException();
        }

        public AstNode ProcessASTNode(SymbolReferenceNode node)
        {
            throw new NotImplementedException();
        }

        public AstNode ProcessASTNode(AlignmentNode node)
        {
            context.CurrentSection.Alignment = (uint)node.Bytes;
            return node;
        }

        public AstNode ProcessASTNode(RelocationInstructionNode node)
        {
            Relocation relocation = new Relocation(context.CurrentSection.SectionRelativeOffset, node.SymbolName, node.Addend, node.RelocationKind, node.RelatedSymbol);
            context.CurrentSection.Relocations.Add(relocation);

            return node;
        }
    }
}