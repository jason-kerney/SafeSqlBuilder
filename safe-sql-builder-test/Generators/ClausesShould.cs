using System.Collections.Generic;
using System.Linq;
using SafeSqlBuilder.Generators;
using Xunit;

namespace SafeSqlBuilder.Tests.Generators
{
    public class ClausesShould
    {
        [Theory]
        [MemberData(nameof(GetClauseTestData))]
        public void ConstructProperString(string expectedClause, (string, object)[] expectedNameValues, Clause sut)
        {
            var parameters = sut.GetParameters().ToArray();
            Assert.Equal(expectedClause, sut.ToString());
            Assert.Equal(expectedNameValues.Length, parameters.Length);

            for (var i = 0; i < expectedNameValues.Length; i++)
            {
                var (expectedName, expectedValue) = expectedNameValues[i];
                var (actualName, actualValue) = parameters[i];
                
                Assert.Equal(expectedName, actualName);
                Assert.Equal(expectedValue, actualValue);
            }
        }

        public static IEnumerable<object[]> GetClauseTestData
        {
            get
            {
                var strings = new[] {"hello", "time"};
                var numbers = new[] {8, 9};
                
                return new List<object[]>
                {
                    new object[] {"[PropertyName] = @PropertyName_EQUAL"            , new (string, object)[] {("@PropertyName_EQUAL", 5)}                       , new EqualClause("PropertyName", 5)},
                    new object[] {"[YourCoolName] = @YourCoolName_EQUAL"            , new (string, object)[] {("@YourCoolName_EQUAL", 6)}                       , new EqualClause("YourCoolName", 6)},

                    new object[] {"[PropertyName] IN @PropertyName_IN"              , new (string, object)[] {("@PropertyName_IN", strings)}                    , new InClause("PropertyName", strings)},
                    new object[] {"[YourOtherCoolName] IN @YourOtherCoolName_IN"    , new (string, object)[] {("@YourOtherCoolName_IN", numbers)}               , new InClause("YourOtherCoolName", numbers)},

                    new object[] {"[YourOtherCoolName] > @YourOtherCoolName_GREATER", new (string, object)[] {("@YourOtherCoolName_GREATER", 6)}                , new GreaterThanClause("YourOtherCoolName", 6)},
                    new object[] {"[CoolName] > @CoolName_GREATER"                  , new (string, object)[] {("@CoolName_GREATER", 4)}                         , new GreaterThanClause("CoolName", 4)},

                    new object[] {"[AName] < @AName_LESS"                           , new (string, object)[] {("@AName_LESS", 7)}                               , new LessThanClause("AName", 7)},
                    new object[] {"[AnotherName] < @AnotherName_LESS"               , new (string, object)[] {("@AnotherName_LESS", 10)}                        , new LessThanClause("AnotherName", 10)},

                    new object[] {"[Boo] >= @Boo_GREATER_EQUAL"                     , new (string, object)[] {("@Boo_GREATER_EQUAL", "DHO!")}                   , new GreaterThanEqualClause("Boo", "DHO!")},
                    new object[] {"[BooTwo] >= @BooTwo_GREATER_EQUAL"               , new (string, object)[] {("@BooTwo_GREATER_EQUAL", 8)}                     , new GreaterThanEqualClause("BooTwo", 8)},

                    new object[] {"[Boo] <= @Boo_LESS_EQUAL"                        , new (string, object)[] {("@Boo_LESS_EQUAL", 4)}                           , new LessThanEqualClause("Boo", 4)},
                    new object[] {"[BooTwo] <= @BooTwo_LESS_EQUAL"                  , new (string, object)[] {("@BooTwo_LESS_EQUAL", 3)}                        , new LessThanEqualClause("BooTwo", 3)},

                    new object[] {"[Boo] BETWEEN @Boo_START AND @Boo_END"           , new (string, object)[] {("@Boo_START", "Hello"), ("@Boo_END", "Good Bye")}, new BetweenClause("Boo", "Hello", "Good Bye")},
                    new object[] {"[BooWho] BETWEEN @BooWho_START AND @BooWho_END"  , new (string, object)[] {("@BooWho_START", 1), ("@BooWho_END", 2)}         , new BetweenClause("BooWho", 1, 2)},

                    new object[] {"[Boo] IS NULL"                                   , new (string, object)[] { }                                                , new NullClause("Boo")},
                    new object[] {"[BooWho] IS NULL"                                , new (string, object)[] { }                                                , new NullClause("BooWho"),},

                    new object[] {"[Boo] IS NOT NULL"                                   , new (string, object)[] { }                                                , new NotNullClause("Boo")},
                    new object[] {"[BooWho] IS NOT NULL"                                , new (string, object)[] { }                                                , new NotNullClause("BooWho"),},
                };
            }
        }
    }
}