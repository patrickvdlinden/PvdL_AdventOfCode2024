namespace AdventOfCode2024.Puzzles;

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

[UsedImplicitly]
public class Puzzle2 : IPuzzle
{
    private readonly List<Report> _reports = [];

    public bool IsInputParsed { get; private set; }

    public void ParseInput(string input)
    {
        IsInputParsed = false;
        _reports.Clear();

        var lines = input.Split(['\r', '\n'], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var lineNumber = 0;

        foreach (var line in lines)
        {
            lineNumber += 1;

            var parts = line.Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                continue;
            }

            var levels = new List<int>();
            foreach (var part in parts)
            {
                if (!int.TryParse(part, out var level))
                {
                    throw new FormatException(
                        $"Could not parse the reports list. Value '{part}' on line {lineNumber} could not be parsed to an integer.");
                }

                levels.Add(level);
            }

            var report = new Report(levels);
            _reports.Add(report);
        }

        IsInputParsed = true;

        Console.WriteLine("Reports Count: {0}", _reports.Count);
    }

    public IPuzzleResult Solve()
    {
        if (!IsInputParsed)
        {
            throw new InvalidOperationException("The input should be parsed first.");
        }

        // Puzzle 2 asks to give the total sum of reports that are considered 'Safe'. For the report to be Safe, each
        // report is checked to see if all of its containing levels are all increasing or all decreasing. Any two
        // adjacent levels must also differ by at least one and at most three. Part B of the puzzle adds one exception
        // to this rule. That is, that one, and only one, level in the sequence can be ignored for the report to be
        // considered Safe.
        var safeReportsCount = 0;

        Console.ForegroundColor = ConsoleColor.Gray;

        // Let's iterate every report to check on its levels.
        for (var index = 0; index < _reports.Count; index++)
        {
            var report = _reports[index];

            var isSafe = IsReportSafe(report);
            if (isSafe)
            {
                safeReportsCount += 1;
            }

            Console.WriteLine(
                "#{0}: {1} is considered {2}",
                index + 1,
                string.Join(", ", report.Levels),
                isSafe ? "Safe" : "Unsafe");
        }

        Console.WriteLine("The total amount of Safe reports is: {0}", safeReportsCount);
        Console.ResetColor();

        return new PuzzleResult(true)
        {
            SafeReportsCount = safeReportsCount,
        };
    }

    private bool IsReportSafe(Report report)
    {
        // If the report contains only one level or less, it can already be marked as Safe as there is no sequence to check.
        if (report.Levels.Count <= 1)
        {
            return true;
        }

        // To make sure all edge-case scenarios are caught, it's needed to compare and track the level differences in
        // both sequence directions (from start to end, and from end to start). Only a single iteration is needed for this.
        var safeListStartToEnd = new List<int> { report.Levels[0] };
        var safeListEndToStart = new List<int> { report.Levels[^1] };

        var directionSafeListStartToEnd = 0;
        var directionSafeListEndToStart = 0;

        var startToEndIndex = 1;
        var endToStartIndex = report.Levels.Count - 2;

        while (endToStartIndex >= 0)
        {
            var levelA = report.Levels[startToEndIndex];
            var levelB = report.Levels[endToStartIndex];

            var previousLevelA = safeListStartToEnd[^1];
            var previousLevelB = safeListEndToStart[^1];

            var directionLevelA = GetLevelDirection(previousLevelA, levelA);
            var directionLevelB = GetLevelDirection(previousLevelB, levelB);

            // Check if the next level in the start-to-end sequence is Safe.
            if (IsLevelDifferenceSafe(previousLevelA, levelA)
                && (safeListStartToEnd.Count == 1 || directionLevelA == directionSafeListStartToEnd))
            {
                safeListStartToEnd.Add(levelA);
                directionSafeListStartToEnd = directionLevelA;
            }

            // Check if the next level in the end-to-start sequence is Safe.
            if (IsLevelDifferenceSafe(previousLevelB, levelB)
                && (safeListEndToStart.Count <= 1 || directionLevelB == directionSafeListEndToStart))
            {
                safeListEndToStart.Add(levelB);
                directionSafeListEndToStart = directionLevelB;
            }

            startToEndIndex += 1;
            endToStartIndex -= 1;
        }

        // The report is considered Safe if any of the two tracked directions list count is equal to or higher than the
        // total report levels count minus 1.
        var isSafe = safeListStartToEnd.Count >= report.Levels.Count - 1
               || safeListEndToStart.Count >= report.Levels.Count - 1;
        return isSafe;
    }

    private int GetLevelDirection(int left, int right)
    {
        var difference = right - left;
        var direction = difference switch
        {
            > 0 => 1,
            0 => 0,
            _ => -1
        };
        return direction;
    }

    private bool IsLevelDifferenceSafe(int left, int right)
    {
        var difference = Math.Abs(left - right);
        return difference is >= 1 and <= 3;
    }

    private record Report(List<int> Levels);

    private record PuzzleResult : PuzzleResultBase
    {
        public PuzzleResult(bool isSolved)
            : base(isSolved)
        {
        }

        public int SafeReportsCount { get; init; }
    }
}