namespace AdventOfCode2024.Puzzles;

using System;
using JetBrains.Annotations;

[UsedImplicitly]
public class Puzzle4 : IPuzzle
{
    private string[] _lines = [];

    public bool IsInputParsed { get; private set; }

    public void ParseInput(string input)
    {
        IsInputParsed = false;

        _lines = input.Split(['\r', '\n'], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        IsInputParsed = true;
    }

    public IPuzzleResult Solve()
    {
        // Puzzle 4 part 1 asks to find the word 'XMAS' through a string of text with all kinds of clutter.
        // The word can occur in all directions. Example:
        //   ..X...
        //   .SAMX.
        //   .A..A.
        //   XMAS.S
        //   .X....
        //
        // The strategy here is to find the letter 'X' first and start looking in all directions from there. Whenever
        // the second letter 'M' is found, the search direction is becoming fixed to look for the 'A' and 'S' as well.
        var occurrences = FindWordOccurrences("XMAS");

        return new PuzzleResult(true)
        {
            Occurrences = occurrences,
        };
    }

    private int FindWordOccurrences(string word)
    {
        if (word.Length == 0)
        {
            return 0;
        }

        var allWordDirections = Enum.GetValues<WordDirections>();

        var occurrences = 0;
        var startChar = word[0];

        Console.ForegroundColor = ConsoleColor.Gray;

        for (var lineNumber = 0; lineNumber < _lines.Length; lineNumber++)
        {
            var line = _lines[lineNumber];
            for (var charIndex = 0; charIndex < line.Length; charIndex++)
            {
                // Continue searching until the first character of the word is found in the text.
                if (startChar != line[charIndex])
                {
                    continue;
                }

                Console.WriteLine($"Start character 'X' found at {lineNumber}:{charIndex}");

                // At this point the first character of the word has been found. Now it's time to find any next character
                // in a sequence in each possible direction.
                foreach (var wordDirections in allWordDirections)
                {

                    Console.WriteLine($"Searching in direction: {wordDirections}");

                    if (FindWordInDirection(lineNumber, charIndex, word, wordDirections))
                    {
                        occurrences += 1;
                    }
                }
            }
        }

        Console.ResetColor();

        return occurrences;
    }

    private bool FindWordInDirection(int currentLineNumber, int currentCharIndex, string word, WordDirections directions)
    {
        var startLineNumber = currentLineNumber;
        var startCharIndex = currentCharIndex;

        for (var i = 1; i < word.Length; i++)
        {
            var @char = word[i];

            if (directions.HasFlag(WordDirections.Down))
            {
                currentLineNumber += 1;
            }
            else if (directions.HasFlag(WordDirections.Up))
            {
                currentLineNumber -= 1;
            }

            if (directions.HasFlag(WordDirections.Right))
            {
                currentCharIndex += 1;
            }
            else if (directions.HasFlag(WordDirections.Left))
            {
                currentCharIndex -= 1;
            }

            // Check if the line index is within bounds, or the match cannot be established.
            if (currentLineNumber < 0 || currentLineNumber >= _lines.Length)
            {
                return false;
            }

            // Check if the char index is within bounds, or the match cannot be established.
            if (currentCharIndex < 0 || currentCharIndex >= _lines[currentLineNumber].Length)
            {
                return false;
            }

            // Check if the character at the current position matches the next expected character in the sequence.
            if (_lines[currentLineNumber][currentCharIndex] != @char)
            {
                return false;
            }
        }

        Console.WriteLine($"Found the word {word} on {startLineNumber}:{startCharIndex} towards {currentLineNumber}:{currentCharIndex} in direction {directions}");

        return true;
    }

    [Flags]
    private enum WordDirections
    {
        Right = 1,
        Down = 2,
        Left = 4,
        Up = 8,
        DownRight = Down | Right,
        DownLeft = Down | Left,
        UpLeft = Up | Left,
        UpRight = Up | Right,
    }

    private record PuzzleResult : PuzzleResultBase
    {
        public PuzzleResult(bool isSolved)
            : base(isSolved)
        {
        }

        public int Occurrences { get; init; }
    }
}