using System.Collections.Generic;
using System.IO;
using System.Linq;

using NUnit.Framework;

namespace XSharp.Tests
{
    [TestFixture]
    public class CompileTests
    {
        private const bool IgnoreCase = false;
        private const bool Trim = false;
        private const bool SkipLineComments = false;

        private static readonly string AssemblyDir = Path.GetDirectoryName(typeof(CompileTests).Assembly.Location);

        private static IEnumerable<string> GetXSharpInput()
        {
            var xInputDir = new DirectoryInfo(Path.Combine(AssemblyDir, "Input"));
            return xInputDir.GetFiles("*.xs").Select(f => f.FullName);
        }

        [TestCaseSource(nameof(GetXSharpInput))]
        public void TestCompilation(string aPath)
        {
            var xExpectedOutputFile = Path.Combine(Path.GetDirectoryName(aPath), "..",
                "ExpectedOutput", Path.ChangeExtension(Path.GetFileName(aPath), ".asm"));

            using (var xOutputStream = new MemoryStream())
            {
                var xWriter = new StreamWriter(xOutputStream);
                var xCompiler = new Compiler(xWriter);

                using (var xReader = new StreamReader(File.OpenRead(aPath)))
                {
                    xCompiler.Emit(xReader);
                }

                xWriter.Flush();

                xOutputStream.Position = 0;

                using (var xExpectedOutputReader = new StreamReader(File.OpenRead(xExpectedOutputFile)))
                {
                    using (var xActualOutputReader = new StreamReader(xOutputStream))
                    {
                        while (!xActualOutputReader.EndOfStream)
                        {
                            var xActualLine = xActualOutputReader.ReadLine();
                            var xExpectedLine = xExpectedOutputReader.ReadLine();

                            if (SkipLineComments
                                && xActualLine.Trim().StartsWith(';')
                                && xExpectedLine.StartsWith(';'))
                            {
                                continue;
                            }

                            if (IgnoreCase)
                            {
                                xActualLine = xActualLine.ToUpperInvariant();
                                xExpectedLine = xExpectedLine.ToUpperInvariant();
                            }

                            if (Trim)
                            {
                                xActualLine = xActualLine.Trim();
                                xExpectedLine = xExpectedLine.Trim();
                            }

                            Assert.That(xActualLine, Is.EqualTo(xExpectedLine));
                        }
                    }
                }
            }
        }
    }
}
