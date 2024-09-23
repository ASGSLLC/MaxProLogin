using NUnit.Framework;

namespace maxprofitness.shared
{
    [TestOf(typeof(StatusRequestMaxProCommand))]
    public class StatusRequestMaxProCommandTests : MaxProCommandTestsBase<StatusRequestMaxProCommand>
    {
        protected override string TestInput => "7B02010100630A0400003F06590A13000A0A0400003F06580A13000A7D";

        protected override CommandType CommandType => CommandType.StatusRequest;
    }
}
