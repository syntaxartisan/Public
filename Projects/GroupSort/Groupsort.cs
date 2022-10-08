using System;
using System.Collections.Generic;
using System.Globalization;
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
            // A CharacterGroup represents a subset of elements of a List<string>.
            // The subset is specified by the location in the List<string> where the elements are located, 
            // the number of contiguous elements in the subset, 
            // and what character those elements are represented by.

            // For example, if there were a List<string> containing 1000 elements,
            // CurrentCharacter = 'c', CountDigitsInGroup = 5, GroupOffset = 30,
            // then that means that there are 5 elements in the List<string>
            // at offset 30 which all share the character 'c'.

            public char CurrentCharacter { get; set; }

            public int CountDigitsInGroup { get; set; }

            public int GroupOffset { get; set; }

            public CharacterGroup(char inCurrentCharacter, int inCountDigitsInGroup, int inGroupOffset)
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

        private static List<string> SortStrings(List<string> listToSort, int currentDepth)
        {
            if (listToSort.Count == 0)
            {
                return new List<string>();
            }

            List<string> finalOutList = new List<string>();
            List<CharacterGroup> characters = new List<CharacterGroup>();

            char currentGroupCharacter = ' ';
            int currentGroupCount = 0;
            int currentGroupOffset = 0;
            int threshold = Convert.ToInt32(listToSort.Count * _algorithmSwitchoverThreshold);
            int listIndex;

            // Build a list of CharacterGroups (characters) from the incoming list of strings (listToSort).
            for (listIndex = 0; listIndex < listToSort.Count; listIndex++)
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
                        finalOutList = QuickSort.Strings.SortStrings(listToSort);
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
                            finalOutList = QuickSort.Strings.SortStrings(listToSort);
                            return finalOutList;
                        }
                    }
                }
            }
            if (listIndex == listToSort.Count)
            {
                if (currentGroupCount > 0)
                {
                    // The last group is still in memory, add it to characters.
                    characters.Add(new CharacterGroup(currentGroupCharacter, currentGroupCount, currentGroupOffset));
                }
            }


            // characters now has all the items in it


            List<CharacterGroup> charactersSorted = QuickSortStructures(characters);

            List<string> characterFoundList = new List<string>();
            char currCharacter = ' ';
            int groupIndex;

            for (groupIndex = 0; groupIndex < charactersSorted.Count; groupIndex++)
            {
                if (groupIndex == 0)
                {
                    // This is the first pass. All items from the original list that pertain to this group can be added to a new list.
                    currCharacter = charactersSorted[groupIndex].CurrentCharacter;
                    characterFoundList.Clear();
                    for (int groupItemIndex = 0; groupItemIndex < charactersSorted[groupIndex].CountDigitsInGroup; groupItemIndex++)
                    {
                        characterFoundList.Add(listToSort[charactersSorted[groupIndex].GroupOffset + groupItemIndex]);
                    }
                    continue;
                }

                if (charactersSorted[groupIndex].CurrentCharacter == currCharacter)
                {
                    // This character matches the current character being searched for.
                    // Add the items from this group to the list too.
                    for (int groupItemIndex = 0; groupItemIndex < charactersSorted[groupIndex].CountDigitsInGroup; groupItemIndex++)
                    {
                        characterFoundList.Add(listToSort[charactersSorted[groupIndex].GroupOffset + groupItemIndex]);
                    }
                    continue;
                }
                else
                {
                    // This is a new character. Sort the existing list and add it to our final list.
                    finalOutList.AddRange(GroupSort.SortStrings(characterFoundList, currentDepth + 1));


                    // Start a new list for the new character.
                    currCharacter = charactersSorted[groupIndex].CurrentCharacter;
                    characterFoundList.Clear();
                    for (int groupItemIndex = 0; groupItemIndex < charactersSorted[groupIndex].CountDigitsInGroup; groupItemIndex++)
                    {
                        characterFoundList.Add(listToSort[charactersSorted[groupIndex].GroupOffset + groupItemIndex]);
                    }
                    continue;
                }
            }
            if (groupIndex == charactersSorted.Count)
            {
                // sort last list
                finalOutList.AddRange(GroupSort.SortStrings(characterFoundList, currentDepth + 1));
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
                //if (String.Compare(i.CurrentCharacter.ToString(), pivotVal.CurrentCharacter.ToString(), CultureInfo.CurrentCulture, CompareOptions.Ordinal) <= 0)
                //    loe.Add(i);
                //else if (String.Compare(i.CurrentCharacter.ToString(), pivotVal.CurrentCharacter.ToString(), CultureInfo.CurrentCulture, CompareOptions.Ordinal) > 0)
                //    gt.Add(i);

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
