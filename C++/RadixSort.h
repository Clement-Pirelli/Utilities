#pragma once
#include <cstring>

class RadixSort
{
public:

	static void sort(int *toSort, unsigned int count)
	{
		unsigned int maxDigits = 1;
		int maxDigitNumber = toSort[0];
		for (unsigned int i = 0; i < count; i++)
		{
			unsigned int iDigit = digit(toSort[i]);
			if (iDigit > maxDigits)
			{
				maxDigits = iDigit;
				maxDigitNumber = toSort[i];
			}
		}

		for (unsigned int i = 1; maxDigitNumber / i > 0; i *= 10)
		{
			countSort(toSort, count, i);
		}
	}

private:

	static unsigned int digit(unsigned int x)
	{
		unsigned int digits = 1;
		while (x)
		{
			x /= 10;
			digits++;
		}
		return digits;
	}

	static void countSort(int *toSort, unsigned int count, unsigned int digit)
	{
		if (digit == 0) return;
		int bucket[10];
		int *returnVal = new int[count];
		int *sortArr = new int[count];
		memcpy(sortArr, toSort, sizeof(int)*count);
		memset(returnVal, 0, sizeof(int) * count);
		memset(bucket, 0, sizeof(int) * 10);

		for (unsigned int i = 0; i < count; i++)
		{
			sortArr[i] /= digit;
			sortArr[i] = sortArr[i] % 10;
		}

		for (unsigned int i = 0; i < count; i++)
		{
			bucket[sortArr[i]]++;
		}

		for (unsigned int i = 1; i < 10; i++)
		{
			bucket[i] += bucket[i - 1];
		}

		for (unsigned int i = count - 1; i >= 0 && i < count; i--)
		{
			returnVal[bucket[sortArr[i]] - 1] = toSort[i];
			bucket[sortArr[i]]--;
		}

		for (unsigned int i = 0; i < count; i++)
		{
			toSort[i] = returnVal[i];
		}

		delete[] returnVal;
	}
};
