using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IMUDevice;

public class MonoPeripheralExample : MonoPeripheralBase {
    Quaternion baseRotarion = Quaternion.identity;

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
        var q = imu.unity.quat;
        if (baseRotarion == Quaternion.identity) {
            baseRotarion = q;
        }
        transform.rotation = Quaternion.Inverse(baseRotarion) * q;
    }

    protected override void OnButtonPush(string button) {
        // base pose reset
        baseRotarion = Quaternion.identity;
    }
}
