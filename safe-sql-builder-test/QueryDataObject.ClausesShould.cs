using System.Collections.Generic;
using System.Linq;
using ApprovalTests;
using SafeSqlBuilder;
using SafeSqlBuilder.Generators;
using SafeSqlBuilder.Models;
using Moq;
using Xunit;

namespace SafeSqlBuilder.Tests
{
    public static partial class QueryDataObject
    {
        public class ClausesShould
        {
            // ReSharper disable once InconsistentNaming
            private readonly object IGNORE_ME = "ignore me";

            [Fact]
            public void HaveSingleItemForSimpleFilter()
            {
                const string propName = "PropName";
                var filters = new[]
                {
                    new Filter
                    {
                        Property = propName,
                        includeNulls = true
                    },
                };
                
                var (query, fakeValidColumns, _) = Builder.InitializeItemsForQueryData(new[] {"Field"}, filters: filters);
                var sut = new QueryData(query, fakeValidColumns, new ClauseFactory(fakeValidColumns), "My_Table");

                Assert.Single(sut.Clauses);
                Assert.Single(sut.Clauses.First());
                Assert.Equal(new NullClause($"{propName}_Cleaned").ToString(), sut.Clauses.First().First().ToString());
            }

            [Fact]
            public void ShouldUseClauseFactoryToReturnClauses()
            {
                var expectedFilter1 = new Filter {Property = "Filter1"};
                var expectedFilter2 = new Filter {Property = "Filter2"};
                var expectedFilter3 = new Filter {Property = "Filter3"};

                var fields = new[] {"My_Field"};
                var filters = new[]
                {
                    expectedFilter1,
                    expectedFilter2,
                    expectedFilter3,
                };
                IEnumerable<Clause> ClauseBuilder(Filter f) => new Clause[]
                {
                    new NullClause(f.Property), 
                    new EqualClause(f.Property, IGNORE_ME)
                };
                
                var (query, fakeValidColumns, mockClauseFactory) = Builder.InitializeFakeItemsForQueryData(fields, filters: filters, clauseBuilder: ClauseBuilder);
                var sut = new QueryData(query, fakeValidColumns.Object, mockClauseFactory.Object, "My_Table");

                var actualClauses = sut.Clauses;
                
                Approvals.VerifyAll(actualClauses, clauses => string.Join(", ", clauses.Select(c => $"\"{c}\"")));

                mockClauseFactory
                    .Verify(factory => factory.Build(
                        It.Is<Filter>(f => ReferenceEquals(f, expectedFilter1)))
                    );

                mockClauseFactory
                    .Verify(factory => factory.Build(
                        It.Is<Filter>(f => ReferenceEquals(f, expectedFilter2)))
                    );

                mockClauseFactory
                    .Verify(factory => factory.Build(
                        It.Is<Filter>(f => ReferenceEquals(f, expectedFilter3)))
                    );
            }

            [Fact]
            public void ShouldHandleNullFilters()
            {
                var strings = new[] {"My_Field"};
                IEnumerable<Clause> ClauseBuilder(Filter f) => new Clause[] {new NullClause(f.Property), new EqualClause(f.Property, IGNORE_ME)};

                var (query, fakeValidColumns, fakeClauseFactory) = Builder.InitializeItemsForQueryData(strings, clauseBuilder: ClauseBuilder);

                var sut = new QueryData(query, fakeValidColumns, fakeClauseFactory, "My_Table");

                Assert.Empty(sut.Clauses);
            }
        }
    }
}