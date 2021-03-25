using System;
using SafeSqlBuilder.Models;
using Xunit;

namespace SafeSqlBuilder.Tests.Models
{
    public class FilterShould
    {
        [Fact]
        public void DetectValidConfigurationAsBeingValid()
        {
            var sut = (IValidatable) new Filter
            {
                Property = "Valid",
                Values = new dynamic[] { 1 },
            };
            
            Assert.True(sut.IsValid);
            Assert.False(sut.IsInvalid);
        }

        [Fact]
        public void DetectNullValuesAsInvalid()
        {
            var sut = (IValidatable) new Filter
            {
                Property = "Valid",
                Values = null,
            };
            
            Assert.False(sut.IsValid);
            Assert.True(sut.IsInvalid);
        }

        [Fact]
        public void DetectNullRangeAsInvalid()
        {
            var sut = (IValidatable) new Filter
            {
                Property = "Valid",
                Range = null,
            };
            
            Assert.False(sut.IsValid);
            Assert.True(sut.IsInvalid);
        }

        [Fact]
        public void DetectEmptyValuesAsInvalid()
        {
            var sut = (IValidatable) new Filter
            {
                Property = "Valid",
                Values = new dynamic[0],
            };
            
            Assert.False(sut.IsValid);
            Assert.True(sut.IsInvalid);
        }

        [Fact]
        public void DetectNullValuesButIncludesNullsAsValid()
        {
            var sut = (IValidatable) new Filter
            {
                Property = "Field",
                Values = null,
                includeNulls = true
            };
            
            Assert.True(sut.IsValid);
            Assert.False(sut.IsInvalid);
        }

        [Fact]
        public void DetectNullRangeButIncludesNullsIsValid()
        {
            var sut = (IValidatable) new Filter
            {
                Property = "Field",
                Range = null,
                includeNulls = true
            };
            
            Assert.True(sut.IsValid);
            Assert.False(sut.IsInvalid);
        }

        [Fact]
        public void DetectValidRangeAsValid()
        {
            var sut = (IValidatable) new Filter
            {
                Property = "Field",
                Range = new FilterRange
                {
                    Start = DateTime.Parse("3/14/1592 6:53:58.97"),
                    RangeOperator = RangeOperator.GreaterThan
                }
            };
            
            Assert.True(sut.IsValid);
            Assert.False(sut.IsInvalid);
        }

        [Fact]
        public void DetectInvalidRangeInvalid()
        {
            var sut = (IValidatable) new Filter
            {
                Property = "Field",
                Range = new FilterRange
                {
                    Start = null,
                    RangeOperator = RangeOperator.GreaterThan
                }
            };
            
            Assert.False(sut.IsValid);
            Assert.True(sut.IsInvalid);
        }

        [Fact]
        public void DetectInvalidRangeInvalidWhenAllowingNulls()
        {
            var sut = (IValidatable) new Filter
            {
                Property = "Field",
                includeNulls = true,
                Range = new FilterRange
                {
                    Start = null,
                    RangeOperator = RangeOperator.GreaterThan
                }
            };
            
            Assert.False(sut.IsValid);
            Assert.True(sut.IsInvalid);
        }

        [Fact]
        public void DetectNullPropertyNameAsInvalid()
        {
            var sut = (IValidatable) new Filter
            {
                Property = null,
                Values = new dynamic[] {new DateTime()}
            };
            
            Assert.False(sut.IsValid);
            Assert.True(sut.IsInvalid);
        }

        [Fact]
        public void DetectEmptyPropertyNameAsInvalid()
        {
            var sut = (IValidatable) new Filter
            {
                Property = string.Empty,
                Values = new dynamic[] {new DateTime()}
            };
            
            Assert.False(sut.IsValid);
            Assert.True(sut.IsInvalid);
        }

        [Fact]
        public void DetectWhiteSpacePropertyNameAsInvalid()
        {
            var sut = (IValidatable) new Filter
            {
                Property = " \t \r\n",
                Values = new dynamic[] {new DateTime()}
            };
            
            Assert.False(sut.IsValid);
            Assert.True(sut.IsInvalid);
        }

        [Fact]
        public void AllowAFilterWhereOnlyAllowNullsIsSetToFalse()
        {
            var sut = (IValidatable) new Filter
            {
                Property = "Some Property",
                Values = null,
                includeNulls = false
            };
            
            Assert.True(sut.IsValid);
            Assert.False(sut.IsInvalid);
        }
    }
}