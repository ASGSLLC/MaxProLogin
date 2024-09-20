using System;

namespace maxprofitness.shared
{
    [Serializable]
    public struct CombatInput
    {
        public ActionSide InputSide;
        public ActionType ActionType;
    }
}
