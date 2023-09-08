
**********************
Control and Status Registers
**********************

For those familiar with x84 or x64 assembly architecture you may find some differences in the KORE (RISC-V) Cpu architecture or assembly.
In this page we will examine the following differences.
* There are no operations dedicated to getting status or setting control vars this is all handeled by a common set of operations and a set of addresses mapped to verious bits of control or status such as the cpu cycle counter (0xB00 mcycle)

Permissions
==============================
The CSR addresses are mapped to 4 permission levels.
The CSR addresses are further mapped to 2 classifications Standard and Custom.
Standard CSRs are defined by the RISC-V ISA and have special defined uses.
Custom CSRs are not defined by the RISC-V ISA and are open to use by the the CPU/OS/Software/User

Notes:
* Any attempt to access a CSR that does not exist will raise an Illegal Instruction exception.
* Any attempt to access a CSR that the software context does not have permission to access will raise an Illegal Instruction exception.
* Any attempt to write to a CSR that that is read-only will raise an Illegal Instruction exception.
* Some writeable CSRs will have bits that are read-only writes to these bits will be ignored and will not raise an Illegal Instruction exception.
* The CSRs from 0x7A0 to 0x7AF are debug csrs that are shared between Mechine Mode and Debug Mode.
* The CSRs from 0x7B0 to 0x7BF only exist in debug mode (Mechine mode software attempting to access these will raise an Illegal Instruction exception).

Permission Address Map
------------------------------

The 4096 possible CSR addresses are divided as follows:

