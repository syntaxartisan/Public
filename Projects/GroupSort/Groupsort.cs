using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Artisan
{
    namespace Sorting
    {

        public class GroupSort
        {
            public class RecursiveSortStrings
            {
                private static readonly double _algorithmSwitchoverThreshold = .1;

                private class CharacterGroup
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

                public List<string> Sort(List<string> listToSort)
                {
                    return Sort(listToSort, 0);
                }

                private List<string> Sort(List<string> listToSort, int currentDepth)
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
                                QuickSort.RecursiveSortStrings stringQuickSorter = new QuickSort.RecursiveSortStrings();
                                finalOutList = stringQuickSorter.Sort(listToSort);
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
                                    QuickSort.RecursiveSortStrings stringQuickSorter = new QuickSort.RecursiveSortStrings();
                                    finalOutList = stringQuickSorter.Sort(listToSort);
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
                            finalOutList.AddRange(Sort(characterFoundList, currentDepth + 1));


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
                        finalOutList.AddRange(Sort(characterFoundList, currentDepth + 1));
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
                    for (int charIndex = 0; charIndex < pivot; charIndex++)
                    {
                        CharacterGroup currentGroup = arr[charIndex];

                        if (currentGroup.CurrentCharacter.CompareTo(pivotVal.CurrentCharacter) <= 0)
                            loe.Add(currentGroup);
                        else if (currentGroup.CurrentCharacter.CompareTo(pivotVal.CurrentCharacter) > 0)
                            gt.Add(currentGroup);
                    }
                    for (int charIndex = pivot + 1; charIndex < arr.Count; charIndex++)
                    {
                        CharacterGroup currentGroup = arr[charIndex];

                        if (currentGroup.CurrentCharacter.CompareTo(pivotVal.CurrentCharacter) <= 0)
                            loe.Add(currentGroup);
                        else if (currentGroup.CurrentCharacter.CompareTo(pivotVal.CurrentCharacter) > 0)
                            gt.Add(currentGroup);
                    }

                    List<CharacterGroup> resultSet = new List<CharacterGroup>();
                    resultSet.AddRange(QuickSortStructures(loe));
                    if (loe.Count == 0)
                    {
                        resultSet.Add(pivotVal);
                    }
                    else
                    {
                        gt.Insert(0, pivotVal);
                    }
                    resultSet.AddRange(QuickSortStructures(gt));
                    return resultSet;
                }
            } // class RecursiveSortStrings

            public class SortStrings
            {
                private class Range
                {
                    public int StartIndex { get; set; }
                    public int EndIndex { get; set; }

                    public Range(int startIndex, int endIndex)
                    {
                        StartIndex = startIndex;
                        EndIndex = endIndex;
                    }

                    public Range(Range range)
                    {
                        StartIndex = range.StartIndex;
                        EndIndex = range.EndIndex;
                    }
                }

                private class CharRange
                {
                    public char CurrentChar { get; set; }
                    public Range IndexRange { get; set; }

                    public CharRange(char character, Range range)
                    {
                        CurrentChar = character;
                        IndexRange = range;
                    }
                }

                private class DepthRange
                {
                    // CharDepth is ones-based depth location within a string.
                    // Depth of 1 means you're analyzing the first character of the string, at location [0].
                    public int CharDepth { get; set; }
                    public Range IndexRange { get; set; }

                    public DepthRange(int depth, Range range)
                    {
                        CharDepth = depth;
                        IndexRange = range;
                    }
                }

                private class CharGroup
                {
                    public char CurrentChar { get; set; }
                    public List<Range> IndexRanges { get; set; }

                    public CharGroup(char currentChar, List<Range> ranges)
                    {
                        CurrentChar = currentChar;
                        IndexRanges = ranges;
                    }
                }

                public List<string> Sort(List<string> unsortedStringList)
                {
                    List<string> sortedStringList = unsortedStringList.ToList<string>();
                    if (sortedStringList.Count == 0)
                    {
                        return sortedStringList;
                    }


                    Stack<DepthRange> stack = new Stack<DepthRange>();
                    int startIndex = 0;
                    int endIndex = sortedStringList.Count - 1;

                    // Add the first DepthRange to the stack. At the first level, the DepthRange 
                    // will contain a single range which encompasses the entire list.
                    // With each successive Push to the stack, that range will shrink, 
                    // and the frequency of Pushes will increase.
                    stack.Push(new DepthRange(1, new Range(startIndex, endIndex)));

                    while (stack.Count > 0)
                    {
                        DepthRange sortCriteria = stack.Peek();
                        stack.Pop();

                        CharGroup shortStringsGroup;
                        List<CharGroup> charGroups = BuildCharGroups(out shortStringsGroup, sortedStringList, sortCriteria);

                        Structures.SortGroupsNoRecursion(charGroups);
                        SortList(sortedStringList, shortStringsGroup, charGroups, sortCriteria);

                        AddRangesToStack(stack, sortedStringList, sortCriteria);

                    }

                    return sortedStringList;
                }

                // This method builds a List of CharGroup structures that represents a subset of the stringList 
                // according to the criteria specified by sortCriteria.
                private List<CharGroup> BuildCharGroups(out CharGroup shortStringsGroup, List<string> stringList, DepthRange sortCriteria)
                {
                    int depthIndex = sortCriteria.CharDepth - 1;
                    int lowIndex = sortCriteria.IndexRange.StartIndex;
                    int highIndex = sortCriteria.IndexRange.EndIndex;

                    List<Range> shortStringsRanges = new List<Range>();
                    List<CharGroup> charGroups = new List<CharGroup>();
                    int newStartIndex = -1;
                    int newEndIndex = -1;
                    char newChar = ' ';
                    int stringIndex;
                    bool rangeOfStringsIsSorted = false;
                    bool nextStringIsSorted = false;

                    for (stringIndex = lowIndex; stringIndex <= highIndex; stringIndex++)
                    {
                        if (stringIndex == lowIndex)
                        {
                            if (StringIsSorted(stringList, stringIndex, sortCriteria.CharDepth))
                            {
                                rangeOfStringsIsSorted = true;
                            }
                            else
                            {
                                newChar = stringList[stringIndex][depthIndex];
                                rangeOfStringsIsSorted = false;
                            }
                            newStartIndex = stringIndex;
                        }
                        else
                        {
                            // Save the range to shortStringsRanges when the strings are short (i.e. already sorted).
                            // Save the range to charGroups when the strings are long enough to need further sorting.
                            // Switching between saving to shortStringsRanges and charGroups is an implied character change.

                            nextStringIsSorted = StringIsSorted(stringList, stringIndex, sortCriteria.CharDepth);
                            if (rangeOfStringsIsSorted && !nextStringIsSorted)
                            {
                                newEndIndex = stringIndex - 1;
                                shortStringsRanges.Add(new Range(newStartIndex, newEndIndex));

                                newChar = stringList[stringIndex][depthIndex];
                                newStartIndex = stringIndex;

                                rangeOfStringsIsSorted = false;
                            }
                            else if (!rangeOfStringsIsSorted && nextStringIsSorted)
                            {
                                newEndIndex = stringIndex - 1;
                                AddCharRangeToCharGroup(charGroups, new CharRange(newChar, new Range(newStartIndex, newEndIndex)));

                                newStartIndex = stringIndex;

                                rangeOfStringsIsSorted = true;
                            }
                            else if (!rangeOfStringsIsSorted && !nextStringIsSorted)
                            {
                                if (stringList[stringIndex][depthIndex] != newChar)
                                {
                                    newEndIndex = stringIndex - 1;
                                    AddCharRangeToCharGroup(charGroups, new CharRange(newChar, new Range(newStartIndex, newEndIndex)));

                                    newChar = stringList[stringIndex][depthIndex];
                                    newStartIndex = stringIndex;
                                }
                            }
                        }
                    }
                    if (stringIndex > highIndex)
                    {
                        if (newStartIndex > -1)
                        {
                            newEndIndex = stringIndex - 1;

                            if (rangeOfStringsIsSorted)
                            {
                                shortStringsRanges.Add(new Range(newStartIndex, newEndIndex));
                            }
                            else
                            {
                                AddCharRangeToCharGroup(charGroups, new CharRange(newChar, new Range(newStartIndex, newEndIndex)));
                            }
                        }
                    }

                    shortStringsGroup = new CharGroup(' ', shortStringsRanges);

                    return charGroups;
                }

                private bool StringIsSorted(List<string> stringList, int stringIndex, int charDepth)
                {
                    if (stringList[stringIndex].Length < charDepth)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                private void AddCharRangeToCharGroup(List<CharGroup> charGroups, CharRange newCharRange)
                {
                    int groupIndex = 0;

                    for (groupIndex = 0; groupIndex < charGroups.Count; groupIndex++)
                    {
                        if (charGroups[groupIndex].CurrentChar == newCharRange.CurrentChar)
                        {
                            charGroups[groupIndex].IndexRanges.Add(newCharRange.IndexRange);
                            break;
                        }
                    }
                    if (groupIndex == charGroups.Count)
                    {
                        List<Range> newList = new List<Range>();
                        newList.Add(newCharRange.IndexRange);
                        charGroups.Add(new CharGroup(newCharRange.CurrentChar, newList));
                    }
                }

                // Sort the string list at the current depth, for the given range, based upon the list of 
                // sorted ranges that are supplied.
                private void SortList(List<string> stringList, CharGroup shortStringsGroup, List<CharGroup> sortedCharGroups, DepthRange sortCriteria)
                {
                    List<string> sortedStrings = new List<string>();

                    // Strings in shortStringsGroup are sorted and can be moved to the front of stringList.
                    for (int rangeIndex = 0; rangeIndex < shortStringsGroup.IndexRanges.Count; rangeIndex++)
                    {
                        Range range = shortStringsGroup.IndexRanges[rangeIndex];

                        for (int stringIndex = range.StartIndex; stringIndex <= range.EndIndex; stringIndex++)
                        {
                            sortedStrings.Add(stringList[stringIndex]);
                        }
                    }
                    int shortStringsCount = sortedStrings.Count;

                    // Strings in sortedCharGroups will need further sorting beyond the current iteration.
                    for (int groupIndex = 0; groupIndex < sortedCharGroups.Count; groupIndex++)
                    {
                        CharGroup group = sortedCharGroups[groupIndex];

                        for (int rangeIndex = 0; rangeIndex < group.IndexRanges.Count; rangeIndex++)
                        {
                            Range range = group.IndexRanges[rangeIndex];

                            for (int stringIndex = range.StartIndex; stringIndex <= range.EndIndex; stringIndex++)
                            {
                                sortedStrings.Add(stringList[stringIndex]);
                            }
                        }
                    }

                    stringList.RemoveRange(sortCriteria.IndexRange.StartIndex, sortedStrings.Count);
                    stringList.InsertRange(sortCriteria.IndexRange.StartIndex, sortedStrings);

                    // Short (sorted) strings have been moved to the front of the list.
                    // Remove them from sortCriteria range so that we don't attempt to sort them further.
                    sortCriteria.IndexRange.StartIndex += shortStringsCount;
                }

                // This method adds future iterations of sortings to the stack for processing.
                private void AddRangesToStack(Stack<DepthRange> stack, List<string> stringList, DepthRange sortCriteria)
                {
                    int depthIndex = sortCriteria.CharDepth - 1;
                    int lowIndex = sortCriteria.IndexRange.StartIndex;
                    int highIndex = sortCriteria.IndexRange.EndIndex;

                    int newStartIndex = -1;
                    int newEndIndex = -1;
                    char newChar = ' ';
                    int stringIndex;

                    for (stringIndex = lowIndex; stringIndex <= highIndex; stringIndex++)
                    {
                        if (stringIndex == lowIndex)
                        {
                            newChar = stringList[stringIndex][depthIndex];
                            newStartIndex = stringIndex;
                        }
                        else
                        {
                            if (stringList[stringIndex][depthIndex] != newChar)
                            {
                                newEndIndex = stringIndex - 1;
                                if (newStartIndex < newEndIndex)
                                {
                                    stack.Push(new DepthRange(sortCriteria.CharDepth + 1, new Range(newStartIndex, newEndIndex)));
                                }

                                newChar = stringList[stringIndex][depthIndex];
                                newStartIndex = stringIndex;
                            }
                        }
                    }
                    if (stringIndex > highIndex)
                    {
                        if (newStartIndex > -1)
                        {
                            newEndIndex = stringIndex - 1;
                            if (newStartIndex < newEndIndex)
                            {
                                stack.Push(new DepthRange(sortCriteria.CharDepth + 1, new Range(newStartIndex, newEndIndex)));
                            }
                        }
                    }

                }


                private class Structures
                {
                    private class Boundary
                    {
                        public int StartIndex { get; set; }
                        public int EndIndex { get; set; }

                        public Boundary(int startIndex, int endIndex)
                        {
                            StartIndex = startIndex;
                            EndIndex = endIndex;
                        }
                    }

                    public static void SortGroupsNoRecursion(List<CharGroup> charGroups)
                    {
                        Stack<Boundary> stack = new Stack<Boundary>();
                        int startIndex = 0;
                        int endIndex = charGroups.Count - 1;

                        if (endIndex >= 0)
                        {
                            stack.Push(new Boundary(startIndex, endIndex));

                            while (stack.Count > 0)
                            {
                                startIndex = stack.Peek().StartIndex;
                                endIndex = stack.Peek().EndIndex;
                                stack.Pop();
                                int pivotIndex = Partition(charGroups, startIndex, endIndex);
                                if (pivotIndex - 1 > startIndex)
                                {
                                    stack.Push(new Boundary(startIndex, pivotIndex - 1));
                                }
                                if (pivotIndex + 1 < endIndex)
                                {
                                    stack.Push(new Boundary(pivotIndex + 1, endIndex));
                                }
                            }
                        }
                    }


                    // Swap the array element
                    private static void Swap(List<CharGroup> charGroups, int val1Index, int val2Index)
                    {
                        CharGroup tempGroup = charGroups[val1Index];
                        charGroups[val1Index] = charGroups[val2Index];
                        charGroups[val2Index] = tempGroup;
                    }

                    // Provide this function a subset of the original array or List.
                    // This sorts the array such that smaller elements are to the left (low indices)
                    // and larger elements are to the right (high indices).
                    // Then it returns the starting index are the larger elements.
                    // "Large" is defined by the pivot value selection, which is the last element of the subset.
                    private static int Partition(List<CharGroup> charGroups, int lowIndex, int highIndex)
                    {
                        // Set the high index element to its 
                        // proper sorted position
                        char pivotValue = charGroups[highIndex].CurrentChar;
                        int i = lowIndex - 1;
                        for (int j = lowIndex; j < highIndex; ++j)
                        {
                            if (charGroups[j].CurrentChar.CompareTo(pivotValue) < 0)
                            //if (String.Compare(charGroups[j].CurrentChar.ToString(), pivotValue.ToString(), CultureInfo.CurrentCulture, CompareOptions.Ordinal) < 0)
                            //if (charGroups[j].CurrentChar < pivotValue)
                            {
                                i++;
                                Swap(charGroups, i, j);
                            }
                        }
                        // Set the high index value to its sorted position
                        Swap(charGroups, i + 1, highIndex);
                        // Returns the next sorting  element location
                        return i + 1;
                    }
                }

            } // class SortStrings
        } // class GroupSort

    }
}
