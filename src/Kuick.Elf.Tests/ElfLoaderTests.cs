using Kuick.Elf.IO;
using NUnit.Framework;

namespace Kuick.Elf.Tests;

public sealed class ElfLoaderTests
{
    [Test]
    public void Load_WithInvalidMagic_Throws()
    {
        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllBytes(path, [0x00, 0x11, 0x22, 0x33]);
            var loader = new ElfLoader();
            Assert.Throws<InvalidDataException>(() => loader.Load(path));
        }
        finally
        {
            File.Delete(path);
        }
    }
}