+-------------------------+-------------+----------+---------------+
| CSR Address             | Hex         | Use      | Accessibility |
+---------+-------+-------+             +          +               +
| [11:10] | [9:8] | [7:4] |             |          |               |
+=========+=======+=======+=============+==========================+
|                           User CSRs                              |
+=========+=======+=======+=============+==========================+
|    00   |  00   |  XXXX | 0x000-0x0FF | Standard |  read/write   |
+---------+-------+-------+-------------+----------+---------------+
|    01   |  00   |  XXXX | 0x400-0x4FF | Standard |  read/write   |
+---------+-------+-------+-------------+----------+---------------+
|    10   |  00   |  XXXX | 0x800-0x8FF | Custom   |  read/write   |
+---------+-------+-------+-------------+----------+---------------+
|    11   |  00   |  0XXX | 0xC00-0xC7F | Standard |  read-only    |
+---------+-------+-------+-------------+----------+---------------+
|    11   |  00   |  10XX | 0xC80-0xCBF | Standard |  read-only    |
+---------+-------+-------+-------------+----------+---------------+
|    11   |  00   |  11XX | 0xCC0-0xCFF | Custom   |  read-only    |
+=========+=======+=======+=============+==========================+
|                         Supervisor CSRs                          |
+=========+=======+=======+=============+==========================+
|    00   |  01   |  XXXX | 0x100-0x1FF | Standard |  read/write   |
+---------+-------+-------+-------------+----------+---------------+
|    01   |  01   |  0XXX | 0x500-0x57F | Standard |  read/write   |
+---------+-------+-------+-------------+----------+---------------+
|    01   |  01   |  10XX | 0x580-0x5BF | Standard |  read/write   |
+---------+-------+-------+-------------+----------+---------------+
|    01   |  01   |  11XX | 0x5C0-0x5FF | Custom   |  read/write   |
+---------+-------+-------+-------------+----------+---------------+
|    10   |  01   |  0XXX | 0x900-0x97F | Standard |  read/write   |
+---------+-------+-------+-------------+----------+---------------+
|    10   |  01   |  10XX | 0x980-0x9bF | Standard |  read/write   |
+---------+-------+-------+-------------+----------+---------------+
|    10   |  01   |  11XX | 0x9C0-0x9FF | Custom   |  read/write   |
+---------+-------+-------+-------------+----------+---------------+
|    11   |  01   |  0XXX | 0xD00-0xD7F | Standard |  read-only    |
+---------+-------+-------+-------------+----------+---------------+
|    11   |  01   |  10XX | 0xD80-0xDBF | Standard |  read-only    |
+---------+-------+-------+-------------+----------+---------------+
|    11   |  01   |  11XX | 0xDC0-0xDFF | Custom   |  read-only    |
+=========+=======+=======+=============+==========================+
|                        Hypervisor CSRs                           |
+=========+=======+=======+=============+==========================+
|    00   |  10   |  XXXX | 0x200-0x2FF | Standard |  read/write   |
+---------+-------+-------+-------------+----------+---------------+
|    01   |  10   |  0XXX | 0x600-0x67F | Standard |  read/write   |
+---------+-------+-------+-------------+----------+---------------+
|    01   |  10   |  10XX | 0x680-0x6BF | Standard |  read/write   |
+---------+-------+-------+-------------+----------+---------------+
|    01   |  10   |  11XX | 0x6C0-0x6FF | Custom   |  read/write   |
+---------+-------+-------+-------------+----------+---------------+
|    10   |  10   |  0XXX | 0xA00-0xA7F | Standard |  read/write   |
+---------+-------+-------+-------------+----------+---------------+
|    10   |  10   |  10XX | 0xA80-0xABF | Standard |  read/write   |
+---------+-------+-------+-------------+----------+---------------+
|    10   |  10   |  11XX | 0xAC0-0xAFF | Custom   |  read/write   |
+---------+-------+-------+-------------+----------+---------------+
|    11   |  10   |  0XXX | 0xE00-0xE7F | Standard |  read-only    |
+---------+-------+-------+-------------+----------+---------------+
|    11   |  10   |  10XX | 0xE80-0xEBF | Standard |  read-only    |
+---------+-------+-------+-------------+----------+---------------+
|    11   |  10   |  11XX | 0xEC0-0xEFF | Custom   |  read-only    |
+=========+=======+=======+=============+==========================+
|                           Machine CSRs                           |
+=========+=======+=======+=============+==========================+
|    00   |  11   |  XXXX | 0x300-0x3FF | Standard |  read/write   |
+---------+-------+-------+-------------+----------+---------------+
|    01   |  11   |  0XXX | 0x700-0x77F | Standard |  read/write   |
+---------+-------+-------+-------------+----------+---------------+
|    01   |  11   |  100X | 0x780-0x79F | Standard |  read/write   |
+---------+-------+-------+-------------+----------+---------------+
|    01   |  11   |  1010 | 0x7A0-0x7AF | Standard debug CSRs | read/write |
+---------+-------+-------+-------------+----------+---------------+
|    01   |  11   |  1011 | 0x7B0-0x7BF | Debug-mode-only | read/write |
+---------+-------+-------+-------------+----------+---------------+
|    01   |  11   |  11XX | 0x7C0-0x7FF | Custom   |  read/write   |
+---------+-------+-------+-------------+----------+---------------+
|    10   |  11   |  0XXX | 0xB00-0xB7F | Standard |  read/write   |
+---------+-------+-------+-------------+----------+---------------+
|    10   |  11   |  10XX | 0xB80-0xBBF | Standard |  read/write   |
+---------+-------+-------+-------------+----------+---------------+
|    10   |  11   |  11XX | 0xBC0-0xBFF | Custom   |  read/write   |
+---------+-------+-------+-------------+----------+---------------+
|    11   |  11   |  0XXX | 0xF00-0xF7F | Standard |  read-only    |
+---------+-------+-------+-------------+----------+---------------+
|    11   |  11   |  10XX | 0xF80-0xFBF | Standard |  read-only    |
+---------+-------+-------+-------------+----------+---------------+
|    11   |  11   |  11XX | 0xFC0-0xFFF | Custom   |  read-only    |
+---------+-------+-------+-------------+----------+---------------+

Permission Levels
------------------------------

User Mode
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

User CSRs are used by any software in User Permission Mode (The least permissive level)

These are the CSRs that standard non-admin/non-root software running in the OS would have access to.

In User Mode the following Standard CSRs are currently allocated for use:

