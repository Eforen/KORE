using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using NUnit.Framework;

namespace Kuick.Tools.Readelf.Tests;

/// <summary>
/// Assembles <c>TestData/readelf_fixture.s</c> with <c>riscv32-unknown-elf-as</c>, links a shared object with
/// <c>ld.lld -shared -m elf32lriscv</c> (LLVM; the embedded <c>riscv32-unknown-elf-ld</c> does not support <c>-shared</c>),
/// runs <c>riscv32-kuick-elf-readelf</c>, and asserts stdout matches <c>Golden/*.txt</c> byte-for-byte.
/// The fixture uses an <b>undefined</b> <c>call</c> so relocations and PLT/GOT are populated; <c>-d</c>, <c>-I</c>, and
/// <c>--got-contents</c> exercise real tables. Minimal links still omit GNU symbol versioning; <c>-V</c> goldens document the “none” case.
/// Golden file names avoid case-only pairs (for example <c>view_flag_a_all.txt</c> vs <c>view_flag_A_arch.txt</c>) so
/// <c>CopyToOutputDirectory</c> does not collapse two files into one on case-sensitive filesystems.
/// Regenerate goldens after changing the fixture or readelf output: <c>scripts/regenerate-readelf-golden.sh</c>.
/// </summary>
[TestFixture]
public sealed class ReadelfCliIntegrationTests
{
    private const string FixtureAsmFileName = "readelf_fixture.s";

    private static readonly string ReadelfExe = ResolveReadelfExe();
    private static string? _objectFile;
    private static string? _riscvAs;
    private static string? _lld;

    [OneTimeSetUp]
    public void AssembleFixtureObject()
    {
        _riscvAs = FindOnPath("riscv32-unknown-elf-as");
        if (_riscvAs is null)
        {
            Assert.Ignore(
                "riscv32-unknown-elf-as not found on PATH; install a RISC-V embedded toolchain to run readelf CLI tests.");
        }

        _lld = FindLld();
        if (_lld is null)
        {
            Assert.Ignore(
                "ld.lld not found on PATH; install LLVM (lld) to link the readelf test fixture as an ELF shared object.");
        }

        if (!File.Exists(ReadelfExe))
        {
            Assert.Ignore($"readelf executable not found at '{ReadelfExe}'. Build kuick-readelf or set KORE_READELF_EXE.");
        }

        var asmPath = Path.Combine(AppContext.BaseDirectory, "TestData", FixtureAsmFileName);
        if (!File.Exists(asmPath))
        {
            Assert.Fail($"Missing test assembly file: {asmPath}");
        }

        var tempDir = Path.Combine(Path.GetTempPath(), $"kore-readelf-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);
        var objPath = Path.Combine(tempDir, "fixture.o");
        _objectFile = Path.Combine(tempDir, "fixture.so");
        RunChecked(_riscvAs, $"-march=rv32i -mabi=ilp32 -o \"{objPath}\" \"{asmPath}\"");
        RunChecked(_lld, $"-shared -m elf32lriscv -o \"{_objectFile}\" \"{objPath}\"");
    }

    [OneTimeTearDown]
    public void DeleteFixtureDirectory()
    {
        if (_objectFile is null)
        {
            return;
        }

        try
        {
            var dir = Path.GetDirectoryName(_objectFile);
            if (dir is not null && Directory.Exists(dir))
            {
                Directory.Delete(dir, recursive: true);
            }
        }
        catch
        {
            // best effort
        }
    }

    [TestCase("-h", "view_h.txt")]
    [TestCase("--file-header", "view_h.txt")]
    [TestCase("--header", "view_h.txt")]
    [TestCase("-l", "view_l.txt")]
    [TestCase("--program-headers", "view_l.txt")]
    [TestCase("-S", "view_flag_S_sections.txt")]
    [TestCase("--sections", "view_flag_S_sections.txt")]
    [TestCase("-s", "view_flag_s_syms.txt")]
    [TestCase("--syms", "view_flag_s_syms.txt")]
    [TestCase("-r", "view_r.txt")]
    [TestCase("--relocs", "view_r.txt")]
    [TestCase("-d", "view_d.txt")]
    [TestCase("--dynamic", "view_d.txt")]
    [TestCase("-V", "view_V.txt")]
    [TestCase("--version-info", "view_V.txt")]
    [TestCase("-A", "view_flag_A_arch.txt")]
    [TestCase("--arch-specific", "view_flag_A_arch.txt")]
    [TestCase("-I", "view_I.txt")]
    [TestCase("--histogram", "view_I.txt")]
    [TestCase("--got-contents", "view_gotcontents.txt")]
    public void SingleView_MatchesGolden(string flag, string goldenFile)
    {
        Assert.That(_objectFile, Is.Not.Null);
        AssertStdoutMatchesGolden($"{flag} \"{_objectFile}\"", goldenFile);
    }

    [TestCase("-S", "--include-empty", "view_flag_S_includeempty.txt")]
    public void IncludeEmpty_WithSections_MatchesGolden(string sectionFlag, string includeEmpty, string goldenFile)
    {
        Assert.That(_objectFile, Is.Not.Null);
        AssertStdoutMatchesGolden($"{sectionFlag} {includeEmpty} \"{_objectFile}\"", goldenFile);
    }

    [TestCase("-h", "--verbose", "view_h_verbose.txt")]
    public void Verbose_WithFileHeader_MatchesGolden(string headerFlag, string verbose, string goldenFile)
    {
        Assert.That(_objectFile, Is.Not.Null);
        AssertStdoutMatchesGolden($"{headerFlag} {verbose} \"{_objectFile}\"", goldenFile);
    }

    [Test]
    public void BundledShortOptions_MatchesGolden()
    {
        Assert.That(_objectFile, Is.Not.Null);
        AssertStdoutMatchesGolden($"-hS \"{_objectFile}\"", "view_hS.txt");
    }

    [Test]
    public void AllFlag_MatchesGolden()
    {
        Assert.That(_objectFile, Is.Not.Null);
        AssertStdoutMatchesGolden($"-a \"{_objectFile}\"", "view_flag_a_all.txt");
    }

    [Test]
    public void AllLongAlias_MatchesShortA()
    {
        Assert.That(_objectFile, Is.Not.Null);
        var expected = LoadGolden("view_flag_a_all.txt");
        var (_, oa, ea) = RunReadelf($"-a \"{_objectFile}\"");
        var (_, oAll, eAll) = RunReadelf($"--all \"{_objectFile}\"");
        Assert.That(ea, Is.Empty);
        Assert.That(eAll, Is.Empty);
        Assert.That(oa, Is.EqualTo(expected));
        Assert.That(oAll, Is.EqualTo(expected));
    }

    [Test]
    public void DefaultBundle_MatchesGolden()
    {
        Assert.That(_objectFile, Is.Not.Null);
        AssertStdoutMatchesGolden($"\"{_objectFile}\"", "view_default.txt");
    }

    private static void AssertStdoutMatchesGolden(string arguments, string goldenFileName)
    {
        var expected = LoadGolden(goldenFileName);
        var (code, stdout, stderr) = RunReadelf(arguments);
        Assert.That(code, Is.EqualTo(0), () => stderr);
        Assert.That(stderr, Is.Empty);
        Assert.That(stdout, Is.EqualTo(expected));
    }

    private static string LoadGolden(string fileName)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Golden", fileName);
        if (!File.Exists(path))
        {
            Assert.Fail($"Missing golden file (copy to output?): {path}");
        }

        return File.ReadAllText(path, Encoding.UTF8);
    }

