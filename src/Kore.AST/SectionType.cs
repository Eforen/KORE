using Kore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kore.AST
{
    public enum SectionType {
        /// <summary>
        /// Indicates that the section does not contain any data, and should not be included in the program's memory image.
        /// </summary>
        [StringValue("@nobits")]
        NOBITS,

        /// <summary>
        /// Indicates that the section contains non-executable data, such as read-only data or uninitialized data.
        /// </summary>
        [StringValue("@note")]
        NOTE,

        /// <summary>
        /// Indicates that the section contains executable code or initialized data that should be included in the program's memory image.
        /// </summary>
        [StringValue("@progbits")]
        PROGBITS,

        /// <summary>
        /// Indicates that the section contains data that is stored in read-only memory, such as firmware or bootloader code.
        /// </summary>
        [StringValue("@rom")]
        ROM,

        /// <summary>
        /// Indicates that the section contains initialized data that should be included in the program's memory image.
        /// </summary>
        [StringValue("@data")]
        DATA,

        /// <summary>
        /// Indicates that the section contains uninitialized data that should be initialized to zero at runtime.
        /// </summary>
        [StringValue("@bss")]
        BSS
    }
}