+--------+-----------+---------------+--------------------------------------------------------+
| Number | Privilege | Name          | Description                                            |
+========+===========+===============+========================================================+
| User Trap Setup                                                                             |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x000  | URW       | ustatus       | User status register.                                  |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x004  | URW       | uie           | User interrupt-enable register.                        |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x005  | URW       | utvec         | User trap handler base address.                        |
+--------+-----------+---------------+--------------------------------------------------------+
| User Trap Handling                                                                          |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x040  | URW       | uscratch      | Scratch register for user trap handlers.               |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x041  | URW       | uepc           | User exception program counter.                       |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x042  | URW       | ucause         | User trap cause.                                      |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x043  | URW       | utval          | User bad address or instruction.                      |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x044  | URW       | uip            | User interrupt pending.                               |
+--------+-----------+---------------+--------------------------------------------------------+
| User Floating-Point CSRs                                                                    |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x001  | URW       | fflags        | Floating-Point Accrued Exceptions.                     |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x002  | URW       | frm           | Floating-Point Dynamic Rounding Mode.                  |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x003  | URW       | fcsr          | Floating-Point Control and Status Register.            |
+--------+-----------+---------------+--------------------------------------------------------+
| User Counter/Timer                                                                          |
+--------+-----------+---------------+--------------------------------------------------------+
| 0xC00  | URO       | cycle         | Cycle counter for RDCYCLE instruction.                 |
+--------+-----------+---------------+--------------------------------------------------------+
| 0xC01  | URO       | time          | Timer for RDTIME instruction.                          |
+--------+-----------+---------------+--------------------------------------------------------+
| 0xC02  | URO       | instret       | Instructions-retired counter for RDINSTRET instruction |
+--------+-----------+---------------+--------------------------------------------------------+
| 0xC03  | URO       | hpmcounter3   | Performance-monitoring counter.                        |
+--------+-----------+---------------+--------------------------------------------------------+
| 0xC04  | URO       | hpmcounter4   | Performance-monitoring counter.                        |
+--------+-----------+---------------+--------------------------------------------------------+
|        |           | hpmcounter#   | ...                                                    |
+--------+-----------+---------------+--------------------------------------------------------+
| 0xC1F  | URO       | hpmcounter31  | Performance-monitoring counter.                        |
+--------+-----------+---------------+--------------------------------------------------------+
| 0xC80  | URO       | cycleh        | Upper 32 bits of cycle, RV32 only                      |
+--------+-----------+---------------+--------------------------------------------------------+
| 0xC81  | URO       | timeh         | Upper 32 bits of time, RV32 only                       |
+--------+-----------+---------------+--------------------------------------------------------+
| 0xC82  | URO       | instreth      | Upper 32 bits of instret, RV32 only                    |
+--------+-----------+---------------+--------------------------------------------------------+
| 0xC83  | URO       | hpmcounter3h  | Upper 32 bits of hpmcounter3, RV32 only                |
+--------+-----------+---------------+--------------------------------------------------------+
| 0xC84  | URO       | hpmcounter4h  | Upper 32 bits of hpmcounter4, RV32 only                |
+--------+-----------+---------------+--------------------------------------------------------+
|        |           | hpmcounter#h  | ...                                                    |
+--------+-----------+---------------+--------------------------------------------------------+
| 0xC9F  | URO       | hpmcounter31h | Upper 32 bits of hpmcounter31, RV32 only               |
+--------+-----------+---------------+--------------------------------------------------------+



Supervisor Mode
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Supervisor CSRs are used by any software in Supervisor Permission Mode (The 2nd least permissive level)

These are the CSRs that standard admin/root software running in the OS would have access to.

In Supervisor Mode the following Standard CSRs are currently allocated for use:

Notes:
* All User Mode CSRs are also accessible in this mode

