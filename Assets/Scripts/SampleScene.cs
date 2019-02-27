using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SampleScene : MonoBehaviour {
    [SerializeField] Text printText = default;

    async void Start() {
        printText.text = "Any Message Here. \n";
        printText.text += await ScanResult();
    }

    async Task<string> ScanResult() {
        var result = await IMUObserverCore.Plugin.Instance.Scan();
        if (result == null) {
            return "<NULL>";
        }
        if (result.Length == 0) {
            return "<EMPTY>";
        }
        return string.Join(",\n", result);
    }
}
