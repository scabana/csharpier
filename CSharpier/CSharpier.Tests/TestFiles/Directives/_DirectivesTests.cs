using CSharpier.Tests.TestFileTests;
using NUnit.Framework;

namespace CSharpier.Tests.TestFiles
{
    public class DirectivesTests : BaseTest
    {
        [Test]
        public void BasicDirectives()
        {
            this.RunTest("Directives", "BasicDirectives");
        }
        [Test]
        public void IfDirectiveEmptyBlock()
        {
            this.RunTest("Directives", "IfDirectiveEmptyBlock");
        }
        [Test]
        public void Regions()
        {
            this.RunTest("Directives", "Regions");
        }
    }
}