    private static (int Code, string StdOut, string StdErr) RunReadelf(string arguments)
    {
        return RunProcess(ReadelfExe, arguments);
    }

    private static void RunChecked(string fileName, string arguments)
    {
        var (code, _, err) = RunProcess(fileName, arguments);
        if (code != 0)
        {
            Assert.Fail($"Command failed ({code}): {fileName} {arguments}\n{err}");
        }
    }

    private static (int Code, string StdOut, string StdErr) RunProcess(string fileName, string arguments)
    {
        var psi = new ProcessStartInfo(fileName, arguments)
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };
        using var p = Process.Start(psi);
        Assert.That(p, Is.Not.Null);
        var proc = p!;
        var stdout = proc.StandardOutput.ReadToEnd();
        var stderr = proc.StandardError.ReadToEnd();
        proc.WaitForExit();
        return (proc.ExitCode, stdout, stderr);
    }

    private static string ResolveReadelfExe()
    {
        var env = Environment.GetEnvironmentVariable("KORE_READELF_EXE");
        if (!string.IsNullOrWhiteSpace(env) && File.Exists(env))
        {
            return env;
        }

        var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
        var config =
#if DEBUG
            "Debug";
#else
            "Release";
#endif
        var name = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? "riscv32-kuick-elf-readelf.exe"
            : "riscv32-kuick-elf-readelf";
        return Path.Combine(repoRoot, "bin", "Kuick.Tools", "bin", config, "net8.0", name);
    }

    private static string? FindOnPath(string fileName)
    {
        var pathEnv = Environment.GetEnvironmentVariable("PATH");
        if (string.IsNullOrEmpty(pathEnv))
        {
            return null;
        }

        var ext = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".exe" : "";
        foreach (var dir in pathEnv.Split(Path.PathSeparator))
        {
            var candidate = Path.Combine(dir, fileName + ext);
            if (File.Exists(candidate))
            {
                return candidate;
            }
        }

        return null;
    }

    /// <summary>LLVM lld (supports <c>-shared</c> for RISC-V ELF32; GNU embedded <c>riscv32-unknown-elf-ld</c> does not).</summary>
    private static string? FindLld()
    {
        foreach (var name in new[] { "ld.lld", "ld.lld.exe" })
        {
            var found = FindOnPath(name);
            if (found is not null)
            {
                return found;
            }
        }

        return null;
    }
}
