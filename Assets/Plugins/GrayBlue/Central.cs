using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

using GrayBlueUWPCore;

namespace GrayBlue {
    [DefaultExecutionOrder(-1)]
    public class Central : MonoBehaviour, IConnectionDelegate, INotifyDelegate, IDisposable {
        private readonly IPlugin blePlugin = default;
        private readonly IDictionary<string, IBLEDevice> bleLostDict;
        private readonly IDictionary<string, IIMUEventDelegate> sensorEventDict;
        private readonly IDictionary<string, IButtonEventDelegate> buttonEventDict;
        private SynchronizationContext context = default;
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
            context = SynchronizationContext.Current;
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

        void IConnectionDelegate.OnConnectDone(string deviceId) {
            // ignore
        }

        void IConnectionDelegate.OnConnectFail(string deviceId) {
            // ignore
        }

        void IConnectionDelegate.OnConnectLost(string deviceId) {
            if (bleLostDict.ContainsKey(deviceId)) {
                bleLostDict[deviceId].NotifyConnectionLost();
                RemoveListenner(deviceId);
            }
        }

        void INotifyDelegate.OnIMUDataUpdate(string deviceId, float[] acc, float[] gyro, float[] mag, float[] quat) {
            if (sensorEventDict.ContainsKey(deviceId)) {
                var accVal = new Vector3(acc[0], acc[1], acc[2]);
                var gyroVal = new Vector3(gyro[0], gyro[1], gyro[2]);
                var magVal = new Vector3(mag[0], mag[1], mag[2]);
                var quatVal = new Quaternion(quat[0], quat[1], quat[2], quat[3]);
                var raw = new SensorData { acc = accVal, gyro = gyroVal, mag = magVal, quat = quatVal };
                var imu = new IMUData { timeUtc = DateTime.UtcNow, raw = raw, unity = raw.ToUnityWorld() };
                var device = sensorEventDict[deviceId];
                context?.Post(_ => {
                    device.NotifyUpdateAccel(accVal);
                    device.NotifyUpdateGyro(gyroVal);
                    device.NotifyUpdateCompass(magVal);
                    device.NotifyUpdateQuaternion(quatVal);
                    device.NotifyUpdateIMU(imu);
                }, null);
            }
        }

        void INotifyDelegate.OnButtonPush(string deviceId, string buttonName) {
            if (buttonEventDict.ContainsKey(deviceId)) {
                var button = new DeviceButton { button = buttonName, pressTime = 0.0F };
                context?.Post(_ => {
                    buttonEventDict[deviceId].NotifyButtonPush(button);
                }, null);
            }
        }

        void INotifyDelegate.OnButtonRelease(string deviceId, string buttonName, float pressTime) {
            if (buttonEventDict.ContainsKey(deviceId)) {
                var button = new DeviceButton { button = buttonName, pressTime = pressTime };
                context?.Post(_ => {
                    buttonEventDict[deviceId].NotifyButtonRelease(button);
                }, null);
            }
        }
    }

    static class SensorToUnityExtension {
        public static SensorData ToUnityWorld(this SensorData data) {
            // x->z, y->y, z->x
            return new SensorData {
                acc = new Vector3(data.acc.z, data.acc.y, data.acc.x),
                gyro = new Vector3(data.gyro.z, data.gyro.y, data.gyro.x),
                mag = new Vector3(-data.mag.z, data.mag.x, data.mag.y),
                quat = new Quaternion(-data.quat.z, data.quat.y, -data.quat.x, data.quat.w).normalized,
            };
        }
    }
}
