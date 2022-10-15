using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SortingAlgorithms.GroupSort.Strings;

namespace SortingAlgorithms
{
    public class GroupSort
    {
        public class Strings
        {
            private static readonly double _algorithmSwitchoverThreshold = .1;

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

            public List<string> SortStrings(List<string> listToSort)
            {
                return SortStrings(listToSort, 0);
            }

            private List<string> SortStrings(List<string> listToSort, int currentDepth)
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
                        finalOutList.AddRange(SortStrings(characterFoundList, currentDepth + 1));


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
                    finalOutList.AddRange(SortStrings(characterFoundList, currentDepth + 1));
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

            public class Range
            {
                private int _startIndex;
                public int StartIndex
                {
                    get { return _startIndex; }
                    set { _startIndex = value; }
                }

                private int _endIndex;
                public int EndIndex
                {
                    get { return _endIndex; }
                    set { _endIndex = value; }
                }

                public Range(int startIndex, int endIndex)
                {
                    _startIndex = startIndex;
                    _endIndex = endIndex;
                }

                public Range(Range range)
                {
                    _startIndex = range.StartIndex;
                    _endIndex = range.EndIndex;
                }
            }

            public class CharRange
            {
                private char _currentChar;
                public char CurrentChar
                {
                    get { return _currentChar; }
                    set { _currentChar = value; }
                }

                private Range _indexRange;
                public Range IndexRange
                {
                    get { return _indexRange; }
                    set { _indexRange = value; }
                }

                public CharRange(char character, Range range)
                {
                    _currentChar = character;
                    _indexRange = range;
                }
            }

            public class DepthRange
            {
                // Ones-based depth location within a string.
                // Depth of 1 means you're analyzing the first character of the string, at location [0].
                private int _charDepth;
                public int CharDepth
                {
                    get { return _charDepth; }
                    set { _charDepth = value; }
                }

                private Range _indexRange;
                public Range IndexRange
                {
                    get { return _indexRange; }
                    set { _indexRange = value; }
                }

                public DepthRange(int depth, Range range)
                {
                    _charDepth = depth;
                    _indexRange = range;
                }
            }

            public class CharGroup
            {
                private char _currentChar;
                public char CurrentChar
                {
                    get { return _currentChar; }
                    set { _currentChar = value; }
                }

                private List<Range> _indexRanges;
                public List<Range> IndexRanges
                {
                    get { return _indexRanges; }
                    set { _indexRanges = value; }
                }

                public CharGroup(char currentChar, List<Range> ranges)
                {
                    _currentChar = currentChar;
                    _indexRanges = ranges;
                }
            }

            public void SortStringsNoRecursion(ref List<string> stringList)
            {
                if (stringList.Count == 0)
                {
                    return;
                }


                Stack<DepthRange> stack = new Stack<DepthRange>();
                int startIndex = 0;
                int endIndex = stringList.Count - 1;

                // Add the first DepthRange to the stack. At the first level, the DepthRange 
                // will contain a single range which encompasses the entire list.
                // With each successive Push to the stack, that range will shrink, 
                // and the frequency of Pushes will increase.
                stack.Push(new DepthRange(1, new Range(startIndex, endIndex)));

                while (stack.Count > 0)
                {
                    DepthRange currentDepthRange = stack.Peek();
                    stack.Pop();

                    RemoveShortStringsFromRange(ref stringList, ref currentDepthRange);
                    if (currentDepthRange.IndexRange.StartIndex > currentDepthRange.IndexRange.EndIndex)
                    {
                        // In the case where the current DepthRange consists entirely of short strings, 
                        // stop processing as no further sorting is necessary.
                        continue;
                    }

                    List<CharRange> charRangesAtGivenDepth = BuildGroups(stringList, currentDepthRange);

                    Structures.SortGroupsNoRecursion(ref charRangesAtGivenDepth);
                    SortList(ref stringList, charRangesAtGivenDepth, currentDepthRange);

                    List<DepthRange> depthRanges = CombineLikeGroups(charRangesAtGivenDepth, currentDepthRange);

                    foreach (DepthRange depthRange in depthRanges)
                    {
                        if (depthRange.IndexRange.EndIndex > depthRange.IndexRange.StartIndex)
                        {
                            stack.Push(depthRange);
                        }
                    }
                }

            }

