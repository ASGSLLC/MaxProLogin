using NUnit.Framework;
using MaxProFitness.Sdk;

namespace maxprofitness.login
{
    public abstract class MaxProCommandTestsBase<T>
        where T : struct, IMaxProCommand
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
