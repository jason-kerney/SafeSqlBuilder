using SafeSqlBuilder.Generators;
using PoorMansTSqlFormatterLib;
using Xunit;

namespace SafeSqlBuilder.Tests.Generators
{
    public class TestGroupByShould
    {
        [Fact]
        public void GenerateGroupByWithSingleColumn()
        {
            var validFields = new[]
            {
                "Distinct_Values_Column",
            };
            

            var queryData = Builder.BuildQueryData(fields: validFields, groupBy: validFields, distinct: true);

            var sut = new GroupByGenerator(queryData);
            var response = sut.ToString();
            var formattedResponse = SqlFormattingManager.DefaultFormat(response);

            var expectedResponse = SqlFormattingManager.DefaultFormat("GROUP BY [Distinct_Values_Column_Cleaned]");
            Assert.Equal(expectedResponse, formattedResponse);
        }

        [Fact]
        public void GenerateGroupByWithManyColumns()
        {
            var validFields = new[]
            {
                "Distinct_Values_Column",
                "Another_Values_Column"
            };
            
            var queryData = Builder.BuildQueryData(validFields, groupBy: validFields, distinct: true);

            var sut = new GroupByGenerator(queryData);
            var response = sut.ToString();
            var formattedResponse = SqlFormattingManager.DefaultFormat(response);

            var expectedResponse =
                SqlFormattingManager.DefaultFormat(
                    "GROUP BY [Distinct_Values_Column_Cleaned], [Another_Values_Column_Cleaned]"
                );
            Assert.Equal(expectedResponse, formattedResponse);
        }

        [Fact]
        public void GenerateGroupByWithManyColumnsRespectsColumnOrder()
        {
            var fields = new[]
            {
                "Distinct_Values_Column",
                "Another_Values_Column"
            };

            var groupBy = new[]
            {
                "Another_Values_Column",
                "Distinct_Values_Column"
            };

            var queryData = Builder.BuildQueryData(fields, groupBy: groupBy, distinct: true);

            var sut = new GroupByGenerator(queryData);
            var response = sut.ToString();
            var formattedResponse = SqlFormattingManager.DefaultFormat(response);

            var expectedResponse = SqlFormattingManager.DefaultFormat("GROUP BY [Another_Values_Column_Cleaned], [Distinct_Values_Column_Cleaned]");
            Assert.Equal(expectedResponse, formattedResponse);
        }
    }
}
