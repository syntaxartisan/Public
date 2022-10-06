using System;
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
	static class GlobalVariables
	{
        public const string inputFolder = @"D:\(A) Professional\Code\Public\Projects\input\";
        public const string outputFolder = @"D:\(A) Professional\Code\Public\Projects\GroupSort\output\";
    }

    static void Main(string[] args)
	{
        string inputFile = String.Empty;

		List<string> allInputFiles = new List<string>();
		//allInputFiles.Add("movies.csv");
		//allInputFiles.Add("moviesforwards.txt");
		//allInputFiles.Add("moviesbackwards.txt");
		//allInputFiles.Add("moviesforwardssplit.txt");
		//allInputFiles.Add("moviesrandomsmall.csv");
		//allInputFiles.Add("moviesrandomlarge.csv");
		//allInputFiles.Add("totallyrandom.txt");
		//allInputFiles.Add("InsuranceGroups.csv");

		allInputFiles.Add("background-checks-original-order.txt");
		allInputFiles.Add("background-checks-combo1-sorted-asc.txt");
		allInputFiles.Add("background-checks-combo1-sorted-desc.txt");
		allInputFiles.Add("background-checks-combo1-sort-permit-asc.txt");
		allInputFiles.Add("background-checks-combo1-sort-permit-desc.txt");

		//allInputFiles.Add("feds3-planes-original-order.txt");
		//allInputFiles.Add("feds3-planes-sorted-asc.txt");
		//allInputFiles.Add("feds3-planes-sorted-desc.txt");
		//allInputFiles.Add("feds3-detailed-string-sorted-asc.txt");
		//allInputFiles.Add("feds3-detailed-string-sorted-desc.txt");

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
				List<string> groupsortSortedList = new List<string>();
				List<string> quicksortSortedList = new List<string>();
				List<string> groupsortListToSort = listToSort.ToList<string>();
				List<string> quicksortListToSort = listToSort.ToList<string>();

				groupsortSortedList.Clear();
				Console.WriteLine("-- GroupSort --");
				timeToSort.Restart();
				// Call the ToList() method here due to underlying call to QuickSort doing a RemoveAt() on our list reference object
				groupsortSortedList = GroupSort.SortStrings(groupsortListToSort.ToList<string>());
				timeToSort.Stop();
				CheckList(groupsortListToSort, groupsortSortedList);
                WriteToFile(groupsortSortedList, "groupsort");
                PrintOverallTimeResult(groupsortSortedList, timeToSort);
                Console.WriteLine("");

                quicksortSortedList.Clear();
				Console.WriteLine("-- QuickSort --");
				timeToSort.Restart();
                // Call the ToList() method here due to underlying call to QuickSort doing a RemoveAt() on our list reference object
                //quicksortSortedList = QuickSort.Strings.SortStrings(quicksortListToSort.ToList<string>());
                //quicksortSortedList = QuickSort.Strings.SortStringsNoRecursion(quicksortListToSort.ToList<string>());
				QuickSort.Strings stringSorter = new QuickSort.Strings();
				quicksortSortedList = stringSorter.SortStringsNoRecursion(quicksortListToSort.ToList<string>());
                timeToSort.Stop();
                CheckList(quicksortListToSort, quicksortSortedList);
                WriteToFile(quicksortSortedList, "quicksort");
                PrintOverallTimeResult(quicksortSortedList, timeToSort);
                PrintQuicksortDetailTimeResult(stringSorter);
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
		if (stringSorter.Stopwatch1.Elapsed.TotalMilliseconds > 0)
	        Console.WriteLine(stringSorter.Stopwatch1MonitorDescription + ": " + ConvertTimeToString(stringSorter.Stopwatch1));
        if (stringSorter.Stopwatch2.Elapsed.TotalMilliseconds > 0)
            Console.WriteLine(stringSorter.Stopwatch2MonitorDescription + ": " + ConvertTimeToString(stringSorter.Stopwatch2));
        if (stringSorter.Stopwatch3.Elapsed.TotalMilliseconds > 0)
            Console.WriteLine(stringSorter.Stopwatch3MonitorDescription + ": " + ConvertTimeToString(stringSorter.Stopwatch3));
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
		if (timeToSort.Elapsed.Minutes > 0)
		{
			timeString += timeToSort.Elapsed.Minutes + "m";
		}
		if (timeToSort.Elapsed.Seconds > 0)
		{
			timeString += timeToSort.Elapsed.Seconds + "s";
		}
		if (timeToSort.Elapsed.Milliseconds > 0)
		{
			timeString += timeToSort.Elapsed.Milliseconds + "ms";
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
		
		return timeString;

    }

}
