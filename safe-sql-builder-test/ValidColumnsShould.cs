using System;
using System.Collections.Generic;
using ApprovalTests;
using SafeSqlBuilder;
using Xunit;

namespace SafeSqlBuilder.Tests
{
    public class ValidColumnsShould
    {
        [Theory]
        [MemberData(nameof(GetInvalidFieldTestData))]
        public void IndicateInvalidColumnSetupsAsInvalid(string[] goodColumns, string[] checkedFields, bool isInvalid)
        {
            var sut = new ValidColumns(goodColumns);
            
            Assert.Equal(isInvalid, sut.ContainsInvalidColumns(checkedFields));
            Assert.Equal(!isInvalid, sut.ContainsOnlyValidColumns(checkedFields));
        }

        [Theory]
        [MemberData(nameof(GetInvalidFieldTestData))]
        public void IndicateValidColumnSetupsAsValid(string[] goodColumns, string[] checkedFields, bool isInvalid)
        {
            var sut = new ValidColumns(goodColumns);
            
            Assert.Equal(!isInvalid, sut.ContainsOnlyValidColumns(checkedFields));
        }


        public static IEnumerable<object[]> GetInvalidFieldTestData =>
            new List<object[]>
            {
                new object[] {new[] { "A_Good_Column" }      , null                                   , true},
                new object[] {new[] { "A_Good_Column" }      , new[] { "A_Good_Column" }              , false},
                new object[] {new[] { "Another_Good_Column" }, new[] { "Another_Good_Column" }        , false},
                new object[] {new[] { "Some_Good_Column" }   , new[] { "Not_Valid" }                  , true},
                new object[] {new[] { "A_Good_Column" }      , new[] { "A_Good_Column", "Bad_Column" }, true},
            };

        [Fact]
        public void GetSafeName()
        {
            IValidColumns sut = new ValidColumns(new [] { "Good_Column" });

            var result = sut.GetColumn("Good_Column");
            
            Assert.Equal("Good_Column", result);
        }

        [Fact]
        public void GetNonSafeNameAsEmpty()
        {
            IValidColumns sut = new ValidColumns(new [] { "Good_Column" });

            var result = sut.GetColumn("Bad_Column");
            
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void ShouldDetectInvalidField()
        {
            IValidColumns sut = new ValidColumns(new [] { "A_Good_Column" });
            Assert.True(sut.IsInvalidColumn("A_Bad_Column"));
        }

        [Fact]
        public void ShouldDetectValidField()
        {
            IValidColumns sut = new ValidColumns(new[] { "A_Good_Column" });
            Assert.False(sut.IsInvalidColumn("A_Good_Column"));
        }

        [Fact]
        public void ShouldDetectSecondValidField()
        {
            IValidColumns sut = new ValidColumns(new[] { "A_Correct_Column" });
            Assert.True(sut.IsInvalidColumn("A_Good_Column"));
        }

        [Fact]
        public void ShouldDetectAnyFieldIsValid()
        {
            IValidColumns sut = new ValidColumns(new[] { "A_Correct_Column", "A_Great_Column" });
            Assert.False(sut.IsInvalidColumn("A_Great_Column"));
        }

        [Fact]
        public void ShouldReturnAllValidColumns()
        {
            IValidColumns sut = new ValidColumns(new[] { "A_Correct_Column", "A_Great_Column" });
            Approvals.VerifyAll(sut.GetColumns(), "Column");
        }

        [Fact]
        public void ShouldGetValidColumnsFromInstancePropertyNames()
        {
            var sut = ValidColumns.From<ModelForColumnNames>();
            Approvals.VerifyAll(sut.GetColumns(), "Column");
        }
    }

    // ReSharper disable UnusedMember.Global
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ModelForColumnNames
    {
        public string Column1 { get; set; }
        public int Column2 { get; set; }
        public DateTime Column3 { get; set; }
        public static int StaticColumn { get; set; }
        // ReSharper disable once UnusedMember.Local
        private bool Column4 { get; set; }
    }
    // ReSharper restore UnusedMember.Global
}