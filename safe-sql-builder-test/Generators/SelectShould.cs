using System;
using SafeSqlBuilder.Generators;
using PoorMansTSqlFormatterLib;
using Xunit;

namespace SafeSqlBuilder.Tests.Generators
{
    public class TestBasicSelectShould
    {
        [Fact]
        public void GenerateSelectOfProductId()
        {
            const string field = "ProductId";

            var queryData = Builder.BuildQueryData(Builder.Pack(field));

            var sut = new SelectGenerator(queryData);
            var response = sut.ToString();
            var formattedResponse = SqlFormattingManager.DefaultFormat(response);
            
            var expectedResponse = SqlFormattingManager.DefaultFormat("SELECT [ProductId_Cleaned] FROM [MySchema].[MyTable]");
            Assert.Equal(expectedResponse, formattedResponse);
        }

        [Fact]
        public void GenerateSelectOfRelevantIso()
        {
            var queryData = Builder.BuildQueryData(Builder.Pack("Style"));
            
            var sut = new SelectGenerator(queryData);
            var response = sut.ToString();
            var formattedResponse = SqlFormattingManager.DefaultFormat(response);
            
            var expectedResponse = SqlFormattingManager.DefaultFormat("SELECT [Style_Cleaned] FROM [MySchema].[MyTable]");
            Assert.Equal(expectedResponse, formattedResponse);
        }

        [Fact]
        public void GenerateSelectOfRelevantIsoAndProductId()
        {
            var fields = new[] {
                "Style",
                "ProductId"
            };

            var queryData = Builder.BuildQueryData(fields);

            var sut = new SelectGenerator(queryData);
            var response = sut.ToString();
            var formattedResponse = SqlFormattingManager.DefaultFormat(response);
            
            var expectedResponse = SqlFormattingManager.DefaultFormat("SELECT [Style_Cleaned], [ProductId_Cleaned] FROM [MySchema].[MyTable]");
            Assert.Equal(expectedResponse, formattedResponse);
        }

        [Fact]
        public void GenerateSelectOfProductIdAndRelevantIsoRespectsFieldOrder()
        {
            var fields = new[] {
                "ProductId",
                "Style"
            };

            var queryData = Builder.BuildQueryData(fields);

            var sut = new SelectGenerator(queryData);
            var response = sut.ToString();
            var formattedResponse = SqlFormattingManager.DefaultFormat(response);
           
            var expectedResponse = SqlFormattingManager.DefaultFormat("SELECT [ProductId_Cleaned], [Style_Cleaned] FROM [MySchema].[MyTable]");
            Assert.Equal(expectedResponse, formattedResponse);
        }

        [Fact]
        public void GenerateDistinctSelectOfProductIdAndRelevantIsoRespectsFieldOrder()
        {
            var fields = new[] {
                "ProductId_ID",
                "Some_Item"
            };

            var queryData = Builder.BuildQueryData(fields);

            var sut = new SelectGenerator(queryData);
            var response = sut.ToString();
            var formattedResponse = SqlFormattingManager.DefaultFormat(response);
           
            var expectedResponse = SqlFormattingManager.DefaultFormat("SELECT [ProductId_ID_Cleaned], [Some_Item_Cleaned] FROM [MySchema].[MyTable]");
            Assert.Equal(expectedResponse, formattedResponse);
        }

        [Fact]
        public void GenerateSelectWithNullQueryThrowsException()
        {
            var sut = new SelectGenerator(null);
            Func<string> buildSut = () => sut.ToString();
            
            var ex = Assert.Throws<ArgumentException>(buildSut);
            Assert.Equal("The provided query specification is invalid.", ex.Message);
        }

        [Fact]
        public void GenerateSelectWithNotAcceptableFieldThrowsException()
        {
            var fakeValidColumns = Builder.BuildValidColumns(_ => true);
            var queryData = Builder.BuildQueryData(Builder.Pack("Bad_Field"), validColumns: fakeValidColumns);

            var sut = new SelectGenerator(queryData);
            Func<string> buildSut = () => sut.ToString();
            
            var ex = Assert.Throws<ArgumentException>(buildSut);
            Assert.Equal("The provided query specification is invalid.", ex.Message);
        }

        [Fact]
        public void GenerateSelectWithDistinctValuesOptionTrue()
        {
            var queryData = Builder.BuildQueryData(Builder.Pack("Distinct_Values_Column"), distinct: true);

            var sut = new SelectGenerator(queryData);
            var response = sut.ToString();
            var formattedResponse = SqlFormattingManager.DefaultFormat(response);

            var expectedResponse = SqlFormattingManager.DefaultFormat("SELECT [Distinct_Values_Column_Cleaned] FROM [MySchema].[MyTable]");
            Assert.Equal(expectedResponse, formattedResponse);
        }

        [Fact]
        public void GenerateSelectWithDistinctValuesOptionFalse()
        {
            var queryData = Builder.BuildQueryData(Builder.Pack("All_Values_Column"), distinct: false);

            var sut = new SelectGenerator(queryData);
            var response = sut.ToString();
            var formattedResponse = SqlFormattingManager.DefaultFormat(response);

            var expectedResponse = SqlFormattingManager.DefaultFormat("SELECT [All_Values_Column_Cleaned] FROM [MySchema].[MyTable]");
            Assert.Equal(expectedResponse, formattedResponse);
        }

        [Fact]
        public void GenerateSelectWithDistinctValuesOptionNullDefaultsToFalse()
        {
            var queryData = Builder.BuildQueryData(Builder.Pack("All_Values_Column"));

            var sut = new SelectGenerator(queryData);
            var response = sut.ToString();
            var formattedResponse = SqlFormattingManager.DefaultFormat(response);

            var expectedResponse = SqlFormattingManager.DefaultFormat("SELECT [All_Values_Column_Cleaned] FROM [MySchema].[MyTable]");
            Assert.Equal(expectedResponse, formattedResponse);
        }

        [Fact]
        public void GenerateDistinctSelectUsingGroupByWithColumnNotInSelectExpectsException()
        {
            var fields = new[]
            {
                "Distinct_Values_Column",
                "Another_Values_Column"
            };
            var groupBy = new[]
            {
                "Another_Values_Column",
                "Some_Other_Column"
            };

            var queryData = Builder.BuildQueryData(fields, groupBy: groupBy, distinct: true);

            var sut = new SelectGenerator(queryData);
            Func<string> buildSut = () => sut.ToString();

            var ex = Assert.Throws<ArgumentException>(buildSut);
            Assert.Equal("The provided query specification is invalid.", ex.Message);
        }
    }
}
