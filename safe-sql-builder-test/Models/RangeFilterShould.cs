using System;
using System.Collections.Generic;
using SafeSqlBuilder.Models;
using Xunit;

namespace SafeSqlBuilder.Tests.Models
{
    public class RangeFilterShould
    {
        [Theory]
        [MemberData(nameof(GetOneSidedOpTestData))]
        public void ValidateCorrectly(bool expectedValid, object start, object end, RangeOperator op)
        {
            var sut = new FilterRange
            {
                Start = start,
                End = end,
                RangeOperator = op
            };
            
            Assert.Equal(expectedValid, sut.IsValid);
            Assert.Equal(!expectedValid, sut.IsInvalid);
        }
        
        public static IEnumerable<object[]> GetOneSidedOpTestData =>
        new List<object[]>
        {
            new object[] { true, DateTime.Parse("3/14/1592 6:53:58.97"), null, RangeOperator.GreaterThan },
            new object[] { false, null, null, RangeOperator.GreaterThan },
            new object[] { true, null, "Some Object", RangeOperator.GreaterThan },
            new object[] { false, "yes", "Some Object", RangeOperator.GreaterThan },
            
            new object[] { true, DateTime.Parse("3/14/1592 6:53:58.97"), null, RangeOperator.LessThan },
            new object[] { false, null, null, RangeOperator.LessThan },
            new object[] { true, null, "Some Object", RangeOperator.LessThan },
            new object[] { false, "yes", "Some Object", RangeOperator.LessThan },
            
            new object[] { true, DateTime.Parse("3/14/1592 6:53:58.97"), null, RangeOperator.GreaterThanEqual },
            new object[] { false, null, null, RangeOperator.GreaterThanEqual },
            new object[] { true, null, "Some Object", RangeOperator.GreaterThanEqual },
            new object[] { false, "yes", "Some Object", RangeOperator.GreaterThanEqual },
            
            new object[] { true, DateTime.Parse("3/14/1592 6:53:58.97"), null, RangeOperator.LessThanEqual },
            new object[] { false, null, null, RangeOperator.LessThanEqual },
            new object[] { true, null, "Some Object", RangeOperator.LessThanEqual },
            new object[] { false, "yes", "Some Object", RangeOperator.LessThanEqual },
            
            new object[] { false, 56, null, RangeOperator.Between },
            new object[] { false, null, "Thing", RangeOperator.Between },
            new object[] { true, 56, 12, RangeOperator.Between },
            
            new object[] { false, 56, 12, (RangeOperator) 33 },
        };
    }
}