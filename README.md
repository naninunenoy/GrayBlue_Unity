GrayBlue_Unity
====

<img src="https://github.com/naninunenoy/GrayBlue/blob/doc/doc/icon.png?raw=true" width="200" />

This is plugin for Unity(C#) to use [GrayBlue](https://github.com/naninunenoy/GrayBlue).

<img src="https://img.shields.io/badge/platform-WSA(.NET)-lightGray.svg" /> 

## Demo
Applied the notified quaternion to the `GameObject`.

<img src="https://github.com/naninunenoy/GrayBlue/blob/doc/doc/demo.gif?raw=true" width="200" />

## Description

<img src="https://img.shields.io/badge/Gray-Blue-blue.svg?labelColor=lightGray" /> notifies 9-DOF motion data.

This plugin helps GrayBlue device operation like scan/connect/disconnect and receiving 9-DOF sensor data (acc/gyro/mag/quaternion) from GrayBlue device.
This plugin show you how to bind your `GameObject` and GrayBlue device easily.

## Environment
* Windows 10 (build 0.1506 and higher)
* Unity 2018.3.7f1

**Would you like to use GrayBlue device in Unity Editor?**
**Yes, it possible with [GrayBlue_WinProxy](https://github.com/naninunenoy/GrayBlue_WinProxy).**

## Library
This project includes [GrayBlue_UWP](https://github.com/naninunenoy/GrayBlue_UWP) and its all dependency libraries as dll.
 * System.Reactive
    - https://github.com/dotnet/reactive
    - LICENSE: [Apache-2.0](https://licenses.nuget.org/Apache-2.0)
    - version: 4.1.2
    - dependency
       * [System.Threading.Tasks.Extensions](https://www.nuget.org/packages/System.Threading.Tasks.Extensions/)
       * [System.ValueTuple](https://www.nuget.org/packages/System.ValueTuple/)
       
And websocket library working on Unity.
 * websocket-sharp
   - https://github.com/sta/websocket-sharp
   - LICENSE: [MIT](https://github.com/sta/websocket-sharp/blob/master/LICENSE.txt)

## Licence
[MIT](https://github.com/naninunenoy/GrayBlue_Unity/blob/master/LICENSE)

## Author
[@naninunenoy](https://github.com/naninunenoy)
