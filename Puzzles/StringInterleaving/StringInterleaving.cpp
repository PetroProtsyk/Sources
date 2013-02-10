//Three strings say A,B,C are given to you. Check weather 3rd string is interleaved from string A and B.
//http://www.careercup.com/question?id=14539805
//Example: A="abcd" B="xyz" C="axybczd". Answer is yes. o(N)

#include <stack>

struct state
{
	wchar_t *A;
	wchar_t *B;
	wchar_t *C;

	state(wchar_t *A, wchar_t *B, wchar_t *C)
		:A(A), B(B), C(C)
	{
	}
};

bool check_interleaving(wchar_t *A, wchar_t *B, wchar_t *C)
{
 std::stack<state> states;
 states.push(state(A, B, C));

 while (!states.empty())
 {
	 state s = states.top(); states.pop();
	 A = s.A; B = s.B; C = s.C;

	 while(*C != 0)
	 {
		 if ((*A == *B) && (*A == *C))
		 {
			 states.push(state(A, B+1, C+1));
			 A++;
		 }
		 else if (*A == *C)
		 {
			 A++;
		 }
		 else if (*B == *C)
		 {
			 B++;
		 }
		 else 
		 {
			 break;
		 }

		 C++;
	 }

	 if ((*A == 0) && (*B == 0))
	 {
		 return true;
	 }
 }

 return false;
}


int main(int argc, wchar_t* argv[])
{
 bool c1 = check_interleaving(L"ABBC", L"ABD", L"AABBDBC"); // Expect TRUE
 bool c2 = check_interleaving(L"abc", L"amn", L"amnbca"); // Expect TRUE

 return 0;
}

