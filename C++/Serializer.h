#pragma once
#include <cstdint>
#include <cassert>

class StreamIn
{
public:

	StreamIn(uint8_t *givenData, size_t givenMaxSize) :data(givenData), maxSize(givenMaxSize) {}

	template<typename T>
	T getNext() {
		size_t oldAt = at;
		at += sizeof(T);

		return deserialize<T>(data + oldAt, maxSize - oldAt);
	}

	template<typename T>
	void getNext(T *dataToFill, size_t amount)
	{
		for (size_t i = 0; i < amount; i++)
		{
			size_t oldAt = at;
			at += sizeof(T);

			dataToFill[i] = deserialize<T>(data + oldAt, maxSize - oldAt);
		}
	}

	template<typename T>
	T peekNext() {
		uint8_t *address = data + at;
		return deserialize<T>(address, maxSize - at);
	}

private:

	template<typename T>
	static T deserialize(uint8_t *data, size_t size)
	{
		assert(size >= sizeof(T));

		return *(reinterpret_cast<T *>(data));
	}

	uint8_t *data = nullptr;
	size_t at = 0U;
	size_t maxSize = 0U;
};


class StreamOut
{
public:

	StreamOut(size_t givenMaxSize) :data(new uint8_t[givenMaxSize]), maxSize(givenMaxSize) {}
	~StreamOut() { delete[] data; }

	template<typename T>
	void setNext(const T &item)
	{
		uint8_t *address = data + at;
		at += sizeof(T);
		memcpy(address, &item, sizeof(T));
	}

	template<typename T>
	void setNext(T *items, size_t amount)
	{
		uint8_t *address = data + at;
		at += sizeof(T) * amount;
		memcpy(address, items, sizeof(T) * amount);
	}

	uint8_t *getData()
	{
		return data;
	}

private:
	uint8_t *data = nullptr;
	size_t at = 0U;
	size_t maxSize = 0U;
};