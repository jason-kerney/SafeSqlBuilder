using System;
using System.Collections.Generic;
using System.Linq;
using SafeSqlBuilder;
using SafeSqlBuilder.Generators;
using SafeSqlBuilder.Models;
using Xunit;
// ReSharper disable InconsistentNaming

namespace SafeSqlBuilder.Tests.Generators
{
    public class ClauseFactoryShould
    {
        private static readonly object IGNORE_ME = "ignore me";

        private readonly IValidColumns _fakeValidColumns;

        public ClauseFactoryShould()
        {
            _fakeValidColumns = Builder.BuildFakeValidColumns().Object;
        }
        
        [Theory]
        [MemberData(nameof(GetClauseFactoryTestData))]
        public void BuildCorrectSingleClause(string expectedClause, Filter filter)
        {
            var clauses = new ClauseFactory(_fakeValidColumns).Build(filter);

            // ReSharper disable once PossibleMultipleEnumeration
            Assert.Single(clauses);
            // ReSharper disable once PossibleMultipleEnumeration
            Assert.Equal(expectedClause, clauses.First().ToString());
        }

        public static IEnumerable<object[]> GetClauseFactoryTestData =>
        new List<object[]>
        {
            new object[] { "[PropertyName_Cleaned] = @PropertyName_Cleaned_EQUAL", new Filter { Property = "PropertyName", Values = new object[] { 1 } }  },
            new object[] { "[YourCoolName_Cleaned] = @YourCoolName_Cleaned_EQUAL", new Filter { Property = "YourCoolName", Values = new object[] { 1 } }  },
            
            new object[] { "[YourCoolName_Cleaned] IN @YourCoolName_Cleaned_IN", new Filter { Property = "YourCoolName", Values = new object[] { "Your", "Momma" } }  },
            new object[] { "[CoolName_Cleaned] IN @CoolName_Cleaned_IN", new Filter { Property = "CoolName", Values = new object[] { "Your", "Momma" } }  },
            
            new object[] { "[YourOtherCoolName_Cleaned] > @YourOtherCoolName_Cleaned_GREATER", new Filter { Property = "YourOtherCoolName", Range = new FilterRange { Start = "Hello", RangeOperator = RangeOperator.GreaterThan } }  },
            new object[] { "[CoolName_Cleaned] > @CoolName_Cleaned_GREATER", new Filter { Property = "CoolName", Range = new FilterRange { Start = "Hello", RangeOperator = RangeOperator.GreaterThan } }  },
            
            new object[] { "[AName_Cleaned] < @AName_Cleaned_LESS", new Filter { Property = "AName", Range = new FilterRange { Start = "Hello", RangeOperator = RangeOperator.LessThan } }  },
            new object[] { "[ASecondName_Cleaned] < @ASecondName_Cleaned_LESS", new Filter { Property = "ASecondName", Range = new FilterRange { Start = "Hello", RangeOperator = RangeOperator.LessThan } }  },
            
            new object[] { "[YourOtherCoolName_Cleaned] >= @YourOtherCoolName_Cleaned_GREATER_EQUAL", new Filter { Property = "YourOtherCoolName", Range = new FilterRange { Start = "Hello", RangeOperator = RangeOperator.GreaterThanEqual } }  },
            new object[] { "[CoolName_Cleaned] >= @CoolName_Cleaned_GREATER_EQUAL", new Filter { Property = "CoolName", Range = new FilterRange { Start = "Hello", RangeOperator = RangeOperator.GreaterThanEqual } }  },
            
            new object[] { "[AName_Cleaned] <= @AName_Cleaned_LESS_EQUAL", new Filter { Property = "AName", Range = new FilterRange { Start = "Hello", RangeOperator = RangeOperator.LessThanEqual } }  },
            new object[] { "[ASecondName_Cleaned] <= @ASecondName_Cleaned_LESS_EQUAL", new Filter { Property = "ASecondName", Range = new FilterRange { Start = "Hello", RangeOperator = RangeOperator.LessThanEqual } }  },
            
            new object[] { "[Boo_Cleaned] BETWEEN @Boo_Cleaned_START AND @Boo_Cleaned_END", new Filter { Property = "Boo", Range = new FilterRange { Start = "Hello", End = "Good Bye", RangeOperator = RangeOperator.Between } }  },
            new object[] { "[BooWho_Cleaned] BETWEEN @BooWho_Cleaned_START AND @BooWho_Cleaned_END", new Filter { Property = "BooWho", Range = new FilterRange { Start = "Hello", End = "Good Bye", RangeOperator = RangeOperator.Between } }  },
            
            new object[] { "[Boo_Cleaned] IS NULL", new Filter { Property = "Boo", includeNulls = true}  },
            new object[] { "[BooWho_Cleaned] IS NULL", new Filter { Property = "BooWho", includeNulls = true}  },
            
            new object[] { "[Boo_Cleaned] IS NOT NULL", new Filter { Property = "Boo", includeNulls = false}  },
            new object[] { "[BooWho_Cleaned] IS NOT NULL", new Filter { Property = "BooWho", includeNulls = false}  },
        };
        
