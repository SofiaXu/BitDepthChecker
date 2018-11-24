using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace BitDepthChecker.Tests
{
    [TestClass()]
    public class CheckerTests
    {
        [TestMethod()]
        public void FullyCheckBitDepthPCM1Test()
        {
            var data = File.ReadAllBytes(@"D:\Source\Repos\BitDepthChecker\BitDepthChecker\bin\Debug\test1.pcm");
            Assert.AreEqual(new Checker(3, data, 24).FullyCheckBitDepth(), Checker.BitDepth.Is24);
        }

        [TestMethod()]
        public void FullyCheckBitDepthPCM2Test()
        {
            var data = File.ReadAllBytes(@"D:\Source\Repos\BitDepthChecker\BitDepthChecker\bin\Debug\test2.pcm");
            Assert.AreEqual(new Checker(3, data, 24).FullyCheckBitDepth(), Checker.BitDepth.Is16);
        }

        [TestMethod()]
        public void Test8BitTest()
        {
            int testNumber = 0;
            Assert.IsTrue(new Checker().Test8Bit(new byte[4] { 0, 0, 0, 0 }), $"test { testNumber++ }");
            Assert.IsFalse(new Checker().Test8Bit(new byte[4] { 1, 0, 0, 0 }), $"test { testNumber++ }");
            Assert.IsFalse(new Checker().Test8Bit(new byte[4] { 0, 1, 0, 0 }), $"test { testNumber++ }");
            Assert.IsFalse(new Checker().Test8Bit(new byte[4] { 0, 0, 1, 0 }), $"test { testNumber++ }");
            Assert.IsTrue(new Checker().Test8Bit(new byte[4] { 0, 0, 0, 1 }), $"test { testNumber++ }");
            Assert.IsFalse(new Checker().Test8Bit(new byte[4] { 1, 1, 1, 1 }), $"test { testNumber++ }");
            Assert.IsFalse(new Checker().Test8Bit(new byte[4] { 0, 1, 1, 1 }), $"test { testNumber++ }");
            Assert.IsFalse(new Checker().Test8Bit(new byte[4] { 0, 0, 1, 1 }), $"test { testNumber++ }");
            Assert.IsTrue(new Checker().Test8Bit(new byte[4] { 0, 0, 0, 1 }), $"test { testNumber++ }");
        }

        [TestMethod()]
        public void Test12BitTest()
        {
            int testNumber = 0;
            Assert.IsTrue(new Checker().Test12Bit(new byte[4] { 0, 0, 0, 0 }), $"test { testNumber++ }");
            Assert.IsFalse(new Checker().Test12Bit(new byte[4] { 1, 0, 0, 0 }), $"test { testNumber++ }");
            Assert.IsFalse(new Checker().Test12Bit(new byte[4] { 0, 1, 0, 0 }), $"test { testNumber++ }");
            Assert.IsFalse(new Checker().Test12Bit(new byte[4] { 0, 0, 1, 0 }), $"test { testNumber++ }");
            Assert.IsTrue(new Checker().Test12Bit(new byte[4] { 0, 0, 0, 1 }), $"test { testNumber++ }");
            Assert.IsFalse(new Checker().Test12Bit(new byte[4] { 1, 1, 1, 1 }), $"test { testNumber++ }");
            Assert.IsFalse(new Checker().Test12Bit(new byte[4] { 0, 1, 1, 1 }), $"test { testNumber++ }");
            Assert.IsFalse(new Checker().Test12Bit(new byte[4] { 0, 0, 1, 1 }), $"test { testNumber++ }");
            Assert.IsTrue(new Checker().Test12Bit(new byte[4] { 0, 0, 0, 1 }), $"test { testNumber++ }");
            Assert.IsTrue(new Checker().Test12Bit(new byte[4] { 0, 0, 16, 1 }), $"test { testNumber++ }");
            Assert.IsTrue(new Checker().Test12Bit(new byte[2] { 0, 1 }), $"test { testNumber++ }");
            Assert.IsTrue(new Checker().Test12Bit(new byte[2] { 16, 1 }), $"test { testNumber++ }");
        }

        [TestMethod()]
        public void Test16BitTest()
        {
            int testNumber = 0;
            Assert.IsTrue(new Checker().Test16Bit(new byte[4] { 0, 0, 0, 0 }), $"test { testNumber++ }");
            Assert.IsFalse(new Checker().Test16Bit(new byte[4] { 1, 0, 0, 0 }), $"test { testNumber++ }");
            Assert.IsFalse(new Checker().Test16Bit(new byte[4] { 0, 1, 0, 0 }), $"test { testNumber++ }");
            Assert.IsTrue(new Checker().Test16Bit(new byte[4] { 0, 0, 1, 0 }), $"test { testNumber++ }");
            Assert.IsTrue(new Checker().Test16Bit(new byte[4] { 0, 0, 0, 1 }), $"test { testNumber++ }");
            Assert.IsFalse(new Checker().Test16Bit(new byte[4] { 1, 1, 1, 1 }), $"test { testNumber++ }");
            Assert.IsFalse(new Checker().Test16Bit(new byte[4] { 0, 1, 1, 1 }), $"test { testNumber++ }");
            Assert.IsTrue(new Checker().Test16Bit(new byte[4] { 0, 0, 1, 1 }), $"test { testNumber++ }");
            Assert.IsTrue(new Checker().Test16Bit(new byte[2] { 0, 1 }), $"test { testNumber++ }");
            Assert.IsTrue(new Checker().Test16Bit(new byte[2] { 16, 1 }), $"test { testNumber++ }");
        }
    }
}