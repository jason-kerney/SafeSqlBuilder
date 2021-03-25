using System.Collections.Generic;
using SafeSqlBuilder.Helpers;
using Xunit;

namespace SafeSqlBuilder.Tests.Helpers
{
    public class EnumeratorHelpersShould
    {
        [Fact]
        public void DetectAnEmptyCollection()
        {
            var emptyValues = (IEnumerable<int>)new int[0];
            
            Assert.True(emptyValues.Empty());
        }
        
        [Fact]
        public void DetectANonEmptyCollection()
        {
            var emptyValues = (IEnumerable<string>)new[]{ "Hello" };
            
            Assert.False(emptyValues.Empty());
        }
        
        [Fact]
        public void DetectAnEmptyCollectionCreatingPredicate()
        {
            var emptyValues = (IEnumerable<bool>)new []{ false, false, false };
            
            Assert.True(emptyValues.Empty(b => b));
        }
        
        [Fact]
        public void DetectNonEmptyCollectionCreatingPredicate()
        {
            var emptyValues = (IEnumerable<string>)new []{ "Hello", "Hi", "Happy", "Days" };
            
            Assert.False(emptyValues.Empty(s => !s.StartsWith('H')));
        }
    }
}