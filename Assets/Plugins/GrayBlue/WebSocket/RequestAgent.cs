using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace GrayBlue.WebSocket {
    public class RequestAgent {
        private TaskCompletionSource<bool> bleTsc;
        private TaskCompletionSource<string[]> scanTsc;
        private IDictionary<string, TaskCompletionSource<bool>> connectTscDict;

        public RequestAgent() {
            connectTscDict = new Dictionary<string, TaskCompletionSource<bool>>();
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
                return new string[] { "" }; // already scan processing
            }
            scanTsc = new TaskCompletionSource<string[]>();
            return await scanTsc.Task;
        }

        public async Task<bool> WaitConnectResultAsync(string id) {
            if (connectTscDict.ContainsKey(id)) {
                return false;// already connect processing
            }
            var tcs = new TaskCompletionSource<bool>();
            connectTscDict.Add(id, tcs);
            var result = await tcs.Task;
            connectTscDict.Remove(id);
            return result;
        }

        public void ExtractResultJson(string contentJson) {

        }

        public string CreateCheckBleJson() {
            var ret = "{}";
            return ret;
        }

        public string CreateScanJson() {
            var ret = "{}";
            return ret;
        }

        public string CreateConnectJson(string id) {
            var ret = "{}";
            return ret;
        }

        public string CreateDisconnectJson(string id) {
            var ret = "{}";
            return ret;
        }

        public string CreateDisconnectAllJson() {
            var ret = "{}";
            return ret;
        }
    }
}
