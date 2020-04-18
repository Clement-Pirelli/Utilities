#pragma once
#include <cstdint>
#include <cassert>

template<class T>
class Serializer{
public:
	static T deserialize(uint8_t *data, size_t size)
	{
		assert(size <= sizeof(T));

		return *(reinterpret_cast<T *>(data));
	}

	static uint8_t *serialize(const T &t) noexcept
	{
		return reinterpret_cast<uint8_t>(&t);
	}
private:
	Serializer() = delete;
	Serializer(const Serializer &other) = delete;
};

class StreamIn
{
public:

	StreamIn(uint8_t *givenData, size_t givenMaxSize):data(givenData), maxSize(givenMaxSize){}

	template<typename T>
	T getNext() {
		uint8_t *address = data + at;
		at += sizeof(T);

		return Serializer<T>::deserialize(address,maxSize-at);
	}

	template<typename T>
	T peekNext() {
		uint8_t *address = data + at;
		return Serializer<T>::deserialize(address, maxSize - at);
	}

private:
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
		assert(at <= maxSize);
		memcpy(address, &item, sizeof(T));
	}

	template<typename T>
	void setNext(T *items, size_t amount) 
	{
		uint8_t *address = data + at;
		at += sizeof(T)*amount;
		assert(at <= maxSize);
		memcpy(address, items, sizeof(T)*amount);
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
