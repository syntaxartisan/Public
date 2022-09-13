using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using SortingAlgorithms;
//using System.Threading;

	class Program
	{
		static void Main(string[] args)
		{
			string inputFolder = "D:\\(A) Professional\\Code\\Public\\Projects\\input\\";
			string inputFile = "";

			List<string> allInputFiles = new List<string>();
			allInputFiles.Add("movies.csv");
			allInputFiles.Add("moviesforwards.txt");
			allInputFiles.Add("moviesbackwards.txt");
			allInputFiles.Add("moviesforwardssplit.txt");
			allInputFiles.Add("moviesrandomsmall.csv");
			allInputFiles.Add("moviesrandomlarge.csv");
			allInputFiles.Add("totallyrandom.txt");
			allInputFiles.Add("InsuranceGroups.csv");

			int selection = GatherMenuSelection(allInputFiles);
			while (selection >= 0)
			{
				List<string> listToSort = new List<string>();

				//public static string fileToOpen = "C:\\testing\\movies.csv";
				//public static string fileToOpen = "C:\\testing\\moviesforwards.txt";	// same as movies2
				//public static string fileToOpen = "C:\\testing\\moviesbackwards.txt";
				//public static string fileToOpen = "C:\\testing\\moviesforwardssplit.txt";
				//public static string fileToOpen = "C:\\testing\\moviesrandomsmall.csv";
				//public static string fileToOpen = "C:\\testing\\moviesrandomlarge.csv";

				// SELECTIONS
				System.Console.WriteLine("Processing " + allInputFiles[selection - 1]);
				inputFile = inputFolder + allInputFiles[selection - 1];

				if (BuildListFromFile(ref listToSort, inputFile))
				{
					Stopwatch timeToSort = new Stopwatch();
					List<string> groupsortSortedList = new List<string>();
					List<string> quicksortSortedList = new List<string>();
					List<string> groupsortListToSort = listToSort.ToList<string>();
					List<string> quicksortListToSort = listToSort.ToList<string>();

					groupsortSortedList.Clear();
					Console.WriteLine("-- GroupSort --");
					timeToSort.Restart();
					groupsortSortedList = GroupSort.SortStrings(groupsortListToSort);
					timeToSort.Stop();
					PrintList(groupsortSortedList, timeToSort);

					quicksortSortedList.Clear();
					Console.WriteLine("-- QuickSort --");
					timeToSort.Restart();
					quicksortSortedList = QuickSort.SortStrings(quicksortListToSort);
					timeToSort.Stop();
					PrintList(quicksortSortedList, timeToSort);
				}
				else
				{
					System.Console.WriteLine("Unable to build a list from " + inputFile);
				}

				selection = GatherMenuSelection(allInputFiles);
			}

			System.Console.WriteLine("");
			System.Console.WriteLine("Press any key to exit...");
			System.Console.Read();
			//SortingAlgorithms.QuickSort.
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
			//System.Console.WriteLine(inputString);

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
			FileStream myFile;
			try
			{
				myFile = File.OpenRead(inputFile);
			}
			catch
			{
				System.Console.WriteLine("Unable to open file: ");
				System.Console.WriteLine(inputFile);
				return false;
			}

			string[] myList = File.ReadAllLines(inputFile, Encoding.ASCII);

			listToBuild.Clear();
			for (int listIndex = 0; listIndex < myList.Length; listIndex++)
			{
				listToBuild.Add(myList[listIndex]);
			}

			return true;
		}

		private static void PrintList(List<string> listToPrint, Stopwatch timeToSort)
		{
			//System.Console.WriteLine("List: ");
			//for (Int32 listIndex = 0; listIndex < listToPrint.Count; listIndex++)
			//{
			//	System.Console.WriteLine(listToPrint[listIndex].ToString());
			//}
			System.Console.WriteLine(listToPrint.Count + " items in list");
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

			string timeString = "";
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
			System.Console.WriteLine("Time: " + timeString);
			System.Console.WriteLine("");
		}

	}
