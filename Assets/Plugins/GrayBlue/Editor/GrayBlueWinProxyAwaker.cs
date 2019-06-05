using System;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace GrayBlue.Editor {

    [InitializeOnLoadAttribute]
    public class GrayBlueWinProxyAwaker {
        const string exePath = "Plugins/GrayBlue/Editor/GrayBlue_WinProxy.exe";
        static Process grayBlueExe = default;
        static GrayBlueWinProxyAwaker() {
            EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
        }

        private static void EditorApplication_playModeStateChanged(PlayModeStateChange state) {
            if (state == PlayModeStateChange.EnteredPlayMode) {
                // GrayBlue_WinProxyを起動
                try {
                    grayBlueExe = new Process();
                    grayBlueExe.StartInfo.FileName = $"{Application.dataPath}/{exePath}";
                    //grayBlueExe.StartInfo.Arguments = "/c" + path;
                    grayBlueExe.Start();
                } catch (Exception e) {
                    UnityEngine.Debug.Log(e.Message);
                }
            } else if (state == PlayModeStateChange.ExitingPlayMode) {
                grayBlueExe?.Close();
            }
        }
    }
}
