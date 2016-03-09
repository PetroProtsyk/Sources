#include <iostream>
#include <vector>
#include <algorithm>
#define MAXN 10000
using namespace std;
unsigned __int64 Combinations[MAXN+1][MAXN+1];
void InitCombinations()
{
	Combinations[0][0] = 0;
	for (int i=1; i<=MAXN; i++){ Combinations[i][0] = 1; Combinations[0][i] = 1; }
	for (int i=1; i<=MAXN; i++)
	{
		Combinations[i][i] = 1;
		for (int j=i+1; j<=MAXN; j++)
		{
			Combinations[i][j] = (Combinations[i-1][j-1] + Combinations[i][j-1]) % (1000000007LL);
			Combinations[j][i] = Combinations[i][j];
		}
	}
}

int main(int argc, wchar_t* argv[])
{
	InitCombinations();
	int nc;
	cin >> nc;
	for (int i=0; i<nc; i++){
		unsigned __int64 n, k, a;
		cin >> n >> k;
		vector<unsigned __int64> v;
		v.reserve(k);
		for (int j=0; j<n; j++){ cin >> a; v.push_back(a); }
		sort(v.begin(), v.end());
		unsigned __int64 s = 0;
		auto m = v.rbegin();
		for (int j=0; j<n-k+1; j++, ++m){
			unsigned __int64 kk = Combinations[k-1][n-j-1];
			s = (s + ((*m) * kk)) % 1000000007LL;
		}
		cout << "Case #" << i+1 << ": " << s << endl;
	}
	return 0;
}