//Given an integer array, sort the integer array such that the concatenated integer
//of the result array is max. e.g. [4, 94, 9, 14, 1] will be sorted to [9,94,4,14,1]
//where the result integer is 9944141
//http://www.careercup.com/question?id=7781671

#include <algorithm>
using namespace std;

struct compare
{
	bool operator()(int a, int b)
	{
		int x1 = 1;
		int x2 = 1;
		int t = a/10;
		while (t > 0) { t/= 10; x1*=10; }
		t = b/10;
		while (t > 0) { t/= 10; x2*=10; }

		int c1 = a / x1;
		int c2 = b / x2;

		while ((c1 == c2) && ((x1 > 1) || (x2 > 1)))
		{
			if (x1 > 1)
			{
				a %= x1;
				x1 /= 10;
				c1 = a/x1;
			}

			if (x2 > 1)
			{
				b %= x2;
				x2/=10;
				c2 = b/x2;
			}
		}

		return c1>c2;
	}
};

int main(int argc, wchar_t* argv[])
{
	int e1[] = {9,94,4,14,1};
	sort(begin(e1), end(e1), compare()); //[9,94,4,14,1]

	int e2[] = {8,89};
	sort(begin(e2), end(e2), compare()); //[89, 8]

	int e3[] = {97,989,9};
	sort(begin(e3), end(e3), compare()); //[9, 989, 97]

	int e4[] = {9,99,91,5,59,8,81,4,44,21}; //[9,99,91,8,81,59,5,4,44,21]
	sort(begin(e4), end(e4), compare());
}