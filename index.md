---
layout: default
title: Home
navigation_weight: 1
---

# Librelancer
A re-implementation of Freelancer in C# and OpenGL.
*Freelancer is not currently playable at current. This is a tech-demo*

Currently running on OSX, Windows and Linux.
Pull Requests are welcome!

To help speed up development, donations can be sent to paypal.paymentsmc@gmail.com 
### General Requirements
* GPU must be capable of OpenGL 3.1+
* A *vanilla* Freelancer installation (demo untested but should work)

### Build Instructions

#### Windows
*Note:* SDL2, OpenAL-Soft and Freetype for windows are included in this repository.

1. Make sure you have the .NET Framework 4.5 installed with Visual Studio 2015
2. Clone this repository and submodules with whichever client you choose
3. Build src/LibreLancer.sln, and launch *Launcher*
4. Enjoy

#### Linux
1. Install mpv, mono, sdl2, openal and freetype
2. Clone this repository with `git clone --recursive https://github.com/CallumDev/Librelancer`
3. Build src/LibreLancer.sln in Xamarin Studio or with xbuild, and launch *Launcher*
4. Enjoy

#### Mac
1. Install Xcode 7
2. Install Xamarin Studio and the Xamarin.Mac SDK
3. Make sure you have homebrew installed, and install the sdl2 and freetype packages
4. Clone this repository with `git clone --recursive https://github.com/CallumDev/Librelancer`
5. Build src/LibreLancer.sln in Xamarin Studio, and launch *Launcher.Mac*
6. **OPTIONAL**: If you are packaging, install mpv.app in packaging/assets (you can run packaging/mpv_minimal to do this)
7. **OPTIONAL**: Run packaging/package_osx to produce an app bundle
8. Enjoy!
