using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;
using IMUObserverCore;

namespace IMUDevice {
    public class Central : MonoBehaviour, IConnectionDelegate, INotifyDelegate, IDisposable {
        private readonly IPlugin blePlugin = default;
        private readonly IDictionary<string, TaskCompletionSource<bool>> bleConnectionDict;
        private readonly IDictionary<string, IBLEDevice> bleLostDict;
        private readonly IDictionary<string, IIMUEventDelegate> sensorEventDict;
        private readonly IDictionary<string, IButtonEventDelegate> buttonEventDict;
        private Central() {
            blePlugin = Plugin.Instance;
            bleConnectionDict = new Dictionary<string, TaskCompletionSource<bool>>();
            bleLostDict = new Dictionary<string, IBLEDevice>();
            sensorEventDict = new Dictionary<string, IIMUEventDelegate>();
            buttonEventDict = new Dictionary<string, IButtonEventDelegate>();
        }
        private static Central instance;
        public static Central Instance {
            get {
                if (!instance) {
                    var go = new GameObject("BLECentral");
                    DontDestroyOnLoad(go);
                    instance = go.AddComponent<Central>();
                }
                return instance;
            }
        }

        void Awake() {
            if (instance == null) {
                instance = this;
            } else {
                Destroy(gameObject);
            }
        }

        public void Dispose() {
            bleLostDict.Clear();
            sensorEventDict.Clear();
            buttonEventDict.Clear();
            blePlugin.Dispose();
        }

        public void OnConnectDone(string deviceId) {
            if (bleConnectionDict.ContainsKey(deviceId)) {
                bleConnectionDict[deviceId].SetResult(true);
            }
        }

        public void OnConnectFail(string deviceId) {
            if (bleConnectionDict.ContainsKey(deviceId)) {
                bleConnectionDict[deviceId].SetResult(false);
            }
        }

        public void OnConnectLost(string deviceId) {
            if (bleLostDict.ContainsKey(deviceId)) {
                bleLostDict[deviceId].NotifyConnectionLost();
            }
        }

        public void OnIMUDataUpdate(string deviceId, float[] acc, float[] gyro, float[] mag, float[] quat) {
            if (sensorEventDict.ContainsKey(deviceId)) {
                var accVal = new Vector3(acc[0], acc[1], acc[2]);
                var gyroVal = new Vector3(gyro[0], gyro[1], gyro[2]);
                var magVal = new Vector3(mag[0], mag[1], mag[2]);
                var quatVal = new Quaternion(quat[0], quat[1], quat[2], quat[3]);
                var devices = sensorEventDict[deviceId];
                devices.NotifyUpdateAccel(accVal);
                devices.NotifyUpdateGyro(gyroVal);
                devices.NotifyUpdateCompass(magVal);
                devices.NotifyUpdateQuaternion(quatVal);
            }
        }

        public void OnButtonPush(string deviceId, string buttonName) {
            if (buttonEventDict.ContainsKey(deviceId)) {
                var button = new DeviceButton { button = buttonName, pressTime = 0.0F };
                buttonEventDict[deviceId].NotifyButtonPush(button);
            }
        }

        public void OnButtonRelease(string deviceId, string buttonName, float pressTime) {
            if (buttonEventDict.ContainsKey(deviceId)) {
                var button = new DeviceButton { button = buttonName, pressTime = pressTime };
                buttonEventDict[deviceId].NotifyButtonPush(button);
            }
        }
    }
}
