using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrayBlue.WebSocket.JsonData {
    public enum JsonType {
        Undefine = 0,
        Method,
        Result,
        DeviceStateChange,
        NotifyIMU,
        NotifyButton
    }

    public enum MethodType {
        Undefine = 0,
        CheckBle,
        Scan,
        Connect,
        Disconnect,
        DisconnectAll
    }

    [Serializable]
    public class GrayBlueJson {
        [SerializeField] string type;
        [SerializeField] string content;
        public JsonType Type {
            set { type = value.ToString(); }
            get => Enum.IsDefined(typeof(JsonType), type) ? (JsonType)Enum.Parse(typeof(JsonType), type) : JsonType.Undefine;
        }
        public string Content { set { content = value; } get => content; }
    }

    [Serializable]
    public class Method {
        [SerializeField] string method_name;
        [SerializeField] string method_param;
        public MethodType Name {
            set { method_name = value.ToString(); }
            get => Enum.IsDefined(typeof(MethodType), method_name) ? (MethodType)Enum.Parse(typeof(MethodType), method_name) : MethodType.Undefine;
        }
        public string Param { set { method_param = value; } get => method_param; }
    }

    [Serializable]
    public class MethodResult {
        [SerializeField] Method method;
        [SerializeField] string result;
        public Method Method { set { method = value; } get => method; }
        public string Result { set { result = value; } get => result; }
    }

    [Serializable]
    public class Device {
        [SerializeField] string device_id;
        [SerializeField] string device_state;
        public string DeviceId { set { device_id = value; } get => device_id; }
        public string State { set { device_state = value; } get => device_state; }
    }

    [Serializable]
    public class IMU {
        [SerializeField] string device_id;
        [SerializeField] Vector3 acc;
        [SerializeField] Vector3 gyro;
        [SerializeField] Vector3 mag;
        [SerializeField] Quaternion quat;
        public string DeviceId { set { device_id = value; } get => device_id; }
        public Vector3 Acc { set { acc = value; } get => acc; }
        public Vector3 Gyro { set { gyro = value; } get => gyro; }
        public Vector3 Mag { set { mag = value; } get => mag; }
        public Quaternion Quat { set { quat = value; } get => quat; }
    }

    [Serializable]
    public class Button {
        [SerializeField] string device_id;
        [SerializeField] string button;
        [SerializeField] bool press;
        [SerializeField] float time;
        public string DeviceId { set { device_id = value; } get => device_id; }
        public string ButtonName { set { button = value; } get => button; }
        public bool IsPressed { set { press = value; } get => press; }
        public float PressTime { set { time = value; } get => time; }
    }
}
