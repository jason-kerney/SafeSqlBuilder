using SafeSqlBuilder;
using SafeSqlBuilder.Generators;
using SafeSqlBuilder.Models;
using Xunit;

namespace SafeSqlBuilder.Tests
{
    public static partial class QueryDataObject
    {
        public class SortShould
        {
            private readonly Query _query;
            private readonly IValidColumns _validColumns;
            private readonly ClauseFactory _clauseFactory;
            
            public SortShould()
            {
                var (q, v, c) = Builder.InitializeBasicItemsForQueryData("some field");
                _query = q;
                _validColumns = v;
                _clauseFactory = c;
            }
            
            [Fact]
            public void GetSortColumn()
            {
                _query.Sort = "My_Sort";
                var sut = new QueryData(_query, _validColumns, _clauseFactory, "My Table");
                
                Assert.Equal("[My_Sort_Cleaned]", sut.GetSortColumn());
            }
            
            [Fact]
            public void GetDifferentSortColumn()
            {
                _query.Sort = "A_Column_To_Sort";
                var sut = new QueryData(_query, _validColumns, _clauseFactory, "My Table");
                
                Assert.Equal("[A_Column_To_Sort_Cleaned]", sut.GetSortColumn());
            }
            
            [Fact]
            public void GetSortDirectionIfDirectionIsNull()
            {
                _query.Direction = null;
                var sut = new QueryData(_query, _validColumns, _clauseFactory, "My Table");
                
                Assert.Equal("ASC", sut.GetSortDirection());
            }
            
            [Fact]
            public void GetSortDirectionIfDirectionIsEmpty()
            {
                _query.Direction = string.Empty;
                var sut = new QueryData(_query, _validColumns, _clauseFactory, "My Table");
                
                Assert.Equal("ASC", sut.GetSortDirection());
            }
            
            [Fact]
            public void GetSortDirectionIfDirectionIs_ASC()
            {
                _query.Direction = "ASC";
                var sut = new QueryData(_query, _validColumns, _clauseFactory, "My Table");
                
                Assert.Equal("ASC", sut.GetSortDirection());
            }
            
            [Fact]
            public void GetSortDirectionIfDirectionIs_asc()
            {
                _query.Direction = "asc";
                var sut = new QueryData(_query, _validColumns, _clauseFactory, "My Table");
                
                Assert.Equal("ASC", sut.GetSortDirection());
            }
            
            [Fact]
            public void GetSortDirectionIfDirectionIsInvalid()
            {
                _query.Direction = "HELLO";
                var sut = new QueryData(_query, _validColumns, _clauseFactory, "My Table");
                
                Assert.Equal("ASC", sut.GetSortDirection());
            }
            
            [Fact]
            public void GetSortDirectionIfDirectionIs_desc()
            {
                _query.Direction = "desc";
                var sut = new QueryData(_query, _validColumns, _clauseFactory, "My Table");
                
                Assert.Equal("DESC", sut.GetSortDirection());
            }
            
            [Fact]
            public void GetSortDirectionIfDirectionIs_DESC()
            {
                _query.Direction = "DESC";
                var sut = new QueryData(_query, _validColumns, _clauseFactory, "My Table");
                
                Assert.Equal("DESC", sut.GetSortDirection());
            }
        }
    }
}