+--------+-----------+---------------+--------------------------------------------------------+
| Number | Privilege | Name          | Description                                            |
+========+===========+===============+========================================================+
| Supervisor Trap Setup                                                                       |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x100  | SRW       | sstatus       | Supervisor status register.                            |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x102  | SRW       | sedeleg       | Supervisor exception delegation register.              |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x103  | SRW       | sideleg       | Supervisor interrupt delegation register.              |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x104  | SRW       | sie           | Supervisor interrupt-enable register.                  |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x105  | SRW       | stvec         | Supervisor trap handler base address.                  |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x106  | SRW       | scounteren    | Supervisor counter enable.                             |
+--------+-----------+---------------+--------------------------------------------------------+
| Supervisor Trap Handling                                                                    |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x140  | SRW       | sscratch      | Scratch register for supervisor trap handlers.         |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x141  | SRW       | sepc          | Supervisor exception program counter.                  |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x142  | SRW       | scause        | Supervisor trap cause.                                 |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x143  | SRW       | stval         | Supervisor bad address or instruction.                 |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x144  | SRW       | sip           | Supervisor interrupt pending.                          |
+--------+-----------+---------------+--------------------------------------------------------+
| Supervisor Protection and Translation                                                       |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x180  | SRW       | satp          | Supervisor address translation and protection.         |
+--------+-----------+---------------+--------------------------------------------------------+
| Debug/Trace Registers                                                                       |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x5A8  | SRW       | scontext      | Supervisor-mode context register.                      |
+--------+-----------+---------------+--------------------------------------------------------+


Hypervisor Mode
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Hypervisor CSRs are used by any software in Hypervisor Permission Mode (The 3nd least permissive level)

These are the CSRs that software running as the OS would have access to.

In Hypervisor Mode the following Standard CSRs are currently allocated for use:

Notes:
* All User Mode CSRs are also accessible in this mode
* All Supervisor Mode CSRs are also accessible in this mode

+--------+-----------+---------------+--------------------------------------------------------+
| Number | Privilege | Name          | Description                                            |
+========+===========+===============+========================================================+
| Hypervisor Trap Setup                                                                       |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x600  | HRW       | hstatus       | Hypervisor status register.                            |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x602  | HRW       | hedeleg       | Hypervisor exception delegation register.              |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x603  | HRW       | hideleg       | Hypervisor interrupt delegation register.              |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x604  | HRW       | hie           | Hypervisor interrupt-enable register.                  |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x606  | HRW       | hcounteren    | Hypervisor counter enable.                             |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x607  | HRW       | htegie        | Hypervisor guest external interrupt-enable register    |
+--------+-----------+---------------+--------------------------------------------------------+
| Hypervisor Trap Handling                                                                    |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x643  | HRW       | htval         | Hypervisor bad address or instruction.                 |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x644  | HRW       | hip           | Hypervisor interrupt pending.                          |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x645  | HRW       | hvip          | Hypervisor virtual interrupt pending.                  |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x64A  | HRW       | htinst        | Hypervisor trap instruction (transformed)              |
+--------+-----------+---------------+--------------------------------------------------------+
| 0xE12  | HRO       | hgeip         | Hypervisor guest external interrupt pending.           |
+--------+-----------+---------------+--------------------------------------------------------+
| Hypervisor Protection and Translation                                                       |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x680  | HRW       | hgatp         | Hypervisor guest address translation and protection.   |
+--------+-----------+---------------+--------------------------------------------------------+
| Debug/Trace Registers                                                                       |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x6A8  | HRW       | hcontext      | Hypervisor-mode context register.                      |
+--------+-----------+---------------+--------------------------------------------------------+
| Hypervisor Counter/Timer Virtualization Registers                                           |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x605  | HRW       | htimedelta    | Delta for VS/VU-mode timer.                            |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x615  | HRW       | htimedeltah   | Upper 32 bits of htimedelta, RV32 only.                |
+--------+-----------+---------------+--------------------------------------------------------+
| Virtual Supervisor Registers                                                                |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x200  | HRW       | vsstatus      | Virtual supervisor status register                     |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x204  | HRW       | vsie          | Virtual supervisor interrupt-enable register           |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x205  | HRW       | vstvec        | Virtual supervisor trap handler base address.          |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x240  | HRW       | vsscratch     | Virtual supervisor scratch register.                   |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x241  | HRW       | vsepc         | Virtual supervisor exception program counter.          |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x242  | HRW       | vscause       | Virtual supervisor trap cause.                         |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x243  | HRW       | vstval        | Virtual supervisor bad address or instruction.         |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x244  | HRW       | vsip          | Virtual supervisor interrupt pending.                  |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x280  | HRW       | bsatp         | Supervisor address translation and protection.         |
+--------+-----------+---------------+--------------------------------------------------------+


