# My Utilities

This is a repository for the utility functions I have written, both for C++ and C# (Unity) applications.

Unless specified otherwise, this code is provided under the MIT License

## What the code does
### C# (Unity)
* `Localization` : Editor window for the easy editing of localization files, scripts for loading/displaying the contents of said files. Parts of it depend on `EventManager`and `IOUtility`, though removing those dependencies is fairly trivial.
* `SceneSwitcher` : Simple script for switching scenes with various basic effects. Parts of it depend on `EventManager`, though again removing those dependencies is a matter of removing a few lines.
* `EventManager` : Simple event system. The event manager is a static class to which EventSubscribers subscribe. Objects can then broadcast events, giving them an `object` as a parameter
* `IOUtility` : Simple IO for Unity Mobile. Not asynchronous.
* `RingBuffer` : Barebones ring buffer. Useful for object pooling.
* `Timer` : Barebones timer
* `ComputeUtilities` : Some utilities I wrote for Unity compute shader work. Of special interest is the `ComputeVariable` attribute, which allows the user to write `[ComputeVariable] SomeType someName;` and have their variable uploaded to a compute shader every frame. Optional parameters are available to specify which kernel, frequency of update and name should be used. `ComputeShaderScript` has a dependency on [NaughtyAttributes](https://github.com/dbrizov/NaughtyAttributes) for one button, but it can be easily removed/replaced.

### C++
* `Logger` : Static class which logs formatted and non-formatted strings to stdout
* `RenderDocApi` : Simple class to load/communicate with the RenderDoc API
* `RenderToWindow` : Barebones class to render a bitmap to a window using the Windows API. Has a dependency to `Logger`, though again it can be removed easily.
* `vec` : templated vectorN using C++20 concepts
* `squaremat` : templated matrixNxN using C++20 concepts
* `BMPWriter` : Function which writes rgba BMPs to a specified file path
* `Delegate` : Delegate class based on the [impossibly fast delegate](https://www.codeproject.com/Articles/11015/The-Impossibly-Fast-C-Delegates)
* `Optional`: Barebones optional type with no operator overloading (std::optional<bool>, I'm looking at you). Essentially amounts to nothing but a wrapper with a bool.
* `RadixSort` : Radix sort implementation I wrote a long time ago. Not too sure about the quality of it, but perhaps it's useful to someone
* `Serializer` : Serialization classes
