using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using WebSocketSharp;
using GrayBlueUWPCore;

namespace GrayBlue.WebSocket {
    public class WebSocketProxy : IDisposable {
        private readonly WebSocketSharp.WebSocket webSocket;
        private readonly INotifyDelegate notify;
        private readonly RequestAgent requestAgent;
        private SynchronizationContext context;

        public WebSocketProxy(string host, int port,  INotifyDelegate grayBlueNotify) {
            webSocket = new WebSocketSharp.WebSocket($"ws://{host}:{port}/");
            notify = grayBlueNotify;
            requestAgent = new RequestAgent();
        }

        public void Open(SynchronizationContext mainThreadContext) {
            context = mainThreadContext;
            webSocket.OnOpen += OnWebSocketOpen;
            webSocket.OnMessage += OnWebSocketMessageReceive;
            webSocket.OnError += OnWebSocketError;
            webSocket.OnClose += OnWebSocketClose;
            webSocket.Connect();
        }

        public void Close() {
            var json = requestAgent.CreateDisconnectAllJson();
            webSocket.Send(json);
            webSocket.OnOpen -= OnWebSocketOpen;
            webSocket.OnMessage -= OnWebSocketMessageReceive;
            webSocket.OnError -= OnWebSocketError;
            webSocket.OnClose -= OnWebSocketClose;
            webSocket.Close();
        }

        public void Dispose() {
            Close();
        }

        public async Task<bool> CheckBluetoothAsync() {
            var json = requestAgent.CreateCheckBleJson();
            webSocket.Send(json);
            return await requestAgent.WaitCheckBluetoothResultAsync();
        }

        public async Task<string[]> ScanAsync() {
            var json = requestAgent.CreateScanJson();
            webSocket.Send(json);
            return await requestAgent.WaitScanResultAsync();
        }

        public async Task<bool> ConnectAsync(string id) {
            var json = requestAgent.CreateConnectJson(id);
            webSocket.Send(json);
            return await requestAgent.WaitConnectResultAsync(id);
        }

        public void Disconnect(string id) {
            var json = requestAgent.CreateDisconnectJson(id);
            webSocket.Send(json);
        }

        // websocket callback

        private void OnWebSocketOpen(object sender, EventArgs e) {
            Debug.Log($"OnWebSocketOpen");
        }

        private void OnWebSocketMessageReceive(object sender, MessageEventArgs e) {
            Debug.Log($"OnWebSocketMessageReceive {e.Data}");
            if (!e.IsText) {
                return;
            }
            var rootJsonData = JsonUtility.FromJson<JsonData.GrayBlueJson>(e.Data);
            Action<INotifyDelegate> notifyAction = delegate { };

            switch (rootJsonData.Type) {
            case JsonData.JsonType.NotifyIMU:
                notifyAction = x => {
                    var data = JsonDataExtractor.ToIMUNotifyData(rootJsonData.Content);
                    x.OnIMUDataUpdate(data.deviceId, data.acc, data.gyro, data.mag, data.quat);
                };
                break;
            case JsonData.JsonType.NotifyButton:
                notifyAction = x => {
                    var data = JsonDataExtractor.ToButtonNotifyData(rootJsonData.Content);
                    if (data.isPress) {
                        x.OnButtonPush(data.deviceId, data.button);
                    } else {
                        x.OnButtonRelease(data.deviceId, data.button, data.time);
                    }
                };
                break;
            default:
                // Do Nothing
                break;
            }
            // メインスレッドで通知を行う
            context?.Post(_ => { notifyAction(notify); }, null);
        }

        private void OnWebSocketError(object sender, ErrorEventArgs e) {
            Debug.LogWarning($"OnWebSocketError {e.Message}");
        }

        private void OnWebSocketClose(object sender, CloseEventArgs e) {
            Debug.LogWarning($"OnWebSocketClose {e.Code} {e.Reason}");
        }
    }
}

