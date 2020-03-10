using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetProvider.Network;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string sss= HttpWebHelper.PathCombine("https://docs.microsoft.com", "/zh-cn/dotnet/breadcrumb/toc.json");
            Console.WriteLine(sss);
        }
    }
}
