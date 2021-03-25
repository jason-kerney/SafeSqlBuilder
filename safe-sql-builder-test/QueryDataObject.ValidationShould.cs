using System.Linq;
using SafeSqlBuilder;
using SafeSqlBuilder.Models;
using Xunit;

namespace SafeSqlBuilder.Tests
{
    public static partial class QueryDataObject
    {
        public class ValidationShould
        {
            [Fact]
            public void InvalidFilterPropertyNameIndicatesInvalidData()
            {
                const string expectedColumnName = "Valid";
                bool ValidateColumn(string s) => s == expectedColumnName;

                var fields = new [] {expectedColumnName};
                var filters = new []
                {
                    new Filter // Badly named filter
                    {
                        Property = "Not_Valid",
                        Values = new dynamic[] { 1 }
                    }, 
                };

                var (query, fakeValidColumns, fakeClauseFactory) = Builder.InitializeFakeItemsForQueryData(fields, filters, columnIsInvalid: ValidateColumn);
                var sut = new QueryData(query, fakeValidColumns.Object, fakeClauseFactory.Object, "MY_TABLE");

                Assert.False(sut.IsValid);
                Assert.True(sut.IsInvalid);
            }
            
            [Fact]
            public void InvalidFilterPropertyNameInSecondFilterIndicatesInvalidData()
            {
                const string expectedColumnName = "Valid";
                var fields = new [] {expectedColumnName}; // Correctly named field
                bool ColumnValidator(string s) => s == expectedColumnName;

                var filters = new []
                {
                    new Filter
                    {
                        Property = expectedColumnName,
                        Values = new dynamic[] { "Here" }
                    }, 
                    new Filter // Badly named filter
                    {
                        Property = "Not_Valid",
                        Values = new dynamic[] { 1 }
                    }, 
                };

                var (query, fakeValidColumns, fakeClauseFactory) = Builder.InitializeFakeItemsForQueryData(fields, filters, columnIsInvalid: ColumnValidator);
                var sut = new QueryData(query, fakeValidColumns.Object, fakeClauseFactory.Object, "MY_TABLE");

                Assert.False(sut.IsValid);
                Assert.True(sut.IsInvalid);
            }
            
            [Fact]
            public void DetectEmptyFilterAsValid()
            {
                var fields = new [] {"Valid"}; // Correctly named field
                var filters = new Filter[0];

                var (query, fakeValidColumns, fakeClauseFactory) = Builder.InitializeItemsForQueryData(fields, filters);
                var sut = new QueryData(query, fakeValidColumns, fakeClauseFactory, "MY_TABLE");

                Assert.True(sut.IsValid);
                Assert.False(sut.IsInvalid);
            }

            [Fact]
            public void DetectSecondInvalidFilterAsInvalid()
            {
                const string expectedField = "Field";
                var fields = new [] {expectedField};

                var filters = new []
                {
                    new Filter
                    {
                        Property = expectedField,
                        Values = new dynamic[] {1},
                    }, 
                    new Filter
                    {
                        Property = "Field2",
                        Values = null,
                    }, 
                };
                
                var (query, fakeValidColumns, fakeClauseFactory) = Builder.InitializeFakeItemsForQueryData(fields, filters, columnIsInvalid: Builder.BuildValidator(expectedField));
                var sut = new QueryData(query, fakeValidColumns.Object, fakeClauseFactory.Object, "My_Table");
                
                Assert.False(sut.IsValid);
                Assert.True(sut.IsInvalid);
            }

            [Fact]
            public void DetectInvalidFilterAsInvalid()
            {
                const string expectedField = "Field";
                var fields = new [] {expectedField};
                

                var filters = new []
                {
                    new Filter
                    {
                        Property = "Field2",
                        Values = null,
                    }, 
                };

                var (query, fakeValidColumns, fakeClauseFactory) = Builder.InitializeFakeItemsForQueryData(fields, filters, columnIsInvalid: Builder.BuildValidator(expectedField));
                var sut = new QueryData(query, fakeValidColumns.Object, fakeClauseFactory.Object, "My_Table");
                
                Assert.False(sut.IsValid);
                Assert.True(sut.IsInvalid);
            }

            [Fact]
            public void ContainsValidFilters_IsTrueIfFiltersIsNull()
            {
                const string expectedColumnName = "Valid";
                bool ValidateColumn(string s) => s == expectedColumnName;

                var fields = new [] {expectedColumnName};

                var (query, fakeValidColumns, fakeClauseFactory) = Builder.InitializeFakeItemsForQueryData(fields, null, columnIsInvalid: ValidateColumn);
                var sut = new QueryData(query, fakeValidColumns.Object, fakeClauseFactory.Object, "MY_TABLE");
                
                Assert.True(sut.ContainsValidFilters());
            }

