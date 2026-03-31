using Kuick.Elf.Models;

namespace Kuick.Elf.Formatting;

public interface IElfFormatter
{
    string Format(ElfObject elfObject, FormatterOptions? options = null);
}
