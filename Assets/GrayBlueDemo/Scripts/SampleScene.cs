using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GrayBlueDemo {
    public class SampleScene : MonoBehaviour {
        [SerializeField] GameObject m5stackPrefab = default;
        [SerializeField] Text printText = default;

        async void Start() {
            printText.text = "Any Message Here. \n";
            string[] deviceIds = default;
            try {
                deviceIds = await BleScanAsync();
            } catch (System.Exception e) {
                printText.text = e.Message;
                return;
            }
            printText.text = string.Join(",\n", deviceIds);
            await CreateCube(deviceIds[0]);
        }

        async Task<string[]> BleScanAsync() {
            var result = await GrayBlue.Central.Instance.ScanAsync();
            if (result == null) {
                throw new System.Exception("scan failed.");
            }
            if (result.Length == 0) {
                throw new System.Exception("no device found.");
            }
            return result;
        }

        async Task<GameObject> CreateCube(string id) {
            var peripheral = await ConnectAsync(id);
            if (peripheral == null) {
                return null;
            }
            var cube = Instantiate(m5stackPrefab);
            cube.name = id;
            var script = cube.AddComponent<MonoPeripheralExample>();
            script.Peripheral = peripheral;
            script.Peripheral.ListenEvent();
            return cube;
        }

        async Task<GrayBlue.Peripheral> ConnectAsync(string id) {
            // Ble connect
            var ble = new GrayBlue.BLEDevice(id);
            var success = await GrayBlue.Central.Instance.ConnectAsync(id, ble);
            if (!success) {
                return null;
            }
            return new GrayBlue.Peripheral(ble);
        }
    }
}
