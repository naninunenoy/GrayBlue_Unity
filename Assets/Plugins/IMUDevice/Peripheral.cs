using System;
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

        public void NotifyConnectionLost() {
            DeviceLostEvent?.Invoke();
        }

        public void NotifyUpdateAccel(Vector3 acc) {
            AccelUpdateEvent?.Invoke(acc);
        }

        public void NotifyUpdateGyro(Vector3 gyro) {
            GyroUpdateEvent?.Invoke(gyro);
        }

        public void NotifyUpdateCompass(Vector3 mag) {
            CompassUpdateEvent?.Invoke(mag);
        }

        public void NotifyUpdateQuaternion(Quaternion quat) {
            QuaternionUpdateEvent?.Invoke(quat);
        }

        public void NotifyUpdateIMU(IMUData imu) {
            IMUSensorUpdateEvent?.Invoke(imu);
        }

        public void NotifyButtonPush(DeviceButton button) {
            ButtonPushEvent?.Invoke(button);
        }

        public void NotifyButtonRelease(DeviceButton button) {
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