            [Fact]
            public void ContainsValidFilters_IsTrueIfFiltersIsEmpty()
            {
                const string expectedColumnName = "Valid";
                bool ValidateColumn(string s) => s == expectedColumnName;

                var fields = new [] {expectedColumnName};

                var (query, fakeValidColumns, fakeClauseFactory) = Builder.InitializeFakeItemsForQueryData(fields, new Filter[0], columnIsInvalid: ValidateColumn);
                var sut = new QueryData(query, fakeValidColumns.Object, fakeClauseFactory.Object, "MY_TABLE");
                
                Assert.True(sut.ContainsValidFilters());
            }

            [Fact]
            public void ContainsValidFilters_IsTrueIfAllFiltersAreValid()
            {
                const string expectedColumnName = "Valid";
                bool ValidateColumn(string s) => false;

                var fields = new [] {expectedColumnName};

                var filters = new []
                {
                    new Filter
                    {
                        Property = expectedColumnName,
                        Values = new object[] {1, 3, 5}
                    }, 
                    new Filter
                    {
                        Property = expectedColumnName,
                        Range = new FilterRange
                        {
                            Start = 0,
                            RangeOperator = RangeOperator.GreaterThan
                        }
                    }, 
                };
                var (query, fakeValidColumns, fakeClauseFactory) = Builder.InitializeFakeItemsForQueryData(fields, filters, columnIsInvalid: ValidateColumn);
                var sut = new QueryData(query, fakeValidColumns.Object, fakeClauseFactory.Object, "MY_TABLE");
                
                Assert.True(sut.ContainsValidFilters());
            }

            [Fact]
            public void ContainsValidFilters_IsFalseIfAllFirstFilterPropertyIsNotValid()
            {
                const string expectedColumnName = "Valid";
                bool ValidateColumn(string s) => s != expectedColumnName;

                var fields = new [] {expectedColumnName};

                var filters = new []
                {
                    new Filter
                    {
                        Property = "Other Name",
                        Values = new object[] {1, 3, 5}
                    }, 
                    new Filter
                    {
                        Property = expectedColumnName,
                        Range = new FilterRange
                        {
                            Start = 0,
                            RangeOperator = RangeOperator.GreaterThan
                        }
                    }, 
                };
                var (query, fakeValidColumns, fakeClauseFactory) = Builder.InitializeFakeItemsForQueryData(fields, filters, columnIsInvalid: ValidateColumn);
                var sut = new QueryData(query, fakeValidColumns.Object, fakeClauseFactory.Object, "MY_TABLE");
                
                Assert.False(sut.ContainsValidFilters());
            }

            [Fact]
            public void ContainsValidFilters_IsFalseIfSecondFilterIsNotValid()
            {
                const string expectedColumnName = "Valid";
                bool ValidateColumn(string s) => s != expectedColumnName;

                var fields = new [] {expectedColumnName};

                var filters = new []
                {
                    new Filter
                    {
                        Property = expectedColumnName,
                        Range = new FilterRange
                        {
                            RangeOperator = RangeOperator.GreaterThan
                        }
                    }, 
                    new Filter
                    {
                    Property = expectedColumnName,
                    Values = new object[] {1, 3, 5}
                    }, 
                };
                var (query, fakeValidColumns, fakeClauseFactory) = Builder.InitializeFakeItemsForQueryData(fields, filters, columnIsInvalid: ValidateColumn);
                var sut = new QueryData(query, fakeValidColumns.Object, fakeClauseFactory.Object, "MY_TABLE");
                
                Assert.False(sut.ContainsValidFilters());
            }
            
            [Fact]
            public void ContainsValidFilters_IsFalseIfFirstFilterIsNotValid()
            {
                const string expectedColumnName = "Valid";
                bool ValidateColumn(string s) => s != expectedColumnName;

                var fields = new [] {expectedColumnName};

                var filters = new []
                {
                    new Filter
                    {
                        Property = expectedColumnName,
                        Values = new object[] {1, 3, 5}
                    }, 
                    new Filter
                    {
                        Property = expectedColumnName,
                        Range = new FilterRange
                        {
                            RangeOperator = RangeOperator.GreaterThan
                        }
                    }, 
                };
                var (query, fakeValidColumns, fakeClauseFactory) = Builder.InitializeFakeItemsForQueryData(fields, filters, columnIsInvalid: ValidateColumn);
                var sut = new QueryData(query, fakeValidColumns.Object, fakeClauseFactory.Object, "MY_TABLE");
                
                Assert.False(sut.ContainsValidFilters());
            }

