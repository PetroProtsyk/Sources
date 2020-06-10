#include <memory>
#include <algorithm>
#include <vector>
#include <iostream>

struct Item
{
	int weight;
	int cost;
};

// Each item might be taken unlimited number of times
int SolveUnboundedDynamic(std::vector<Item> items, int capacity)
{
	// Dynamic Programming solution

	// best_cost[0] = 0;
	// best_cost[Weight] = Max(items[j].cost + best_cost[Weight-items[j].weight]) where items[j].weight <= Weight

	std::vector<int> best_cost;
	best_cost.resize(capacity+1);

	// Sort items by weight
	std::sort(items.begin(), items.end(), [&](Item &a, Item &b){ return a.weight < b.weight; });

	for (int i=1; i<=capacity; i++)
	{
		int maxCost = best_cost[i-1];

		for (int j=0; j<items.size() && items[j].weight <= i; j++)
		{
			int newCost = best_cost[i-items[j].weight] + items[j].cost;
			if (newCost > maxCost) maxCost = newCost;
		}

		best_cost[i] = maxCost;
	}

	return best_cost[capacity];
}

// Each item might be taken only once
int SolveBinaryDynamic(std::vector<Item> items, int capacity)
{
	// Dynamic Programming solution

	std::vector<std::vector<int>> best_solution;
	best_solution.resize(items.size());
	for (int i=0; i<items.size(); i++)
	{
		best_solution[i].resize(capacity+1);
	}

	for (int i=1; i<items.size(); i++)
	{
		for (int j=1; j<=capacity; j++)
		{
			if (j >= items[i].weight)
			{
				best_solution[i][j] = __max(best_solution[i-1][j], best_solution[i-1][j-items[i].weight] + items[i].cost);
			}
			else
			{
				best_solution[i][j] = best_solution[i-1][j];
			}
		}
	}

	return best_solution[items.size()-1][capacity];
}

int _tmain(int argc, _TCHAR* argv[])
{
	// Input
	int capacity;
	size_t items_count;
	std::vector<Item> items;

	std::cin >> capacity >> items_count;
	for (size_t i=0; i<items_count; i++)
	{
		Item item;
		std::cin >> item.weight >> item.cost;
		items.push_back(item);
	}

	std::cout << SolveBinaryDynamic(items, capacity) << std::endl;
	std::cout << SolveUnboundedDynamic(items, capacity) << std::endl;
	return 0;
}

