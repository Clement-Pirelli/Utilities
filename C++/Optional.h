#pragma once
template <class T>
class Optional
{
public:
	void setValue(T givenValue)
	{
		value = givenValue;
		set = true;
	}

	T getValue()
	{
		return value;
	}

	bool isSet()
	{
		return set;
	}

private:

	T value;
	bool set;

};