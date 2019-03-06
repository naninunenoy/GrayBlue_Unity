using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace IMUDevice {
    public class Peripheral : IBLEDevice, IIMUEventSet, IIMUEventDelegate, IButtonEventSet, IButtonEventDelegate {
        public event Action OnLostDevice;
        public event Action<Vector3> OnUpdateAccel;
        public event Action<Vector3> OnUpdateGyro;
        public event Action<Vector3> OnUpdateCompass;
        public event Action<Quaternion> OnUpdateQuaternion;
        public event Action<DeviceButton> OnButtonPush;
        public event Action<DeviceButton> OnButtonRelease;

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
            OnLostDevice?.Invoke();
        }

        public void NotifyUpdateAccel(Vector3 acc) {
            OnUpdateAccel?.Invoke(acc);
        }

        public void NotifyUpdateGyro(Vector3 gyro) {
            OnUpdateGyro?.Invoke(gyro);
        }

        public void NotifyUpdateCompass(Vector3 mag) {
            OnUpdateCompass?.Invoke(mag);
        }

        public void NotifyUpdateQuaternion(Quaternion quat) {
            OnUpdateQuaternion?.Invoke(quat);
        }

        public void NotifyButtonPush(DeviceButton button) {
            OnButtonPush?.Invoke(button);
        }

        public void NotifyButtonRelease(DeviceButton button) {
            OnButtonRelease?.Invoke(button);
        }
    }
}