Machine Mode
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Machine CSRs are used by any software in Machine Permission Mode (The most permissive level excluding debug)

These are the CSRs that software below the OS would have access to such as bios or a system on a chip implementation.
Software at this level can do almost anything and everything, and has access to almost any interrupts or memory the system has.

In Machine Mode the following Standard CSRs are currently allocated for use:

Notes:
* All User Mode CSRs are also accessible in this mode
* All Supervisor Mode CSRs are also accessible in this mode
* All Hypervisor Mode CSRs are also accessible in this mode

+--------+-----------+---------------+--------------------------------------------------------+
| Number | Privilege | Name          | Description                                            |
+========+===========+===============+========================================================+
| Machine Information Registers                                                               |
+--------+-----------+---------------+--------------------------------------------------------+
| 0xF11  | MRO       | mvendorid     | Vendor ID.                                             |
+--------+-----------+---------------+--------------------------------------------------------+
| 0xF12  | MRO       | marchid       | Architecture ID.                                       |
+--------+-----------+---------------+--------------------------------------------------------+
| 0xF13  | MRO       | mimpid        | Implementation ID.                                     |
+--------+-----------+---------------+--------------------------------------------------------+
| 0xF14  | MRO       | mhartid       | Hardware thread ID.                                    |
+--------+-----------+---------------+--------------------------------------------------------+
| Machine Trap Setup                                                                          |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x300  | MRW       | mstatus       | Machine status register.                               |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x301  | MRW       | misa          | ISA and extentions                                     |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x302  | MRW       | medeleg       | Machine exception delegation register.                 |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x303  | MRW       | mideleg       | Machine interrupt delegation register.                 |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x304  | MRW       | mie           | Machine interrupt-enable register.                     |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x305  | MRW       | mtvec         | Machine trap-handler base address.                     |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x306  | MRW       | mcounteren    | Machine counter enable.                                |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x310  | MRW       | mstatush      | Additional machine status register, RV32 only.         |
+--------+-----------+---------------+--------------------------------------------------------+
| Machine Trap Handling                                                                       |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x340  | MRW       | mscratch      | Scratch register for machine trap handlers.            |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x341  | MRW       | mepc          | Machine exception program counter.                     |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x342  | MRW       | hcause        | Machine trap cause.                                    |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x343  | MRW       | htval         | Machine bad address or instruction.                    |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x344  | MRW       | hip           | Machine interrupt pending.                             |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x34A  | MRW       | htinst        | Machine trap instruction (transformed)                 |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x34B  | MRW       | htval2        | Machine guest bad address or instruction.              |
+--------+-----------+---------------+--------------------------------------------------------+
| Machine Memory Protection                                                                   |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x3A0  | MRW       | pmpcfg0       | Physical memory protection configuration.              |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x3A1  | MRW       | pmpcfg1       | Physical memory protection configuration, RV32 only.   |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x3A2  | MRW       | pmpcfg2       | Physical memory protection configuration.              |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x3A3  | MRW       | pmpcfg3       | Physical memory protection configuration, RV32 only.   |
+--------+-----------+---------------+--------------------------------------------------------+
|        |           | ...           |                                                        |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x3AE  | MRW       | pmpcfg14      | Physical memory protection configuration.              |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x3AF  | MRW       | pmpcfg15      | Physical memory protection configuration, RV32 only.   |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x3B0  | MRW       | pmpaddr0      | Physical memory protection address configuration.      |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x3B1  | MRW       | pmpaddr1      | Physical memory protection address configuration.      |
+--------+-----------+---------------+--------------------------------------------------------+
|        |           | ...           |                                                        |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x3EF  | MRW       | pmpaddr63     | Physical memory protection address configuration.      |
+--------+-----------+---------------+--------------------------------------------------------+
| Machine Counter/Timers                                                                       |
+--------+-----------+---------------+--------------------------------------------------------+
| 0xB00  | MRO       | cycle         | Cycle counter for RDCYCLE instruction.                 |
+--------+-----------+---------------+--------------------------------------------------------+
| 0xB02  | MRO       | instret       | Instructions-retired counter for RDINSTRET instruction |
+--------+-----------+---------------+--------------------------------------------------------+
| 0xB03  | MRO       | hpmcounter3   | Performance-monitoring counter.                        |
+--------+-----------+---------------+--------------------------------------------------------+
| 0xB04  | MRO       | hpmcounter4   | Performance-monitoring counter.                        |
+--------+-----------+---------------+--------------------------------------------------------+
|        |           | hpmcounter#   | ...                                                    |
+--------+-----------+---------------+--------------------------------------------------------+
| 0xB1F  | MRO       | hpmcounter31  | Performance-monitoring counter.                        |
+--------+-----------+---------------+--------------------------------------------------------+
| 0xB80  | MRO       | cycleh        | Upper 32 bits of cycle, RV32 only                      |
+--------+-----------+---------------+--------------------------------------------------------+
| 0xB82  | MRO       | instreth      | Upper 32 bits of instret, RV32 only                    |
+--------+-----------+---------------+--------------------------------------------------------+
| 0xB83  | MRO       | hpmcounter3h  | Upper 32 bits of hpmcounter3, RV32 only                |
+--------+-----------+---------------+--------------------------------------------------------+
| 0xB84  | MRO       | hpmcounter4h  | Upper 32 bits of hpmcounter4, RV32 only                |
+--------+-----------+---------------+--------------------------------------------------------+
|        |           | hpmcounter#h  | ...                                                    |
+--------+-----------+---------------+--------------------------------------------------------+
| 0xB9F  | MRO       | hpmcounter31h | Upper 32 bits of hpmcounter31, RV32 only               |
+--------+-----------+---------------+--------------------------------------------------------+
| Machine Counter Setup                                                                       |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x320  | MRW       | mcountinhibit | Machine counter-inhibit register.                      |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x323  | MRW       | mhpmevent3    | Machine performance-monitoring event selector.         |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x324  | MRW       | mhpmevent4    | Machine performance-monitoring event selector.         |
+--------+-----------+---------------+--------------------------------------------------------+
|        |           | mhpmevent#    | ...                                                    |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x33F  | MRW       | mhpmevent31   | Machine performance-monitoring event selector.         |
+--------+-----------+---------------+--------------------------------------------------------+
| Debug/Trace Registers                                                                       |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x7A0  | MRW       | tselect       | Debug/Trace trigger register select.                   |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x7A1  | MRW       | tdata1        | First Debug/Trace trigger data register.               |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x7A2  | MRW       | tdata2        | Sedcond Debug/Trace trigger data register.             |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x7A3  | MRW       | tdata3        | Third Debug/Trace trigger data register.               |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x7A8  | MRW       | mcontext      | Machine-mode context register.                         |
+--------+-----------+---------------+--------------------------------------------------------+


