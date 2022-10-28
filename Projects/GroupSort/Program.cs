﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using SortingAlgorithms;
using System.Globalization;

class Program
{
    /* Conclusions:
	 * For a list of completely unique items, QuickSort is faster than GroupSort.
	 * For a list of completely duplicate items, GroupSort is faster than QuickSort.
	 * GroupSort is much faster at sorting duplicate items than QuickSort is at sorting unique items.
	 * In a list containing both unique and duplicate items, the percentage of duplicates has to be 
	 * relatively low in order for GroupSort and QuickSort to sort the list in the same amount of time.
	 * I'm seeing that the percentage of duplicates would need to stay below 10% in order for 
	 * QuickSort to be more viable than GroupSort for heftier lists. However I haven't run tests 
	 * in data sets of 1 million records or more.
	*/
    static class GlobalVariables
	{
        public const string inputFolder = @"D:\(A) Professional\Code\Public\Projects\input\";
        public const string outputFolder = @"D:\(A) Professional\Code\Public\Projects\GroupSort\output\";
    }

    static void Main(string[] args)
	{
        string inputFile = String.Empty;

		List<string> allInputFiles = new List<string>();
		//allInputFiles.Add("test1.txt"); // QuickSort is 4x faster
		//allInputFiles.Add("test2.txt"); // QuickSort is 2x faster
		//allInputFiles.Add("movies.csv"); // QuickSort is 2x faster
		//allInputFiles.Add("moviesforwards.txt"); // QuickSort is 6x faster
		//allInputFiles.Add("moviesbackwards.txt"); // GroupSort is 250x faster
		//allInputFiles.Add("moviesforwardssplit.txt"); // QuickSort is 3x faster
		//allInputFiles.Add("moviesrandomsmall.csv"); // QuickSort is 8x faster
		//allInputFiles.Add("moviesrandomlarge.csv"); // QuickSort is 45x faster
		//allInputFiles.Add("totallyrandom.txt"); // QuickSort is 2x faster
		//allInputFiles.Add("InsuranceGroups.csv"); // QuickSort is 5x faster

		//allInputFiles.Add("background-checks-original-order.txt"); // about the same
		//allInputFiles.Add("background-checks-combo1-sorted-asc.txt"); // GroupSort is 233x faster
		//allInputFiles.Add("background-checks-combo1-sorted-desc.txt"); // GroupSort is 167x faster
		//allInputFiles.Add("background-checks-combo1-sort-permit-asc.txt"); // QuickSort is 57% faster
		//allInputFiles.Add("background-checks-combo1-sort-permit-desc.txt"); // GroupSort is 10x faster

		//allInputFiles.Add("feds3-planes-original-order.txt"); // GroupSort is 433x faster
		//allInputFiles.Add("feds3-planes-sorted-asc.txt"); // GroupSort is 587x faster
		//allInputFiles.Add("feds3-planes-sorted-desc.txt"); // GroupSort is 578x faster
		allInputFiles.Add("feds3-detailed-string-sorted-asc.txt"); // GroupSort is 114x faster
		allInputFiles.Add("feds3-detailed-string-sorted-desc.txt"); // GroupSort is 114x faster

        //allInputFiles.Add("feds3-6char.txt"); // GroupSort is 3762x faster
        //allInputFiles.Add("feds3-10char.txt"); // GroupSort is 34600x faster
        //allInputFiles.Add("feds3-14char.txt"); // GroupSort is 26556x faster

        //allInputFiles.Add("feds3-5k-unique.txt"); // QuickSort is 2.5x faster
        //allInputFiles.Add("DOB_Job-800k-unique.txt"); // QuickSort is 64x faster
        //allInputFiles.Add("DOB_Job-500k-unique.txt"); // QuickSort is 41x faster
        //allInputFiles.Add("DOB_Job-100k-unique.txt"); // QuickSort is 5.5x faster
        //allInputFiles.Add("DOB_Job-50k-unique.txt"); // QuickSort is 4x faster
        //allInputFiles.Add("DOB_Job-10k-unique.txt"); // QuickSort is 1.6x faster
        //allInputFiles.Add("the-same-17char-string-repeated.txt"); // 1k records: GroupSort is 23x faster
        //allInputFiles.Add("the-same-17char-string-repeated.txt"); // 5k records: GroupSort is 110x faster
        //allInputFiles.Add("the-same-17char-string-repeated.txt"); // 10k records: GroupSort is 160x faster
        //allInputFiles.Add("the-same-17char-string-repeated.txt"); // 50k records: GroupSort is 982x faster
        //allInputFiles.Add("the-same-17char-string-repeated.txt"); // 100k records: GroupSort is 2155x faster
        //allInputFiles.Add("200k-unique-and-duplicate.txt"); // GroupSort is 45x faster
        //allInputFiles.Add("20k-unique-and-duplicate.txt"); // GroupSort is 29x faster
        //allInputFiles.Add("unique-and-duplicate-mix.txt"); // 100 uniques, 90 dupes: same speed
        //allInputFiles.Add("unique-and-duplicate-mix.txt"); // 1,000 uniques, 220 dupes: same speed
        //allInputFiles.Add("unique-and-duplicate-mix.txt"); // 10,000 uniques, 865 dupes: same speed
        //allInputFiles.Add("unique-and-duplicate-mix.txt"); // 20,000 uniques, 1770 dupes: same speed
        //allInputFiles.Add("unique-and-duplicate-mix.txt"); // 100,000 uniques, 8790 dupes: same speed
        //allInputFiles.Add("unique-and-duplicate-mix.txt"); // 50,000 uniques, 4400 dupes: same speed
        //allInputFiles.Add("unique-and-duplicate-mix.txt"); // 500,000 uniques, 49500 dupes: same speed

        int selection = GatherMenuSelection(allInputFiles);
		while (selection >= 0)
		{
			List<string> listToSort = new List<string>();

			// SELECTIONS
			System.Console.WriteLine("Processing " + allInputFiles[selection - 1]);
			inputFile = GlobalVariables.inputFolder + allInputFiles[selection - 1];

			if (BuildListFromFile(ref listToSort, inputFile))
			{
                WriteToFile(listToSort, "master");
                
				Stopwatch timeToSort = new Stopwatch();

                List<string> groupsortListToSort = listToSort.ToList<string>();
                List<string> groupsortSortedList = listToSort.ToList<string>();

                Console.WriteLine("-- GroupSort --");
                timeToSort.Restart();
                // Call the ToList() method here due to underlying call to QuickSort doing a RemoveAt() on our list reference object
                //groupsortSortedList = GroupSort.SortStrings(groupsortListToSort.ToList<string>());
                //groupsortSortedList = stringGroupSorter.SortStrings(groupsortListToSort.ToList<string>());
                GroupSort.Strings stringGroupSorter = new GroupSort.Strings();
                stringGroupSorter.SortStringsNoRecursion(ref groupsortSortedList);
                timeToSort.Stop();
                CheckList(groupsortListToSort, groupsortSortedList);
                WriteToFile(groupsortSortedList, "groupsort");
                PrintOverallTimeResult(groupsortSortedList, timeToSort);
                Console.WriteLine("");

                List<string> quicksortListToSort = listToSort.ToList<string>();
                List<string> quicksortSortedList = new List<string>();

				Console.WriteLine("-- QuickSort --");
				timeToSort.Restart();
                // Call the ToList() method here due to underlying call to QuickSort doing a RemoveAt() on our list reference object
                //quicksortSortedList = QuickSort.Strings.SortStrings(quicksortListToSort.ToList<string>());
                //quicksortSortedList = QuickSort.Strings.SortStringsNoRecursion(quicksortListToSort.ToList<string>());
				QuickSort.Strings stringQuickSorter = new QuickSort.Strings();
				quicksortSortedList = stringQuickSorter.SortStringsNoRecursion(quicksortListToSort.ToList<string>());
                timeToSort.Stop();
                CheckList(quicksortListToSort, quicksortSortedList);
                WriteToFile(quicksortSortedList, "quicksort");
                PrintOverallTimeResult(quicksortSortedList, timeToSort);
                PrintQuicksortDetailTimeResult(stringQuickSorter);
                Console.WriteLine("");
            }

            selection = GatherMenuSelection(allInputFiles);
		}

		System.Console.WriteLine("");
		System.Console.WriteLine("Press any key to exit...");
		System.Console.Read();
	}

