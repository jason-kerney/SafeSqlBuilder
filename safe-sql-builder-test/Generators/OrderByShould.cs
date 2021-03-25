using SafeSqlBuilder.Generators;
using Xunit;

namespace SafeSqlBuilder.Tests.Generators
{
    public class OrderByShould
    {
        [Fact]
        public void GenerateOrderByColumnWithoutDirectionDefaultsToAsc()
        {
            const string field = "A_Select_Column";
            const string sortColumn = "A_Sort_Column";

            var queryData = Builder.BuildQueryData(Builder.Pack(field), sort: sortColumn);
            var sut = new OrderByGenerator(queryData);

            Assert.Equal("ORDER BY CASE WHEN [A_Sort_Column_Cleaned] IS NULL THEN 1 ELSE 0 END, [A_Sort_Column_Cleaned]", sut.ToString());
        }

        [Fact]
        public void GenerateAnotherOrderByColumnWithoutDirection()
        {
            var queryData = Builder.BuildQueryData(Builder.Pack("A_Select_Column"), sort: "Another_Sort_Column");
            var sut = new OrderByGenerator(queryData);

            Assert.Equal("ORDER BY CASE WHEN [Another_Sort_Column_Cleaned] IS NULL THEN 1 ELSE 0 END, [Another_Sort_Column_Cleaned]", sut.ToString());
        }

        [Fact]
        public void GenerateOrderByColumnWithDirectionAscending()
        {
            var queryData = Builder.BuildQueryData(Builder.Pack("A_Select_Column"), sort: "Some_New_Sort_Column", direction: "asc");
            var sut = new OrderByGenerator(queryData);

            Assert.Equal("ORDER BY CASE WHEN [Some_New_Sort_Column_Cleaned] IS NULL THEN 1 ELSE 0 END, [Some_New_Sort_Column_Cleaned]", sut.ToString());
        }

        [Fact]
        public void GenerateOrderByColumnWithDirectionDescending()
        {
            var queryData = Builder.BuildQueryData(Builder.Pack("A_Select_Column"), sort: "Some_Stupid_Sort_Column", direction: "desc");
            var sut = new OrderByGenerator(queryData);

            Assert.Equal("ORDER BY [Some_Stupid_Sort_Column_Cleaned] DESC", sut.ToString());
        }

        [Fact]
        public void GenerateOrderByColumnWithNullSortProperty()
        {
            var queryData = Builder.BuildQueryData(Builder.Pack("A_Select_Column"));
            var sut = new OrderByGenerator(queryData);

            Assert.Equal(string.Empty, sut.ToString());
        }

        [Fact]
        public void GenerateOrderByColumnWithEmptySortProperty()
        {
            var queryData = Builder.BuildQueryData(Builder.Pack("A_Select_Column"), sort: "");
            var sut = new OrderByGenerator(queryData);

            Assert.Equal(string.Empty, sut.ToString());
        }
    }
}
