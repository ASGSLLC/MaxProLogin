using NUnit.Framework;

namespace maxprofitness.shared
{
    public abstract class AppCommandTestsBase<T>
        where T : struct, IAppCommand
    {
        protected abstract string TestInput { get; }

        protected abstract CommandType CommandType { get; }

        [Test]
        public void CommandType_MatchesInputAfter_Deserialize()
        {
            T maxProCommand = new T();
            Assert.IsTrue(maxProCommand.Deserialize(ConversionUtility.ToBytes(TestInput)));
            Assert.That(maxProCommand.CommandType, Is.EqualTo(CommandType));
        }

        [Test]
        public void ToHexData_MatchesInputAfter_Deserialize()
        {
            T maxProCommand = new T();
            Assert.IsTrue(maxProCommand.Deserialize(ConversionUtility.ToBytes(TestInput)));
            Assert.That(maxProCommand.ToHexData().ToUpper(), Is.EqualTo(TestInput.Substring(4, TestInput.Length - 6)));
        }
    }
}
