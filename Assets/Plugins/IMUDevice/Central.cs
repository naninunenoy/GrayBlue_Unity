using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using IMUObserverCore;

namespace IMUDevice {
    public class Central : MonoBehaviour, IConnectionDelegate, INotifyDelegate, IDisposable {
        private readonly IPlugin blePlugin = default;
        private readonly IDictionary<string, IBLEDevice> bleLostDict;
        private readonly IDictionary<string, IIMUEventDelegate> sensorEventDict;
        private readonly IDictionary<string, IButtonEventDelegate> buttonEventDict;
        private Central() {
            blePlugin = Plugin.Instance;
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

        public async Task<bool> CheckBluetoothAsync() {
            return await blePlugin.CanUseBle().ConfigureAwait(false);
        }

        public async Task<string[]> ScanAsync() {
            return await blePlugin.Scan().ConfigureAwait(false);
        }

        public async Task<bool> ConnectAsync(string id, IBLEDevice device) {
            var success = await blePlugin.ConnectTo(id, this, this).ConfigureAwait(false);
            if (success && !bleLostDict.ContainsKey(id)) {
                bleLostDict.Add(id, device);
            }
            return success;
        }

        public void Disconnect(string id) {
            blePlugin.DisconnectTo(id);
            RemoveListenner(id);
        }

        public void AddListenner(string id, IIMUEventDelegate imu, IButtonEventDelegate button) {
            if (string.IsNullOrEmpty(id)) {
                return;
            }
            if (!sensorEventDict.ContainsKey(id) && imu != null) {
                sensorEventDict.Add(id, imu);
            }
            if (!buttonEventDict.ContainsKey(id) && button != null) {
                buttonEventDict.Add(id, button);
            }
        }

        public void RemoveListenner(string id) {
            if (bleLostDict.ContainsKey(id)) {
                bleLostDict.Remove(id);
            }
            if (sensorEventDict.ContainsKey(id)) {
                sensorEventDict.Remove(id);
            }
            if (buttonEventDict.ContainsKey(id)) {
                buttonEventDict.Remove(id);
            }
        }

        public void OnConnectDone(string deviceId) {
            // ignore
        }

        public void OnConnectFail(string deviceId) {
            // ignore
        }

        public void OnConnectLost(string deviceId) {
            if (bleLostDict.ContainsKey(deviceId)) {
                bleLostDict[deviceId].NotifyConnectionLost();
                RemoveListenner(deviceId);
            }
        }

        public void OnIMUDataUpdate(string deviceId, float[] acc, float[] gyro, float[] mag, float[] quat) {
            if (sensorEventDict.ContainsKey(deviceId)) {
                var accVal = new Vector3(acc[0], acc[1], acc[2]);
                var gyroVal = new Vector3(gyro[0], gyro[1], gyro[2]);
                var magVal = new Vector3(mag[0], mag[1], mag[2]);
                var quatVal = new Quaternion(quat[0], quat[1], quat[2], quat[3]);
                var imu = new IMUData { acc = accVal, gyro = gyroVal, mag = magVal, quat = quatVal, timeUtc = DateTime.UtcNow };
                var device = sensorEventDict[deviceId];
                device.NotifyUpdateAccel(accVal);
                device.NotifyUpdateGyro(gyroVal);
                device.NotifyUpdateCompass(magVal);
                device.NotifyUpdateQuaternion(quatVal);
                device.NotifyUpdateIMU(imu);
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
