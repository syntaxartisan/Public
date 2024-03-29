﻿using System;
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
		LogFile results = new LogFile("benchmark_results.tab");
		List<string> filesToBenchmark = new List<string>();


        // -- DISPLAY MENUS --
        int selection = SelectAllOrSingleFileMenu();
		if (selection == (int)MenuSelectIndividualOrAllFiles.AllFiles)
		{
            filesToBenchmark = SelectAllFilesForBenchmarking();
            BenchmarkFilesAgainstSorters(filesToBenchmark, results);
        }
        else if (selection == (int)MenuSelectIndividualOrAllFiles.IndividualFile)
		{
            int sortSelection = SelectIndividualFileSortingMenu();
			List<string> sortedFileSelection = SortFilesForUserSelection(sortSelection);
            filesToBenchmark = SelectIndividualFileForBenchmarkingMenu(sortSelection, sortedFileSelection);

            while (sortSelection > 0 && filesToBenchmark.Count > 0)
            {
                BenchmarkFilesAgainstSorters(filesToBenchmark, results);

                sortSelection = SelectIndividualFileSortingMenu();
                sortedFileSelection = SortFilesForUserSelection(sortSelection);
                filesToBenchmark = SelectIndividualFileForBenchmarkingMenu(sortSelection, sortedFileSelection);
            }
        }

        Console.WriteLine("");
		Console.WriteLine("Press any key to exit...");
		Console.Read();
	}

    private static void BenchmarkFilesAgainstSorters(List<string> filesToBenchmark, LogFile results)
    {
        foreach (string file in filesToBenchmark)
        {
            List<string> originalList = new List<string>();

            Console.WriteLine("Processing " + file);

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

                bool outputListsMatch = OutputFilesAreEqual("groupsort", "quicksort");

                results.SaveEntry(file, timeToGroupSort, timeToQuickSort, outputListsMatch);
            }

        }
    }


    private static int SelectAllOrSingleFileMenu()
	{
        Console.WriteLine("-------------------------");
        Console.WriteLine("Do you want to benchmark a single file or all files?");
		Console.WriteLine((int)MenuSelectIndividualOrAllFiles.IndividualFile + " - Benchmark individual file");
        Console.WriteLine((int)MenuSelectIndividualOrAllFiles.AllFiles + " - Benchmark all files");
        Console.WriteLine("-------------------------");

        int selection = -1;
        while (selection < 0)
        {
            string inputString = Console.ReadLine();
            int minSelection = 1;
            int maxSelection = 2;
            try
            {
                int inputInt = Convert.ToInt16(inputString);
                if ((inputInt >= minSelection) && (inputInt <= maxSelection))
                {
                    selection = inputInt;
                }
                else
                {
                    Console.WriteLine("Try again.");
                }
            }
            catch
            {
                Console.WriteLine("Invalid input " + inputString);
                Environment.Exit(-1);
            }
        }

        return selection;
    } // SelectAllOrSingleFileMenu

    private static int SelectIndividualFileSortingMenu()
	{
        Console.WriteLine("-------------------------");
        Console.WriteLine("Benchmarking a single file.");
        Console.WriteLine("We will display all files and you will select which one you want to benchmark.");
        Console.WriteLine("How would you like to sort the files?");
        Console.WriteLine((int)MenuSelectIndividualSortOption.NameAsc + " - Name (ascending)");
        Console.WriteLine((int)MenuSelectIndividualSortOption.NameDesc + " - Name (descending)");
        Console.WriteLine((int)MenuSelectIndividualSortOption.SizeAsc + " - Size (ascending)");
        Console.WriteLine((int)MenuSelectIndividualSortOption.SizeDesc + " - Size (descending)");
        Console.WriteLine((int)MenuSelectIndividualSortOption.ModifiedDateAsc + " - Modified Date (ascending)");
        Console.WriteLine((int)MenuSelectIndividualSortOption.ModifiedDateDesc + " - Modified Date (descending)");
        Console.WriteLine("-------------------------");

        int selection = -1;
        while (selection < 0)
        {
            string inputString = Console.ReadLine();
            int minSelection = 1;
            int maxSelection = 6;
            try
            {
                int inputInt = Convert.ToInt16(inputString);
                if ((inputInt >= minSelection) && (inputInt <= maxSelection))
                {
                    selection = inputInt;
                }
                else
                {
                    Console.WriteLine("Try again.");
                }
            }
            catch
            {
                Console.WriteLine("Invalid input " + inputString);
                Environment.Exit(-1);
            }
        }

        return selection;
    } // SelectIndividualFileSortingMenu

    private static List<string> SelectIndividualFileForBenchmarkingMenu(int sortSelection, List<string> sortedFileSelection)
	{
		List<string> filesToBenchmark = new List<string>();
		int offset = 1;

        Console.WriteLine("-------------------------");
		switch (sortSelection)
		{
			case (int)MenuSelectIndividualSortOption.NameAsc:
				Console.WriteLine("Sorting by Name (ascending).");
				break;
            case (int)MenuSelectIndividualSortOption.NameDesc:
                Console.WriteLine("Sorting by Name (descending).");
                break;
            case (int)MenuSelectIndividualSortOption.SizeAsc:
                Console.WriteLine("Sorting by Size (ascending).");
                break;
            case (int)MenuSelectIndividualSortOption.SizeDesc:
                Console.WriteLine("Sorting by Size (descending).");
                break;
            case (int)MenuSelectIndividualSortOption.ModifiedDateAsc:
                Console.WriteLine("Sorting by Modified Date (ascending).");
                break;
            case (int)MenuSelectIndividualSortOption.ModifiedDateDesc:
                Console.WriteLine("Sorting by Modified Date (descending).");
                break;
        }
        Console.WriteLine("Select a file to benchmark:");
		for (int fileIndex = 0; fileIndex < sortedFileSelection.Count; fileIndex++)
		{
			int fileNumber = fileIndex + offset;
			int slashIndex = sortedFileSelection[fileIndex].LastIndexOf("\\");
			string fileName = sortedFileSelection[fileIndex].Substring(slashIndex + 1);
            string extraAttribute = ObtainFileAttribute(sortSelection, sortedFileSelection[fileIndex]);
            Console.WriteLine(fileNumber + " - " + fileName + "  " + extraAttribute);
		}
		Console.WriteLine("-------------------------");

        while (filesToBenchmark.Count == 0)
        {
            string inputString = Console.ReadLine();
            int minSelection = 1;
            int maxSelection = sortedFileSelection.Count;
            try
            {
                int inputInt = Convert.ToInt16(inputString);
                if ((inputInt >= minSelection) && (inputInt <= maxSelection))
                {
                    filesToBenchmark.Add(sortedFileSelection[inputInt - offset]);

                }
                else
                {
                    Console.WriteLine("Try again.");
                }
            }
            catch
            {
                Console.WriteLine("Invalid input " + inputString);
                Environment.Exit(-1);
            }
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

    private static string ObtainFileAttribute(int sortSelection, string fileNameAndPath)
    {
        switch (sortSelection)
        {
            case (int)MenuSelectIndividualSortOption.NameAsc:
            case (int)MenuSelectIndividualSortOption.NameDesc:
                break;
            case (int)MenuSelectIndividualSortOption.SizeAsc:
            case (int)MenuSelectIndividualSortOption.SizeDesc:
                return new FileInfo(fileNameAndPath).Length.ToString() + "B";
            case (int)MenuSelectIndividualSortOption.ModifiedDateAsc:
            case (int)MenuSelectIndividualSortOption.ModifiedDateDesc:
                return new FileInfo(fileNameAndPath).LastWriteTime.ToString();
        }

        return String.Empty;
    }

    private static bool BuildListFromFile(ref List<string> listToBuild, string inputFile)
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

    private static bool OutputFilesAreEqual(string sorter1, string sorter2)
    {
        const int BYTES_TO_READ = sizeof(Int64);
        string fileNamePath1 = Path.Combine(GlobalVariables.outputFolder, sorter1 + ".csv");
        string fileNamePath2 = Path.Combine(GlobalVariables.outputFolder, sorter2 + ".csv");

        FileInfo file1 = new FileInfo(fileNamePath1);
        FileInfo file2 = new FileInfo(fileNamePath2);

        if (file1.Length != file2.Length)
            return false;

        if (string.Equals(file1.FullName, file2.FullName, StringComparison.OrdinalIgnoreCase))
            return true;

        int iterations = (int)Math.Ceiling((double)file1.Length / BYTES_TO_READ);

        using (FileStream fs1 = file1.OpenRead())
        using (FileStream fs2 = file2.OpenRead())
        {
            byte[] one = new byte[BYTES_TO_READ];
            byte[] two = new byte[BYTES_TO_READ];

            for (int i = 0; i < iterations; i++)
            {
                fs1.Read(one, 0, BYTES_TO_READ);
                fs2.Read(two, 0, BYTES_TO_READ);

                if (BitConverter.ToInt64(one, 0) != BitConverter.ToInt64(two, 0))
                    return false;
            }
        }

        return true;
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
                    string text = "DateTime" + _delim + "FileName" + _delim 
                        + "GroupSortTime" + _delim + "QuickSortTime" + _delim 
                        + "GroupSortTimesFaster" + _delim + "FileSizeBytes" + _delim 
                        + "ModifiedDate" + _delim + "OutputListsMatch";
                    File.WriteAllText(fileName, text + Environment.NewLine);
                }
                catch
                {
                    Console.WriteLine("Unable to save file " + fileName);
                }
            }
        }

		// Entry: DateTime, input filename, GroupSort time, QuickSort time
        public void SaveEntry(string fileNameAndPath, Stopwatch groupSortTime, Stopwatch quickSortTime, bool outputListsMatch)
		{
            int slashIndex = fileNameAndPath.LastIndexOf("\\");
            string fileName = fileNameAndPath.Substring(slashIndex + 1);

            string logFile = Path.Combine(GlobalVariables.outputFolder, Filename);
			string dateTime = DateTime.Now.ToString("yyyy-MM-dd h:mm:ss,fff tt");
            double gsTimesFaster = 1;
            if (groupSortTime.Elapsed.TotalMilliseconds < quickSortTime.Elapsed.TotalMilliseconds)
            {
                gsTimesFaster = quickSortTime.Elapsed.TotalMilliseconds / groupSortTime.Elapsed.TotalMilliseconds;
            }
            else if (quickSortTime.Elapsed.TotalMilliseconds < groupSortTime.Elapsed.TotalMilliseconds)
            {
                gsTimesFaster = -(groupSortTime.Elapsed.TotalMilliseconds / quickSortTime.Elapsed.TotalMilliseconds);
            }
            long filesizeBytes = new FileInfo(fileNameAndPath).Length;
            string modDate = new FileInfo(fileNameAndPath).LastWriteTime.ToString();
            string listsMatch = outputListsMatch ? "YES" : "NO";

            try
            {
                string text = dateTime + _delim + fileName + _delim 
                    + ConvertTimeToString(groupSortTime) + _delim + ConvertTimeToString(quickSortTime) + _delim 
                    + gsTimesFaster + _delim + filesizeBytes + _delim 
                    + modDate + _delim + listsMatch;
                File.AppendAllText(logFile, text + Environment.NewLine);
            }
            catch
            {
                Console.WriteLine("Unable to save file " + logFile);
            }
        }
    } // class LogFile

}