Debug Mode
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Debug CSRs are used by any software in Debug Permission Mode (The most permissive level)

These are the CSRs that software below everything.
Software at this level can do anything and everything, and has access to any interrupts or memory the system has.
This mode is generally used only for testing the CPU itself (Current assumption (I might be wrong about this as I am not 100% on it.)).

In Debug Mode the following Standard CSRs are currently allocated for use:

Notes:
* All User Mode CSRs are also accessible in this mode
* All Supervisor Mode CSRs are also accessible in this mode
* All Hypervisor Mode CSRs are also accessible in this mode
* All Machine Mode CSRs are also accessible in this mode

+--------+-----------+---------------+--------------------------------------------------------+
| Number | Privilege | Name          | Description                                            |
+========+===========+===============+========================================================+
| Debug Mode Registers                                                                        |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x7B0  | DRW       | dcsr          | Debug control and status register.                     |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x7B1  | DRW       | dpc           | Debug PC.                                              |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x7B2  | DRW       | dscratch0     | Debug scratch register 0.                              |
+--------+-----------+---------------+--------------------------------------------------------+
| 0x7B3  | DRW       | dscratch1     | Debug scratch register 1.                              |
+--------+-----------+---------------+--------------------------------------------------------+

Registers and Stack
==============================

There are 16 Registers if you want to see them check the `Architecture Specification <../architecture/structure.html#map-of-registers>`_, 16 64-bit "general-purpose" registers;
the low-order 32, 16, and 8 bits of each register can be accessed independently under other names, as shown in the `Map of Registers <../architecture/structure.html#map-of-registers>`_

