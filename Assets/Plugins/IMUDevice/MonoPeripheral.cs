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
                }
            }
            get { return peripheral; }
        }

        private void AddListenner(Peripheral p) {
            if (p == null) {
                return;
            }
            p.DeviceLostEvent += OnPeripheralLost;
            p.AccelUpdateEvent += OnAccelUpdate;
            p.GyroUpdateEvent += OnGyroUpdate;
            p.CompassUpdateEvent += OnCompassUpdate;
            p.QuaternionUpdateEvent += OnQuaternionUpdate;
            p.ButtonPushEvent += b => OnButtonPush(b.button);
            p.ButtonReleaseEvent += b => OnButtonRelease(b.button, b.pressTime);
        }

        private void RemoveListenner(Peripheral p) {
            if (p == null) {
                return;
            }
            p.DeviceLostEvent -= OnPeripheralLost;
            p.AccelUpdateEvent -= OnAccelUpdate;
            p.GyroUpdateEvent -= OnGyroUpdate;
            p.CompassUpdateEvent -= OnCompassUpdate;
            p.QuaternionUpdateEvent -= OnQuaternionUpdate;
            p.ButtonPushEvent -= b => OnButtonPush(b.button);
            p.ButtonReleaseEvent -= b => OnButtonRelease(b.button, b.pressTime);
        }

        protected virtual void OnPeripheralLost() { }
        protected virtual void OnAccelUpdate(Vector3 acc) { }
        protected virtual void OnGyroUpdate(Vector3 gyro) { }
        protected virtual void OnCompassUpdate(Vector3 mag) { }
        protected virtual void OnQuaternionUpdate(Quaternion quat) { }
        protected virtual void OnButtonPush(string button) { }
        protected virtual void OnButtonRelease(string button, float sec) { }
    }
}
