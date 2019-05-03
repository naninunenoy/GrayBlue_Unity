using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GrayBlue.WebSocket.JsonData;

namespace GrayBlue.WebSocket {

    public static class JsonDataExtractor {
        public struct IMUNotifyData {
            public string deviceId;
            public float[] acc;
            public float[] gyro;
            public float[] mag;
            public float[] quat;
            public static IMUNotifyData Empty => new IMUNotifyData {
                deviceId = string.Empty,
                acc = new float[] { 0.0F, 0.0F, 0.0F },
                gyro = new float[] { 0.0F, 0.0F, 0.0F },
                mag = new float[] { 0.0F, 0.0F, 0.0F },
                quat = new float[] { 0.0F, 0.0F, 0.0F, 1.0F }
            };
        }

        public struct ButtonNotifyData {
            public string deviceId;
            public string button;
            public bool isPress;
            public float time;
            public static ButtonNotifyData Empty => new ButtonNotifyData {
                deviceId = string.Empty,
                button = string.Empty,
                isPress = false,
                time = 0.0F
            };
        }

        public static IMUNotifyData ToIMUNotifyData(string jsonContent) {
            var ret = IMUNotifyData.Empty;
            try {
                var data = JsonUtility.FromJson<IMU>(jsonContent);
                ret.deviceId = data.DeviceId;
                ret.acc[0] = data.Acc.x;
                ret.acc[1] = data.Acc.y;
                ret.acc[2] = data.Acc.z;
                ret.gyro[0] = data.Gyro.x;
                ret.gyro[1] = data.Gyro.y;
                ret.gyro[2] = data.Gyro.z;
                ret.mag[0] = data.Mag.x;
                ret.mag[1] = data.Mag.y;
                ret.mag[2] = data.Mag.z;
                ret.quat[0] = data.Quat.x;
                ret.quat[1] = data.Quat.y;
                ret.quat[2] = data.Quat.z;
                ret.quat[3] = data.Quat.w;
            } catch (Exception e) {
                Debug.LogWarning(e.Message);
            }
            return ret;
        }

        public static ButtonNotifyData ToButtonNotifyData(string jsonContent) {
            var ret = ButtonNotifyData.Empty;
            try {
                var data = JsonUtility.FromJson<Button>(jsonContent);
                ret.deviceId = data.DeviceId;
                ret.button = data.ButtonName;
                ret.isPress = data.IsPressed;
                ret.time = data.PressTime;
            } catch (Exception e) {
                Debug.LogWarning(e.Message);
            }
            return ret;
        }
    }
}