	static int GatherMenuSelection(List<string> allInputFiles)
	{
		// SELECTIONS
		System.Console.WriteLine("-------------------------");
		System.Console.WriteLine("Select your option:");
		for (int fileIndex = 0; fileIndex < allInputFiles.Count; fileIndex++)
		{
			int fileNumber = fileIndex + 1;
			System.Console.WriteLine(fileNumber + " - " + allInputFiles[fileIndex]);
		}
		System.Console.WriteLine("-------------------------");
		string inputString = System.Console.ReadLine();

		int selection = -1;
		try
		{
			int inputInt = Convert.ToInt16(inputString);
			if ((inputInt >= 1) && (inputInt <= allInputFiles.Count))
			{
				selection = inputInt;
			}
		}
		catch
		{
			System.Console.WriteLine("Invalid input " + inputString);
		}

		return selection;
	}

	private static Boolean BuildListFromFile(ref List<string> listToBuild, string inputFile)
	{
		try
		{
            string[] fileLines = File.ReadAllLines(inputFile, Encoding.ASCII);
            listToBuild = new List<string>(fileLines);

            return true;
        }
		catch
		{
			Console.WriteLine("Unable to read the content of " + inputFile);
		}

        return false;
	}

	private static void CheckList(List<string> originalList, List<string> sortedListToCheck)
	{
		int falseSortCount = 0;

		if (originalList.Count == sortedListToCheck.Count)
		{
			if (sortedListToCheck.Count > 1)
			{
				for (int listIndex = 1; listIndex < sortedListToCheck.Count; listIndex++)
				{
					if (String.Compare(sortedListToCheck[listIndex - 1], sortedListToCheck[listIndex], CultureInfo.CurrentCulture, CompareOptions.Ordinal) > 0)
					{
						if (falseSortCount < 3)
						{
							string previous = sortedListToCheck[listIndex - 1];
							if (previous.Length > 10)
							{
								previous = previous.Substring(0, 10);
							}
							string current = sortedListToCheck[listIndex];
							if (current.Length > 10)
							{
								current = current.Substring(0, 10);
							}
							Console.WriteLine(current + " is before " + previous);
						}
						falseSortCount++;
					}
				}
			}

			if (falseSortCount > 0)
			{
				Console.WriteLine(falseSortCount + " mis-sorts found.");
			}
        }
		else
		{
			Console.WriteLine("The original list count (" + originalList.Count + ") doesn't match the sorted list count (" + sortedListToCheck.Count + ")");

        }
    }

