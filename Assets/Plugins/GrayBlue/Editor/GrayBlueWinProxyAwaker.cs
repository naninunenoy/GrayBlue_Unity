using System;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace GrayBlue.Editor {

    [InitializeOnLoadAttribute]
    public class GrayBlueWinProxyAwaker {
        const string exePath = "ExternalTools/GrayBlue_WinProxy/GrayBlue_WinProxy.exe";
        static Process grayBlueExe = default;
        static GrayBlueWinProxyAwaker() {
            EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
        }

        private static void EditorApplication_playModeStateChanged(PlayModeStateChange state) {
            if (state == PlayModeStateChange.EnteredPlayMode) {
                // GrayBlue_WinProxyを起動
                try {
                    var projectRootPath = System.IO.Directory.GetParent(Application.dataPath).FullName;
                    grayBlueExe = new Process();
                    grayBlueExe.StartInfo.FileName = $"{projectRootPath}/{exePath}";
                    UnityEngine.Debug.Log($"{grayBlueExe.StartInfo.FileName}");
                    //grayBlueExe.StartInfo.Arguments = "/c" + path;
                    grayBlueExe.Start();
                } catch (Exception e) {
                    UnityEngine.Debug.Log(e.Message);
                }
            } else if (state == PlayModeStateChange.ExitingPlayMode) {
                try {
                    grayBlueExe?.CloseMainWindow();
                } catch (Exception e) {
                    UnityEngine.Debug.Log(e.Message);
                }
            }
        }
    }
}