            [Fact]
            public void ContainsValidFilters_IsFalseIfSecondFilterNameIsNotValid()
            {
                const string expectedColumnName = "Valid";
                bool ValidateColumn(string s) => s != expectedColumnName;

                var fields = new [] {expectedColumnName};

                var filters = new []
                {
                    new Filter
                    {
                        Property = expectedColumnName,
                        Values = new object[] {1, 3, 5}
                    }, 
                    new Filter
                    {
                        Property = "Bad Name (Mitten)",
                        Range = new FilterRange
                        {
                            Start = 0,
                            RangeOperator = RangeOperator.GreaterThan
                        }
                    }, 
                };
                var (query, fakeValidColumns, fakeClauseFactory) = Builder.InitializeFakeItemsForQueryData(fields, filters, columnIsInvalid: ValidateColumn);
                var sut = new QueryData(query, fakeValidColumns.Object, fakeClauseFactory.Object, "MY_TABLE");
                
                Assert.False(sut.ContainsValidFilters());
            }

            [Fact]
            public void ContainsValidGroupByReturns_TrueIfGroupByNull()
            {
                const string expectedColumnName = "Valid";
                bool ValidateColumn(string s) => false;

                var fields = new [] {expectedColumnName};

                var (query, fakeValidColumns, fakeClauseFactory) = Builder.InitializeFakeItemsForQueryData(fields, columnIsInvalid: ValidateColumn, groupBy: null);
                var sut = new QueryData(query, fakeValidColumns.Object, fakeClauseFactory.Object, "MY_TABLE");
                
                Assert.True(sut.ContainsValidGroupBy());
            }

            [Fact]
            public void ContainsValidGroupByReturns_TrueIfGroupByEmpty()
            {
                const string expectedColumnName = "Valid";
                bool ValidateColumn(string s) => false;

                var fields = new [] {expectedColumnName};

                var (query, fakeValidColumns, fakeClauseFactory) = Builder.InitializeFakeItemsForQueryData(fields, columnIsInvalid: ValidateColumn, groupBy: new string[0]);
                var sut = new QueryData(query, fakeValidColumns.Object, fakeClauseFactory.Object, "MY_TABLE");
                
                Assert.True(sut.ContainsValidGroupBy());
            }

            [Fact]
            public void ContainsValidGroupByReturns_TrueIfGroupContainsOnlyValidFields()
            {
                bool ValidateColumn(string s) => false;

                var fields = new [] { "Second Valid", "Another Valid"};

                var groupBy = fields.Select(a => a).ToArray();
                
                var (query, fakeValidColumns, fakeClauseFactory) = Builder.InitializeFakeItemsForQueryData(fields, columnIsInvalid: ValidateColumn, groupBy: groupBy);
                var sut = new QueryData(query, fakeValidColumns.Object, fakeClauseFactory.Object, "MY_TABLE");
                
                Assert.True(sut.ContainsValidGroupBy());
            }

            [Fact]
            public void ContainsValidGroupByReturns_TrueIfGroupContainsOnlyValidFieldsOutOfOrder()
            {
                bool ValidateColumn(string s) => false;

                var fields = new [] { "Second Valid", "Another Valid"};

                var groupBy = fields.Select(a => a).OrderBy(a => a).ToArray();
                
                var (query, fakeValidColumns, fakeClauseFactory) = Builder.InitializeFakeItemsForQueryData(fields, columnIsInvalid: ValidateColumn, groupBy: groupBy);
                var sut = new QueryData(query, fakeValidColumns.Object, fakeClauseFactory.Object, "MY_TABLE");
                
                Assert.True(sut.ContainsValidGroupBy());
            }

            [Fact]
            public void ContainsValidGroupByReturns_TrueIfGroupContainsOnlyValidFieldsOutOfOrder_DistinctTrue()
            {
                bool ValidateColumn(string s) => false;

                var fields = new [] { "Second Valid", "Another Valid"};

                var groupBy = fields.Select(a => a).OrderBy(a => a).ToArray();
                
                var (query, fakeValidColumns, fakeClauseFactory) = Builder.InitializeFakeItemsForQueryData(fields, columnIsInvalid: ValidateColumn, groupBy: groupBy);
                query.Distinct = true;
                var sut = new QueryData(query, fakeValidColumns.Object, fakeClauseFactory.Object, "MY_TABLE");
                
                Assert.True(sut.ContainsValidGroupBy());
            }

            [Fact]
            public void ContainsValidGroupByReturns_TrueIfGroupContainsOnlyValidFieldsOutOfOrder_DistinctFalse()
            {
                bool ValidateColumn(string s) => false;

                var fields = new [] { "Second Valid", "Another Valid"};

                var groupBy = fields.Select(a => a).OrderBy(a => a).ToArray();
                
                var (query, fakeValidColumns, fakeClauseFactory) = Builder.InitializeFakeItemsForQueryData(fields, columnIsInvalid: ValidateColumn, groupBy: groupBy);
                query.Distinct = false;
                var sut = new QueryData(query, fakeValidColumns.Object, fakeClauseFactory.Object, "MY_TABLE");
                
                Assert.True(sut.ContainsValidGroupBy());
            }

