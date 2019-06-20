using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using GrayBlue.WebSocket.JsonData;

namespace GrayBlue.WebSocket {
    public class RequestAgent : IDisposable {
        private readonly IDictionary<string, TaskCompletionSource<bool>> connectTscDict;
        private TaskCompletionSource<bool> bleTsc;
        private TaskCompletionSource<string[]> scanTsc;

        public RequestAgent() {
            connectTscDict = new Dictionary<string, TaskCompletionSource<bool>>();
        }

        public void Dispose() {
            bleTsc.TrySetCanceled();
            scanTsc.TrySetCanceled();
            foreach (var tsc in connectTscDict) {
                tsc.Value.TrySetCanceled();
            }
            connectTscDict.Clear();
        }

        public async Task<bool> WaitCheckBluetoothResultAsync() {
            if (bleTsc != null) {
                return false; // already check processing
            }
            bleTsc = new TaskCompletionSource<bool>();
            return await bleTsc.Task;
        }

        public async Task<string[]> WaitScanResultAsync() {
            if (scanTsc != null) {
                return new string[0]; // already scan processing
            }
            scanTsc = new TaskCompletionSource<string[]>();
            return await scanTsc.Task;
        }

        public async Task<bool> WaitConnectResultAsync(string id) {
            if (connectTscDict.ContainsKey(id)) {
                return false; // already connect processing
            }
            var tcs = new TaskCompletionSource<bool>();
            connectTscDict.Add(id, tcs);
            var result = await tcs.Task;
            connectTscDict.Remove(id);
            return result;
        }

        public void ExtractResultJson(MethodResult result) {
            switch (result.Method.Name) {
            case MethodType.CheckBle:
                if (bool.TryParse(result.Result, out bool bleOK)) {
                    bleTsc?.SetResult(bleOK);
                } else {
                    bleTsc?.SetResult(false);
                }
                break;
            case MethodType.Scan:
                if (string.IsNullOrEmpty(result.Result)) {
                    scanTsc?.SetResult(new string[0]);
                } else {
                    scanTsc?.SetResult(result.Result.Split(','));
                }
                break;
            case MethodType.Connect:
                var id = result.Method.Param;
                if (connectTscDict.ContainsKey(id)) {
                    if (bool.TryParse(result.Result, out bool success)) {
                        connectTscDict[id]?.SetResult(success);
                    } else {
                        connectTscDict[id]?.SetResult(false);
                    }
                }
                break;
            default:
                // Do Nothing
                break;
            }
        }

        public string CreateCheckBleJson() {
            var method = new Method { Name = MethodType.CheckBle, Param = "" };
            var json = new GrayBlueJson<Method> { Type = JsonType.Method, Content = method };
            return JsonUtility.ToJson(json);
        }

        public string CreateScanJson() {
            var method = new Method { Name = MethodType.Scan, Param = "" };
            var json = new GrayBlueJson<Method> { Type = JsonType.Method, Content = method };
            return JsonUtility.ToJson(json);
        }

        public string CreateConnectJson(string id) {
            var method = new Method { Name = MethodType.Connect, Param = id };
            var json = new GrayBlueJson<Method> { Type = JsonType.Method, Content = method };
            return JsonUtility.ToJson(json);
        }

        public string CreateDisconnectJson(string id) {
            var method = new Method { Name = MethodType.Disconnect, Param = id };
            var json = new GrayBlueJson<Method> { Type = JsonType.Method, Content = method };
            return JsonUtility.ToJson(json);
        }

        public string CreateDisconnectAllJson() {
            var method = new Method { Name = MethodType.DisconnectAll, Param = "" };
            var json = new GrayBlueJson<Method> { Type = JsonType.Method, Content = method };
            return JsonUtility.ToJson(json);
        }
    }
}
