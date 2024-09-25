using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace maxprofitness.shared
{
    public static class MetricsListCondenser
    {
        public static List<int> CondenseList(this List<int> valuesList)
        {
            const int maxValues = FitFighterRevampConstants.METRICS_MAX_VALUES;

            List<int> condensedList = new List<int>();
            List<int> listAverage = new List<int>();

            int averageSeparator = valuesList.Count / maxValues;
            averageSeparator = Mathf.Clamp(averageSeparator, 1, int.MaxValue);

            for (int i = 0; i < valuesList.Count; i++)
            {
                listAverage.Add(valuesList[i]);

                if (i % averageSeparator != 0)
                {
                    continue;
                }

                if (condensedList.Count >= maxValues)
                {
                    condensedList[maxValues - 1] = ((int)listAverage.Average() + condensedList[maxValues - 1]) / 2;
                }
                else
                {
                    condensedList.Add((int)listAverage.Average());
                }

                listAverage.Clear();
            }
            return condensedList;
        }
    }
}
