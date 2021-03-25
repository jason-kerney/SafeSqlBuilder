using System.Linq;
using SafeSqlBuilder;
using SafeSqlBuilder.Generators;
using Moq;
using Xunit;

namespace SafeSqlBuilder.Tests
{
    public static partial class QueryDataObject
    {
        public class ColumnShould
        {
            [Fact]
            public void GetColumns()
            {
                var expectedColumns = new[] {"Good_Field", "Second_Good"};
                var (query, fakeValidColumns, fakeClauseFactory) = Builder.InitializeBasicItemsForQueryData(expectedColumns);
                var sut = new QueryData(query, fakeValidColumns, fakeClauseFactory, "FakeTableName");

                var columns = sut.GetColumns();

                Assert.Equal(expectedColumns.Select(c => $"[{c}_Cleaned]"), columns);
            }

            [Fact]
            public void BeInvalidIfQueryIsNull()
            {
                var fakeValidColumns = new Mock<IValidColumns>().Object;
                var sut = new QueryData(null, fakeValidColumns, new ClauseFactory(fakeValidColumns), "FakeTableName");

                Assert.True(sut.IsInvalid);
                Assert.False(sut.IsValid);
            }

            [Fact]
            public void BeInvalidIfValidColumnsSaysItIsInvalid()
            {
                var (query, fakeValidColumns, fakeClauseFactory) = Builder.InitializeItemsForQueryData(new[] {"Bad_Column"}, columnsAreInvalid: true);

                var sut = new QueryData(query, fakeValidColumns, fakeClauseFactory, "FakeTableName");

                Assert.True(sut.IsInvalid);
                Assert.False(sut.IsValid);
            }
        }
    }
}