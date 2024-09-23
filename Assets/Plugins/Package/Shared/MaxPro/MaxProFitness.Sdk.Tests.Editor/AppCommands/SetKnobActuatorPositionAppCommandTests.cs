using NUnit.Framework;

namespace maxprofitness.shared
{
    [TestOf(typeof(SetKnobActuatorPositionAppCommand))]
    public class SetKnobActuatorPositionAppCommandTests : AppCommandTestsBase<SetKnobActuatorPositionAppCommand>
    {
        protected override string TestInput => "5B0403FF5D";

        protected override CommandType CommandType => CommandType.SetKnobActuatorPosition;
    }
}
