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
        [TestMethod]
        public void ConditionManipulations()
        {
            // -1-
            PrimitiveCondition primitiveCondition1 = new PrimitiveCondition("IsOnline", "true", ComparisonOperator.NE);

            Condition condition1 = new Condition("IsOnline", true, ComparisonOperator.NE);

            PrimitiveCondition primitiveCondition2 = condition1.GetPrimitives()[0];

            Condition condition2 = new Condition(primitiveCondition1);

            Assert.AreEqual(primitiveCondition1.GetQueryString(), condition1.GetQueryString());
            Assert.AreEqual(primitiveCondition1.GetQueryString(), primitiveCondition2.GetQueryString());
            Assert.AreEqual(primitiveCondition1.GetQueryString(), condition2.GetQueryString());
            Assert.AreEqual(primitiveCondition1.GetQueryString(), "IsOnline%20ne%20true");

            // -2-
            Condition conditionDate1 = new Condition("CreationTime", new DateTime(2020, 1, 10), ComparisonOperator.GE);
            Condition conditionDate2 = new Condition("CreationTime", new DateTime(2020, 1, 15), ComparisonOperator.LE);

            Interval<DateTime> interval1 = new Interval<DateTime>(new DateTime(2020, 1, 10), new DateTime(2020, 1, 15));

            IntervalCondition<DateTime> intervalConditionDate1 = new IntervalCondition<DateTime>(
                "CreationTime",
                new Interval<DateTime>(new DateTime(2020, 1, 10), new DateTime(2020, 1, 15))
            );

            PrimitiveCondition primitiveConditionDate1 = conditionDate1.GetPrimitives()[0];
            PrimitiveCondition primitiveConditionDate2 = conditionDate2.GetPrimitives()[0];
            
            PrimitiveCondition[] primitiveConditionDates1 = interval1.GeneratePrimitiveConditions("CreationTime");
            PrimitiveCondition[] primitiveConditionDates2 = intervalConditionDate1.GetPrimitives();

            Assert.AreEqual(primitiveConditionDate1.GetQueryString(), primitiveConditionDates1[0].GetQueryString());
            Assert.AreEqual(primitiveConditionDate1.GetQueryString(), primitiveConditionDates2[0].GetQueryString());
            Assert.AreEqual(primitiveConditionDate1.GetQueryString(), "CreationTime%20ge%202020-01-10T00:00:00Z");
            Assert.AreEqual(primitiveConditionDate2.GetQueryString(), primitiveConditionDates1[1].GetQueryString());
            Assert.AreEqual(primitiveConditionDate2.GetQueryString(), primitiveConditionDates2[1].GetQueryString());
            Assert.AreEqual(primitiveConditionDate2.GetQueryString(), "CreationTime%20le%202020-01-15T00:00:00Z");

            // -3-
            Condition conditionDate3 = new Condition("CreationTime", new DateTime(2019, 12, 26), ComparisonOperator.GE);
            Condition conditionDate4 = new Condition("CreationTime", new DateTime(2019, 12, 25), ComparisonOperator.GT);
            IntervalCondition<DateTime> intervalConditionDate2 = new IntervalCondition<DateTime>(
                "CreationTime",
                new Interval<DateTime> { Start = new DateTime(2019, 12, 26) }
            );
            IntervalCondition<DateTime> intervalConditionDate3 = new IntervalCondition<DateTime>(
                "CreationTime",
                new Interval<DateTime> { Start = new DateTime(2019, 12, 25), IncludeStart = false }
            );

            Assert.AreEqual(conditionDate3.GetQueryString(), intervalConditionDate2.GetQueryString());
            Assert.AreEqual(conditionDate3.GetQueryString(), "CreationTime%20ge%202019-12-26T00:00:00Z");
            Assert.AreEqual(conditionDate4.GetQueryString(), intervalConditionDate3.GetQueryString());
            Assert.AreEqual(conditionDate4.GetQueryString(), "CreationTime%20gt%202019-12-25T00:00:00Z");

            DateTime dateTimeTest1 = new DateTime(2019, 12, 25);
            DateTime dateTimeTest2 = new DateTime(2019, 12, 26);

            Assert.IsFalse(intervalConditionDate2.Interval.ContainsValue(dateTimeTest1));
            Assert.IsFalse(intervalConditionDate3.Interval.ContainsValue(dateTimeTest1));
            Assert.IsTrue(intervalConditionDate2.Interval.ContainsValue(dateTimeTest2));
            Assert.IsTrue(intervalConditionDate3.Interval.ContainsValue(dateTimeTest2));

            // -4-
            Condition condition3 = new Condition();
            condition3.Name = "Robot/Type";
            condition3.Value = "Attended";

            Condition condition4 = new Condition();
            condition4.BaseName = "Robot";
            condition4.PropertyName = "Type";
            condition4.Value = "Attended";

            Condition condition5 = new Condition("Robot/Type", "Attended");

            Condition condition6 = new Condition("Robot/Type = Attended");

            Assert.AreEqual(condition3.GetQueryString(), condition4.GetQueryString());
            Assert.AreEqual(condition3.GetQueryString(), condition5.GetQueryString());
            Assert.AreEqual(condition3.GetQueryString(), condition6.GetQueryString());
            Assert.AreEqual(condition3.GetQueryString(), "Robot/Type%20eq%20%27Attended%27");
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
