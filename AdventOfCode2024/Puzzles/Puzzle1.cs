namespace AdventOfCode2024.Puzzles;

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

[UsedImplicitly]
public class Puzzle1 : IPuzzle
{
	private readonly List<int> _list1 = [];
	private readonly List<int> _list2 = [];

	public bool IsInputParsed { get; private set; }

	public void ParseInput(string input)
	{
		var lines = input.Split(['\r', '\n'], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
		var lineNumber = 0;
		foreach (var line in lines)
		{
			lineNumber += 1;

			var parts = line.Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length != 2)
			{
				throw new FormatException(
					$"Could not parse the lists, unexpected format. Line number {lineNumber} does not contain 2 values.");
			}

			if (!int.TryParse(parts[0], out var leftPart))
			{
				throw new FormatException(
					$"Could not parse the lists. The left part value '{parts[0]}' on line {lineNumber} could not be parsed to an integer.");
			}

			if (!int.TryParse(parts[1], out var rightPart))
			{
				throw new FormatException(
					$"Could not parse the lists. The right part value '{parts[1]}' on line {lineNumber} could not be parsed to an integer.");
			}

			_list1.Add(leftPart);
			_list2.Add(rightPart);
		}

		IsInputParsed = true;

		Console.WriteLine("List 1 Count: {0}, List 2 Count: {1}", _list1.Count, _list2.Count);
	}

	public IPuzzleResult? Solve()
	{
		if (!IsInputParsed)
		{
			throw new InvalidOperationException("The input should be parsed first.");
		}

		// This puzzle requires 2 lists with the same size.
		if (_list1.Count != _list2.Count)
		{
			throw new InvalidOperationException("Could not solve the puzzle, as the list counts are not equal.");
		}

		// First, let's sort both lists from lowest to highest. This is the puzzle's requirement for part A to pair the
		// subsequent lowest value for each list.
		_list1.Sort();
		_list2.Sort();

		// The 1st part of this puzzle requires to keep track of the total difference. This is calculated by iterating
		// list 1 and then compare each value to the value with the same index from list 2. The absolute difference
		// between the two values is then added to the total.
		var totalDifference = 0;

		// The 2nd part of this puzzle requires a total similarity score. This is calculated by iterating list 1 and
		// checking how many times that number is represented in list 2. The value for that index from list 1 is then
		// multiplied by the amount of times it is represented in list 2, which is then added to the total similarity score.
		var totalSimilarityScore = 0;

		Console.ForegroundColor = ConsoleColor.Gray;

		// Let's now make a loop that is equal to both list counts.
		for (var index = 0; index < _list1.Count; index++)
		{
			// The left part is the value from list 1, the right part the value from list 2. The values are each the
			// lowest possible value per index.
			var leftPartValue = _list1[index];
			var rightPartValue = _list2[index];

			var difference = CalculateDifference(leftPartValue, rightPartValue);
			var similarityScore = CalculateSimilarityScore(leftPartValue);

			// Now the totals need to be updated for the final answer.
			totalDifference += difference;
			totalSimilarityScore += similarityScore;

			Console.WriteLine(
				"#{0}: {1} from list1 has a absolute difference of {2} towards {3} from list2 and has a similarity score of {4}",
				index + 1,
				leftPartValue,
				difference,
				rightPartValue,
				similarityScore);
		}

		// Done! Both answers can now be returned.
		Console.WriteLine("The total difference, based on sort order, is: {0}", totalDifference);
		Console.WriteLine("The total similarity score is: {0}", totalSimilarityScore);
		Console.ResetColor();

		return new PuzzleResult(true)
		{
			TotalDifference = totalDifference,
			TotalSimilarityScore = totalSimilarityScore,
		};
	}

	private static int CalculateDifference(int left, int right)
	{
		// The difference between the left value and the right value can be negative, hence the Math.Abs to always
		// get a positive number.
		var difference = Math.Abs(left - right);

		return difference;
	}

	private int CalculateSimilarityScore(int leftPartValue)
	{
		// For each value from list 1 (left part), we also check how many times this number occurs in list 2 (right part).
		var occurCount = 0;
		foreach (var rightPartValue in _list2)
		{
			if (rightPartValue != leftPartValue)
			{
				continue;
			}

			occurCount += 1;
		}

		// The similarity score for this value is then calculated by multiplying the occur count to the left part value.
		var similarityScore = leftPartValue * occurCount;

		return similarityScore;
	}

	private record PuzzleResult : PuzzleResultBase
	{
		public PuzzleResult(bool isSolved)
			: base(isSolved)
		{
		}

		public int TotalDifference { get; init; }

		public int TotalSimilarityScore { get; init; }
	}
}