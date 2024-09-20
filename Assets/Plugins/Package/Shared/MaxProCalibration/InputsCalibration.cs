using System;

namespace maxprofitness.shared
{
    [Serializable]
    public struct InputsCalibration
    {
        public InputCalibration Left;
        public InputCalibration Right;
        public InputCalibration Both;
    }
}
