using ApprovalTests;
using SafeSqlBuilder.Generators;
using Xunit;

namespace SafeSqlBuilder.Tests.Generators
{
    public class WhereShould
    {
        // ReSharper disable once InconsistentNaming
        private readonly object IGNORE_ME = 5;

        [Fact]
        public void ProduceAnEqualStatementForSingleClause()
        {
            var clauses = Builder.DoublePack<Clause>(
                new EqualClause("Some_Good_Property", IGNORE_ME)
            );
            
            var fakeQueryData = Builder.GetQueryData(clauses);

            var queryData = fakeQueryData;
            var sut = new WhereGenerator(queryData);

            Assert.Equal("WHERE [Some_Good_Property] = @Some_Good_Property_EQUAL", sut.ToString());
        }

        [Fact]
        public void ProduceAnStatementForAnotherSingleClause()
        {
            var doublePack = Builder.DoublePack<Clause>(
                new EqualClause("Another_Good_Property", IGNORE_ME)
            );
            var fakeQueryData = Builder.GetQueryData(doublePack);

            var sut = new WhereGenerator(fakeQueryData);

            Assert.Equal("WHERE [Another_Good_Property] = @Another_Good_Property_EQUAL", sut.ToString());
        }

        [Fact]
        public void ProduceAnOrStatementForMultipleClauses()
        {
            var clauses = Builder.DoublePack<Clause>(
                new EqualClause("PropOne", IGNORE_ME),
                new BetweenClause("PropTwo", IGNORE_ME, IGNORE_ME)
            );
            var fakeQueryData = Builder.GetQueryData(clauses);

            var sut = new WhereGenerator(fakeQueryData);

            Assert.Equal("WHERE [PropOne] = @PropOne_EQUAL OR [PropTwo] BETWEEN @PropTwo_START AND @PropTwo_END", sut.ToString());
        }
        
        [Fact]
        public void ProduceAnAndStatementForMultipleClauses()
        {
            var clauses1 = Builder.Pack<Clause>(new EqualClause("PropOne", IGNORE_ME));
            var clauses2 = Builder.Pack<Clause>(new BetweenClause("PropTwo", IGNORE_ME, IGNORE_ME));
            var fakeQueryData = Builder.GetQueryData(Builder.Pack(clauses1, clauses2));

            var sut = new WhereGenerator(fakeQueryData);

            Assert.Equal("WHERE ([PropOne] = @PropOne_EQUAL) AND ([PropTwo] BETWEEN @PropTwo_START AND @PropTwo_END)", sut.ToString());
        }

        [Fact]
        public void ProduceAnAndOfOrStatementsForMultipleClauses()
        {
            var clauses1 = Builder.Pack<Clause>(new EqualClause("PropOne", IGNORE_ME), new NullClause("PropNull"));
            var clauses2 = Builder.Pack<Clause>(new BetweenClause("PropTwo", IGNORE_ME, IGNORE_ME), new LessThanClause("PropThree", IGNORE_ME));
            var fakeQueryData= Builder.GetQueryData(Builder.Pack(clauses1, clauses2));

            var sut = new WhereGenerator(fakeQueryData);

            Assert.Equal("WHERE ([PropOne] = @PropOne_EQUAL OR [PropNull] IS NULL) AND ([PropTwo] BETWEEN @PropTwo_START AND @PropTwo_END OR [PropThree] < @PropThree_LESS)", sut.ToString());
        }

        [Fact]
        public void ProduceAnAndOfOrStatementsForUnbalancedMultipleClauses()
        {
            var clauses1 = Builder.Pack<Clause>(new EqualClause("PropOne", IGNORE_ME), new NullClause("PropNull"));
            var clauses2 = Builder.Pack<Clause>(new BetweenClause("PropTwo", IGNORE_ME, IGNORE_ME));
            var fakeQueryData = Builder.GetQueryData(Builder.Pack(clauses1, clauses2));

            var sut = new WhereGenerator(fakeQueryData);

            Assert.Equal("WHERE ([PropOne] = @PropOne_EQUAL OR [PropNull] IS NULL) AND ([PropTwo] BETWEEN @PropTwo_START AND @PropTwo_END)", sut.ToString());
        }

        [Fact]
        public void ProduceEmptyStringForNoClauses()
        {
            var fakeQueryData = Builder.GetQueryData();

            var sut = new WhereGenerator(fakeQueryData);

            Assert.Equal("", sut.ToString());
        }

        [Fact]
        public void ProduceEmptyStringForNoInnerClauses()
        {
            var fakeQueryData = Builder.GetQueryData(new Clause[0][]);

            var sut = new WhereGenerator(fakeQueryData);

            Assert.Equal("", sut.ToString());
        }

        [Fact]
        public void ProduceAllParameters()
        {
            var fakeQueryData = Builder.GetQueryData(
                Builder.Pack(
                    Builder.Pack<Clause>(
                        new EqualClause("Param1", "One"),
                        new NullClause("Param2")
                    ),
                    Builder.Pack<Clause>(
                        new BetweenClause("Param3", 3, 4),
                        new NullClause("Param4")
                    ),
                    Builder.Pack<Clause>(
                        new GreaterThanClause("Param5", 600)
                    )
                )
            );
            
            var sut = new WhereGenerator(fakeQueryData);

            var parameters = sut.GetParameters();
            
            Approvals.VerifyAll(parameters, "parameter");
        }
    }
}