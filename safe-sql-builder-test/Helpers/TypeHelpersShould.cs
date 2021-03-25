using SafeSqlBuilder.Helpers;
using Xunit;

namespace SafeSqlBuilder.Tests.Helpers
{
    public class TypeHelpersShould
    {
        [Fact]
        public void DetectNullObject()
        {
            object t = null;
            Assert.True(t.IsNull());
            Assert.False(t.IsNotNull());
        }
        
        [Fact]
        public void DetectNullString()
        {
            string t = null;
            Assert.True(t.IsNull());
            Assert.False(t.IsNotNull());
        }
        
        [Fact]
        public void DetectNonNullObject()
        {
            object t = new { T = "test" };
            Assert.False(t.IsNull());
            Assert.True(t.IsNotNull());
        }
        
        [Fact]
        public void DetectNonNullString()
        {
            // ReSharper disable once ConvertToConstant.Local
            var t = "null";
            Assert.False(t.IsNull());
            Assert.True(t.IsNotNull());
        }
    }
}