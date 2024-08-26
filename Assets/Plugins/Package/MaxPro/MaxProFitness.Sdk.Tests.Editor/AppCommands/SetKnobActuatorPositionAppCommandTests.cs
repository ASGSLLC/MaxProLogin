using NUnit.Framework;

namespace MaxProFitness.Sdk
{
    [TestOf(typeof(SetKnobActuatorPositionAppCommand))]
    public class SetKnobActuatorPositionAppCommandTests : AppCommandTestsBase<SetKnobActuatorPositionAppCommand>
    {
        protected override string TestInput => "5B0403FF5D";

        protected override CommandType CommandType => CommandType.SetKnobActuatorPosition;
    }
}
