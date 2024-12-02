// See https://aka.ms/new-console-template for more information

namespace AdventOfCode2024;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

public static class Program
{
    private static readonly List<Type> _puzzleTypes = Assembly.GetExecutingAssembly()
        .GetExportedTypes()
        .Where(x => typeof(IPuzzle).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
        .ToList();

    public static int Main()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Advent of Code 2024. Currently implemented puzzles are:");

        Console.ForegroundColor = ConsoleColor.White;
        foreach (var puzzleType in _puzzleTypes)
        {
            Console.WriteLine("- {0}", puzzleType.Name);
        }

        Console.ResetColor();
        Console.WriteLine();

        var puzzleNameToSolve = AskWhichPuzzle();
        var puzzleToSolve = ActivatePuzzle(puzzleNameToSolve);
        var puzzleInputFilePath = AskPuzzleInputFilePath();

        if (!TryParsePuzzleInput(puzzleToSolve, puzzleInputFilePath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("The puzzle input could not be parsed.");
            Console.ResetColor();
            return Exit(1);
        }

        if (!TrySolvePuzzle(puzzleToSolve, out var puzzleResult) || puzzleResult is null)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("The puzzle could not be solved.");
            Console.ResetColor();
            return Exit(2);
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("The puzzle has been solved! See the result below:");
        Console.WriteLine(puzzleResult.ToString());
        return Exit(0);
    }

    private static string AskWhichPuzzle()
    {
        Console.WriteLine(
            "Which puzzle would you like to solve? Please enter the exact puzzle's name as stated above...");
        var puzzleToSolve = Console.ReadLine();
        while (puzzleToSolve is null || _puzzleTypes.All(x => x.Name != puzzleToSolve))
        {
            Console.WriteLine("Puzzle name not found, please enter again...");
            puzzleToSolve = Console.ReadLine();
        }

        return puzzleToSolve;
    }

    private static int Exit(int code)
    {
        Console.ReadKey();
        return code;
    }

    private static IPuzzle ActivatePuzzle(string puzzleName)
    {
        var puzzleType = _puzzleTypes.First(x => x.Name == puzzleName);
        var puzzle = (IPuzzle) Activator.CreateInstance(puzzleType)!;
        return puzzle;
    }

    private static string AskPuzzleInputFilePath()
    {
        Console.WriteLine("Please enter the File path to the puzzle's input...");
        var puzzleInputFilePath = Console.ReadLine();
        while (!File.Exists(puzzleInputFilePath))
        {
            Console.WriteLine("File could not be found. Please enter the file path to the puzzle's input...");
            puzzleInputFilePath = Console.ReadLine();
        }

        return puzzleInputFilePath;
    }

    private static string ReadPuzzleInput(string puzzleInputFilePath)
    {
        var puzzleInput = File.ReadAllText(puzzleInputFilePath);
        return puzzleInput;
    }

    private static bool TryParsePuzzleInput(IPuzzle puzzle, string filePath)
    {
        try
        {
            var puzzleInput = ReadPuzzleInput(filePath);
            puzzle.ParseInput(puzzleInput);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("Could not parse the puzzle input due to an error: {0}", e.Message);
            Console.WriteLine("Do you want to try again? (y/n)");

            return Console.ReadKey().Key == ConsoleKey.Y
                   && TryParsePuzzleInput(puzzle, filePath);
        }
    }

    private static bool TrySolvePuzzle(IPuzzle puzzle, out IPuzzleResult? puzzleResult)
    {
        try
        {
            puzzleResult = puzzle.Solve();
            return true;
        }
        catch (Exception exception)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(exception.Message);
            Console.ResetColor();

            puzzleResult = null;
            return false;
        }
    }
}