using Microsoft.VisualStudio.TestTools.UnitTesting;

using NetProvider;
using NetProvider.Network;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace UnitTestProject
{
    /// <summary>
    /// ResponseTypeTest 的摘要说明
    /// </summary>
    [TestClass]
    public class ResponseTypeTest
    {
        ITestService testService;
        public ResponseTypeTest()
        {
            testService= ApiServiceCreater.CreateObject<ITestService>("https://docs.microsoft.com/");
            //
            //TODO:  在此处添加构造函数逻辑
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，该上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 附加测试特性
        //
        // 编写测试时，可以使用以下附加特性: 
        //
        // 在运行类中的第一个测试之前使用 ClassInitialize 运行代码
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // 在类中的所有测试都已运行之后使用 ClassCleanup 运行代码
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // 在运行每个测试之前，使用 TestInitialize 来运行代码
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // 在每个测试运行完之后，使用 TestCleanup 来运行代码
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestMethod1()
        {
            string s1 = testService.GetJson();
            TestContext.WriteLine(s1);
            Stream stream = testService.GetStream();
            TestContext.WriteLine(new StreamReader(stream).ReadToEnd());
            HttpResponseMessage message = testService.GetResponseMessage();
            TestContext.WriteLine(message.Content.ToString());
            byte[] vs = testService.GetBytes();
            TestContext.WriteLine(Encoding.UTF8.GetString(vs));
        }
    }
    public interface ITestService
    {
        /// <summary>
        /// 请求数据调用
        /// </summary>
        /// <returns></returns>
        [Request(RequestType.Get, "/zh-cn/dotnet/fundamentals/toc.json")]
        string GetJson();

        [Request(RequestType.Get, "/zh-cn/dotnet/fundamentals/toc.json")]
        Stream GetStream();

        [Request(RequestType.Get, "/zh-cn/dotnet/fundamentals/toc.json")]
        HttpResponseMessage GetResponseMessage();

        [Request(RequestType.Get, "/zh-cn/dotnet/fundamentals/toc.json")]
        byte[] GetBytes();
    }
}
