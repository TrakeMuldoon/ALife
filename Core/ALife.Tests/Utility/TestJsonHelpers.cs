using ALife.Core.Utility;

namespace ALife.Tests.Utility
{
    /// <summary>
    /// Tests for the JsonHelpers class.
    /// </summary>
    [TestClass]
    public class TestJsonHelpers
    {
        [TestMethod]
        public void TestDeserialize()
        {
            var obj = JsonHelpers.DeserializeContents<Example>("{\"Field\":5,\"Property\":true}");
            var expected = new Example(true, 5);

            Assert.AreEqual(expected, obj);
        }

        [TestMethod]
        public void TestSerialize()
        {
            var example = new Example(true, 5);
            var actual = JsonHelpers.SerializeObject(example);
            var expected = "{\"Property\":true,\"Field\":5}";
            var actualWhitespaceCleaned = string.Concat(actual.Where(c => !char.IsWhiteSpace(c)));
            Assert.AreEqual(expected, actualWhitespaceCleaned);
        }

        private class Example
        {
            public int Field;

            public Example(bool property, int field)
            {
                Field = field;
                Property = property;
            }

            public bool Property { get; }

            public override bool Equals(object obj)
            {
                if(obj is Example other)
                {
                    return Field == other.Field && Property == other.Property;
                }
                return false;
            }
        }
    }
}
