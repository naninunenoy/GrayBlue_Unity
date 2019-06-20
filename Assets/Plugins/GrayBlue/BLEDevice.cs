using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GrayBlue {
    public class BLEDevice : IBLEDevice {
        public event Action DeviceLostEvent;
        public string ID { private set; get; }
        public bool IsConnected { private set; get; }

        public BLEDevice(string deviceId) {
            ID = deviceId;
            IsConnected = false;
        }

        public async Task<IBLEDevice> ConnectAsync() {
            IsConnected = await Central.Instance.ConnectAsync(ID, this).ConfigureAwait(false);
            return IsConnected ? this : null;
        }

        public void Disconnect() {
            Central.Instance.Disconnect(ID);
            IsConnected = false;
        }

        public void NotifyConnectionLost() {
            DeviceLostEvent?.Invoke();
        }
    }
}
