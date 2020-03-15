#pragma once

template<class T>
class Singleton
{
public:

	static T* getInstance()
	{
#ifndef NDEBUG
		if (!instanceSet)
		{
			throw std::runtime_error("Singleton not set when trying to remove it!");
			return nullptr;
		}
#endif
		return instance;
	}

	static void setInstance(T &givenInstance)
	{
#ifndef NDEBUG
		if (instanceSet)
		{
			throw std::runtime_error("Singleton already set!");
			return;
		}
		instanceSet = true;
#endif
		instance = givenInstance;
	}

	static void removeInstance()
	{
#ifndef NDEBUG
		if (!instanceSet)
		{
			throw std::runtime_error("Singleton not set when trying to remove it!");
			return;
		}
		instanceSet = false;
#endif
		instance = nullptr;
	}
private:

#ifndef NDEBUG
	bool instanceSet = false;
#endif

	static T *instance;
};