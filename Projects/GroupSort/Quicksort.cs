using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Artisan
{
    namespace Sorting
    {
        public class QuickSort
        {
            public class RecursiveSortIntegers
            {
                public static List<int> Sort(List<int> intList)
                {
                    List<int> loe = new List<int>(), gt = new List<int>();
                    if (intList.Count < 2)
                        return intList;
                    int pivot = intList.Count / 2;
                    int pivotVal = intList[pivot];

                    for (int intIndex = 0; intIndex < pivot; intIndex++)
                    {
                        int currentInt = intList[intIndex];

                        if (currentInt <= pivotVal)
                            loe.Add(currentInt);
                        else if (currentInt > pivotVal)
                            gt.Add(currentInt);
                    }
                    for (int intIndex = pivot + 1; intIndex < intList.Count; intIndex++)
                    {
                        int currentInt = intList[intIndex];

                        if (currentInt <= pivotVal)
                            loe.Add(currentInt);
                        else if (currentInt > pivotVal)
                            gt.Add(currentInt);
                    }

                    List<int> resultSet = new List<int>();
                    resultSet.AddRange(Sort(loe));
                    if (loe.Count == 0)
                    {
                        resultSet.Add(pivotVal);
                    }
                    else
                    {
                        gt.Insert(0, pivotVal);
                    }
                    resultSet.AddRange(Sort(gt));
                    return resultSet;
                }
            }

            public class SortIntegers
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

                public static List<int> Sort(List<int> intList)
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

            public class RecursiveSortStrings
            {
                public List<string> Sort(List<string> stringList)
                {
                    List<string> loe = new List<string>(), gt = new List<string>();
                    if (stringList.Count < 2)
                        return stringList;
                    int pivot = stringList.Count / 2;
                    string pivotVal = stringList[pivot];

                    for (int stringIndex = 0; stringIndex < pivot; stringIndex++)
                    {
                        string currentString = stringList[stringIndex];

                        if (String.Compare(currentString, pivotVal, CultureInfo.CurrentCulture, CompareOptions.Ordinal) <= 0)
                            loe.Add(currentString);
                        else if (String.Compare(currentString, pivotVal, CultureInfo.CurrentCulture, CompareOptions.Ordinal) > 0)
                            gt.Add(currentString);
                    }
                    for (int stringIndex = pivot + 1; stringIndex < stringList.Count; stringIndex++)
                    {
                        string currentString = stringList[stringIndex];

                        if (String.Compare(currentString, pivotVal, CultureInfo.CurrentCulture, CompareOptions.Ordinal) <= 0)
                            loe.Add(currentString);
                        else if (String.Compare(currentString, pivotVal, CultureInfo.CurrentCulture, CompareOptions.Ordinal) > 0)
                            gt.Add(currentString);
                    }

                    List<string> resultSet = new List<string>();
                    resultSet.AddRange(Sort(loe));
                    if (loe.Count == 0)
                    {
                        resultSet.Add(pivotVal);
                    }
                    else
                    {
                        gt.Insert(0, pivotVal);
                    }
                    resultSet.AddRange(Sort(gt));
                    return resultSet;
                }
            }

            public class SortStrings
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

                public List<string> Sort(List<string> stringList)
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
