using NUnit.Framework;

namespace maxprofitness.shared
{
    [TestOf(typeof(GameEventRequestUpdateMaxProCommand))]
    public class GameEventRequestUpdateMaxProCommandTests : MaxProCommandTestsBase<GameEventRequestUpdateMaxProCommand>
    {
        protected override string TestInput => "7B8A0A1234FA23457D";

        protected override CommandType CommandType => CommandType.GameEventRequestUpdate;
    }
}
