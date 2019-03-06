using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using IMUObserverCore;

namespace IMUDevice {
    public class BLEDevice : IBLEDevice {
        public event Action OnLostDevice;
        public string ID { private set; get; }

        public BLEDevice(string deviceId) {
            ID = deviceId;
        }

        public async Task<IBLEDevice> ConnectAsync() {
            throw new NotImplementedException();
        }

        public void Disconnect() {
            throw new NotImplementedException();
        }

        public void NotifyConnectionLost() {
            OnLostDevice?.Invoke();
        }
    }
}
