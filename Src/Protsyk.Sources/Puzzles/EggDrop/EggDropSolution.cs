// Egg Dropping Puzzle
//
// Given N eggs and K floors find the minimum number of trials 
// required to determine the lowest floor in a building from which 
// when we drop an egg it should not break.
// Notes:
//  1. An egg that survives a fall can be used again.
//  2. A broken egg must be discarded.
//  3. The effect of a fall is the same for all eggs.
//  4. If an egg breaks when dropped, then it would break if dropped from a higher window.
//  5. If an egg survives a fall then it would survive a shorter fall.

using System;

namespace Protsyk.Sources.Puzzles.EggDrop
{
    public class EggDropSolution
    {
        private int maxEggs;
        private int maxFloors;
        private int[] solutions;

        private void Initialize(int maxEggs, int maxFloors)
        {
            this.maxEggs = maxEggs;
            this.maxFloors = maxFloors;
            this.solutions = new int[this.maxEggs * this.maxFloors];
            Array.Fill(solutions, -1);
        }

        public int BestDrop(int eggs, int floors)
        {
            Initialize(eggs, floors);
            return BestDropInternal(eggs, floors);
        }

        private int BestDropInternal(int eggs, int floors)
        {
            if (eggs == 1)
            {
                return floors;
            }

            if (floors == 1 || floors == 0)
            {
                return floors;
            }

            var index = eggs * maxEggs + floors;
            if (solutions[index] != -1)
            {
                return solutions[index];
            }

            int worst = floors + 1;
            for (int i = 1; i < floors; i++)
            {
                // Calculate solution in case egg broke on floor i
                int crashCase = BestDropInternal(eggs - 1, i - 1);

                // Calculate solution in case egg did not brake on floor i
                int noCrash = BestDropInternal(eggs, floors - i);

                // Can we improve worst case?
                worst = Math.Min(worst, 1 + Math.Max(crashCase, noCrash));
            }

            solutions[index] = worst;
            return worst;
        }
    }
}