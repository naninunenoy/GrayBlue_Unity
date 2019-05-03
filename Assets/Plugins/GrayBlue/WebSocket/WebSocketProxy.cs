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
        public readonly string URL;
        private readonly WebSocketSharp.WebSocket webSocket;
        private readonly INotifyDelegate notifyDelegate;
        private readonly IConnectionDelegate connectDelegate;
        private readonly RequestAgent requestAgent;
        private SynchronizationContext context;

        public WebSocketProxy(string host, int port,
                              IConnectionDelegate connectDelegate, INotifyDelegate notifyDelegate) {
            URL = $"ws://{host}:{port}/";
            webSocket = new WebSocketSharp.WebSocket(URL);
            this.connectDelegate = connectDelegate;
            this.notifyDelegate = notifyDelegate;
            requestAgent = new RequestAgent();
        }

        public void Open(SynchronizationContext mainThreadContext) {
            context = mainThreadContext;
            webSocket.OnOpen += OnWebSocketOpen;
            webSocket.OnMessage += OnWebSocketMessageReceive;
            webSocket.OnError += OnWebSocketError;
            webSocket.OnClose += OnWebSocketClose;
            try {
                webSocket.Connect();
            } catch (Exception e) {
                throw e;
            }
        }

        public void Close() {
            if (webSocket.IsAlive) {
                var json = requestAgent.CreateDisconnectAllJson();
                webSocket.Send(json);
            }
            webSocket.OnOpen -= OnWebSocketOpen;
            webSocket.OnMessage -= OnWebSocketMessageReceive;
            webSocket.OnError -= OnWebSocketError;
            webSocket.OnClose -= OnWebSocketClose;
            webSocket.Close();
        }

        public void Dispose() {
            Close();
            requestAgent?.Dispose();
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
            switch (rootJsonData.Type) {
            case JsonData.JsonType.Result:
                requestAgent?.ExtractResultJson(rootJsonData.Content);
                break;
            case JsonData.JsonType.DeviceStateChange:
                var device = JsonUtility.FromJson<JsonData.Device>(rootJsonData.Content);
                if (device.State == "Lost") {
                    context?.Post(_ => {
                        connectDelegate?.OnConnectLost(device.DeviceId);
                    }, null);
                }
                break;
            case JsonData.JsonType.NotifyIMU:
                var imuData = JsonDataExtractor.ToIMUNotifyData(rootJsonData.Content);
                context?.Post(_ => {
                    notifyDelegate?.OnIMUDataUpdate(imuData.deviceId, imuData.acc, imuData.gyro, imuData.mag, imuData.quat);
                }, null);
                break;
            case JsonData.JsonType.NotifyButton:
                var btnData = JsonDataExtractor.ToButtonNotifyData(rootJsonData.Content);
                context?.Post(_ => {
                    if (btnData.isPress) {
                        notifyDelegate?.OnButtonPush(btnData.deviceId, btnData.button);
                    } else {
                        notifyDelegate?.OnButtonRelease(btnData.deviceId, btnData.button, btnData.time);
                    }
                }, null);
                break;
            default:
                // Do Nothing
                break;
            }
        }

        private void OnWebSocketError(object sender, ErrorEventArgs e) {
            Debug.LogWarning($"OnWebSocketError {e.Message}");
        }

        private void OnWebSocketClose(object sender, CloseEventArgs e) {
            Debug.LogWarning($"OnWebSocketClose {e.Code} {e.Reason}");
        }
    }
}

