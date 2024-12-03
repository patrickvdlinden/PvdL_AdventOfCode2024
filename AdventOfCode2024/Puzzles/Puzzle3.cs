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
            throw new FormatException("The input does not contain any known instructions.");
        }

        do
        {
            // All the relevant instructions have been found. It's now time to implement them. The instructions will be
            // converted to more readable Enum types.
            var instruction = match.Groups[0].Value;
            if (instruction.StartsWith("mul"))
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

                _instructions.Add(Instruction.Multiply(leftPartValue, rightPartValue));
            }
            else if (instruction.StartsWith("don't"))
            {
                _instructions.Add(Instruction.Dont());
            }
            else if (instruction.StartsWith("do"))
            {
                _instructions.Add(Instruction.Do());
            }

            match = match.NextMatch();
        }
        while (match.Success);


        IsInputParsed = true;
    }

    public IPuzzleResult Solve()
    {
        // The first part of Puzzle 3 is to find "Multiplier" instructions within a long string of text, containing all
        // kinds of other texts, assignments, codes, and other distractions. The sum of all valid multipliers is the
        // answer to this puzzle.
        var totalScore = 0;

        // The second part of Puzzle 3 makes it a bit more interesting. The input text can contain "Do" and
        // "Don't" instructions. All "Multiply" instructions should be ignored between instructions "Don't" and "Do".
        var shouldApplyInstructions = true;

        foreach (var instruction in _instructions)
        {
            switch (instruction.Type)
            {
                case InstructionType.Multiply:
                    // When a "Don't" instruction has appeared, all the "Multiply" instructions must be ignored until a
                    // "Do" instruction happens again.
                    if (!shouldApplyInstructions)
                    {
                        break;
                    }

                    // The instruction says to multiply the two values.
                    var multiplied = instruction.Left.GetValueOrDefault() * instruction.Right.GetValueOrDefault();

                    // The multiplied value should be added to the total score.
                    totalScore += multiplied;
                    break;

                case InstructionType.Dont:
                    shouldApplyInstructions = false;
                    break;

                case InstructionType.Do:
                    shouldApplyInstructions = true;
                    break;
            }
        }

        return new PuzzleResult(true)
        {
            TotalScore = totalScore,
        };
    }

    private enum InstructionType
    {
        Multiply,
        Do,
        Dont,
    }

    private record Instruction
    {
        private Instruction(InstructionType type, int? left, int? right)
        {
            Type = type;
            Left = left;
            Right = right;
        }

        public InstructionType Type { get; }

        public int? Left { get; }

        public int? Right { get; }

        public static Instruction Multiply(int left, int right)
            => new(InstructionType.Multiply, left, right);

        public static Instruction Do()
            => new(InstructionType.Do, null, null);

        public static Instruction Dont()
            => new(InstructionType.Dont, null, null);
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