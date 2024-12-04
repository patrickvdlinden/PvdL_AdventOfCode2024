namespace AdventOfCode2024.Puzzles;

using System;
using System.Linq;
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
        var xmasOccurrences = FindWordOccurrences("XMAS");

        Console.WriteLine();
        Console.WriteLine("================== PART TWO ==================");
        Console.ResetColor();

        // Part 2 seems trickier, because the task is to only find the words 'MAS' that are in an X-formation.
        // For example:
        //   ..M.S..
        //   ...A...
        //   ..M.S..
        //
        // However, it is actually quite simpler because now only two directions needs to be searched.
        // For words in reverse direction order the search for the word can simply be reversed by detecting the last
        // character first.
        var crossWordOccurrences = FindCrossWordOccurrences("MAS");

        return new PuzzleResult(true)
        {
            WordOccurrences = xmasOccurrences,
            CrossWordOccurrences = crossWordOccurrences,
        };
    }

    private int FindWordOccurrences(string word)
    {
        if (word.Length == 0)
        {
            return 0;
        }

        // Get all possible word direction enum values to make iteration possible.
        var wordDirections = Enum.GetValues<WordDirections>();

        var occurrences = 0;
        var startChar = word[0];

        Console.ForegroundColor = ConsoleColor.Gray;

        for (var lineIndex = 0; lineIndex < _lines.Length; lineIndex++)
        {
            var line = _lines[lineIndex];
            for (var charIndex = 0; charIndex < line.Length; charIndex++)
            {
                // Continue searching until the first character of the word is found in the text.
                if (startChar != line[charIndex])
                {
                    continue;
                }

                Console.WriteLine($"Start character '{startChar}' found at {lineIndex + 1}:{charIndex + 1}");

                // At this point the first character of the word has been found. Now it's time to find any next character
                // in a sequence in each possible direction.
                foreach (var directions in wordDirections)
                {
                    Console.WriteLine($"Searching in direction: {directions}");

                    if (FindWordInDirection(lineIndex, charIndex, word, directions))
                    {
                        occurrences += 1;
                        Console.WriteLine($"The word was found! New total word occurrences: {occurrences}");
                    }
                }
            }
        }

        Console.ResetColor();

        return occurrences;
    }

    private int FindCrossWordOccurrences(string word)
    {
        if (word.Length == 0)
        {
            return 0;
        }

        // For the simplicity of this puzzle, only words with an odd length are allowed to search for as a cross-word.
        // This makes it easier to find the middle-point (e.g. MAS, RED, GREEN)
        if (word.Length % 3 != 0)
        {
            return 0;
        }

        // For this assignment, it's a bit easier to find the correct word as only the 'Down, Right' direction is needed
        // at first. This is because an X-formation can only exist if the word is already found in that direction.
        var crossWordOccurrences = 0;
        var startChar = word[0];
        var endChar = word[^1];
        var middlePoint = (int)Math.Ceiling(word.Length / 2f);

        Console.ForegroundColor = ConsoleColor.Gray;

        for (var lineIndex = 0; lineIndex < _lines.Length; lineIndex++)
        {
            var line = _lines[lineIndex];
            for (var charIndex = 0; charIndex < line.Length; charIndex++)
            {
                // Continue searching until the first OR last character of the word is found in the text.
                var firstCharFound = startChar == line[charIndex];
                var lastCharFound = endChar == line[charIndex];
                if (!firstCharFound && !lastCharFound)
                {
                    continue;
                }

                Console.WriteLine(
                    $"{(firstCharFound ? "Start" : "Last")} character '{(firstCharFound ? startChar : endChar)}' found at {lineIndex + 1}:{charIndex + 1}");

                // At this point the first OR last character of the word has been found. Now it's time to find any next
                // character in a sequence in the "Down, Right" direction.
                //
                // If the last character of the word was found, it's easier to find the word in reverse to minimize
                // the amount of directions to look for.
                var wordReversed = new string(word.Reverse().ToArray());
                var wordToFindFirst = firstCharFound
                    ? word
                    : wordReversed;

                if (FindWordInDirection(lineIndex, charIndex, wordToFindFirst, WordDirections.DownRight))
                {
                    // If this point is reached, the word has been found in the "Down, Right" direction. The only task
                    // remaining is to also find the same word in the "Down, Left" direction while it's crossing the
                    // previous word. The easiest way to do this, is to just look again but add the word's middle-point
                    // to the charIndex. The second search is for the same word but reversed, in case the word in normal
                    // order cannot be found.
                    if (FindWordInDirection(lineIndex, charIndex + middlePoint, word, WordDirections.DownLeft)
                        || FindWordInDirection(lineIndex, charIndex + middlePoint, wordReversed, WordDirections.DownLeft))
                    {
                        crossWordOccurrences += 1;
                        Console.WriteLine($"Cross-word found! New total cross-word occurrences: {crossWordOccurrences}");
                    }
                    else
                    {
                        Console.WriteLine("-- Second cross-word was not found.");
                    }
                }
                else
                {
                    Console.WriteLine("-- Word was not found.");
                }
            }
        }

        Console.ResetColor();

        return crossWordOccurrences;
    }

    private bool FindWordInDirection(
        int currentLineIndex,
        int currentCharIndex,
        string word,
        WordDirections directions)
    {
        var startLineIndex = currentLineIndex;
        var startCharIndex = currentCharIndex;

        foreach (var @char in word)
        {
            // Check if the line index is within bounds, or the match cannot be established.
            if (currentLineIndex < 0 || currentLineIndex >= _lines.Length)
            {
                return false;
            }

            // Check if the char index is within bounds, or the match cannot be established.
            if (currentCharIndex < 0 || currentCharIndex >= _lines[currentLineIndex].Length)
            {
                return false;
            }

            Console.WriteLine($"Trying to look for '{@char}'");

            // Check if the character at the current position matches the next expected character in the sequence.
            if (_lines[currentLineIndex][currentCharIndex] != @char)
            {
                return false;
            }

            if (directions.HasFlag(WordDirections.Down))
            {
                currentLineIndex += 1;
            }
            else if (directions.HasFlag(WordDirections.Up))
            {
                currentLineIndex -= 1;
            }

            if (directions.HasFlag(WordDirections.Right))
            {
                currentCharIndex += 1;
            }
            else if (directions.HasFlag(WordDirections.Left))
            {
                currentCharIndex -= 1;
            }
        }

        Console.WriteLine(
            $"Found the word {word} on {startLineIndex + 1}:{startCharIndex + 1} towards {currentLineIndex + 1}:{currentCharIndex + 1} in direction {directions}");
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

        public int WordOccurrences { get; init; }

        public int CrossWordOccurrences { get; init; }
    }
}