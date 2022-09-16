﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortingAlgorithms
{
    public class GroupSort
    {
        private static double _algorithmSwitchoverThreshold = .1;

        public class CharacterGroup
        {
            private char _currentCharacter;
            private Int32 _countDigitsInGroup;
            private Int32 _groupOffset;

            public char CurrentCharacter
            {
                get { return _currentCharacter; }
                set { _currentCharacter = value; }
            }
            public Int32 CountDigitsInGroup
            {
                get { return _countDigitsInGroup; }
                set { value = _countDigitsInGroup; }
            }
            public Int32 GroupOffset
            {
                get { return _groupOffset; }
                set { value = _groupOffset; }
            }

            public CharacterGroup(char inCurrentCharacter, Int32 inCountDigitsInGroup, Int32 inGroupOffset)
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
            List<CharacterGroup> characters = new List<CharacterGroup>();

            char currentGroupCharacter = ' ';
            Int32 currentGroupCount = 0;
            Int32 currentGroupOffset = 0;
            Int32 threshold = Convert.ToInt32(listToSort.Count * _algorithmSwitchoverThreshold);

            for (Int32 listIndex = 0; listIndex < listToSort.Count; listIndex++)
            {
                if (listToSort[listIndex].Length <= currentDepth)
                {
                    finalOutList.Add(listToSort[listIndex]);

                    characters.Add(new CharacterGroup(currentGroupCharacter, currentGroupCount, currentGroupOffset));

                    if (characters.Count < threshold)
                    {
                        currentGroupCharacter = ' ';
                        currentGroupCount = 0;
                        currentGroupOffset = 0;
                        continue;
                    }
                    else
                    {
                        // If we are here then we have determined that it will be quicker to use
                        // QuickSort instead of GroupSort to sort our list. Stop where we're at,
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
                        characters.Add(new CharacterGroup(currentGroupCharacter, currentGroupCount, currentGroupOffset));

                        if (characters.Count < threshold)
                        {
                            currentGroupCharacter = listToSort[listIndex][currentDepth];
                            currentGroupOffset = listIndex;
                            currentGroupCount = 1;
                        }
                        else
                        {
                            // If we are here then we have determined that it will be quicker to use
                            // QuickSort instead of GroupSort to sort our list. Stop where we're at,
                            // start resorting (this level and below) using QuickSort.
                            finalOutList.Clear();
                            finalOutList = QuickSort.SortStrings(listToSort);
                            return finalOutList;
                        }
                    }
                }
            }

            // The last group is still in memory, add it to characters.
            if (currentGroupCount > 0)
            {
                characters.Add(new CharacterGroup(currentGroupCharacter, currentGroupCount, currentGroupOffset));
            }


            // characters now has all the items in it


            List<CharacterGroup> charactersSorted = QuickSortStructures(characters);

            char currCharacter = ' ';

            List<string> characterFoundList = new List<string>();

            for (Int32 eachGroupIndex = 0; eachGroupIndex < charactersSorted.Count; eachGroupIndex++)
            {
                if (eachGroupIndex == 0)
                {
                    // This is the first pass. All items from the original list that pertain to this group can be added to a new list.
                    currCharacter = charactersSorted[eachGroupIndex].CurrentCharacter;
                    characterFoundList.Clear();
                    for (Int32 groupItemIndex = 0; groupItemIndex < charactersSorted[eachGroupIndex].CountDigitsInGroup; groupItemIndex++)
                    {
                        characterFoundList.Add(listToSort[charactersSorted[eachGroupIndex].GroupOffset + groupItemIndex]);
                    }
                    continue;
                }

                if (charactersSorted[eachGroupIndex].CurrentCharacter == currCharacter)
                {
                    // This character matches the current character being searched for.
                    // Add the items from this group to the list too.
                    for (Int32 groupItemIndex = 0; groupItemIndex < charactersSorted[eachGroupIndex].CountDigitsInGroup; groupItemIndex++)
                    {
                        characterFoundList.Add(listToSort[charactersSorted[eachGroupIndex].GroupOffset + groupItemIndex]);
                    }
                    continue;
                }
                else
                {
                    // This is a new character. Sort the existing list and add it to our final list.
                    Int32 nextDepth = Convert.ToInt32(currentDepth + 1);
                    finalOutList.AddRange(GroupSort.SortStrings(characterFoundList, nextDepth));


                    // Start a new list for the new character.
                    currCharacter = charactersSorted[eachGroupIndex].CurrentCharacter;
                    characterFoundList.Clear();
                    for (Int32 groupItemIndex = 0; groupItemIndex < charactersSorted[eachGroupIndex].CountDigitsInGroup; groupItemIndex++)
                    {
                        characterFoundList.Add(listToSort[charactersSorted[eachGroupIndex].GroupOffset + groupItemIndex]);
                    }
                    continue;
                }
            }

            // sort last list
            {
                Int32 nextDepth = Convert.ToInt32(currentDepth + 1);
                finalOutList.AddRange(GroupSort.SortStrings(characterFoundList, nextDepth));
            }

            return finalOutList;
        }

        private static List<CharacterGroup> QuickSortStructures(List<CharacterGroup> arr)
        {
            List<CharacterGroup> loe = new List<CharacterGroup>(), gt = new List<CharacterGroup>();
            if (arr.Count < 2)
                return arr;
            int pivot = arr.Count / 2;
            CharacterGroup pivotVal = arr[pivot];
            arr.RemoveAt(pivot);
            foreach (CharacterGroup i in arr)
            {
                if (i.CurrentCharacter.CompareTo(pivotVal.CurrentCharacter) <= 0)
                    loe.Add(i);
                else if (i.CurrentCharacter.CompareTo(pivotVal.CurrentCharacter) > 0)
                    gt.Add(i);
            }

            List<CharacterGroup> resultSet = new List<CharacterGroup>();
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
