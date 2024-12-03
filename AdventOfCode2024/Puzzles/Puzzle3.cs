namespace AdventOfCode2024.Puzzles;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

[UsedImplicitly]
public class Puzzle3 : IPuzzle
{
    private readonly List<Instruction> _instructions = new();

    public bool IsInputParsed { get; private set; }

    public void ParseInput(string input)
    {
        IsInputParsed = false;
        _instructions.Clear();

        // Look for instructions:
        //  - mul(x,x)
        //  - do()
        //  - don't()
        var match = Regex.Match(input, @"mul\((\d{1,3}),(\d{1,3})\)|do\(\)|don't\(\)");
        if (!match.Success)
        {
            throw new FormatException("The input does not contain any 'mul(x,x)' statements.");
        }

        do
        {
            var instructionMatch = Regex.Match(match.Groups[0].Value, @"([\w']+)\(");
            if (!instructionMatch.Success)
            {
                throw new FormatException($"Could not parse the instruction value '{match.Groups[0].Value}'.");
            }

            // All the relevant instructions have been found. It's now time to implement them.
            var instruction = instructionMatch.Groups[1].Value;
            if (instruction == "mul")
            {
                // The "mul" instruction has two value parts (left, right) which needs parsing to integers.
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

                _instructions.Add(new Instruction(instruction, leftPartValue, rightPartValue));
            }
            else
            {
                // The "do" and "don't" instructions have no value parts.
                _instructions.Add(new Instruction(instruction, null, null));
            }

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

        // The second part of Puzzle 3 makes it a bit more interesting. The input text can contain "do()" and
        // "don't()" instructions. All "mul(x,x)" instructions should be ignored between the "don't()" and "do()"
        // instructions.
        var shouldApplyInstructions = true;

        foreach (var instruction in _instructions)
        {
            switch (instruction.Name)
            {
                case "mul":
                    // When a "don't()" instruction has appeared, all the "mul" instructions must be ignored until a
                    // "do()" instruction happens again.
                    if (!shouldApplyInstructions)
                    {
                        break;
                    }

                    // The instruction says to multiply the two values.
                    var multiplied = instruction.Left.GetValueOrDefault() * instruction.Right.GetValueOrDefault();

                    // The multiplied value should be added to the total score.
                    totalScore += multiplied;
                    break;

                case "don't":
                    shouldApplyInstructions = false;
                    break;

                case "do":
                    shouldApplyInstructions = true;
                    break;
            }
        }

        return new PuzzleResult(true)
        {
            TotalScore = totalScore,
        };
    }

    private record Instruction(string Name, int? Left, int? Right);

    private record PuzzleResult : PuzzleResultBase
    {
        public PuzzleResult(bool isSolved)
            : base(isSolved)
        {
        }

        public int TotalScore { get; init; }
    }
}