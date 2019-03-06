﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace IMUDevice {
    public interface IDevice {
        string ID { get; }
    }

    public interface IBLEDevice : IDevice {
        Task<IBLEDevice> ConnectAsync();
        void Disconnect();
        event Action DeviceLostEvent;
        void NotifyConnectionLost();
    }

    public interface IIMUEventSet {
        event Action<Vector3> AccelUpdateEvent;
        event Action<Vector3> GyroUpdateEvent;
        event Action<Vector3> CompassUpdateEvent;
        event Action<Quaternion> QuaternionUpdateEvent;
    }

    public interface IIMUEventDelegate {
        void NotifyUpdateAccel(Vector3 acc);
        void NotifyUpdateGyro(Vector3 gyro);
        void NotifyUpdateCompass(Vector3 mag);
        void NotifyUpdateQuaternion(Quaternion quat);
    }

    public interface IButtonEventSet {
        event Action<DeviceButton> ButtonPushEvent;
        event Action<DeviceButton> ButtonReleaseEvent;
    }

    public interface IButtonEventDelegate {
        void NotifyButtonPush(DeviceButton button);
        void NotifyButtonRelease(DeviceButton button);
    }

    public struct DeviceButton {
        public string button;
        public float pressTime;
    }
}
