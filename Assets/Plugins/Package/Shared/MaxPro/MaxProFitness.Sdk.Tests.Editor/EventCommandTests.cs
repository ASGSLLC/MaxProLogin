using NUnit.Framework;
using MaxProFitness.Sdk;
namespace maxprofitness.login
{
    [TestOf(typeof(EventCommand))]
    public class EventCommandTests
    {
        private const string TestInput = "7B02010100630A0400003F06590A13000A0A0400003F06580A13000A7D";
        private const CommandType CommandType = maxprofitness.login.CommandType.Event;

        [Test]
        public void CommandType_MatchesInputAfter_Deserialize()
        {
            EventCommand maxProCommand = new EventCommand();
            Assert.IsTrue(maxProCommand.Deserialize(ConversionUtility.ToBytes(TestInput)));
            Assert.That(maxProCommand.CommandType, Is.EqualTo(CommandType));
        }

        [Test]
        public void ToHexData_MatchesInputAfter_Deserialize()
        {
            EventCommand maxProCommand = new EventCommand();
            Assert.IsTrue(maxProCommand.Deserialize(ConversionUtility.ToBytes(TestInput)));
            Assert.That(maxProCommand.ToHexData().ToUpper(), Is.EqualTo(TestInput.Substring(4, TestInput.Length - 6)));
        }
    }
}