	public static void WriteToFile(List<string> listToSave, string baseFileName)
	{
        string fileName = GlobalVariables.outputFolder + baseFileName + ".csv";
        
		try
        {
            File.WriteAllLines(fileName, listToSave);
        }
		catch
		{
			Console.WriteLine("Unable to save file " + fileName);
        }
    }

	private static void PrintOverallTimeResult(List<string> listToPrint, Stopwatch timeToSort)
	{
        Console.WriteLine(listToPrint.Count + " items in list");
        Console.WriteLine("Time: " + ConvertTimeToString(timeToSort));
    }

    private static void PrintQuicksortDetailTimeResult(QuickSort.Strings stringSorter)
    {
		if (stringSorter.UseStopwatches)
		{
            if (stringSorter.Stopwatch1.Elapsed.TotalMilliseconds > 0)
                Console.WriteLine(stringSorter.Stopwatch1MonitorDescription + ": " + ConvertTimeToString(stringSorter.Stopwatch1));
            if (stringSorter.Stopwatch2.Elapsed.TotalMilliseconds > 0)
                Console.WriteLine(stringSorter.Stopwatch2MonitorDescription + ": " + ConvertTimeToString(stringSorter.Stopwatch2));
            if (stringSorter.Stopwatch3.Elapsed.TotalMilliseconds > 0)
                Console.WriteLine(stringSorter.Stopwatch3MonitorDescription + ": " + ConvertTimeToString(stringSorter.Stopwatch3));
        }
    }

    private static string ConvertTimeToString(Stopwatch timeToSort)
	{
		long million = 1000L * 1000L;
		long billion = 1000L * 1000L * 1000L;
		string microseconds;
		string nanoseconds;
		try
		{
			microseconds = Convert.ToString((timeToSort.ElapsedTicks / (Stopwatch.Frequency / million)) % 1000);
		}
		catch
		{
			microseconds = "*";
		}
		try
		{
			nanoseconds = Convert.ToString(timeToSort.ElapsedTicks / (Stopwatch.Frequency / billion));
		}
		catch
		{
			nanoseconds = "*";
		}

		string timeString = String.Empty;
		bool displayFurtherUnits = false;
		if (displayFurtherUnits || (timeToSort.Elapsed.Minutes > 0))
		{
			timeString += timeToSort.Elapsed.Minutes + "m";
			displayFurtherUnits = true;
        }
        if (displayFurtherUnits || (timeToSort.Elapsed.Seconds > 0))
        {
			timeString += timeToSort.Elapsed.Seconds + "s";
            displayFurtherUnits = true;
        }
        if (displayFurtherUnits || (timeToSort.Elapsed.Milliseconds > 0))
        {
			timeString += timeToSort.Elapsed.Milliseconds + "ms";
            displayFurtherUnits = true;
        }
        if (displayFurtherUnits || (microseconds != "*"))
        {
			timeString += microseconds + "us";
			displayFurtherUnits = true;
		}
/*
		if (displayFurtherUnits || (nanoseconds != "*"))
		{
			timeString += nanoseconds + "ns";
			displayFurtherUnits = true;
		}
*/
		
		return timeString;

    }

}
