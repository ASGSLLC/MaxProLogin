using NUnit.Framework;
using MaxProFitness.Sdk;
namespace maxprofitness.login
{
    [TestOf(typeof(SetKnobActuatorPositionAppCommand))]
    public class SetKnobActuatorPositionAppCommandTests : AppCommandTestsBase<SetKnobActuatorPositionAppCommand>
    {
        protected override string TestInput => "5B0403FF5D";

        protected override CommandType CommandType => CommandType.SetKnobActuatorPosition;
    }
}