In principle, almost any register can be used to hold any data, but some have special or restricted uses.
Using these extra registers can have unexpected behavior.



Addressing Mode
==============================

Register :math:`R_2 \leftarrow R_2 + R_1`

.. code-block:: kasm

    ADD R1, R2

Direct :math:`Mem[200] \leftarrow Mem[200] + R_1`

.. code-block:: kasm

    ADD R1, [200]

Indirect :math:`Mem[R_1] \leftarrow Mem[R_1] + A`

.. code-block:: kasm

    ADD A, [R1]

Immediate :math:`R_1 \leftarrow R_1 + 69`

.. code-block:: kasm

    ADI 69, R1

Instructions
==============================

Data Transfer Instructions
------------------------------

.. list-table:: Key
    :widths: 10 50
    :header-rows: 1

    * - Placeholder
      - Denotes
    * - *s*
      - immediate, register, or memory Address
    * - *d*
      - a register or memory address
    * - *m*
      - a memory address
    * - *r*
      - a register
    * - *imm*
      - immediate
    * - *l*
      - label, program line number, or memory address

Most transfers use the mov instruction which works between to registers or between registers and a memory address.

.. note::
    Data Transfers from one memory address to another memory address is not supported.
    If you need to do this a register or the stack would need to be used instead.

.. list-table:: Data transfer instructions
    :widths: 50 50
    :header-rows: 0

    * - mov [b|w|l|q] s,d
      - move s to d
    * - movs [bw|bl|bq|wl|wq|lq] s,d
      - move with sign extension
    * - movz [bw|bl|bq|wl|wq] s,d
      - move with zero extension
    * - movabsq imm, r
      - move absolute quad word (64-bits)
    * - pushq s
      - push onto the stack
    * - popq s
      - pop from the stack stack

.. note::
    It may be desirable at a later point in time to make some instructions for direct memory to memory mov ops to increase preformance of memcopy commands if users are using that frequently.

.. warning::
    Remember that the stack must stay 8 byte aligned at all times thus remember to pad your data if you need to push less then 8 byte.

Integer Arithmetic and Logical Operations
-----------------------------------------

