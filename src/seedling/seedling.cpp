#include <iostream>
#include <random>
#include <fstream>

using namespace std;

void alloc_array(int size);

int main()
{
	int count = 0x1269AE40;

	alloc_array(count);
}

void alloc_array(int size)
{
	cout << "allocating file...";

	random_device rd;
	mt19937 gen(rd());

	int* arr = new int[size] { 0 };

	for (int i = 0; i < size; i++)
	{
		uniform_int_distribution<> dist(0, i);

		int j = dist(gen);

		// swap
		if (j != i)
			arr[i] = arr[j];

		// write
		arr[j] = i + 1; // exclude 0
	}

	cout << "verifying range of integer values...";

	long long sum = 0;
	long long expected = size * (size + 1LL) / 2LL;

	for (int i = 0; i < size; i++)
		sum = sum + arr[i];

	if (sum != expected)
	{
		cout << "incorrect range of values.";
		return;
	}

	cout << "writing integers to disk...";

	ofstream file;

	file.open("rand_int32_0x1269AE40.bin", ios::binary | ios::out);

	for (int i = 0; i < size; i++)
	{
		file.put(arr[i]);
		file.put(arr[i] >> 8);
		file.put(arr[i] >> 16);
		file.put(arr[i] >> 24);
	}

	file.close();

	cout << "...done.";
}