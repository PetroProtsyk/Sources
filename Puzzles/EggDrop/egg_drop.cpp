// Egg Dropping Puzzle
//
// Given N eggs and K floors find minimum number of trials required to find
// the floor from which it is not safe to drop an egg, i.e. egg will brake
// when dropping from that floor.

#include "stdafx.h"
#include <algorithm>
#include <vector>
#include <memory>

std::unique_ptr<int []> solutions;
int MAX_EGGS = 100;
int MAX_FLOORS = 100;

int Index(int eggs, int floors)
{
	return eggs*MAX_EGGS + floors;
}

int BestDrop(int eggs, int floors)
{
	if (eggs == 1)
	{
		return floors;
	}

	if (floors == 1 || floors == 0)
	{
		return floors;
	}

	size_t index = Index(eggs, floors);
	if (solutions[index] != -1)
	{
		return solutions[index];
	}

	int worst = floors + 1;
	int floor = 0;

	for (int i=1; i<floors; i++)
	{
		// Calculate solution in case egg broke on floor i
		int crash_case = BestDrop(eggs-1, i-1);

		// Calculate solution in case egg did not brake on floor i
		int no_crash   = BestDrop(eggs, floors-i);

		// Can we improve worst case?
		worst = std::min(worst, 1 + std::max(crash_case, no_crash));
	}

	solutions[index] = worst;
	return worst;
}


int _tmain(int argc, _TCHAR* argv[])
{
	size_t size = MAX_EGGS * MAX_FLOORS;
	solutions.reset(new int[size]);
	memset(solutions.get(), -1, size * sizeof(int));

	int eggs = 2;
	int floors = 36;
	int result = BestDrop(eggs, floors);

	printf("Number of trials:%d for %d eggs and %d floors", result, eggs, floors);

	return 0;
}

