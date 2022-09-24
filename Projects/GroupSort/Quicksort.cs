using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortingAlgorithms
{
    public class QuickSort
    {
        public static List<int> SortIntegers(List<int> intList)
        {
            List<int> loe = new List<int>(), gt = new List<int>();
            if (intList.Count < 2)
                return intList;
            int pivot = intList.Count / 2;
            int pivotVal = intList[pivot];
            intList.RemoveAt(pivot);
            foreach (int i in intList)
            {
                if (i <= pivotVal)
                    loe.Add(i);
                else if (i > pivotVal)
                    gt.Add(i);
            }

            List<int> resultSet = new List<int>();
            resultSet.AddRange(QuickSort.SortIntegers(loe));
            if (loe.Count == 0)
            {
                loe.Add(pivotVal);
            }
            else
            {
                gt.Add(pivotVal);
            }
            resultSet.AddRange(QuickSort.SortIntegers(gt));
            return resultSet;
        }

        public static List<string> SortStrings(List<string> stringList)
        {
            List<string> loe = new List<string>(), gt = new List<string>();
            if (stringList.Count < 2)
                return stringList;
            int pivot = stringList.Count / 2;
            string pivotVal = stringList[pivot];
            stringList.RemoveAt(pivot);
            foreach (string i in stringList)
            {
                if (String.Compare(i, pivotVal, CultureInfo.CurrentCulture, CompareOptions.Ordinal) <= 0)
                    loe.Add(i);
                else if (String.Compare(i, pivotVal, CultureInfo.CurrentCulture, CompareOptions.Ordinal) > 0)
                    gt.Add(i);
            }

            List<string> resultSet = new List<string>();
            resultSet.AddRange(QuickSort.SortStrings(loe));
            if (loe.Count == 0)
            {
                resultSet.Add(pivotVal);
            }
            else
            {
                //gt.Add(pivotVal);
                gt.Insert(0, pivotVal);
            }
            resultSet.AddRange(QuickSort.SortStrings(gt));
            return resultSet;
        }
    }
}
