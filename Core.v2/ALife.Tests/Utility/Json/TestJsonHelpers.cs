using ALife.Core.Utility.Json;

namespace ALife.Tests.Utility.Json
{
    /// <summary>
    /// Tests for the Settings class.
    /// </summary>
    internal class TestIoHelpers
    {
        [Test]
        public void TestDeserialize()
        {
            var obj = JsonHelpers.DeserializeContents<Example>("{\"Field\":5,\"Property\":true}");
            var expected = new Example(true, 5);

            Assert.AreEqual(expected, obj);
        }

        [Test]
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
                this.Field = field;
                this.Property = property;
            }

            public bool Property { get; }

            public override bool Equals(object obj)
            {
                if(obj is Example other)
                {
                    return this.Field == other.Field && this.Property == other.Property;
                }
                return false;
            }
        }
    }
}
