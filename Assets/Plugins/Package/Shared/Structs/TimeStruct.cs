using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace maxprofitness.shared
{
    [Serializable]
    public struct TimeStruct
    {
        public int Hours;
        public int Minutes;
        public int Seconds;

        public void AddTime(TimeStruct timeToAdd)
        {
            Hours += timeToAdd.Hours;
            Minutes += timeToAdd.Minutes;
            Seconds += timeToAdd.Seconds;

            if (Seconds >= 60)
            {
                Seconds -= 60;
                Minutes++;
            }

            if (Minutes >= 60)
            {
                Minutes -= 60;
                Hours++;
            }
        }
    }
}
