using NUnit.Framework;
using MaxProFitness.Sdk;
namespace maxprofitness.login
{
    [TestOf(typeof(MinimumLengthToPullAppCommand))]
    public class MinimumLengthToPullAppCommandTests : AppCommandTestsBase<MinimumLengthToPullAppCommand>
    {
        protected override string TestInput => "5B0603FF5D";

        protected override CommandType CommandType => CommandType.MinimumLengthToPull;
    }
}
