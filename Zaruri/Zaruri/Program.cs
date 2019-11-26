﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Zaruri
{
    internal class Program
    {
        private static void Main()
        {
            var amount = 100;
            while (amount > 0)
            {
                MakeBet(ref amount);

                var roll1 = InitialRoll();
                var roll2 = FinalRoll(roll1);

                var hand = GetHand(roll2);
                ComputeResult(hand, ref amount);

                Console.WriteLine();
            }
        }

        //

        private static readonly Random RND = new Random();

        private static IEnumerable<int> GenerateDice() => Enumerable.Range(1, Constants.DICE_COUNT).Select(_ => GenerateDie()).ToArray();
        private static int GenerateDie() => RND.Next(1, Constants.DIE_MAX + 1);

        private static void MakeBet(ref int amount)
        {
            amount -= 1;
            Console.WriteLine($"1$ bet; current amount: {amount}$");
        }

        private static IEnumerable<int> InitialRoll()
        {
            var roll1 = GenerateDice().ToArray();
            ShowRoll("Initial roll", roll1);

            return roll1;
        }

        private static int[] FinalRoll(IEnumerable<int> roll1)
        {
            var indices = ReadIndices();

            var roll2 = roll1.Select((value, i) => indices.Contains(i + 1) ? value : GenerateDie()).ToArray();
            ShowRoll("Final roll", roll2);

            return roll2;
        }

        private static void ShowRoll(string prefix, IEnumerable<int> roll) => Console.WriteLine(prefix + $": {string.Join(" ", roll)}");

        private static Indices ReadIndices()
        {
            while (true)
            {
                Console.Write("Enter the dice to keep (1 .. 5), separated with spaces: ");
                try
                {
                    return ParseKeep(Console.ReadLine());
                }
                catch
                {
                    // try again
                }
            }
        }

        private static Indices ParseKeep(string line)
        {
            var values = line.Split(' ').Select(int.Parse).Select(Index.Create).ToArray();
            return Indices.Create(values);
        }

        private static void ComputeResult(Hand hand, ref int amount)
        {
            amount += hand.Score;
            Console.WriteLine($"{hand.Name}: {hand.Score}$ -- amount {amount}$");
        }

        [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
        private static Hand GetHand(int[] roll)
        {
            roll = roll.OrderBy(it => it).ToArray();

            var groups = roll.GroupBy(it => it).ToArray();
            var groupCounts = groups.Select(it => it.Count()).OrderByDescending(it => it).ToArray();

            if (roll.SequenceEqual(new[] { 2, 3, 4, 5, 6 }))
                return new HighFlush();
            if (roll.SequenceEqual(new[] { 1, 2, 3, 4, 5 }))
                return new LowFlush();

            if (groupCounts.SequenceEqual(new[] { 5 }))
                return new FiveOfAKind();
            if (groupCounts.SequenceEqual(new[] { 4, 1 }))
                return new FourOfAKind();
            if (groupCounts.SequenceEqual(new[] { 3, 2 }))
                return new FullHouse();
            if (groupCounts.SequenceEqual(new[] { 3, 1, 1 }))
                return new ThreeOfAKind();
            if (groupCounts.SequenceEqual(new[] { 2, 2, 1 }))
                return new TwoPairs();
            if (groupCounts.SequenceEqual(new[] { 2, 1, 1, 1 }))
                return new OnePair();

            return new Nothing();
        }
    }
}