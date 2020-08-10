# Welcome to the xr-navigator, a PoC for AR/XR!

## Setting Up the Development Environment

1. Download and install Unity Hub: https://store.unity.com/download?ref=personal
2. Install Unity version: `2019.4.7f1` with the Android and iOS Build Support modules. (You can add modules after installing as well)
3. Download and install VSCode: https://code.visualstudio.com/
4. Open VSCode and install C# and Unity Code Snippets extensions.
5. Open the project in Unity and go to Unity > Preferences > External Tools > External Script Editor and set the drop-down to Visual Studio Code.
6. Download and install Mono (.NET framework for C#): https://www.mono-project.com/
7. In the open Unity project, go to Assets > Open C# Project.
   This should open the Unity project in VSCode. You should see two extra files in the file explorer, `Assembly-CSharp.csproj` and `xr-navigator.sln`. Try writing some Unity specific code and you should have auto-completion/IntelliSense now :)

## Building and Running the Project (Android)

1. Download and install Vysor (Allows you to project your phone screen to the computer) https://www.vysor.io/
2. Plug in your phone and click `Connect Network Device` in Vysor.
3. In Unity, click File > Build and Run. or (command + B)

## Debugging

Execute the following the commands in the terminal:

- `cd Library/Android/sdk/platform-tools/`
- `./adb logcat -s Unity dalvikvm DEBUG`
