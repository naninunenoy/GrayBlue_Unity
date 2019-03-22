using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IMUDevice;

public class MonoPeripheralExample : MonoPeripheralBase {

    void OnDisable() {
        Peripheral?.UnlistenEvent();
    }

    void OnDestroy() {
        Peripheral?.Dispose();
    }

    protected override void OnPeripheralLost() {
        Destroy(gameObject);
    }

    protected override void OnIMUSensorUpdate(IMUData imu) {
        transform.rotation = imu.quat;
    }

    bool isBig = false;
    protected override void OnButtonPush(string button) {
        transform.localScale = isBig ? Vector3.one : Vector3.one * 1.5F;
        isBig = !isBig;
    }
}
