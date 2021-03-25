using SafeSqlBuilder;
using Xunit;

namespace SafeSqlBuilder.Tests
{
    public static partial class QueryDataObject
    {
        public class GroupByShould
        {
            [Fact]
            public void DetectInvalidGroupByColumnInvalid()
            {
                const string expectedField = "Valid";
                var fields = new[] { expectedField };

                var (query, fakeValidColumns, fakeClauseFactory) = Builder.InitializeFakeItemsForQueryData(fields, groupBy: new[] {"Bad"}, columnIsInvalid: Builder.BuildValidator(expectedField));
                var sut = new QueryData(query, fakeValidColumns.Object, fakeClauseFactory.Object, "My Table");
                
                Assert.False(sut.IsValid);
                Assert.True(sut.IsInvalid);
            }

            [Fact]
            public void DetectValidGroupByColumnAsValid()
            {
                const string expectedField = "Valid";
                var fields = new[] { expectedField };

                var (query, fakeValidColumns, fakeClauseFactory) = Builder.InitializeFakeItemsForQueryData(fields, groupBy: new[] {expectedField}, columnIsInvalid: Builder.BuildValidator(expectedField));
                var sut = new QueryData(query, fakeValidColumns.Object, fakeClauseFactory.Object, "My Table");
                
                Assert.True(sut.IsValid);
                Assert.False(sut.IsInvalid);
            }

            [Fact]
            public void DetectValidNullGroupByAsValid()
            {
                const string expectedField = "Valid";
                var fields = new[] { expectedField };

                var (query, fakeValidColumns, fakeClauseFactory) = Builder.InitializeFakeItemsForQueryData(fields, columnIsInvalid: Builder.BuildValidator(expectedField));
                query.GroupBy = null;
                
                var sut = new QueryData(query, fakeValidColumns.Object, fakeClauseFactory.Object, "My Table");
                
                Assert.True(sut.IsValid);
                Assert.False(sut.IsInvalid);
            }

            [Fact]
            public void DetectValidEmptyGroupByAsValid()
            {
                const string expectedField = "Valid";
                var fields = new[] { expectedField };

                var (query, fakeValidColumns, fakeClauseFactory) = Builder.InitializeFakeItemsForQueryData(fields, columnIsInvalid: Builder.BuildValidator(expectedField));
                query.GroupBy = new string[0];
                
                var sut = new QueryData(query, fakeValidColumns.Object, fakeClauseFactory.Object, "My Table");
                
                Assert.True(sut.IsValid);
                Assert.False(sut.IsInvalid);
            }

            [Fact]
            public void DetectAnyInvalidColumnAsInvalid()
            {
                const string expectedField = "Valid";
                var fields = new[] { expectedField };

                var (query, fakeValidColumns, fakeClauseFactory) = Builder.InitializeFakeItemsForQueryData(fields, columnIsInvalid: Builder.BuildValidator(expectedField));
                query.GroupBy = new [] {expectedField, "Bad_Field"};
                
                var sut = new QueryData(query, fakeValidColumns.Object, fakeClauseFactory.Object, "My Table");
                
                Assert.False(sut.IsValid);
                Assert.True(sut.IsInvalid);
            }
        }
    }
}