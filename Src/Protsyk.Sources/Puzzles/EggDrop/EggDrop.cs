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

public class EggDrop
{
    private const int MAX_EGGS = 100;
    private const int MAX_FLOORS = 100;

    private static readonly int[] solutions = new int[MAX_EGGS * MAX_FLOORS];

    static EggDrop()
    {
        Array.Fill(solutions, -1);
    }

    public static int BestDrop(int eggs, int floors)
    {
        if (eggs == 1)
        {
            return floors;
        }

        if (floors == 1 || floors == 0)
        {
            return floors;
        }

        var index = eggs * MAX_EGGS + floors;
        if (solutions[index] != -1)
        {
            return solutions[index];
        }

        int worst = floors + 1;
        for (int i = 1; i < floors; i++)
        {
            // Calculate solution in case egg broke on floor i
            int crashCase = BestDrop(eggs - 1, i - 1);

            // Calculate solution in case egg did not brake on floor i
            int noCrash = BestDrop(eggs, floors - i);

            // Can we improve worst case?
            worst = Math.Min(worst, 1 + Math.Max(crashCase, noCrash));
        }

        solutions[index] = worst;
        return worst;
    }


    public static void Test()
    {
        int eggs = 2;
        int floors = 36;
        int result = BestDrop(eggs, floors);

        Console.WriteLine($"Number of trials: {result} for {eggs} eggs and {floors} floors");
    }

}