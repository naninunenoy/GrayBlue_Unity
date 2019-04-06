GrayBlue_Unity
====

<img src="https://github.com/naninunenoy/GrayBlue/blob/doc/doc/icon.png" width="200" />

This is plugin for Unity(C#) to use [GrayBlue](https://github.com/naninunenoy/GrayBlue).

<img src="https://img.shields.io/badge/platform-WSA(.NET)-lightGray.svg" /> 

## Description

<img src="https://img.shields.io/badge/Gray-Blue-blue.svg?labelColor=lightGray" /> notifies 9-DOF motion data.

This is plugin helps GrayBlue device operation like scan/connect/disconnect and receiving 9-DOF sensor data (acc/gyro/mag/quaternion) from GrayBlue device.
This plugin show you how to bind your `GameObject` and GrayBlue device easily.

## Environment
Windows 10 (build 0.1506 and higher)

**You cannot use GrayBlue device in Unity Editor. You have to build your project as WSA(.NET) to use GrayBlue device actually.**

## Demo
Applied the notified quaternion to the `GameObject`.

<img src="https://github.com/naninunenoy/GrayBlue/blob/doc/doc/demo.gif" width="200" />

## Library
This project includes [GrayBlue_UWP](https://github.com/naninunenoy/GrayBlue_UWP) and its all dependency libraries as dll.
 * System.Reactive
    - https://github.com/dotnet/reactive
    - LICENSE: [Apache-2.0](https://licenses.nuget.org/Apache-2.0)
    - version: 4.1.2
    - dependency
       * [System.Threading.Tasks.Extensions](https://www.nuget.org/packages/System.Threading.Tasks.Extensions/)
       * [System.ValueTuple](https://www.nuget.org/packages/System.ValueTuple/)

## Licence
[MIT](https://github.com/naninunenoy/GrayBlue_Unity/blob/master/LICENSE)

## Author
[@naninunenoy](https://github.com/naninunenoy)