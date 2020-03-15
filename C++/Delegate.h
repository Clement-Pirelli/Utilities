#pragma once

//base code from : https://www.codeproject.com/Articles/11015/The-Impossibly-Fast-C-Delegates
//INFO is the type given to the delegate as a parameter
template<typename INFO>
class Delegate
{
public:

	Delegate() : object(0), function(0){}

	//creates the delegate
	template <class T, void (T::*TMethod)(INFO&)>
	static Delegate makeDelegate(T *givenObject)
	{
		Delegate returnDelegate;
		returnDelegate.object = givenObject;
		returnDelegate.function = method_stub<T, TMethod>;
		return returnDelegate;
	}

	void operator()(INFO &info)
	{
		(*function)(object, info);
	}

private:
	//function which takes a void pointer and event information
	typedef void(*intermediaryFunction)(void*object, INFO&);

	template <class T, void (T::*TMethod)(INFO&)>
	static void method_stub(void* givenObject, INFO&givenInfo)
	{
		T* p = static_cast<T*>(givenObject);
		(p->*TMethod)(givenInfo); // #2
	}

	//anonymous object which we cast to its actual type in the function
	void* object;
	intermediaryFunction function;
};