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
        var unityQuat = (new Quaternion(-imu.quat.z, imu.quat.y, -imu.quat.x, imu.quat.w)).normalized;
        if (baseRotarion == Quaternion.identity) {
            baseRotarion = unityQuat;
        }
        transform.rotation = Quaternion.Inverse(baseRotarion) * unityQuat;
    }

    bool isBig = false;
    protected override void OnButtonPush(string button) {
        transform.localScale = isBig ? Vector3.one : Vector3.one * 1.5F;
        isBig = !isBig;
    }
}