.. list-table:: Arithmetic instructions
    :widths: 50 50
    :header-rows: 0

    * - lea [b|wl|q] m, r
      - load effective address of m into r
    * - inc[b|w|l|q] d
      - d = d + 1
    * - dec[b|w|l|q] d
      - d = d - 1
    * - neg[b|w|l|q] d
      - d = -d
    * - not[b|w|l|q] d
      - d = ~d(bitwise complement)
    * - add[b|w|l|q] s, d
      - d = d + s
    * - sub[b|w|l|q] s, d
      - d = d - s
    * - imul[b|w|l|q] s, r
      - r = r * s (throws away high-order half of the result)
    * - xor[b|w|l|q] s, d
      - d = d ∧ s (bitwise)
    * - or[b|w|l|q] s, d
      - d = d | s (bitwise)
    * - and[b|w|l|q] s,d
      - d = d & s (bitwise)
    * - idivl s
      - signed division of edx by s, place quotient in eax, and remainder in edx
    * - divl s
      - unsigned division of edx by s, place quotient in eax, and remainder in edx
    * - cltd
      - sign extend eax into edx
    * - idivq s
      - signed devision of rdx by s, place quotient in rdx, and remainder in rdx
    * - divq s
      - unsigned devision of rdx by s, place quotient in rdx, and remainder in rdx
    * - clto
      - sign extend rax into rdx
    * - sal[b|w|l|q] imm, d
      - d = d << imm (left shift)
    * - sar[b|w|l|q] imm, d
      - d = d >> imm (arithmetic right shift)
    * - shr[b|w|l|q] imm, d
      - d = d >> imm (logical right shift)

.. note::
    A very common trick is to zero a register by using xor on itself.

.. note::
    When data is loaded into a register it zeros out the high order bits of the register.
    If a signed operation is performed on a low order register its high order bits will be zeroed except for the sign bit.

.. warning::
    Multiplication of two n-byte values has the potential to result in 2n-byte.
    The imul instruction simply discards the high-order half of the result, so it still fits in n bytes.
    This is common in many programming languages.

Condition Codes
-----------------------------------------

Almost all arithmetic instructions set processor condition codes based on their result.

.. list-table:: The Condition Codes
    :widths: 50 50
    :header-rows: 0

    * - ZF
      - result was Zero
    * - CF
      - result caused carry out of most significant bit
    * - SF
      - result was negative (Sign bit was set)
    * - OF
      - result caused overflow

In general, compilers usually set the condition codes using one of the following instructions,
which do not change any register:

.. list-table:: Conditional Instructions
    :widths: 50 50
    :header-rows: 0

    * - cmp[b|w|l|q] s2, s1
      - set flags based on s1 - s2
    * - test[b|w|l|q] s2, s1
      - set flags based on s1 & s2 (logical and)

In the following instructions cc will stand for any of the condition codes that come after the instructions.

.. list-table:: Condition Sensitive Instructions
    :widths: 50 50
    :header-rows: 0

    * - j\ *cc* *l*
      - transfers control to *l* if the specified *cc* evaluates to true
    * - set\ *cc* *d*
      - sets the single byte destination *d* to 1 or 0 according to whether the specified *cc* evaluates to true
    * - cmov\ *cc* s, d
      - instructions perform mov only if the specified *cc* holds.
    * - cmovs\ *cc* s, d
      - instructions perform movs only if the specified *cc* holds.
    * - cmovz\ *cc* s, d
      - instructions perform movz only if the specified *cc* holds.
    * - cmovabsq\ *cc* s, d
      - instructions perform movabsq only if the specified *cc* holds.

.. list-table:: Condition Codes
    :widths: 25 25 50
    :header-rows: 1

    * - cc
      - condition tested
      - meaning after cmp
    * - e
      - **ZF**
      - equal to zero
    * - ne
      - **˜ZF**
      - not equal to zero
    * - n
      - **SF**
      - negitive
    * - nn
      - **˜SF**
      - not negative
    * - g
      - **˜(SF xor OF) & ˜ZF**
      - greater (>) (signed sensitive)
    * - ge
      - **˜(SF xor OF)**
      - greater or equal (>=) (signed sensitive)
    * - l
      - **SF xor OF**
      - less (<) (signed sensitive)
    * - le
      - **(SF xor OF) | ZF**
      - less or equal (<=) (signed sensitive)
    * - a
      - **˜CF & ˜ZF**
      - above (abs >) (not signed sensitive)
    * - ae
      - **˜CF**
      - above or equal (abs >=) (not signed sensitive)
    * - b
      - **CF**
      - below (abs <) (not signed sensitive)
    * - be
      - **CF | ZF**
      - below or equal (abs <) (not signed sensitive)

Flow Control Transfers
-----------------------------------------

Floating Point Arithmetic
-----------------------------------------