            // We are sorting strings, one character at a time.
            // Depth is used to indicate which character of the strings we are sorting.
            // If a string is shorter than the depth, the string should be moved 
            // to the beginning of the range and not sorted further.
            // Also update the range to excluded the already sorted strings.
            private void RemoveShortStringsFromRange(ref List<string> stringList, ref DepthRange currentDepthRange)
            {
                int startIndex = currentDepthRange.IndexRange.StartIndex;
                int endIndex = currentDepthRange.IndexRange.EndIndex;

                int insertIndex = startIndex;
                for (int stringListIndex = startIndex; stringListIndex <= endIndex; stringListIndex++)
                {
                    if (stringList[stringListIndex].Length < currentDepthRange.CharDepth)
                    {
                        // String is too short to be sorted further, thus it is already sorted.
                        // Move it to the beginning of the range.
                        if (stringListIndex > insertIndex)
                        {
                            stringList.Insert(insertIndex, stringList[stringListIndex]);
                            stringList.RemoveAt(stringListIndex + 1);
                        }
                        insertIndex++;
                    }
                }

                currentDepthRange.IndexRange.StartIndex = insertIndex;
            }

            // This method builds a List of CharRange structures that represents a subset of the stringList 
            // according to the criteria specified by currentDepthRange.
            private List<CharRange> BuildGroups(List<string> stringList, DepthRange currentDepthRange)
            {
                int depthIndex = currentDepthRange.CharDepth - 1;
                int lowIndex = currentDepthRange.IndexRange.StartIndex;
                int highIndex = currentDepthRange.IndexRange.EndIndex;

                List<CharRange> charRanges = new List<CharRange>();
                int newStartIndex = 0;
                int newEndIndex = 0;
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
                            charRanges.Add(new CharRange(newChar, new Range(newStartIndex, newEndIndex)));

                            newChar = stringList[stringIndex][depthIndex];
                            newStartIndex = stringIndex;
                        }
                    }
                }
                if (stringIndex > highIndex)
                {
                    newEndIndex = stringIndex - 1;
                    charRanges.Add(new CharRange(newChar, new Range(newStartIndex, newEndIndex)));
                }

