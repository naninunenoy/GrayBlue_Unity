using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace IMUDevice {
    public class BLEDevice : IBLEDevice {
        public event Action OnLostDevice;
        public string ID { private set; get; }

        public BLEDevice(string deviceId) {
            ID = deviceId;
        }

        public async Task<IBLEDevice> ConnectAsync() {
            var success = await Central.Instance.ConnectAsync(ID, this).ConfigureAwait(false);
            return success ? this : null;
        }

        public void Disconnect() {
            Central.Instance.Disconnect(ID);
        }

        public void NotifyConnectionLost() {
            OnLostDevice?.Invoke();
        }
    }
}
