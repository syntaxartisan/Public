using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Artisan.Sorting;

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
        public const string inputFolder = @"D:\Professional\Code\Public\Projects\input\";
        public const string outputFolder = @"D:\Professional\Code\Public\Projects\GroupSort\output\";
    }

	enum MenuSelectIndividualOrAllFiles
	{
		IndividualFile = 1,
		AllFiles
	}

	enum MenuSelectIndividualSortOption
	{
		NameAsc = 1,
		NameDesc,
		SizeAsc,
		SizeDesc,
		ModifiedDateAsc,
		ModifiedDateDesc
	}

    static void Main()
	{
		List<string> allInputFiles = new List<string>();
		//allInputFiles.Add("tinytest1.txt"); // QuickSort is 4x faster
		//allInputFiles.Add("tinytest2.txt"); // QuickSort is 2x faster
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
		//allInputFiles.Add("feds3-detailed-string-sorted-asc.txt"); // GroupSort is 114x faster
		//allInputFiles.Add("feds3-detailed-string-sorted-desc.txt"); // GroupSort is 114x faster

		//allInputFiles.Add("feds3-6char.txt"); // GroupSort is 3762x faster
		//allInputFiles.Add("feds3-10char.txt"); // GroupSort is 34600x faster
		//allInputFiles.Add("feds3-14char.txt"); // GroupSort is 26556x faster

		//allInputFiles.Add("feds3-5k-unique.txt"); // QuickSort is 2.5x faster
		//allInputFiles.Add("DOB_Job-10k-unique.txt"); // QuickSort is 1.6x faster
		allInputFiles.Add("DOB_Job-50k-unique.txt"); // QuickSort is 7x faster
		allInputFiles.Add("DOB_Job-50k-unique-asc.txt"); // GroupSort is 120x faster
		allInputFiles.Add("DOB_Job-50k-unique-desc.txt"); // GroupSort is 93x faster
		allInputFiles.Add("DOB_Job-100k-unique.txt"); // QuickSort is 9x faster
		allInputFiles.Add("DOB_Job-100k-unique-asc.txt"); // GroupSort is 130x faster
		allInputFiles.Add("DOB_Job-100k-unique-desc.txt"); // GroupSort is 114x faster
		allInputFiles.Add("DOB_Job-500k-unique.txt"); // QuickSort is 41x(60x) faster
		allInputFiles.Add("DOB_Job-500k-unique-asc.txt"); // GroupSort is 50x faster
		allInputFiles.Add("DOB_Job-500k-unique-desc.txt"); // GroupSort is 27x faster
		//allInputFiles.Add("DOB_Job-800k-unique.txt"); // QuickSort is 64x faster
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


		LogFile results = new LogFile("benchmark_results.tab");
		List<string> filesToBenchmark = new List<string>();


        int selection = SelectAllOrSingleFileMenu();
		if (selection == (int)MenuSelectIndividualOrAllFiles.AllFiles)
		{
            filesToBenchmark = SelectAllFilesForBenchmarking();
        }
		else if (selection == (int)MenuSelectIndividualOrAllFiles.IndividualFile)
		{
            int sortSelection = SelectIndividualFileSorting();
			List<string> sortedFileSelection = SortFilesForUserSelection(sortSelection);
            filesToBenchmark = SelectIndividualFileForBenchmarkingMenu(sortSelection, sortedFileSelection);
        }


        foreach (string file in filesToBenchmark)
		{
			List<string> originalList = new List<string>();

			System.Console.WriteLine("Processing " + file);

			if (BuildListFromFile(ref originalList, file))
			{
                Stopwatch timeToGroupSort = new Stopwatch();
                Stopwatch timeToQuickSort = new Stopwatch();
                WriteListToFile(originalList, "master");


                Console.WriteLine("-- GroupSort --");
                timeToGroupSort.Restart();

				//GroupSort.RecursiveSortStrings stringGroupSorter = new GroupSort.RecursiveSortStrings();
				//List<string> groupsortSortedList = stringGroupSorter.Sort(originalList);
				GroupSort.SortStrings stringGroupSorter = new GroupSort.SortStrings();
				List<string> groupsortSortedList = stringGroupSorter.Sort(originalList);

				timeToGroupSort.Stop();
                ValidateList(groupsortSortedList, originalList.Count);
                WriteListToFile(groupsortSortedList, "groupsort");
                PrintOverallTimeResult(groupsortSortedList, timeToGroupSort);
                Console.WriteLine("");


				Console.WriteLine("-- QuickSort --");
				timeToQuickSort.Restart();

				//QuickSort.RecursiveSortStrings stringQuickSorter = new QuickSort.RecursiveSortStrings();
				//List<string> quicksortSortedList = stringQuickSorter.Sort(originalList);
				QuickSort.SortStrings stringQuickSorter = new QuickSort.SortStrings();
				List<string> quicksortSortedList = stringQuickSorter.Sort(originalList);

				timeToQuickSort.Stop();
                ValidateList(quicksortSortedList, originalList.Count);
                WriteListToFile(quicksortSortedList, "quicksort");
                PrintOverallTimeResult(quicksortSortedList, timeToQuickSort);
                Console.WriteLine("");

                int slashIndex = file.LastIndexOf("\\");
                string fileName = file.Substring(slashIndex + 1);
                results.SaveEntry(fileName, timeToGroupSort, timeToQuickSort);
            }

			if (selection == (int)MenuSelectIndividualOrAllFiles.IndividualFile)
			{
                int sortSelection = SelectIndividualFileSorting();
                List<string> sortedFileSelection = SortFilesForUserSelection(sortSelection);
                filesToBenchmark = SelectIndividualFileForBenchmarkingMenu(sortSelection, sortedFileSelection);
            }
        }

        System.Console.WriteLine("");
		System.Console.WriteLine("Press any key to exit...");
		System.Console.Read();
	}

	private static int SelectAllOrSingleFileMenu()
	{
        System.Console.WriteLine("-------------------------");
        System.Console.WriteLine("Do you want to benchmark a single file or all files?");
		System.Console.WriteLine((int)MenuSelectIndividualOrAllFiles.IndividualFile + " - Benchmark individual file");
        System.Console.WriteLine((int)MenuSelectIndividualOrAllFiles.AllFiles + " - Benchmark all files");
        System.Console.WriteLine("-------------------------");

        string inputString = System.Console.ReadLine();
		int minSelection = 1;
        int maxSelection = 2;
        int selection = -1;
        try
        {
            int inputInt = Convert.ToInt16(inputString);
            if ((inputInt >= minSelection) && (inputInt <= maxSelection))
            {
                selection = inputInt;
            }
        }
        catch
        {
            System.Console.WriteLine("Invalid input " + inputString);
			Environment.Exit(-1);
        }

        return selection;
    } // SelectAllOrSingleFileMenu

    private static int SelectIndividualFileSorting()
	{
        System.Console.WriteLine("-------------------------");
        System.Console.WriteLine("Benchmarking a single file.");
        System.Console.WriteLine("We will display all files and you will select which one you want to benchmark.");
        System.Console.WriteLine("How would you like to sort the files?");
        System.Console.WriteLine((int)MenuSelectIndividualSortOption.NameAsc + " - Name (ascending)");
        System.Console.WriteLine((int)MenuSelectIndividualSortOption.NameDesc + " - Name (descending)");
        System.Console.WriteLine((int)MenuSelectIndividualSortOption.SizeAsc + " - Size (ascending)");
        System.Console.WriteLine((int)MenuSelectIndividualSortOption.SizeDesc + " - Size (descending)");
        System.Console.WriteLine((int)MenuSelectIndividualSortOption.ModifiedDateAsc + " - Modified Date (ascending)");
        System.Console.WriteLine((int)MenuSelectIndividualSortOption.ModifiedDateDesc + " - Modified Date (descending)");
        System.Console.WriteLine("-------------------------");

        string inputString = System.Console.ReadLine();
        int minSelection = 1;
        int maxSelection = 6;
        int selection = -1;
        try
        {
            int inputInt = Convert.ToInt16(inputString);
            if ((inputInt >= minSelection) && (inputInt <= maxSelection))
            {
                selection = inputInt;
            }
        }
        catch
        {
            System.Console.WriteLine("Invalid input " + inputString);
            Environment.Exit(-1);
        }

        return selection;
    } // SelectIndividualFileSorting

    private static List<string> SelectIndividualFileForBenchmarkingMenu(int sortSelection, List<string> sortedFileSelection)
	{
		List<string> filesToBenchmark = new List<string>();
		int offset = 1;

        System.Console.WriteLine("-------------------------");
		switch (sortSelection)
		{
			case (int)MenuSelectIndividualSortOption.NameAsc:
				System.Console.WriteLine("Sorting by Name (ascending).");
				break;
            case (int)MenuSelectIndividualSortOption.NameDesc:
                System.Console.WriteLine("Sorting by Name (descending).");
                break;
            case (int)MenuSelectIndividualSortOption.SizeAsc:
                System.Console.WriteLine("Sorting by Size (ascending).");
                break;
            case (int)MenuSelectIndividualSortOption.SizeDesc:
                System.Console.WriteLine("Sorting by Size (descending).");
                break;
            case (int)MenuSelectIndividualSortOption.ModifiedDateAsc:
                System.Console.WriteLine("Sorting by Modified Date (ascending).");
                break;
            case (int)MenuSelectIndividualSortOption.ModifiedDateDesc:
                System.Console.WriteLine("Sorting by Modified Date (descending).");
                break;
        }
        System.Console.WriteLine("Select a file to benchmark:");
		for (int fileIndex = 0; fileIndex < sortedFileSelection.Count; fileIndex++)
		{
			int fileNumber = fileIndex + offset;
			int slashIndex = sortedFileSelection[fileIndex].LastIndexOf("\\");
			string fileName = sortedFileSelection[fileIndex].Substring(slashIndex + 1);
			System.Console.WriteLine(fileNumber + " - " + fileName);
		}
		System.Console.WriteLine("-------------------------");

		string inputString = System.Console.ReadLine();
        int minSelection = 1;
        int maxSelection = sortedFileSelection.Count;
		try
		{
			int inputInt = Convert.ToInt16(inputString);
			if ((inputInt >= minSelection) && (inputInt <= maxSelection))
			{
				filesToBenchmark.Add(sortedFileSelection[inputInt - offset]);

            }
		}
		catch
		{
			System.Console.WriteLine("Invalid input " + inputString);
            Environment.Exit(-1);
        }

        return filesToBenchmark;
    } // SelectIndividualFileForBenchmarkingMenu

    private static List<string> SelectAllFilesForBenchmarking()
	{
        List<string> filesToBenchmark = Directory.EnumerateFiles
			(GlobalVariables.inputFolder, "*", SearchOption.TopDirectoryOnly)
			.OrderByDescending(f => new FileInfo(f).Length).ToList<string>();

		return filesToBenchmark;
    }

    private static List<string> SortFilesForUserSelection(int sortSelection)
	{
		List<string> sortedFileSelection = new List<string>();

        switch (sortSelection)
		{
			case (int)MenuSelectIndividualSortOption.NameAsc:
                sortedFileSelection = Directory.EnumerateFiles
                    (GlobalVariables.inputFolder, "*", SearchOption.TopDirectoryOnly)
                    .OrderBy(f => new FileInfo(f).Name).ToList<string>();
                break;
			case (int)MenuSelectIndividualSortOption.NameDesc:
                sortedFileSelection = Directory.EnumerateFiles
                    (GlobalVariables.inputFolder, "*", SearchOption.TopDirectoryOnly)
                    .OrderByDescending(f => new FileInfo(f).Name).ToList<string>();
                break;
			case (int)MenuSelectIndividualSortOption.SizeAsc:
                sortedFileSelection = Directory.EnumerateFiles
                    (GlobalVariables.inputFolder, "*", SearchOption.TopDirectoryOnly)
                    .OrderBy(f => new FileInfo(f).Length).ToList<string>();
                break;
			case (int)MenuSelectIndividualSortOption.SizeDesc:
                sortedFileSelection = Directory.EnumerateFiles
                    (GlobalVariables.inputFolder, "*", SearchOption.TopDirectoryOnly)
                    .OrderByDescending(f => new FileInfo(f).Length).ToList<string>();
                break;
			case (int)MenuSelectIndividualSortOption.ModifiedDateAsc:
                sortedFileSelection = Directory.EnumerateFiles
                    (GlobalVariables.inputFolder, "*", SearchOption.TopDirectoryOnly)
                    .OrderBy(f => new FileInfo(f).LastWriteTime).ToList<string>();
                break;
			case (int)MenuSelectIndividualSortOption.ModifiedDateDesc:
                sortedFileSelection = Directory.EnumerateFiles
                    (GlobalVariables.inputFolder, "*", SearchOption.TopDirectoryOnly)
                    .OrderByDescending(f => new FileInfo(f).LastWriteTime).ToList<string>();
                break;
		}


        return sortedFileSelection;
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

	private static void ValidateList(List<string> sortedListToCheck, int originalListItemCount)
	{
		int falseSortCount = 0;

		if (originalListItemCount == sortedListToCheck.Count)
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
			Console.WriteLine("The original list count (" + originalListItemCount + ") doesn't match the sorted list count (" + sortedListToCheck.Count + ")");

        }
    }

	private static void WriteListToFile(List<string> listToSave, string baseFileName)
	{
        string fileName = Path.Combine(GlobalVariables.outputFolder, baseFileName + ".csv");

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

    public static string ConvertTimeToString(Stopwatch timeToSort)
	{
		bool displayNanoseconds = false;

		long million = 1000L * 1000L;
		long billion = 1000L * 1000L * 1000L;
		long microseconds;
		long nanoseconds;
		try
		{
			microseconds = (timeToSort.ElapsedTicks / (Stopwatch.Frequency / million)) % 1000;
		}
		catch
		{
			microseconds = -1;
		}
		try
		{
			nanoseconds = (long)((((double)timeToSort.ElapsedTicks / Stopwatch.Frequency) * billion) % 1000);
		}
        catch
		{
			nanoseconds = -1;
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
        if (displayFurtherUnits || (microseconds > 0) || !displayNanoseconds)
        {
			timeString += microseconds + "us";
			displayFurtherUnits = true;
		}
		if (displayNanoseconds && (displayFurtherUnits || (nanoseconds > 0)))
		{
			timeString += nanoseconds + "ns";
			displayFurtherUnits = true;
		}
		
		return timeString;

    }

	private class LogFile
	{
		public string Filename { get; private set; }
		private readonly string _delim = "\t";

		public LogFile(string filename)
        {
            Filename = filename;
			MakeSureLogExists();
        }

		private void MakeSureLogExists()
		{
            string fileName = Path.Combine(GlobalVariables.outputFolder, Filename);

			if(!File.Exists(fileName))
			{
                try
                {
                    File.WriteAllText(fileName, "DateTime" + _delim + "FileName" + _delim + "GroupSortTime" + _delim + "QuickSortTime" + Environment.NewLine);
                }
                catch
                {
                    Console.WriteLine("Unable to save file " + fileName);
                }
            }
        }

		// Entry: DateTime, input filename, GroupSort time, QuickSort time
        public void SaveEntry(string fileName, Stopwatch groupSortTime, Stopwatch quickSortTime)
		{
            string logFile = Path.Combine(GlobalVariables.outputFolder, Filename);
			string dateTime = DateTime.Now.ToString("yyyy-MM-dd h:mm:ss,fff tt");

            try
            {
                File.AppendAllText(logFile, dateTime + _delim + fileName + _delim + ConvertTimeToString(groupSortTime) + _delim + ConvertTimeToString(quickSortTime) + Environment.NewLine);
            }
            catch
            {
                Console.WriteLine("Unable to save file " + logFile);
            }
        }
    } // class LogFile

}
