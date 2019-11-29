﻿using System;
using System.Linq;
using Zaruri.Contracts;
using Zaruri.Models;

namespace Zaruri.Services
{
    public class Player
    {
        public Player(IRoller roller, IHandFactory factory, int amount)
        {
            this.roller = roller;
            this.factory = factory;

            this.amount = amount;
        }

        public bool IsBroke() => amount <= 0;

        public void MakeBet()
        {
            amount -= 1;
            Console.WriteLine($"1$ bet; current amount: {amount}$");
        }

        public void InitialRoll()
        {
            roll = roller.GenerateDice().ToArray();
            ShowRoll("Initial roll");
        }

        public void FinalRoll()
        {
            var indices = ReadIndices();

            roll = roll.Select((value, i) => indices.Contains(i + 1) ? value : roller.GenerateDie()).ToArray();
            ShowRoll("Final roll");
        }

        public void ComputeHand()
        {
            var hand = factory.Create(roll);
            amount += hand.Score;
            Console.WriteLine($"{hand.Name}: {hand.Score}$ -- amount {amount}$");
        }

        //

        private readonly IRoller roller;
        private readonly IHandFactory factory;

        private int amount;
        private int[] roll;

        private void ShowRoll(string prefix) => Console.WriteLine(prefix + $": {string.Join(" ", roll)}");

        private static Indices ReadIndices()
        {
            while (true)
            {
                Console.Write("Enter the dice to keep (1 .. 5), separated with spaces: ");
                try
                {
                    return Parse(Console.ReadLine());
                }
                catch
                {
                    // try again
                }
            }
        }

        private static Indices Parse(string line)
        {
            var values = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).Select(Index.Create).ToArray();
            return Indices.Create(values);
        }
    }
}