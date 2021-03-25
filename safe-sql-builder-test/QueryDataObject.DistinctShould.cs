using SafeSqlBuilder;
using SafeSqlBuilder.Generators;
using SafeSqlBuilder.Models;
using Xunit;

namespace SafeSqlBuilder.Tests
{
    public static partial class QueryDataObject
    {
        public class DistinctShould
        {
            private readonly Query _query;
            private readonly IValidColumns _validColumns;
            private readonly ClauseFactory _clauseFactory;
            
            public DistinctShould()
            {
                var (q, v, c) = Builder.InitializeBasicItemsForQueryData("some field");
                _query = q;
                _validColumns = v;
                _clauseFactory = c;
            }

            [Fact]
            public void BeFalseIfDistinctIsNull()
            {
                _query.Distinct = null;
                var sut = new QueryData(_query, _validColumns, _clauseFactory, "TableName");
                
                Assert.False(sut.IsDistinctValues);
                Assert.True(sut.IsNotDistinctValues);
            }
            
            [Fact]
            public void BeFalseIfDistinctIsFalse()
            {
                _query.Distinct = false;
                var sut = new QueryData(_query, _validColumns, _clauseFactory, "TableName");
                
                Assert.False(sut.IsDistinctValues);
                Assert.True(sut.IsNotDistinctValues);
            }
            
            [Fact]
            public void BeTrueIfDistinctIsTrue()
            {
                _query.Distinct = true;
                var sut = new QueryData(_query, _validColumns, _clauseFactory, "TableName");
                
                Assert.True(sut.IsDistinctValues);
                Assert.False(sut.IsNotDistinctValues);
            }
        }
    }
}