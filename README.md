# My Utilities

This is a repository for the utility functions I have written, both for C++ and C# (Unity) applications.

Unless specified otherwise, this code is provided under the [NMHL license](https://github.com/infuanfu/NMHL)

## What the code does
### C# (Unity)
* `Localization` : Editor window for the easy editing of localization files, scripts for loading/displaying the contents of said files. Parts of it depend on `EventManager`and `IOUtility`, though removing those dependencies is fairly trivial.
* `SceneSwitcher` : Simple script for switching scenes with various basic effects. Parts of it depend on `EventManager`, though again removing those dependencies is a matter of removing a few lines.
* `EventManager` : Simple event system. The event manager is a static class to which EventSubscribers subscribe. Objects can then broadcast events, giving them an `object` as a parameter
* `IOUtility` : Simple IO for Unity Mobile. Not asynchronous.
* `RingBuffer` : Barebones ring buffer. Useful for object pooling.
* `Timer` : Barebones timer

### C++
* `Logger` : Static class which logs formatted and non-formatted strings to stdout
* `RenderDocApi` : Simple class to load/communicate with the RenderDoc API
* `RenderToWindow` : Barebones class to render a bitmap to a window using the Windows API. Has a dependency to `Logger`, though again it can be removed easily.
* `vec` : templated vector of floats using C++20 concepts
* `squaremat` : templated square matrix of floats using C++20 concepts
* `BMPWriter` : Function which writes rgba BMPs to a specified file path
* `Delegate` : Delegate class based on the [impossibly fast delegate](https://www.codeproject.com/Articles/11015/The-Impossibly-Fast-C-Delegates)
* `Optional`: Barebones optional type with no operator overloading (std::optional<bool>, I'm looking at you). Essentially amounts to nothing but a wrapper with a bool.
* `RadixSort` : Radix sort implementation I wrote a long time ago. Not too sure about the quality of it, but perhaps it's useful to someone
* `Serializer` : Serialization classes
