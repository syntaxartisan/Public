using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortingAlgorithms
{
    public class QuickSort
    {
        public static List<int> SortIntegers(List<int> int_list)
        {
            List<int> loe = new List<int>(), gt = new List<int>();
            if (int_list.Count < 2)
                return int_list;
            int pivot = int_list.Count / 2;
            int pivot_val = int_list[pivot];
            int_list.RemoveAt(pivot);
            foreach (int i in int_list)
            {
                if (i <= pivot_val)
                    loe.Add(i);
                else if (i > pivot_val)
                    gt.Add(i);
            }

            List<int> resultSet = new List<int>();
            resultSet.AddRange(QuickSort.SortIntegers(loe));
            if (loe.Count == 0)
            {
                loe.Add(pivot_val);
            }
            else
            {
                gt.Add(pivot_val);
            }
            resultSet.AddRange(QuickSort.SortIntegers(gt));
            return resultSet;
        }

        public static List<string> SortStrings(List<string> string_list)
        {
            List<string> loe = new List<string>(), gt = new List<string>();
            if (string_list.Count < 2)
                return string_list;
            int pivot = string_list.Count / 2;
            string pivot_val = string_list[pivot];
            string_list.RemoveAt(pivot);
            foreach (string i in string_list)
            {
                if (i.CompareTo(pivot_val) <= 0)
                    loe.Add(i);
                else if (i.CompareTo(pivot_val) > 0)
                    gt.Add(i);
            }

            List<string> resultSet = new List<string>();
            resultSet.AddRange(QuickSort.SortStrings(loe));
            if (loe.Count == 0)
            {
                resultSet.Add(pivot_val);
            }
            else
            {
                //gt.Add(pivot_val);
                gt.Insert(0, pivot_val);
            }
            resultSet.AddRange(QuickSort.SortStrings(gt));
            return resultSet;
        }
    }
}
