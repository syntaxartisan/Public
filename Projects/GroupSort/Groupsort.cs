using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            public class CharGroup
            {
                private char _currentChar;
                private CharRange _range;

                public char CurrentChar
                {
                    get { return _currentChar; }
                    set { _currentChar = value; }
                }
                public CharRange Range
                {
                    get { return _range; }
                    set { _range = value; }
                }

                public class CharRange
                {
                    private int _startIndex;
                    private int _endIndex;
                    private int _charDepth;

                    public int StartIndex
                    {
                        get { return _startIndex; }
                        set { _startIndex = value; }
                    }

                    public int EndIndex
                    {
                        get { return _endIndex; }
                        set { _endIndex = value; }
                    }

                    public int CharDepth
                    {
                        get { return _charDepth; }
                        set { _charDepth = value; }
                    }

                    public CharRange(int startIndex, int endIndex, int charDepth)
                    {
                        _startIndex = startIndex;
                        _endIndex = endIndex;
                        _charDepth = charDepth;
                    }
                }

                public CharGroup(char currentChar, CharRange range)
                //public CharGroup(int charDepth, List<CharRange> ranges)
                //public CharGroup(char currentChar, int startIndex, int endIndex, int charDepth)
                {
                    _currentChar = currentChar;
                    _range = range;
                }
            }

            // unsortedStringList will be left untouched by this method.
            public List<string> SortStringsNoRecursion(List<string> unsortedStringList)
            {
                if (unsortedStringList.Count == 0)
                {
                    return new List<string>();
                }


                List<string> sortedStringList = unsortedStringList.ToList<string>();
                Stack<CharGroup.CharRange> stack = new Stack<CharGroup.CharRange>();
                int startIndex = 0;
                int endIndex = sortedStringList.Count - 1;
                int charDepth = 0;

                // Add the first CharRange to the stack. At the first level, the CharRange 
                // will contain a single range which encompasses the entire list.
                // With each successive Push to the stack, that range will shrink, 
                // and the frequency of Pushes will increase.
                //stack.Push(new CharGroup(charDepth, new List<CharGroup.CharRange> { new CharGroup.CharRange(startIndex, endIndex) }));
                stack.Push(new CharGroup.CharRange(startIndex, endIndex, charDepth));

                int counter = 0;
                while (stack.Count > 0)
                {
                    startIndex = stack.Peek().StartIndex;
                    endIndex = stack.Peek().EndIndex;
                    charDepth = stack.Peek().CharDepth;
                    stack.Pop();

                    List<CharGroup> groups = BuildGroups(sortedStringList, startIndex, endIndex, charDepth);
                    SortGroups(ref groups);
                    sortedStringList = SortList(sortedStringList, groups);
                    foreach (CharGroup group in groups)
                    {
                        stack.Push(group.Range);
                    }
                    counter++;
                }

                return sortedStringList;
            }
            private List<CharGroup> BuildGroups(List<string> stringList, int lowIndex, int highIndex, int depth)
            {
                List<CharGroup> groups = new List<CharGroup>();
                int newStartIndex = 0;
                int newEndIndex = 0;
                int newDepth = depth + 1;
                char currentChar = ' ';
                int stringIndex;

                for (stringIndex = lowIndex; stringIndex <= highIndex; stringIndex++)
                {
                    if (stringIndex == lowIndex)
                    {
                        currentChar = stringList[stringIndex][depth];
                        newStartIndex = stringIndex;
                    }
                    else
                    {
                        if (stringList[stringIndex][depth] != currentChar)
                        {
                            newEndIndex = stringIndex - 1;
                            groups.Add(new CharGroup(currentChar, new CharGroup.CharRange(newStartIndex, newEndIndex, newDepth)));

                            currentChar = stringList[stringIndex][depth];
                            newStartIndex = stringIndex;
                        }
                    }
                }
                if (stringIndex > highIndex)
                {
                    newEndIndex = stringIndex - 1;
                    groups.Add(new CharGroup(currentChar, new CharGroup.CharRange(newStartIndex, newEndIndex, newDepth)));
                }

                return groups;
            }

            private void SortGroups(ref List<CharGroup> groups)
            {
                Structures.SortGroupsNoRecursion(ref groups);
            }
            private List<string> SortList(List<string> stringList, List<CharGroup> groups)
            {
                List<string> newStringList = new List<string>();
                foreach (CharGroup group in groups)
                {
                    for (int stringIndex = group.Range.StartIndex; stringIndex <= group.Range.EndIndex; stringIndex++)
                    {
                        newStringList.Add(stringList[stringIndex]);
                    }
                }

                return newStringList;
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

            public static void SortGroupsNoRecursion(ref List<Strings.CharGroup> groups)
            {
                Stack<Boundary> stack = new Stack<Boundary>();
                int startIndex = 0;
                int endIndex = groups.Count - 1;

                stack.Push(new Boundary(startIndex, endIndex));

                int counter = 0;//##remove
                while (stack.Count > 0)
                {
                    startIndex = stack.Peek().StartIndex;
                    endIndex = stack.Peek().EndIndex;
                    stack.Pop();
                    int pivotIndex = Partition(ref groups, startIndex, endIndex);
                    if (pivotIndex - 1 > startIndex)
                    {
                        stack.Push(new Boundary(startIndex, pivotIndex - 1));
                    }
                    if (pivotIndex + 1 < endIndex)
                    {
                        stack.Push(new Boundary(pivotIndex + 1, endIndex));
                    }
                    counter++;
                }
                Console.WriteLine(counter);//##remove
            }


            // Swap the array element
            private static void Swap(ref List<Strings.CharGroup> groups, int val1Index, int val2Index)
            {
                if (val1Index > val2Index)
                {
                    groups.Insert(val2Index, groups[val1Index]);
                    groups.Insert(val1Index + 1, groups[val2Index + 1]);
                    groups.RemoveAt(val1Index + 2);
                    groups.RemoveAt(val2Index + 1);
                }
                else if (val2Index > val1Index)
                {
                    groups.Insert(val1Index, groups[val2Index]);
                    groups.Insert(val2Index + 1, groups[val1Index + 1]);
                    groups.RemoveAt(val2Index + 2);
                    groups.RemoveAt(val1Index + 1);
                }
                //string temp = stringList[val1Index];
                //stringList[val1Index] = stringList[val2Index];
                //stringList[val2Index] = temp;
            }

            // Provide this function a subset of the original array or List.
            // This sorts the array such that smaller elements are to the left (low indices)
            // and larger elements are to the right (high indices).
            // Then it returns the starting index are the larger elements.
            // "Large" is defined by the pivot value selection, which is the last element of the subset.

            // Provide this function a subset of the original array or List.
            // This sorts the array such that smaller elements are to the left (low indices)
            // and larger elements are to the right (high indices).
            // Then it returns the starting index are the larger elements.
            // "Large" is defined by the pivot value selection, which is the last element of the subset.
            private static int Partition(ref List<Strings.CharGroup> groups, int lowIndex, int highIndex)
            {
                // Set the high index element to its 
                // proper sorted position
                char pivotValue = groups[highIndex].CurrentChar;
                int i = lowIndex - 1;
                for (int j = lowIndex; j < highIndex; ++j)
                {
                    if (groups[j].CurrentChar.CompareTo(pivotValue) < 0)
                    //if (String.Compare(groups[j].CurrentChar, pivotValue, CultureInfo.CurrentCulture, CompareOptions.Ordinal) < 0)
                    //if (stringList[j] < pivotValue)
                    {
                        i++;
                        Swap(ref groups, i, j);
                    }
                }
                // Set the high index value to its sorted position
                Swap(ref groups, i + 1, highIndex);
                // Returns the next sorting  element location
                return i + 1;
            }
        }
    }
}
