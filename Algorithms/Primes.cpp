#include "stdafx.h"
#include <vector>
#include <iostream>
#include <Windows.h>

using namespace std;

void primes(int max, vector<int> &result) {
	vector<bool> p(max + 1, true);
	result.push_back(2);
	for (int i = 3; i <= max; i += 2) {
		if (p[i]) {
			for (int j = 2 * i; j <= max; j += i) {
				p[j] = false;
			}
			result.push_back(i);
		}
	}
}

int _tmain(int argc, _TCHAR* argv[])
{
	auto sw = GetTickCount64();
	vector<int> r;
	primes(1000000000, r);
	wcout << r.size() << L" " << GetTickCount64() - sw;
	return 0;
}

