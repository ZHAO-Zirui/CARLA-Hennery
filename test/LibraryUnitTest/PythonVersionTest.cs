using System.Collections.Generic;
using CARLA_Hennery.Library.Python;
using NUnit.Framework;

namespace LibraryUnitTest
{
    [TestFixture]
    public class PythonVersionTest
    {
        private Version version1;
        private Version version2;
        private Version version3;
        private Version version4;
        private Version version5;
        private Version version6;
        
        
        [SetUp]
        public void Setup()
        {
            version1 = new Version(1, 0, 0);
            version2 = new Version(1, 0, 0);
            version3 = new Version(2, 1, 3);
            version4 = new Version(2, 2, 1);
            version5 = new Version(2, 1, 4);
            version6 = new Version(3, 1, 4);
        }

        [Test]
        public void TestConstructor()
        {
            Assert.AreEqual(1,version1.Major);
            Assert.AreEqual(0,version1.Minor);
            Assert.AreEqual(0,version1.Patch);
        }

        [Test]
        public void TestToString()
        {
            Assert.AreEqual("2.1.3",version3.ToString());
        }

        [Test]
        public void TestEqual()
        {
            Assert.AreEqual(true,Equals(version1, version2));
        }

        [Test]
        public void TestNotEqual()
        {
            Assert.AreEqual(false,Equals(version2, version1));
            Assert.AreEqual(false,Equals(version3, version5));
            Assert.AreEqual(false,Equals(version3, version4));
        }

        [Test]
        public void TestSort()
        {
            var list = new List<Version>(){version1, version3, version4, version5, version6};
            list.Sort();
            Assert.AreEqual(version1,list[0]);
            Assert.AreEqual(version3,list[1]);
            Assert.AreEqual(version5,list[2]);
            Assert.AreEqual(version4,list[3]);
            Assert.AreEqual(version6,list[4]);
        }
    }
}