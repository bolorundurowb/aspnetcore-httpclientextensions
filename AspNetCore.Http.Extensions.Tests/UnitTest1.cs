using FluentAssertions;
using NUnit.Framework;

namespace AspNetCore.Http.Extensions.Tests
{
    [TestFixture]
    public class UnitTest1
    {
        [Test]
        public void Test1()
        {
            true.Should().BeTrue();
        }
    }
}