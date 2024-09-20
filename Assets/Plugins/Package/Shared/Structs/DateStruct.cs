using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace maxprofitness.shared
{
    [Serializable]
    public struct DateStruct
    {
        public int Day;
        public int Month;
        public int Year;

        public int TotalDays()
        {
            if (Day == 0 || Month == 0 || Year == 0)
            {
                Debug.Log("#2 date was null");
                return 0;
            }
            int[] daysToMonth365 = new int[13]
            {
                0,
                31,
                59,
                90,
                120,
                151,
                181,
                212,
                243,
                273,
                304,
                334,
                365
            };
            
            int actualMonthDay = Day;
            int totalDaysByMonth = daysToMonth365[Month - 1];
            int totalDaysByYear = Year * 365;

            return totalDaysByYear + totalDaysByMonth + actualMonthDay;
        }
    }
}
