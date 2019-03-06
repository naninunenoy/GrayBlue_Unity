using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IMUDevice {
    public abstract class MonoPeripheral : MonoBehaviour {
        Peripheral peripheral = default;
        public Peripheral Peripheral {
            set {
                if (peripheral != value) {
                    RemoveListenner(peripheral);
                    AddListenner(value);
                    peripheral = value;
                    PeripheralId = peripheral?.ID.Replace("BluetoothLE#BluetoothLE", "") ?? string.Empty;
                }
            }
            get { return peripheral; }
        }

        public string PeripheralId { private set; get; } = string.Empty;
        public string RawPeripheralId { get => peripheral?.ID ?? string.Empty; }
        public bool HasPeripheral { get => !string.IsNullOrEmpty(RawPeripheralId); }

        void AddListenner(Peripheral p) {
            if (p == null) {
                return;
            }
            p.DeviceLostEvent += OnPeripheralLost;
            p.AccelUpdateEvent += OnAccelUpdate;
            p.GyroUpdateEvent += OnGyroUpdate;
            p.CompassUpdateEvent += OnCompassUpdate;
            p.QuaternionUpdateEvent += OnQuaternionUpdate;
            p.IMUSensorUpdateEvent += OnIMUSensorUpdate;
            p.ButtonPushEvent += InvokeButtonPushEvent;
            p.ButtonReleaseEvent += InvokeButtonReleaseEvent;
        }

        void RemoveListenner(Peripheral p) {
            if (p == null) {
                return;
            }
            p.DeviceLostEvent -= OnPeripheralLost;
            p.AccelUpdateEvent -= OnAccelUpdate;
            p.GyroUpdateEvent -= OnGyroUpdate;
            p.CompassUpdateEvent -= OnCompassUpdate;
            p.QuaternionUpdateEvent -= OnQuaternionUpdate;
            p.IMUSensorUpdateEvent -= OnIMUSensorUpdate;
            p.ButtonPushEvent -= InvokeButtonPushEvent;
            p.ButtonReleaseEvent -= InvokeButtonReleaseEvent;
        }

        void InvokeButtonPushEvent(DeviceButton b) {
            OnButtonPush(b.button);
        }

        void InvokeButtonReleaseEvent(DeviceButton b) {
            OnButtonRelease(b.button, b.pressTime);
        }

        protected virtual void OnPeripheralLost() { }
        protected virtual void OnAccelUpdate(Vector3 acc) { }
        protected virtual void OnGyroUpdate(Vector3 gyro) { }
        protected virtual void OnCompassUpdate(Vector3 mag) { }
        protected virtual void OnQuaternionUpdate(Quaternion quat) { }
        protected virtual void OnIMUSensorUpdate(IMUData imu) { }
        protected virtual void OnButtonPush(string button) { }
        protected virtual void OnButtonRelease(string button, float sec) { }
    }
}
