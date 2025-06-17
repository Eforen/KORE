using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kore.RiscMeta.ControlStateRegisters {
    // See: https://www.five-embeddev.com/riscv-isa-manual/latest/priv-csrs.html#sec:csrwidthmodulation
    public enum Regions {
        User,
        Supervisor,
        Hypervisor,
        Mechine
    }
    [Flags]
    public enum RegionFlags {
        // In the RISC-V architecture, the privilege levels have a hierarchical nature:
        // Machine (M), Hypervisor (H), Supervisor (S), and User (U). Typically,
        // a higher privilege level (like Machine) can access the CSRs of a lower privilege level
        // (like Supervisor or User), but not vice versa.

        // This is the default flag for all CSRs.
        StandardReadOnly = 0b000000,

        /// <summary>
        /// <code>xURx</code> UserLevel is the least privileged level.<br/>
        /// This level is where applications run and have limited access to system resources.<br/>
        /// <b><i>It has the least CSR access, mainly limited to user-level performance counters and thread state.</i></b>
        /// </summary>
        UserLevel = 0b000001,
        /// <summary>
        /// <code>xSRx</code> SupervisorLevel is an intermediate privilege level.<br/>
        /// This level primarily manages system resources and executes system calls on behalf of user-level applications.<br/>
        /// <b><i>It can access a subset of the CSRs, mainly those required for OS operation. Includes access to User level.</i></b>
        /// </summary>
        SupervisorLevel = 0b000010,
        /// <summary>
        /// <code>xHRx</code> HypervisorLevel is primarily designed for virtualization.<br/>
        /// It sits between the machine and supervisor levels, allowing for creation and management of virtual machines.<br/>
        /// <b><i>It can access CSRs pertinent to virtualization, but not all machine-level CSRs. Includes access to Supervisor and User levels.</i></b>
        /// </summary>
        HypervisorLevel = 0b000100,
        /// <summary> <code>xMRx</code> MechineLevel is the highest privilege level.<br/>This is lowest level code can run at. <br/> <b><i>It is the only level that can access all CSRs. </i></b></summary>
        MechineLevel    = 0b001000,
        /// <summary> <code>xxRW</code> If it is not a read-only CSR then it is a writeable CSR. </summary>
        Writeable       = 0b010000,
        /// <summary> <code>CxRx</code> A Custom CSR is one that is not defined by the RISC-V ISA specification. <br/>If not custom then it is a Standard CSR, they are those that are defined by the RISC-V ISA specification. </summary>
        Custom          = 0b100000,
    }
    public static class ConversionsUtil {
        /// <summary>
        /// The top two bits (csr[11:10]) indicate whether the register is read/write (00, 01, or 10) or read-only (11).
        /// </summary>
        public static readonly uint MASK_READWRITE = 0b11_00_0000_000;
        public static readonly int SHIFT_READWRITE = 9;
        /// <summary>
        /// The second two bits (csr[9:8]) encode the lowest privilege level that can access the CSR.
        /// </summary>
        public static readonly uint MASK_PRIVILAGE = 0b00_11_0000_000;
        public static readonly int SHIFT_PRIVILAGE = 7;

        public static readonly uint MASK_REGION = 0b00_00_1111_000;
        public static readonly int SHIFT_REGION = 3;

        public static readonly uint MASK_REGION1 = 0b00_00_1000_000;
        public static readonly int SHIFT_REGION1 = 6;
        public static readonly uint MASK_REGION2 = 0b00_00_1100_000;
        public static readonly int SHIFT_REGION2 = 5;
        public static readonly uint MASK_REGION3 = 0b00_00_1110_000;
        public static readonly int SHIFT_REGION3 = 4;



        public static RegionFlags getFlags(uint CSR) {
            RegionFlags level = getLevel(CSR);
            switch(level) {
                case RegionFlags.UserLevel:
                    if((CSR & MASK_READWRITE) >> SHIFT_READWRITE == 10) return RegionFlags.UserLevel | RegionFlags.Writeable | RegionFlags.Custom;
                    if((CSR & 0b11_00_1100_000) == 0b11_00_1100_000) return RegionFlags.UserLevel | RegionFlags.Custom;
                    return RegionFlags.UserLevel | isWriteable(CSR);
                case RegionFlags.SupervisorLevel:
                    if((CSR & MASK_READWRITE) != 0 && (CSR * MASK_REGION2 >> SHIFT_REGION2) == 11) 
                        return RegionFlags.SupervisorLevel | 
                               RegionFlags.Custom | 
                               ((CSR & MASK_READWRITE >> SHIFT_READWRITE) != 11 ? RegionFlags.StandardReadOnly : RegionFlags.Writeable);
                    return RegionFlags.SupervisorLevel | isWriteable(CSR);
                case RegionFlags.HypervisorLevel:
                    return RegionFlags.HypervisorLevel | isWriteable(CSR);
                case RegionFlags.MechineLevel:
                    return RegionFlags.MechineLevel | isWriteable(CSR);
                default:
                    throw new Exception("This should not be possible, 8f6033b4-9429-4692-9318-8a2fc7a3585e");
            }
        }
        public static RegionFlags getLevel(uint CSR) {
            switch(CSR & MASK_PRIVILAGE) {
                case 0b00_00_0000000:
                    return RegionFlags.UserLevel;
                case 0b00_01_0000000:
                    return RegionFlags.SupervisorLevel;
                case 0b00_10_0000000:
                    return RegionFlags.HypervisorLevel;
                case 0b00_11_0000000:
                    return RegionFlags.MechineLevel;
                default:
                    throw new Exception("This should not be posible, 74d34818-10be-4514-8398-436d856728bb");
            }
        }
        public static RegionFlags isWriteable(uint CSR) {
            return (CSR & MASK_READWRITE) == 11 ? RegionFlags.StandardReadOnly : RegionFlags.Writeable;
        }
    }
    public enum CSR {

    }
    namespace User {
        // |       CSR Address       | Hex Range | Description |
        // | [11:10] | [9:8] | [7:4] |           |             |
        // | :-----: | :---: | :---: | :-------: | :---------- |
        // | 00 | 00 | XXXX | 0x000-0x0FF | User Standard read/write CSRs |
        // | 01 | 00 | XXXX | 0x400-0x4FF | User Standard read/write CSRs |
        // | 10 | 00 | XXXX | 0x800-0x8FF | User Custom read/write CSRs |
        // | 11 | 00 | 0XXX | 0xC00-0xC7F | User Standard read-only CSRs |
        // | 11 | 00 | 10XX | 0xC80-0xCBF | User Standard read-only CSRs |
        // | 11 | 00 | 11XX | 0xCC0-0xCFF | User Custom read-only CSRs |
    }
    namespace Supervisor {
        // |       CSR Address       | Hex Range | Description |
        // | [11:10] | [9:8] | [7:4] |           |             |
        // | :-----: | :---: | :---: | :-------: | :---------- |
        // | 00 | 01 | XXXX | 0x100-0x1FF | Supervisor Standard read/write CSRs |
        // | 01 | 01 | 0XXX | 0x500-0x57F | Supervisor Standard read/write CSRs |
        // | 01 | 01 | 10XX | 0x580-0x5BF | Supervisor Standard read/write CSRs |
        // | 01 | 01 | 11XX | 0x5C0-0x5FF | Supervisor Custom read/write CSRs |
        // | 10 | 01 | 0XXX | 0x900-0x97F | Supervisor Custom read/write CSRs |
        // | 10 | 01 | 10XX | 0x980-0x9BF | Supervisor Standard read/write CSRs |
        // | 10 | 01 | 11XX | 0x9C0-0x9FF | Supervisor Custom read/write CSRs |
        // | 11 | 01 | 0XXX | 0xD00-0xD7F | Supervisor Standard read-only CSRs |
        // | 11 | 01 | 10XX | 0xD80-0xDBF | Supervisor Standard read-only CSRs |
        // | 11 | 01 | 11XX | 0xDC0-0xDFF | Supervisor Custom read-only CSRs |
    }
    namespace Hypervisor {
        // |       CSR Address       | Hex Range | Description |
        // | [11:10] | [9:8] | [7:4] |           |             |
        // | :-----: | :---: | :---: | :-------: | :---------- |
        // | 00 | 10 | XXXX | 0x200-0x2FF | Hypervisor Standard read/write CSRs |
        // | 01 | 10 | 0XXX | 0x600-0x67F | Hypervisor Standard read/write CSRs |
        // | 01 | 10 | 10XX | 0x680-0x6BF | Hypervisor Standard read/write CSRs |
        // | 01 | 10 | 11XX | 0x6C0-0x6FF | Hypervisor Custom read/write CSRs |
        // | 10 | 10 | 0XXX | 0xA00-0xA7F | Hypervisor Standard read/write CSRs |
        // | 10 | 10 | 10XX | 0xA80-0xABF | Hypervisor Standard read/write CSRs |
        // | 10 | 10 | 11XX | 0xAC0-0xAFF | Hypervisor Custom read/write CSRs |
        // | 11 | 10 | 0XXX | 0xE00-0xE7F | Hypervisor Standard read-only CSRs |
        // | 11 | 10 | 10XX | 0xE80-0xEBF | Hypervisor Standard read-only CSRs |
        // | 11 | 10 | 11XX | 0xEC0-0xEFF | Hypervisor Custom read-only CSRs |

    }
    namespace Mechine {
        // |       CSR Address       | Hex Range | Description |
        // | [11:10] | [9:8] | [7:4] |           |             |
        // | :-----: | :---: | :---: | :-------: | :---------- |
        // | 00 | 11 | XXXX | 0x300-0x3FF | Machine Standard read/write CSRs |
        // | 01 | 11 | 0XXX | 0x700-0x77F | Machine Standard read/write CSRs |
        // | 01 | 11 | 100X | 0x780-0x79F | Machine Standard read/write CSRs |
        // | 01 | 11 | 1010 | 0x7A0-0x7AF | Machine Standard read/write debug CSRs |
        // | 01 | 11 | 1011 | 0x7B0-0x7BF | Machine Debug-mode-only CSRs |
        // | 01 | 11 | 11XX | 0x7C0-0x7FF | Machine Custom read/write CSRs |
        // | 10 | 11 | 0XXX | 0xB00-0xB7F | Machine Standard read/write CSRs |
        // | 10 | 11 | 10XX | 0xB80-0xBBF | Machine Standard read/write CSRs |
        // | 10 | 11 | 11XX | 0xBC0-0xBFF | Machine Custom read/write CSRs |
        // | 11 | 11 | 0XXX | 0xF00-0xF7F | Machine Standard read-only CSRs |
        // | 11 | 11 | 10XX | 0xF80-0xFBF | Machine Standard read-only CSRs |
        // | 11 | 11 | 11XX | 0xFC0-0xFFF | Machine Custom read-only CSRs |
    }
}
