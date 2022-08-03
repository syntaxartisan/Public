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

	public class GroupSort
	{
		public static double algorithm_switchover_threshold_factor = .1;

		public class list_grouped
		{
			public char current_character { get; set; }
			public Int32 count_digits_in_group { get; set; }
			public Int32 group_offset { get; set; }

			public list_grouped(char in_current_character, Int32 in_count_digits_in_group, Int32 in_group_offset)
			{
				this.current_character = in_current_character;
				this.count_digits_in_group = in_count_digits_in_group;
				this.group_offset = in_group_offset;
			}
		}

		public static List<string> SortStrings(List<string> list_to_sort)
		{
			return SortStrings(list_to_sort, 0);
		}

		private static List<string> SortStrings(List<string> list_to_sort, Int32 current_depth/*, FallbackType insertion_sort, CharacterSet latin1*/)
		{
			if (list_to_sort.Count == 0)
			{
				return new List<string>();
			}
			//else if (list_to_sort.Count == 1)
			//{
			//	return list_to_sort;
			//}

			List<string> final_out_list = new List<string>();
			List<list_grouped> groups_in_list = new List<list_grouped>();

			char current_group_character = ' ';
			Int32 current_group_count = 0;
			Int32 current_group_offset = 0;
			Int32 threshold = Convert.ToInt32(list_to_sort.Count * algorithm_switchover_threshold_factor);

			for (Int32 listIndex = 0; listIndex < list_to_sort.Count; listIndex++)
			{
				if (list_to_sort[listIndex].Length <= current_depth)
				{
					final_out_list.Add(list_to_sort[listIndex]);

					groups_in_list.Add(new list_grouped(current_group_character, current_group_count, current_group_offset));

					if (groups_in_list.Count < threshold)
					{
						current_group_character = ' ';
						current_group_count = 0;
						current_group_offset = 0;
						continue;
					}
					else
					{
						// If we are here then we have determined that it will be quicker to use
						// QuickSort instead of GroupSort to sort our list. Step where we're at,
						// start resorting (this level and below) using QuickSort.
						final_out_list.Clear();
						final_out_list = QuickSort.SortStrings(list_to_sort);
						return final_out_list;
					}
				}

				if (current_group_count == 0)
				{
					current_group_character = list_to_sort[listIndex][current_depth];
					current_group_offset = listIndex;
					current_group_count = 1;
				}
				else
				{
					if (list_to_sort[listIndex][current_depth] == current_group_character)
					{
						current_group_count++;
					}
					else
					{
						groups_in_list.Add(new list_grouped(current_group_character, current_group_count, current_group_offset));

						if (groups_in_list.Count < threshold)
						{
							current_group_character = list_to_sort[listIndex][current_depth];
							current_group_offset = listIndex;
							current_group_count = 1;
						}
						else
						{
							// If we are here then we have determined that it will be quicker to use
							// QuickSort instead of GroupSort to sort our list. Step where we're at,
							// start resorting (this level and below) using QuickSort.
							final_out_list.Clear();
							final_out_list = QuickSort.SortStrings(list_to_sort);
							return final_out_list;
						}
					}
				}
			}

			// The last group is still in memory, add it to groups_in_list.
			if (current_group_count > 0)
			{
				groups_in_list.Add(new list_grouped(current_group_character, current_group_count, current_group_offset));
			}


			// groups_in_list now has all the items in it


			List<list_grouped> groups_in_list_sorted = QuickSort_Structures(groups_in_list);

			char current_character = ' ';

			List<string> character_found_list = new List<string>();

			for (Int32 eachGroupIndex = 0; eachGroupIndex < groups_in_list_sorted.Count; eachGroupIndex++)
			{
				if (eachGroupIndex == 0)
				{
					// This is the first pass. All items from the original list that pertain to this group can be added to a new list.
					current_character = groups_in_list_sorted[eachGroupIndex].current_character;
					character_found_list.Clear();
					for (Int32 groupItemIndex = 0; groupItemIndex < groups_in_list_sorted[eachGroupIndex].count_digits_in_group; groupItemIndex++)
					{
						character_found_list.Add(list_to_sort[groups_in_list_sorted[eachGroupIndex].group_offset + groupItemIndex]);
					}
					continue;
				}

				if (groups_in_list_sorted[eachGroupIndex].current_character == current_character)
				{
					// This character matches the current character being searched for.
					// Add the items from this group to the list too.
					for (Int32 groupItemIndex = 0; groupItemIndex < groups_in_list_sorted[eachGroupIndex].count_digits_in_group; groupItemIndex++)
					{
						character_found_list.Add(list_to_sort[groups_in_list_sorted[eachGroupIndex].group_offset + groupItemIndex]);
					}
					continue;
				}
				else
				{
					// This is a new character. Sort the existing list and add it to our final list.
					List<string> character_sorted_list = new List<string>();
					Int32 next_depth = Convert.ToInt32(current_depth + 1);
					character_sorted_list = GroupSort.SortStrings(character_found_list, next_depth);

					// This character is completely done. Add these items to the final list.
					for (Int32 listIndex = 0; listIndex < character_sorted_list.Count; listIndex++)
					{
						final_out_list.Add(character_sorted_list[listIndex]);
					}


					// Start a new list for the new character.
					current_character = groups_in_list_sorted[eachGroupIndex].current_character;
					character_found_list.Clear();
					for (Int32 groupItemIndex = 0; groupItemIndex < groups_in_list_sorted[eachGroupIndex].count_digits_in_group; groupItemIndex++)
					{
						character_found_list.Add(list_to_sort[groups_in_list_sorted[eachGroupIndex].group_offset + groupItemIndex]);
					}
					continue;
				}
			}

			// sort last list
			{
				List<string> character_sorted_list = new List<string>();
				Int32 next_depth = Convert.ToInt32(current_depth + 1);
				character_sorted_list = GroupSort.SortStrings(character_found_list, next_depth);

				for (Int32 listIndex = 0; listIndex < character_sorted_list.Count; listIndex++)
				{
					final_out_list.Add(character_sorted_list[listIndex]);
				}
			}

			return final_out_list;
		}

		private static List<list_grouped> QuickSort_Structures(List<list_grouped> arr)
		{
			List<list_grouped> loe = new List<list_grouped>(), gt = new List<list_grouped>();
			if (arr.Count < 2)
				return arr;
			int pivot = arr.Count / 2;
			list_grouped pivot_val = arr[pivot];
			arr.RemoveAt(pivot);
			foreach (list_grouped i in arr)
			{
				if (i.current_character.CompareTo(pivot_val.current_character) <= 0)
					loe.Add(i);
				else if (i.current_character.CompareTo(pivot_val.current_character) > 0)
					gt.Add(i);
			}

			List<list_grouped> resultSet = new List<list_grouped>();
			resultSet.AddRange(QuickSort_Structures(loe));
			if (loe.Count == 0)
			{
				resultSet.Add(pivot_val);
			}
			else
			{
				//gt.Add(pivot_val);
				gt.Insert(0, pivot_val);
			}
			resultSet.AddRange(QuickSort_Structures(gt));
			return resultSet;
		}

	}
}
