namespace AdventOfCode2024.Puzzles;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

[UsedImplicitly]
public class Puzzle3 : IPuzzle
{
    private readonly List<(int Left, int Right)> _multiplierStatements = new();

    public bool IsInputParsed { get; private set; }

    public void ParseInput(string input)
    {
        IsInputParsed = false;
        _multiplierStatements.Clear();

        var regex = new Regex(@"mul\((\d{1,3}),(\d{1,3})\)");
        var match = regex.Match(input);
        if (!match.Success)
        {
            throw new FormatException("The input does not contain any 'mul(x,x)' statements.");
        }

        do
        {
            var leftPart = match.Groups[1].Value;
            if (!int.TryParse(leftPart, out var leftPartValue))
            {
                throw new FormatException($"Could not parse the left part value '{leftPart}' to an integer.");
            }

            var rightPart = match.Groups[2].Value;
            if (!int.TryParse(rightPart, out var rightPartValue))
            {
                throw new FormatException($"Could not parse the right part value '{rightPart}' to an integer.");
            }

            _multiplierStatements.Add((leftPartValue, rightPartValue));

            match = match.NextMatch();
        }
        while (match.Success);


        IsInputParsed = true;
    }

    public IPuzzleResult Solve()
    {
        // The first part of Puzzle 3 is to find "mul(x,x)" statements within a long string of text, containing all
        // kinds of other texts, assignments, codes, and other distractions. Even texts like "mul( x, x )" which should
        // be ignored. The sum of all valid multipliers is the answer to this puzzle.
        var totalScore = 0;

        foreach (var statement in _multiplierStatements)
        {
            // The statement says to multiply the two values.
            var multiplied = statement.Left * statement.Right;

            // The multiplied value should be added to the total score.
            totalScore += multiplied;
        }

        return new PuzzleResult(true)
        {
            TotalScore = totalScore,
        };
    }

    private record PuzzleResult : PuzzleResultBase
    {
        public PuzzleResult(bool isSolved)
            : base(isSolved)
        {
        }

        public int TotalScore { get; init; }
    }
}