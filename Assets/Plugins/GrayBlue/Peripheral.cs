﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace IMUDevice {
    public class Peripheral : IBLEDevice, IIMUEventSet, IIMUEventDelegate, IButtonEventSet, IButtonEventDelegate, IDisposable {
        public event Action DeviceLostEvent;
        public event Action<Vector3> AccelUpdateEvent;
        public event Action<Vector3> GyroUpdateEvent;
        public event Action<Vector3> CompassUpdateEvent;
        public event Action<Quaternion> QuaternionUpdateEvent;
        public event Action<DeviceButton> ButtonPushEvent;
        public event Action<DeviceButton> ButtonReleaseEvent;
        public event Action<IMUData> IMUSensorUpdateEvent;

        private IBLEDevice ble = default;
        public string ID => ble?.ID ?? string.Empty;
        public bool IsConnected => ble?.IsConnected ?? false;

        public Peripheral(IBLEDevice bleDevice) {
            ble = bleDevice;
        }

        public async Task<IBLEDevice> ConnectAsync() {
            var newBle = await ble.ConnectAsync().ConfigureAwait(false);
            if (newBle != null) {
                ble = newBle;
            }
            return newBle;
        }

        public void Disconnect() {
            ble?.Disconnect();
        }

        public void ListenEvent() {
            Central.Instance.AddListenner(ID, this, this);
        }

        public void UnlistenEvent() {
            Central.Instance.RemoveListenner(ID);
        }

        void IBLEDevice.NotifyConnectionLost() {
            DeviceLostEvent?.Invoke();
        }

        void IIMUEventDelegate.NotifyUpdateAccel(Vector3 acc) {
            AccelUpdateEvent?.Invoke(acc);
        }

        void IIMUEventDelegate.NotifyUpdateGyro(Vector3 gyro) {
            GyroUpdateEvent?.Invoke(gyro);
        }

        void IIMUEventDelegate.NotifyUpdateCompass(Vector3 mag) {
            CompassUpdateEvent?.Invoke(mag);
        }

        void IIMUEventDelegate.NotifyUpdateQuaternion(Quaternion quat) {
            QuaternionUpdateEvent?.Invoke(quat);
        }

        void IIMUEventDelegate.NotifyUpdateIMU(IMUData imu) {
            IMUSensorUpdateEvent?.Invoke(imu);
        }

        void IButtonEventDelegate.NotifyButtonPush(DeviceButton button) {
            ButtonPushEvent?.Invoke(button);
        }

        void IButtonEventDelegate.NotifyButtonRelease(DeviceButton button) {
            ButtonReleaseEvent?.Invoke(button);
        }

        public void Dispose() {
            UnlistenEvent();
            ble?.Disconnect();
            DeviceLostEvent = null;
            AccelUpdateEvent = null;
            GyroUpdateEvent = null;
            CompassUpdateEvent = null;
            QuaternionUpdateEvent = null;
            IMUSensorUpdateEvent = null;
            ButtonPushEvent = null;
            ButtonReleaseEvent = null;
            ble = null;
        }
    }
}
