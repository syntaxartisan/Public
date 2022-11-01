using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace SortingAlgorithms
{
    public class QuickSort
    {
        private class Boundary
        {
            public int startIndex;
            public int endIndex;
            public Boundary(int startIndex, int endIndex)
            {
                this.startIndex = startIndex;
                this.endIndex = endIndex;
            }
        }

        public class Integers
        {
            public class Recursive
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
                    resultSet.AddRange(SortIntegers(loe));
                    if (loe.Count == 0)
                    {
                        loe.Add(pivotVal);
                    }
                    else
                    {
                        gt.Add(pivotVal);
                    }
                    resultSet.AddRange(SortIntegers(gt));
                    return resultSet;
                }
            }

            public class NonRecursive
            {
                public static List<int> SortIntegersNoRecursion(List<int> intList)
                {
                    Stack<Boundary> stack = new Stack<Boundary>();
                    int startIndex = 0;
                    int endIndex = intList.Count - 1;

                    stack.Push(new Boundary(startIndex, endIndex));

                    while (stack.Count > 0)
                    {
                        startIndex = stack.Peek().startIndex;
                        endIndex = stack.Peek().endIndex;
                        stack.Pop();
                        int pivotIndex = Partition(intList, startIndex, endIndex);
                        if (pivotIndex - 1 > startIndex)
                        {
                            stack.Push(new Boundary(startIndex, pivotIndex - 1));
                        }
                        if (pivotIndex + 1 < endIndex)
                        {
                            stack.Push(new Boundary(pivotIndex + 1, endIndex));
                        }
                    }

                    return intList;
                }

                // Swap the array element
                private static void Swap(List<int> intList, int val1Index, int val2Index)
                {
                    int temp = intList[val1Index];
                    intList[val1Index] = intList[val2Index];
                    intList[val2Index] = temp;
                }

                private static int Partition(List<int> intList, int lowIndex, int highIndex)
                {
                    // Set the high index element to its 
                    // proper sorted position
                    int pivotValue = intList[highIndex];
                    int i = lowIndex - 1;
                    for (int j = lowIndex; j < highIndex; ++j)
                    {
                        if (intList[j] < pivotValue)
                        {
                            i++;
                            Swap(intList, i, j);
                        }
                    }
                    // Set the high index value to its sorted position
                    Swap(intList, i + 1, highIndex);
                    // Returns the next sorting  element location
                    return i + 1;
                }
            }

        }

        public class Strings
        {
            public class Recursive
            {
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
                    resultSet.AddRange(SortStrings(loe));
                    if (loe.Count == 0)
                    {
                        resultSet.Add(pivotVal);
                    }
                    else
                    {
                        //gt.Add(pivotVal);
                        gt.Insert(0, pivotVal);
                    }
                    resultSet.AddRange(SortStrings(gt));
                    return resultSet;
                }
            }

            public class NonRecursive
            {
                public List<string> SortStringsNoRecursion(List<string> stringList)
                {
                    Stack<Boundary> stack = new Stack<Boundary>();
                    int startIndex = 0;
                    int endIndex = stringList.Count - 1;

                    stack.Push(new Boundary(startIndex, endIndex));

                    while (stack.Count > 0)
                    {
                        startIndex = stack.Peek().startIndex;
                        endIndex = stack.Peek().endIndex;
                        stack.Pop();
                        int pivotIndex = this.Partition(stringList, startIndex, endIndex);
                        if (pivotIndex - 1 > startIndex)
                        {
                            stack.Push(new Boundary(startIndex, pivotIndex - 1));
                        }
                        if (pivotIndex + 1 < endIndex)
                        {
                            stack.Push(new Boundary(pivotIndex + 1, endIndex));
                        }
                    }

                    return stringList;
                }


                // Swap the array element
                private static void Swap(List<string> stringList, int val1Index, int val2Index)
                {
                    string temp = stringList[val1Index];
                    stringList[val1Index] = stringList[val2Index];
                    stringList[val2Index] = temp;
                }

                // Provide this function a subset of the original array or List.
                // This sorts the array such that smaller elements are to the left (low indices)
                // and larger elements are to the right (high indices).
                // Then it returns the starting index are the larger elements.
                // "Large" is defined by the pivot value selection, which is the last element of the subset.
                private int Partition(List<string> stringList, int lowIndex, int highIndex)
                {
                    // Set the high index element to its 
                    // proper sorted position
                    string pivotValue = stringList[highIndex];
                    int i = lowIndex - 1;
                    for (int j = lowIndex; j < highIndex; ++j)
                    {
                        if (String.Compare(stringList[j], pivotValue, CultureInfo.CurrentCulture, CompareOptions.Ordinal) < 0)
                        //if (stringList[j] < pivotValue)
                        {
                            i++;
                            Swap(stringList, i, j);
                        }
                    }
                    // Set the high index value to its sorted position
                    Swap(stringList, i + 1, highIndex);
                    // Returns the next sorting  element location
                    return i + 1;
                }

            }
        }
    }
}
