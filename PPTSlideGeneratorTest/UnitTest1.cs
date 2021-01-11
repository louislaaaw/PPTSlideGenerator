using System;
using PPTSlideGenerator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace PPTSlideGeneratorTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ParseBoldElementsFromXaml_ThreeMatches_ReturnsThree()
        {
            string xml = "<Run FontWeight=\"Bold\">Done</Run>" + "<Run FontWeight=\"Bold\">something</Run>" + "<Run FontWeight=\"Bold\">bad</Run>";
            MainWindow mainWindow = new MainWindow();
            List<string> BoldList = mainWindow.ParseBoldElementsFromXaml(xml);
            int expectedResult = 3;
            Assert.AreEqual(expectedResult, BoldList.Count);
        }
    }
}
