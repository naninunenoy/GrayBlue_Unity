using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SampleScene : MonoBehaviour {
    [SerializeField] Text printText = default;
    void Start() {
        printText.text = "Any Message Here.";
    }

    void Update() {

    }
}