        [Theory]
        [MemberData(nameof(GetMultipleClauseFactoryTestData))]
        public void BuildCorrectMultipleClause(IEnumerable<Clause> expectedClauses, Filter filter)
        {
            var clauses = new ClauseFactory(_fakeValidColumns).Build(filter);

            var expectedClausesHash = new HashSet<string>(expectedClauses.Select(c => c.ToString()));
            expectedClausesHash.ExceptWith(clauses.Select(c => c.ToString()));
            
            Assert.Empty(expectedClausesHash);
        }

        public static IEnumerable<object[]> GetMultipleClauseFactoryTestData
        {
            get
            {
                const string propName = "ColumnName";
                var cleanPropName = Builder.Clean(propName);

                return new List<object[]>
                {
                    // Null
                    new object[] {new Clause[] {new NullClause(cleanPropName)}, new Filter {Property = propName, Values = null, includeNulls = true}},
                    new object[] {new Clause[] {new NullClause(cleanPropName)}, new Filter {Property = propName, Values = null, includeNulls = true}},

                    // Not Null
                    new object[] {new Clause[] {new NotNullClause(cleanPropName)}, new Filter {Property = propName, Values = null, includeNulls = false}},
                    new object[] {new Clause[] {new NotNullClause(cleanPropName)}, new Filter {Property = propName, Values = null, includeNulls = false}},
                    
                    // values and Null
                    new object[] {new Clause[] {new EqualClause(cleanPropName, IGNORE_ME), new NullClause(cleanPropName)}, new Filter {Property = propName, Values = new object[] {1}, includeNulls = true}},
                    new object[] {new Clause[] {new InClause(cleanPropName, IGNORE_ME), new NullClause(cleanPropName)}, new Filter {Property = propName, Values = new object[] {1, 3}, includeNulls = true}},
                    
                    // Values and Not Null
                    // Throws exception so not valid for this test
                    
                    // Ranges and Null
                    new object[] {new Clause[] {new GreaterThanClause(cleanPropName, IGNORE_ME), new NullClause(cleanPropName)}, new Filter {Property = propName, Range = new FilterRange{ End = "Bye", RangeOperator = RangeOperator.GreaterThan}, includeNulls = true}},
                    new object[] {new Clause[] {new LessThanClause(cleanPropName, IGNORE_ME), new NullClause(cleanPropName)}, new Filter {Property = propName, Range = new FilterRange{ End = "Bye", RangeOperator = RangeOperator.LessThan}, includeNulls = true}},
                    new object[] {new Clause[] {new GreaterThanEqualClause(cleanPropName, IGNORE_ME), new NullClause(cleanPropName)}, new Filter {Property = propName, Range = new FilterRange{ End = "Bye", RangeOperator = RangeOperator.GreaterThanEqual}, includeNulls = true}},
                    new object[] {new Clause[] {new LessThanEqualClause(cleanPropName, IGNORE_ME), new NullClause(cleanPropName)}, new Filter {Property = propName, Range = new FilterRange{ End = "Bye", RangeOperator = RangeOperator.LessThanEqual}, includeNulls = true}},
                    new object[] {new Clause[] {new BetweenClause(cleanPropName, IGNORE_ME, IGNORE_ME), new NullClause(cleanPropName)}, new Filter {Property = propName, Range = new FilterRange{ Start = "Hello", End = "Bye", RangeOperator = RangeOperator.Between}, includeNulls = true}},
                    
                    // Ranges and Not Null
                    new object[] {new Clause[] {new GreaterThanClause(cleanPropName, IGNORE_ME), new NotNullClause(cleanPropName)}, new Filter {Property = propName, Range = new FilterRange{ End = "Bye", RangeOperator = RangeOperator.GreaterThan}, includeNulls = false}},
                    new object[] {new Clause[] {new LessThanClause(cleanPropName, IGNORE_ME), new NotNullClause(cleanPropName)}, new Filter {Property = propName, Range = new FilterRange{ End = "Bye", RangeOperator = RangeOperator.LessThan}, includeNulls = false}},
                    new object[] {new Clause[] {new GreaterThanEqualClause(cleanPropName, IGNORE_ME), new NotNullClause(cleanPropName)}, new Filter {Property = propName, Range = new FilterRange{ End = "Bye", RangeOperator = RangeOperator.GreaterThanEqual}, includeNulls = false}},
                    new object[] {new Clause[] {new LessThanEqualClause(cleanPropName, IGNORE_ME), new NotNullClause(cleanPropName)}, new Filter {Property = propName, Range = new FilterRange{ End = "Bye", RangeOperator = RangeOperator.LessThanEqual}, includeNulls = false}},
                    new object[] {new Clause[] {new BetweenClause(cleanPropName, IGNORE_ME, IGNORE_ME), new NotNullClause(cleanPropName)}, new Filter {Property = propName, Range = new FilterRange{ Start = "Hello", End = "Bye", RangeOperator = RangeOperator.Between}, includeNulls = false}},
                    
                    //Ranges and Equal
                    new object[] {new Clause[] {new GreaterThanClause(cleanPropName, IGNORE_ME), new EqualClause(cleanPropName, IGNORE_ME)}, new Filter {Property = propName, Values = new object[] { 1 }, Range = new FilterRange{ End = "Bye", RangeOperator = RangeOperator.GreaterThan}}},
                    new object[] {new Clause[] {new LessThanClause(cleanPropName, IGNORE_ME), new EqualClause(cleanPropName, IGNORE_ME)}, new Filter {Property = propName, Values = new object[] { 1 },Range = new FilterRange{ End = "Bye", RangeOperator = RangeOperator.LessThan}}},
                    new object[] {new Clause[] {new GreaterThanEqualClause(cleanPropName, IGNORE_ME), new EqualClause(cleanPropName, IGNORE_ME)}, new Filter {Property = propName, Values = new object[] { 1 },Range = new FilterRange{ End = "Bye", RangeOperator = RangeOperator.GreaterThanEqual}}},
                    new object[] {new Clause[] {new LessThanEqualClause(cleanPropName, IGNORE_ME), new EqualClause(cleanPropName, IGNORE_ME)}, new Filter {Property = propName, Values = new object[] { 1 },Range = new FilterRange{ End = "Bye", RangeOperator = RangeOperator.LessThanEqual}}},
                    new object[] {new Clause[] {new BetweenClause(cleanPropName, IGNORE_ME, IGNORE_ME), new EqualClause(cleanPropName, IGNORE_ME)}, new Filter {Property = propName, Values = new object[] { 1 },Range = new FilterRange{ Start = "Hello", End = "Bye", RangeOperator = RangeOperator.Between}}},
                    
                    //Ranges and In
                    new object[] {new Clause[] {new GreaterThanClause(cleanPropName, IGNORE_ME), new InClause(cleanPropName, IGNORE_ME)}, new Filter {Property = propName, Values = new object[] { 1, 2 }, Range = new FilterRange{ End = "Bye", RangeOperator = RangeOperator.GreaterThan}}},
                    new object[] {new Clause[] {new LessThanClause(cleanPropName, IGNORE_ME), new InClause(cleanPropName, IGNORE_ME)}, new Filter {Property = propName, Values = new object[] { 1, 2 },Range = new FilterRange{ End = "Bye", RangeOperator = RangeOperator.LessThan}}},
                    new object[] {new Clause[] {new GreaterThanEqualClause(cleanPropName, IGNORE_ME), new InClause(cleanPropName, IGNORE_ME)}, new Filter {Property = propName, Values = new object[] { 1, 2 },Range = new FilterRange{ End = "Bye", RangeOperator = RangeOperator.GreaterThanEqual}}},
                    new object[] {new Clause[] {new LessThanEqualClause(cleanPropName, IGNORE_ME), new InClause(cleanPropName, IGNORE_ME)}, new Filter {Property = propName, Values = new object[] { 1, 2 },Range = new FilterRange{ End = "Bye", RangeOperator = RangeOperator.LessThanEqual}}},
                    new object[] {new Clause[] {new BetweenClause(cleanPropName, IGNORE_ME, IGNORE_ME), new InClause(cleanPropName, IGNORE_ME)}, new Filter {Property = propName, Values = new object[] { 1, 2 },Range = new FilterRange{ Start = "Hello", End = "Bye", RangeOperator = RangeOperator.Between}}},
                    
                    //Ranges, Equal and Null
                    new object[] {new Clause[] {new GreaterThanClause(cleanPropName, IGNORE_ME), new EqualClause(cleanPropName, IGNORE_ME), new NullClause(cleanPropName)}, new Filter {Property = propName, Values = new object[] { 1 }, Range = new FilterRange{ End = "Bye", RangeOperator = RangeOperator.GreaterThan}, includeNulls = true}},
                    new object[] {new Clause[] {new LessThanClause(cleanPropName, IGNORE_ME), new EqualClause(cleanPropName, IGNORE_ME), new NullClause(cleanPropName)}, new Filter {Property = propName, Values = new object[] { 1 },Range = new FilterRange{ End = "Bye", RangeOperator = RangeOperator.LessThan}, includeNulls = true}},
                    new object[] {new Clause[] {new GreaterThanEqualClause(cleanPropName, IGNORE_ME), new EqualClause(cleanPropName, IGNORE_ME), new NullClause(cleanPropName)}, new Filter {Property = propName, Values = new object[] { 1 },Range = new FilterRange{ End = "Bye", RangeOperator = RangeOperator.GreaterThanEqual}, includeNulls = true}},
                    new object[] {new Clause[] {new LessThanEqualClause(cleanPropName, IGNORE_ME), new EqualClause(cleanPropName, IGNORE_ME), new NullClause(cleanPropName)}, new Filter {Property = propName, Values = new object[] { 1 },Range = new FilterRange{ End = "Bye", RangeOperator = RangeOperator.LessThanEqual}, includeNulls = true}},
                    new object[] {new Clause[] {new BetweenClause(cleanPropName, IGNORE_ME, IGNORE_ME), new EqualClause(cleanPropName, IGNORE_ME), new NullClause(cleanPropName)}, new Filter {Property = propName, Values = new object[] { 1 },Range = new FilterRange{ Start = "Hello", End = "Bye", RangeOperator = RangeOperator.Between}, includeNulls = true}},
                    
                    //Ranges, Equal and Not Null
                    //Throws exception not valid for this test
                    
                    //Ranges, IN and Null
                    new object[] {new Clause[] {new GreaterThanClause(cleanPropName, IGNORE_ME), new InClause(cleanPropName, IGNORE_ME), new NullClause(cleanPropName)}, new Filter {Property = propName, Values = new object[] { 1, 2 }, Range = new FilterRange{ End = "Bye", RangeOperator = RangeOperator.GreaterThan}, includeNulls = true}},
                    new object[] {new Clause[] {new LessThanClause(cleanPropName, IGNORE_ME), new InClause(cleanPropName, IGNORE_ME), new NullClause(cleanPropName)}, new Filter {Property = propName, Values = new object[] { 1, 2 },Range = new FilterRange{ End = "Bye", RangeOperator = RangeOperator.LessThan}, includeNulls = true}},
                    new object[] {new Clause[] {new GreaterThanEqualClause(cleanPropName, IGNORE_ME), new InClause(cleanPropName, IGNORE_ME), new NullClause(cleanPropName)}, new Filter {Property = propName, Values = new object[] { 1, 2 },Range = new FilterRange{ End = "Bye", RangeOperator = RangeOperator.GreaterThanEqual}, includeNulls = true}},
                    new object[] {new Clause[] {new LessThanEqualClause(cleanPropName, IGNORE_ME), new InClause(cleanPropName, IGNORE_ME), new NullClause(cleanPropName)}, new Filter {Property = propName, Values = new object[] { 1, 2 },Range = new FilterRange{ End = "Bye", RangeOperator = RangeOperator.LessThanEqual}, includeNulls = true}},
                    new object[] {new Clause[] {new BetweenClause(cleanPropName, IGNORE_ME, IGNORE_ME), new InClause(cleanPropName, IGNORE_ME), new NullClause(cleanPropName)}, new Filter {Property = propName, Values = new object[] { 1, 2 },Range = new FilterRange{ Start = "Hello", End = "Bye", RangeOperator = RangeOperator.Between}, includeNulls = true}},
                    
                    //Ranges, IN and Not Null
                    // Throws exception not valid for this test
                };
            }
        }

        [Fact]
        public void ThrowExceptionIfFiltersContainsValuesAndIncludeNullsIsFalse()
        {
            var filter = new Filter
            {
                Property = "MyProperty",
                includeNulls = false,
                Values = new object[]{ 1, "one", .1 }
            };

            var sut = new ClauseFactory(_fakeValidColumns);

            var ex = Assert.Throws<ArgumentException>(() => sut.Build(filter));
            Assert.Equal("When providing values in a filter, you cannot also set include nulls to false. This negates the filter entirely.", ex.Message);
        }
    }
}