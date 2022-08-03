using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
//using System.Threading;

namespace GroupSort
{
	class Program
	{
		static void Main(string[] args)
		{
			string input_folder = "D:\\(A) Professional\\Code\\Public\\Projects\\input\\";
			string input_file = "";

			List<string> all_input_files = new List<string>();
			all_input_files.Add("movies.csv");
			all_input_files.Add("moviesforwards.txt");
			all_input_files.Add("moviesbackwards.txt");
			all_input_files.Add("moviesforwardssplit.txt");
			all_input_files.Add("moviesrandomsmall.csv");
			all_input_files.Add("moviesrandomlarge.csv");
			all_input_files.Add("totallyrandom.txt");
			all_input_files.Add("InsuranceGroups.csv");

			int selection = GatherMenuSelection(all_input_files);
			while (selection >= 0)
			{
				List<string> list_to_sort = new List<string>();

				//public static string file_to_open = "C:\\testing\\movies.csv";
				//public static string file_to_open = "C:\\testing\\moviesforwards.txt";	// same as movies2
				//public static string file_to_open = "C:\\testing\\moviesbackwards.txt";
				//public static string file_to_open = "C:\\testing\\moviesforwardssplit.txt";
				//public static string file_to_open = "C:\\testing\\moviesrandomsmall.csv";
				//public static string file_to_open = "C:\\testing\\moviesrandomlarge.csv";

				// SELECTIONS
				System.Console.WriteLine("Processing " + all_input_files[selection - 1]);
				input_file = input_folder + all_input_files[selection - 1];

				if (BuildListFromFile(ref list_to_sort, input_file))
				{
					Stopwatch time_to_sort = new Stopwatch();
					List<string> groupsort_sorted_list = new List<string>();
					List<string> quicksort_sorted_list = new List<string>();
					List<string> groupsort_list_to_sort = list_to_sort.ToList<string>();
					List<string> quicksort_list_to_sort = list_to_sort.ToList<string>();

					groupsort_sorted_list.Clear();
					Console.WriteLine("-- GroupSort --");
					time_to_sort.Restart();
					groupsort_sorted_list = SortingAlgorithms.GroupSort.SortStrings(groupsort_list_to_sort);
					time_to_sort.Stop();
					PrintList(groupsort_sorted_list, time_to_sort);

					quicksort_sorted_list.Clear();
					Console.WriteLine("-- QuickSort --");
					time_to_sort.Restart();
					quicksort_sorted_list = SortingAlgorithms.QuickSort.SortStrings(quicksort_list_to_sort);
					time_to_sort.Stop();
					PrintList(quicksort_sorted_list, time_to_sort);
				}
				else
				{
					System.Console.WriteLine("Unable to build a list from " + input_file);
				}

				selection = GatherMenuSelection(all_input_files);
			}

			System.Console.WriteLine("");
			System.Console.WriteLine("Press any key to exit...");
			System.Console.Read();
			//SortingAlgorithms.QuickSort.
		}

		static int GatherMenuSelection(List<string> all_input_files)
		{
			// SELECTIONS
			System.Console.WriteLine("-------------------------");
			System.Console.WriteLine("Select your option:");
			for (int fileIndex = 0; fileIndex < all_input_files.Count; fileIndex++)
			{
				int file_number = fileIndex + 1;
				System.Console.WriteLine(file_number + " - " + all_input_files[fileIndex]);
			}
			System.Console.WriteLine("-------------------------");
			string input_string = System.Console.ReadLine();
			//System.Console.WriteLine(input_string);

			int selection = -1;
			try
			{
				int input_int = Convert.ToInt16(input_string);
				if ((input_int >= 1) && (input_int <= all_input_files.Count))
				{
					selection = input_int;
				}
			}
			catch
			{
				System.Console.WriteLine("Invalid input " + input_string);
			}

			return selection;
		}

		private static Boolean BuildListFromFile(ref List<string> list_to_build, string input_file)
		{
			FileStream my_file;
			try
			{
				my_file = File.OpenRead(input_file);
			}
			catch
			{
				System.Console.WriteLine("Unable to open file: ");
				System.Console.WriteLine(input_file);
				return false;
			}

			string[] my_list = File.ReadAllLines(input_file, Encoding.ASCII);

			list_to_build.Clear();
			for (int listIndex = 0; listIndex < my_list.Length; listIndex++)
			{
				list_to_build.Add(my_list[listIndex]);
			}

			return true;
		}

		private static void PrintList(List<string> list_to_print, Stopwatch time_to_sort)
		{
			//System.Console.WriteLine("List: ");
			//for (Int32 listIndex = 0; listIndex < list_to_print.Count; listIndex++)
			//{
			//	System.Console.WriteLine(list_to_print[listIndex].ToString());
			//}
			System.Console.WriteLine(list_to_print.Count + " items in list");
			long million = 1000L * 1000L;
			long billion = 1000L * 1000L * 1000L;
			string microseconds;
			string nanoseconds;
			try
			{
				microseconds = Convert.ToString((time_to_sort.ElapsedTicks / (Stopwatch.Frequency / million)) % 1000);
			}
			catch
			{
				microseconds = "*";
			}
			try
			{
				nanoseconds = Convert.ToString(time_to_sort.ElapsedTicks / (Stopwatch.Frequency / billion));
			}
			catch
			{
				nanoseconds = "*";
			}

			string timeString = "";
			if (time_to_sort.Elapsed.Minutes > 0)
			{
				timeString += time_to_sort.Elapsed.Minutes + "m";
			}
			if (time_to_sort.Elapsed.Seconds > 0)
			{
				timeString += time_to_sort.Elapsed.Seconds + "s";
			}
			if (time_to_sort.Elapsed.Milliseconds > 0)
			{
				timeString += time_to_sort.Elapsed.Milliseconds + "ms";
			}
			if (microseconds != "*")
			{
				timeString += microseconds + "us";
			}
/*
			if (nanoseconds != "*")
			{
				timeString += nanoseconds + "ns";
			}
*/
			//System.Console.WriteLine("Time: " + time_to_sort.Elapsed.Minutes + "m" + time_to_sort.Elapsed.Seconds + "s" + time_to_sort.Elapsed.Milliseconds + "ms" + microseconds + "us"/* + nanoseconds + "ns"*/);
			System.Console.WriteLine("Time: " + timeString);
			System.Console.WriteLine("");
		}

	}
}
