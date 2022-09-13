using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortingAlgorithms
{
    public class GroupSort
    {
        private static double _algorithmSwitchoverThreshold = .1;

        public class listGrouped
        {
            public char CurrentCharacter { get; set; }
            public Int32 CountDigitsInGroup { get; set; }
            public Int32 GroupOffset { get; set; }

            public listGrouped(char inCurrentCharacter, Int32 inCountDigitsInGroup, Int32 inGroupOffset)
            {
                CurrentCharacter = inCurrentCharacter;
                CountDigitsInGroup = inCountDigitsInGroup;
                GroupOffset = inGroupOffset;
            }
        }

        public static List<string> SortStrings(List<string> listToSort)
        {
            return SortStrings(listToSort, 0);
        }

        private static List<string> SortStrings(List<string> listToSort, Int32 currentDepth/*, FallbackType insertionSort, CharacterSet latin1*/)
        {
            if (listToSort.Count == 0)
            {
                return new List<string>();
            }
            //else if (listToSort.Count == 1)
            //{
            //	return listToSort;
            //}

            List<string> finalOutList = new List<string>();
            List<listGrouped> groupsInList = new List<listGrouped>();

            char currentGroupCharacter = ' ';
            Int32 currentGroupCount = 0;
            Int32 currentGroupOffset = 0;
            Int32 threshold = Convert.ToInt32(listToSort.Count * _algorithmSwitchoverThreshold);

            for (Int32 listIndex = 0; listIndex < listToSort.Count; listIndex++)
            {
                if (listToSort[listIndex].Length <= currentDepth)
                {
                    finalOutList.Add(listToSort[listIndex]);

                    groupsInList.Add(new listGrouped(currentGroupCharacter, currentGroupCount, currentGroupOffset));

                    if (groupsInList.Count < threshold)
                    {
                        currentGroupCharacter = ' ';
                        currentGroupCount = 0;
                        currentGroupOffset = 0;
                        continue;
                    }
                    else
                    {
                        // If we are here then we have determined that it will be quicker to use
                        // QuickSort instead of GroupSort to sort our list. Step where we're at,
                        // start resorting (this level and below) using QuickSort.
                        finalOutList.Clear();
                        finalOutList = QuickSort.SortStrings(listToSort);
                        return finalOutList;
                    }
                }

                if (currentGroupCount == 0)
                {
                    currentGroupCharacter = listToSort[listIndex][currentDepth];
                    currentGroupOffset = listIndex;
                    currentGroupCount = 1;
                }
                else
                {
                    if (listToSort[listIndex][currentDepth] == currentGroupCharacter)
                    {
                        currentGroupCount++;
                    }
                    else
                    {
                        groupsInList.Add(new listGrouped(currentGroupCharacter, currentGroupCount, currentGroupOffset));

                        if (groupsInList.Count < threshold)
                        {
                            currentGroupCharacter = listToSort[listIndex][currentDepth];
                            currentGroupOffset = listIndex;
                            currentGroupCount = 1;
                        }
                        else
                        {
                            // If we are here then we have determined that it will be quicker to use
                            // QuickSort instead of GroupSort to sort our list. Step where we're at,
                            // start resorting (this level and below) using QuickSort.
                            finalOutList.Clear();
                            finalOutList = QuickSort.SortStrings(listToSort);
                            return finalOutList;
                        }
                    }
                }
            }

            // The last group is still in memory, add it to groupsInList.
            if (currentGroupCount > 0)
            {
                groupsInList.Add(new listGrouped(currentGroupCharacter, currentGroupCount, currentGroupOffset));
            }


            // groupsInList now has all the items in it


            List<listGrouped> groupsInListSorted = QuickSortStructures(groupsInList);

            char currCharacter = ' ';

            List<string> characterFoundList = new List<string>();

            for (Int32 eachGroupIndex = 0; eachGroupIndex < groupsInListSorted.Count; eachGroupIndex++)
            {
                if (eachGroupIndex == 0)
                {
                    // This is the first pass. All items from the original list that pertain to this group can be added to a new list.
                    currCharacter = groupsInListSorted[eachGroupIndex].CurrentCharacter;
                    characterFoundList.Clear();
                    for (Int32 groupItemIndex = 0; groupItemIndex < groupsInListSorted[eachGroupIndex].CountDigitsInGroup; groupItemIndex++)
                    {
                        characterFoundList.Add(listToSort[groupsInListSorted[eachGroupIndex].GroupOffset + groupItemIndex]);
                    }
                    continue;
                }

                if (groupsInListSorted[eachGroupIndex].CurrentCharacter == currCharacter)
                {
                    // This character matches the current character being searched for.
                    // Add the items from this group to the list too.
                    for (Int32 groupItemIndex = 0; groupItemIndex < groupsInListSorted[eachGroupIndex].CountDigitsInGroup; groupItemIndex++)
                    {
                        characterFoundList.Add(listToSort[groupsInListSorted[eachGroupIndex].GroupOffset + groupItemIndex]);
                    }
                    continue;
                }
                else
                {
                    // This is a new character. Sort the existing list and add it to our final list.
                    List<string> characterSortedList = new List<string>();
                    Int32 nextDepth = Convert.ToInt32(currentDepth + 1);
                    characterSortedList = GroupSort.SortStrings(characterFoundList, nextDepth);

                    // This character is completely done. Add these items to the final list.
                    for (Int32 listIndex = 0; listIndex < characterSortedList.Count; listIndex++)
                    {
                        finalOutList.Add(characterSortedList[listIndex]);
                    }


                    // Start a new list for the new character.
                    currCharacter = groupsInListSorted[eachGroupIndex].CurrentCharacter;
                    characterFoundList.Clear();
                    for (Int32 groupItemIndex = 0; groupItemIndex < groupsInListSorted[eachGroupIndex].CountDigitsInGroup; groupItemIndex++)
                    {
                        characterFoundList.Add(listToSort[groupsInListSorted[eachGroupIndex].GroupOffset + groupItemIndex]);
                    }
                    continue;
                }
            }

            // sort last list
            {
                List<string> characterSortedList = new List<string>();
                Int32 nextDepth = Convert.ToInt32(currentDepth + 1);
                characterSortedList = GroupSort.SortStrings(characterFoundList, nextDepth);

                for (Int32 listIndex = 0; listIndex < characterSortedList.Count; listIndex++)
                {
                    finalOutList.Add(characterSortedList[listIndex]);
                }
            }

            return finalOutList;
        }

        private static List<listGrouped> QuickSortStructures(List<listGrouped> arr)
        {
            List<listGrouped> loe = new List<listGrouped>(), gt = new List<listGrouped>();
            if (arr.Count < 2)
                return arr;
            int pivot = arr.Count / 2;
            listGrouped pivotVal = arr[pivot];
            arr.RemoveAt(pivot);
            foreach (listGrouped i in arr)
            {
                if (i.CurrentCharacter.CompareTo(pivotVal.CurrentCharacter) <= 0)
                    loe.Add(i);
                else if (i.CurrentCharacter.CompareTo(pivotVal.CurrentCharacter) > 0)
                    gt.Add(i);
            }

            List<listGrouped> resultSet = new List<listGrouped>();
            resultSet.AddRange(QuickSortStructures(loe));
            if (loe.Count == 0)
            {
                resultSet.Add(pivotVal);
            }
            else
            {
                //gt.Add(pivotVal);
                gt.Insert(0, pivotVal);
            }
            resultSet.AddRange(QuickSortStructures(gt));
            return resultSet;
        }

    }
}