                return charRanges;
            }

            private void SortList(ref List<string> stringList, List<CharRange> sortedCharRanges, DepthRange currentDepthRange)
            {
                List<string> sortedStrings = new List<string>();

                foreach (CharRange range in sortedCharRanges)
                {
                    for (int sourceIndex = range.IndexRange.StartIndex; sourceIndex <= range.IndexRange.EndIndex; sourceIndex++)
                    {
                        sortedStrings.Add(stringList[sourceIndex]);
                    }
                }

                stringList.RemoveRange(currentDepthRange.IndexRange.StartIndex, sortedStrings.Count);
                stringList.InsertRange(currentDepthRange.IndexRange.StartIndex, sortedStrings);
            }

            private List<DepthRange> CombineLikeGroups(List<CharRange> charRanges, DepthRange currentDepthRange)
            {
                List<DepthRange> depthRanges = new List<DepthRange>();
                int rangeIndex;
                char newChar = ' ';

                int newStartIndex = -1;
                int newEndIndex = -1;
                int stringCounter = 0;

                for (rangeIndex = 0; rangeIndex < charRanges.Count; rangeIndex++)
                {
                    if (rangeIndex == 0)
                    {
                        newChar = charRanges[rangeIndex].CurrentChar;
                        newStartIndex = stringCounter;
                        stringCounter += charRanges[rangeIndex].IndexRange.EndIndex - charRanges[rangeIndex].IndexRange.StartIndex + 1;
                    }
                    else
                    {
                        if (charRanges[rangeIndex].CurrentChar == newChar)
                        {
                            stringCounter += charRanges[rangeIndex].IndexRange.EndIndex - charRanges[rangeIndex].IndexRange.StartIndex + 1;
                        }
                        else
                        {
                            newEndIndex = stringCounter - 1;
                            depthRanges.Add(new DepthRange(currentDepthRange.CharDepth + 1, new Range(newStartIndex, newEndIndex)));

                            newChar = charRanges[rangeIndex].CurrentChar;
                            newStartIndex = stringCounter;
                            stringCounter += charRanges[rangeIndex].IndexRange.EndIndex - charRanges[rangeIndex].IndexRange.StartIndex + 1;
                        }
                    }
                }
                if (rangeIndex == charRanges.Count)
                {
                    if (stringCounter > 0)
                    {
                        newEndIndex = stringCounter - 1;
                        depthRanges.Add(new DepthRange(currentDepthRange.CharDepth + 1, new Range(newStartIndex, newEndIndex)));
                    }
                }

                return depthRanges;
            }

        }

        private class Structures
        {
            private class Boundary
            {
                private int _startIndex;
                private int _endIndex;

                public int StartIndex
                {
                    get { return _startIndex; }
                    set { _startIndex = value; }
                }
                public int EndIndex
                {
                    get
                    {
                        if (_endIndex < 0)
                            _endIndex = -1;
                        return _endIndex;
                    }
                    set 
                    {
                        if (value < 0)
                            _endIndex = -1;
                        _endIndex = value;
                    }
                }
                public Boundary(int startIndex, int endIndex)
                {
                    this._startIndex = startIndex;
                    this._endIndex = endIndex;
                }
            }

            public static void SortGroupsNoRecursion(ref List<CharRange> charRanges)
            {
                Stack<Boundary> stack = new Stack<Boundary>();
                int startIndex = 0;
                int endIndex = charRanges.Count - 1;

                stack.Push(new Boundary(startIndex, endIndex));

                while (stack.Count > 0)
                {
                    startIndex = stack.Peek().StartIndex;
                    endIndex = stack.Peek().EndIndex;
                    stack.Pop();
                    int pivotIndex = Partition(ref charRanges, startIndex, endIndex);
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


            // Swap the array element
            private static void Swap(ref List<CharRange> charRanges, int val1Index, int val2Index)
            {
                if (val1Index > val2Index)
                {
                    charRanges.Insert(val2Index, charRanges[val1Index]);
                    charRanges.Insert(val1Index + 1, charRanges[val2Index + 1]);
                    charRanges.RemoveAt(val1Index + 2);
                    charRanges.RemoveAt(val2Index + 1);
                }
                else if (val2Index > val1Index)
                {
                    charRanges.Insert(val1Index, charRanges[val2Index]);
                    charRanges.Insert(val2Index + 1, charRanges[val1Index + 1]);
                    charRanges.RemoveAt(val2Index + 2);
                    charRanges.RemoveAt(val1Index + 1);
                }
            }

            // Provide this function a subset of the original array or List.
            // This sorts the array such that smaller elements are to the left (low indices)
            // and larger elements are to the right (high indices).
            // Then it returns the starting index are the larger elements.
            // "Large" is defined by the pivot value selection, which is the last element of the subset.
            private static int Partition(ref List<CharRange> charRanges, int lowIndex, int highIndex)
            {
                // Set the high index element to its 
                // proper sorted position
                char pivotValue = charRanges[highIndex].CurrentChar;
                int i = lowIndex - 1;
                for (int j = lowIndex; j < highIndex; ++j)
                {
                    if (charRanges[j].CurrentChar.CompareTo(pivotValue) < 0)
                    //if (String.Compare(groups[j].CurrentChar, pivotValue, CultureInfo.CurrentCulture, CompareOptions.Ordinal) < 0)
                    //if (stringList[j] < pivotValue)
                    {
                        i++;
                        Swap(ref charRanges, i, j);
                    }
                }
                // Set the high index value to its sorted position
                Swap(ref charRanges, i + 1, highIndex);
                // Returns the next sorting  element location
                return i + 1;
            }
        }
    }
}
