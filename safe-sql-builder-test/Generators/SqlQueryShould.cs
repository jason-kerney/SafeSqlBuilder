using System;
using System.Collections.Generic;
using SafeSqlBuilder.Generators;
using Moq;
using Xunit;

namespace SafeSqlBuilder.Tests.Generators
{
    public class SqlQueryShould
    {
        [Theory]
        [MemberData(nameof(GetQueryPartsData))]
        public void BuildCorrectQuery(string expectedSql, string @select, string @where, string @groupBy, string orderBy)
        {
            var fakeSelect = GetFakeSelect(@select);
            var fakeWhere = GetFakeWhere(@where);
            var fakeGroupBy = GetFakeGroupBy(@groupBy);
            var fakeOrderBy = GetFakeOrderBy(orderBy);
            
            var sut = new SqlQueryGenerator(fakeSelect?.Object, fakeWhere?.Object, fakeGroupBy?.Object, fakeOrderBy?.Object);
            
            Assert.Equal(expectedSql, sut.ToString());
        }

        public static IEnumerable<object[]> GetQueryPartsData =>
            new List<object[]>
            {
                new object[]
                {
                    "SelectClause WhereClause GroupByClause OrderClause", "SelectClause", "WhereClause",
                    "GroupByClause", "OrderClause"
                },
                new object[]
                {
                    "SecondSelect OtherWhere GroupByClauseAgain OrderAgain", "SecondSelect", "OtherWhere",
                    "GroupByClauseAgain", "OrderAgain"
                },
                new object[] {"SelectClause"                , "SelectClause", ""            , ""             , ""},
                new object[] {"SelectClause"                , "SelectClause", null          , null           , null},
                new object[] {"SelectClause"                , "SelectClause", null          , ""             , ""},
                new object[] {"SelectClause"                , "SelectClause", null          , null           , ""},
                new object[] {"SelectClause WhereClause"    , "SelectClause", "WhereClause", null            , ""},
                new object[] {"SelectClause WhereClause"    , "SelectClause", "WhereClause", ""              , null},
                new object[] {"SelectClause WhereClause"    , "SelectClause", "WhereClause", ""              , ""},
                new object[] {"SelectClause WhereClause"    , "SelectClause", "WhereClause", null            , null},
                new object[] {"SelectClause AGroupByClause" , "SelectClause", ""           , "AGroupByClause", null},
                new object[] {"SelectClause AGroupByClause" , "SelectClause", null         , "AGroupByClause", null},
                new object[] {"SelectClause AGroupByClause" , "SelectClause", ""           , "AGroupByClause", ""},
                new object[] {"SelectClause AGroupByClause" , "SelectClause", ""           , "AGroupByClause", ""},
                new object[] {"SelectClause OrderClause"    , "SelectClause", ""           , null            , "OrderClause"},
                new object[] {"SelectClause OrderClause"    , "SelectClause", null         , ""              , "OrderClause"},
                new object[] {"SelectClause OrderClause"    , "SelectClause", null         , null            , "OrderClause"},
                new object[] {"SelectClause OrderClause"    , "SelectClause", ""           , ""              , "OrderClause"},
                new object[]
                {
                    "SelectClause WhereClause AnotherGroupByClause", "SelectClause", "WhereClause",
                    "AnotherGroupByClause", null
                },
                new object[]
                {
                    "SelectClause WhereClause AnotherGroupByClause OrderClause", "SelectClause", "WhereClause",
                    "AnotherGroupByClause", "OrderClause"
                },
                new object[]
                {
                    "SELECT [Toys] FROM [Santa].[Clause] WHERE [Children] IN @Good GROUP BY [Toys] ORDER BY [AGE]",
                    "SELECT [Toys] FROM [Santa].[Clause]",
                    "WHERE [Children] IN @Good",
                    "GROUP BY [Toys]",
                    "ORDER BY [AGE]"
                }
            };
        
        [Fact]
        public void ThrowExceptionIfSelectIsNull()
        {
            var sut = new SqlQueryGenerator(null, null, null, null);
            var argumentException = Assert.Throws<ArgumentException>(() => sut.ToString());
            
            Assert.Equal("Query Must Have a Select Part", argumentException.Message);
        }

        private static Mock<SelectGenerator> GetFakeSelect(string @select)
        {
            var fakeSelect = new Mock<SelectGenerator>(Builder.EmptyFakeQueryData);
            fakeSelect
                .Setup(generator => generator.ToString())
                .Returns(@select);

            return fakeSelect;
        }

        private static Mock<WhereGenerator> GetFakeWhere(string @where)
        {
            if (@where == null)
            {
                return null;
            }

            var fakeQueryData = Builder.GetQueryData(new IEnumerable<Clause>[0]);

            var fakeWhere = new Mock<WhereGenerator>(fakeQueryData);
            fakeWhere
                .Setup(generator => generator.ToString())
                .Returns(@where);

            return fakeWhere;
        }

        private static Mock<OrderByGenerator> GetFakeOrderBy(string orderBy)
        {
            if (orderBy == null)
            {
                return null;
            }
            
            var fakeOrderBy = new Mock<OrderByGenerator>(null);
            fakeOrderBy
                .Setup(generator => generator.ToString())
                .Returns(orderBy);

            return fakeOrderBy;
        }

        private static Mock<GroupByGenerator> GetFakeGroupBy(string @groupBy)
        {
            var fakeGroupBy = new Mock<GroupByGenerator>(Builder.EmptyFakeQueryData);
            fakeGroupBy
                .Setup(generator => generator.ToString())
                .Returns(@groupBy);

            return fakeGroupBy;
        }
    }
}