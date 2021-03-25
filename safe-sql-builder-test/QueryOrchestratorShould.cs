using System.Collections.Generic;
using ApprovalTests;
using SafeSqlBuilder;
using SafeSqlBuilder.Models;
using Xunit;

namespace SafeSqlBuilder.Tests
{
    //Examples From: https://github.com/infinuendo/AdventureWorks/blob/master/OLTP/images/
    public class QueryOrchestratorShould
    {
        private const string AdventureWorksTableName = "[AdventureWorks].[Product]";

        [Fact]
        public void ReturnQueryBuilder()
        {
            var validColumns = new ValidColumns(new []
            {
                "ProductId",
                "Name",
                "ProductNumber",
                "MakeFlag",
                "FinishedGoodsFlag",
                "Color",
                "SafetyStockLevel",
                "ReorderPoint",
                "StandardCost",
                "ProductLine",
            });
            
            var query = new Query
            {
                Fields = new[]
                {
                    "ProductId",
                    "Color",
                    "Name",
                    "ProductNumber",
                    "ProductLine",
                    "StandardCost",
                },
                Distinct = true,
                Filters = new[]
                {
                    new Filter {Property = "ProductLine", Values = new object[] {"Canceled"}},
                    new Filter {Property = "Name", Values = new object[] {"Joe's Car"}},
                },
                Sort = "ProductNumber",
                Direction = "asc"
            };

            var builder = new QueryOrchestrator();
            var (sqlQueryGenerator, _) = builder.BuildQuery(query, validColumns, AdventureWorksTableName);
            var actual = PoorMansTSqlFormatterLib.SqlFormattingManager.DefaultFormat(sqlQueryGenerator);
            Approvals.Verify(actual);
        }
        
        [Fact]
        public void ReturnParameters()
        {
            var validColumns = new ValidColumns(new []
            {
                "ProductId",
                "Name",
                "ProductNumber",
                "MakeFlag",
                "FinishedGoodsFlag",
                "Color",
                "SafetyStockLevel",
                "ReorderPoint",
                "StandardCost",
                "ProductLine",
            });
            
            var query = new Query
            {
                Fields = new[]
                {
                    "ProductId",
                    "Color",
                    "Name",
                    "ProductNumber",
                    "ProductLine",
                    "StandardCost",
                },
                Filters = new[]
                {
                    new Filter {Property = "ProductLine", Values = new object[] {"Canceled"}},
                    new Filter {Property = "StandardCost", Values = new object[] {50}},
                },
                Sort = "Name",
                Direction = "asc"
            };

            var builder = new QueryOrchestrator();
            var (_, parameters) = builder.BuildQuery(query, validColumns, AdventureWorksTableName);
            Approvals.VerifyAll(parameters, "parameter");
        }
        
        [Fact]
        public void ReturnMoreComplexQueryBuilder()
        {
            var validColumns = new ValidColumns(new []
            {
                "ProductId",
                "Name",
                "ProductNumber",
                "MakeFlag",
                "FinishedGoodsFlag",
                "Color",
                "SafetyStockLevel",
                "ReorderPoint",
                "StandardCost",
                "ProductLine",
            });

            var query = new Query
            {
                Fields = new[]
                {
                    "ProductId",
                    "Name",
                    "ProductNumber",
                    "ProductLine",
                    "ReorderPoint",
                    "StandardCost",
                    "MakeFlag",
                    "Color",
                },
                Filters = new[]
                {
                    new Filter
                    {
                        Property = "Name",
                        Values = new object[] {"Widget-D", "Enameled Widget"}
                    },
                    new Filter
                    {
                        Property = "ProductLine",
                        Values = new object[]
                        {
                            "(Blanks)",
                            "Books",
                            "Widgets",
                            "Tools",
                            "Food",
                        }
                    },
                    new Filter
                    {
                        Property = "Color",
                        Values = new object[] {"#AB558A"}
                    },
                },
                Sort = "ProductNumber",
                Direction = "asc"
            };

            var builder = new QueryOrchestrator();
            var (sqlQueryGenerator, _) = builder.BuildQuery(query, validColumns, AdventureWorksTableName);
            var sql = sqlQueryGenerator;
            
            var actual = PoorMansTSqlFormatterLib.SqlFormattingManager.DefaultFormat(sql);
            Approvals.Verify(actual);
        }
        
        [Fact]
        public void ReturnMoreComplexParameters()
        {
            var validColumns = new ValidColumns(new []
            {
                "ProductId",
                "Name",
                "ProductNumber",
                "MakeFlag",
                "FinishedGoodsFlag",
                "Color",
                "SafetyStockLevel",
                "ReorderPoint",
                "StandardCost",
                "ProductLine",
            });

            var query = new Query
            {
                Fields = new[]
                {
                    "ProductId",
                    "Name",
                    "ProductNumber",
                    "ProductLine",
                    "ReorderPoint",
                    "StandardCost",
                    "MakeFlag",
                    "Color",
                },
                Filters = new[]
                {
                    new Filter
                    {
                        Property = "Name",
                        Values = new object[] {"Widget-D", "Enameled Widget"}
                    },
                    new Filter
                    {
                        Property = "ProductLine",
                        Values = new object[]
                        {
                            "(Blanks)",
                            "Books",
                            "Widgets",
                            "Tools",
                            "Food",
                        }
                    },
                    new Filter
                    {
                        Property = "Color",
                        Values = new object[] {"#AB558A"},
                    },
                },
                Sort = "ProductNumber",
                Direction = "asc",
            };

            var builder = new QueryOrchestrator();
            var (_, parameters) = builder.BuildQuery(query, validColumns, AdventureWorksTableName);
            
            Approvals.VerifyAll(parameters, "parameter", parameter =>
            {
                var (name, value) = parameter;
                var valueString =
                    value is IEnumerable<object> objects
                        ? string.Join(", ", objects)
                        : value.ToString();

                return $"(\"{name}\", {valueString})";
            });
        }

        [Fact]
        public void ReturnDistinctQueryBuilder()
        {
            var validColumns = new ValidColumns(new[]
            {
                "ProductId",
                "Name",
                "ProductNumber",
                "MakeFlag",
                "FinishedGoodsFlag",
                "Color",
                "SafetyStockLevel",
                "ReorderPoint",
                "StandardCost",
                "ProductLine",
            });

            var query = new Query
            {
                Fields = new[]
                {
                    "ProductId",
                },
                Distinct = true,
                GroupBy = new[]
                {
                    "ProductId",
                },
                Sort = "ProductId",
                Direction = "asc",
            };

            var builder = new QueryOrchestrator();
            var (sqlQueryGenerator, _) = builder.BuildQuery(query, validColumns, AdventureWorksTableName);
            var actual = PoorMansTSqlFormatterLib.SqlFormattingManager.DefaultFormat(sqlQueryGenerator);
            Approvals.Verify(actual);
        }
    }
}