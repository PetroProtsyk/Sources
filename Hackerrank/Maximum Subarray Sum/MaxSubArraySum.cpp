#include <cmath>
#include <cstdio>
#include <vector>
#include <iostream>
#include <fstream>
#include <algorithm>
#include <set>

using namespace std;

//https://www.hackerrank.com/challenges/maximum-subarray-sum
int main() {
	std::ifstream in("input13.txt");
	std::streambuf *cinbuf = std::cin.rdbuf(); //save old buf
	std::cin.rdbuf(in.rdbuf()); //redirect std::cin to in.txt!

	int T;
	cin >> T;
	for (int t = 0; t<T; ++t)
	{
		long int N, M;
		cin >> N >> M;
		long long int *a = new long long int[N];

		for (int k = 0; k<N; ++k)
		{
			cin >> a[k];
		}

		/* long int best1 = 0;
		for (int k=0; k<N; ++k)
		{
		long int sum = 0;
		for (int j=k; j<N; ++j)
		{
		sum += a[j];
		best1 = max(best1, sum%M);
		}
		}*/

		long long int *s = new long long int[N];
		s[0] = a[0] % M;
		set<int> sums;
		sums.insert(s[0]);
		long long int best = s[0];
		for (int k = 1; k<N; ++k)
		{
			s[k] = (s[k - 1] + a[k]) % M;
			auto it = sums.upper_bound(s[k]);
			if (it != sums.end())
			{
				best = max(best, (s[k] - (*it) + M) % M);
			}
			else
			{
				best = max(best, s[k]);
			}
			sums.insert(s[k]);
		}
		cout << best << endl;
	}
	return 0;
}
