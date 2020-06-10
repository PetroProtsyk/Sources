The task is described in this article: http://www.cs.nott.ac.uk/~pszgmh/countdown.pdf

*Countdown Problem*
Given a sequence of numbers and a single target number, construct a valid arithmetic expression
by combining numbers from the sequence using addition, subtraction, multiplication, division and parentheses,
and such that the result of evaluating the expression is the target number.

A valid expression should satisfy the following conditions:
- Each number from the sequence can only be used once in the expression.
- It is not required to use all values from the sequence.
- All intermediate values must be positive natural numbers (1, 2, 3, etc). The use of negative numbers, zero and fractions are not permitted. Expressions like these should not appear in the valid expression: 2/3, (1-2), (1+2)-3.

For example, given the sequence of source numbers [1, 3, 7, 10, 25, 50] and the
target number 765, the expression (1 + 50) * (25 - 10) solves the problem.