            [Fact]
            public void ContainsValidGroupByReturns_TrueIfGroupContainsOnlyValidFieldsOutOfOrder_DistinctNull()
            {
                bool ValidateColumn(string s) => false;

                var fields = new [] { "Second Valid", "Another Valid"};

                var groupBy = fields.Select(a => a).OrderBy(a => a).ToArray();
                
                var (query, fakeValidColumns, fakeClauseFactory) = Builder.InitializeFakeItemsForQueryData(fields, columnIsInvalid: ValidateColumn, groupBy: groupBy);
                query.Distinct = null;
                var sut = new QueryData(query, fakeValidColumns.Object, fakeClauseFactory.Object, "MY_TABLE");
                
                Assert.True(sut.ContainsValidGroupBy());
            }

            [Fact]
            public void ContainsValidGroupByReturns_FalseIfGroupContainsAnyInvalidName()
            {
                const string goodName = "Valid";
                bool ValidateColumn(string s) => s != goodName;

                var fields = new [] {goodName, "Another Valid"};

                var groupBy = new [] { goodName, "Another Valid"};
                
                var (query, fakeValidColumns, fakeClauseFactory) = Builder.InitializeFakeItemsForQueryData(fields, columnIsInvalid: ValidateColumn, groupBy: groupBy);
                var sut = new QueryData(query, fakeValidColumns.Object, fakeClauseFactory.Object, "MY_TABLE");
                
                Assert.False(sut.ContainsValidGroupBy());
            }

            [Fact]
            public void ContainsValidGroupByReturns_FalseIfGroupContainsAnyInvalidName_AndFieldsContainsOnlyValidNames()
            {
                const string goodName = "Valid";
                bool ValidateColumn(string s) => s != goodName;

                var fields = new [] {goodName};

                var groupBy = new [] { goodName, "Another Valid"};
                
                var (query, fakeValidColumns, fakeClauseFactory) = Builder.InitializeFakeItemsForQueryData(fields, columnIsInvalid: ValidateColumn, groupBy: groupBy);
                var sut = new QueryData(query, fakeValidColumns.Object, fakeClauseFactory.Object, "MY_TABLE");
                
                Assert.False(sut.ContainsValidGroupBy());
            }

            [Fact]
            public void ContainsValidGroupByReturns_FalseIfGroupContainsMoreThenFieldsAndDistinctIsTrue()
            {
                bool ValidateColumn(string s) => false;

                var fields = new [] {"Valid"};

                var groupBy = new [] { "Valid", "Another Valid"};
                
                var (query, fakeValidColumns, fakeClauseFactory) = Builder.InitializeFakeItemsForQueryData(fields, columnIsInvalid: ValidateColumn, groupBy: groupBy);
                query.Distinct = true;
                var sut = new QueryData(query, fakeValidColumns.Object, fakeClauseFactory.Object, "MY_TABLE");
                
                Assert.False(sut.ContainsValidGroupBy());
            }

            [Fact]
            public void ContainsValidGroupByReturns_FalseIfGroupContainsMoreThenFieldsAndDistinctIsFalse()
            {
                bool ValidateColumn(string s) => false;

                var fields = new [] {"Valid"};

                var groupBy = new [] { "Valid", "Another Valid"};
                
                var (query, fakeValidColumns, fakeClauseFactory) = Builder.InitializeFakeItemsForQueryData(fields, columnIsInvalid: ValidateColumn, groupBy: groupBy);
                query.Distinct = false;
                var sut = new QueryData(query, fakeValidColumns.Object, fakeClauseFactory.Object, "MY_TABLE");
                
                Assert.False(sut.ContainsValidGroupBy());
            }

            [Fact]
            public void ContainsValidGroupByReturns_FalseIfGroupContainsMoreThenFieldsAndDistinctIsNull()
            {
                bool ValidateColumn(string s) => false;

                var fields = new [] {"Valid"};

                var groupBy = new [] { "Valid", "Another Valid"};
                
                var (query, fakeValidColumns, fakeClauseFactory) = Builder.InitializeFakeItemsForQueryData(fields, columnIsInvalid: ValidateColumn, groupBy: groupBy);
                query.Distinct = null;
                var sut = new QueryData(query, fakeValidColumns.Object, fakeClauseFactory.Object, "MY_TABLE");
                
                Assert.False(sut.ContainsValidGroupBy());
            }
        }
    }
}