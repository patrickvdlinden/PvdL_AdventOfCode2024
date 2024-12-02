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

        // Puzzle 2 asks to give the total sum of reports that are considered 'safe'. For the report to be safe, each
        // report is checked to see if all of its containing levels are all increasing or all decreasing. Any two
        // adjacent levels must also differ by at least one and at most three.
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
        var lastLevel = -1;
        var wasIncreasing = false;
        var wasDecreasing = false;

        foreach (var level in report.Levels)
        {
            if (lastLevel == -1)
            {
                lastLevel = level;
                continue;
            }

            if (lastLevel == level)
            {
                // Unsafe. The current level has the same value as the previous level, but the levels must change by at
                // least 1.
                return false;
            }

            var isIncreasing = level > lastLevel;
            var isDecreasing = level < lastLevel;

            if (isDecreasing && wasIncreasing)
            {
                // Unsafe. The current level is decreasing, while the previous level was increasing.
                return false;
            }

            if (isIncreasing && wasDecreasing)
            {
                // Unsafe. The current level is increasing, while the previous level was decreasing.
                return false;
            }

            if ((isIncreasing || isDecreasing) && Math.Abs(lastLevel - level) > 3)
            {
                // Unsafe. The current level differs by more than 3 compared to the previous level.
                return false;
            }

            lastLevel = level;
            wasIncreasing = isIncreasing;
            wasDecreasing = isDecreasing;
        }

        // When the code reaches this point, all fail-safe checks have been passed.
        return true;
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