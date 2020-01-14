using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UiPathCloudAPISharp.Query;

namespace UiPathCloudAPISharp.Tests
{
    [TestClass]
    public class DataTransformTests
    {
        private IConfiguration _configuration;
        private UiPathCloudAPI uiPath;
        
        [TestMethod]
        public void ConditionManipulations()
        {
            
        }

        [TestMethod]
        public void FilterManipulations()
        {
            Filter filter1 = new Filter();
            filter1.AddCondition("Name = Bob");
            filter1.AddCondition("Name = Lex");

            Filter filter2 = new Filter("Name = Bob and Name = Lex");
            
            Filter filter3 = new Filter(new Condition("Name", "Bob", ComparisonOperator.EQ));
            filter3.AddCondition(new Condition("Name", "Lex"));

            Filter filter4 = new Filter();
            filter4.AddCondition("Name = Tom");
            filter4.AddCondition("Name = Lex");
            var condition1 = filter4.ConditionLine.Where(x => x is Condition)
                .Select(e => e as Condition)
                .Where(c => c.Value.ToString() == "Tom")
                .First();
            condition1.Value = "Bob";

            Assert.AreEqual(filter1.GetQueryString(), filter2.GetQueryString());
            Assert.AreEqual(filter1.GetQueryString(), filter3.GetQueryString());
            Assert.AreEqual(filter1.GetQueryString(), filter4.GetQueryString());
            Assert.AreEqual(filter1.GetQueryString(), "$filter=Name%20eq%20%27Bob%27%20and%20Name%20eq%20%27Lex%27");
        }
    }
}
