using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using WebSocketSharp;
using GrayBlueUWPCore;

namespace GrayBlue {
    public class WebSocketProxy {
        private readonly WebSocket webSocket;
        private readonly SynchronizationContext context;
        private readonly INotifyDelegate notify;
        public WebSocketProxy(string host, int port, 
                              INotifyDelegate grayBlueNotify, 
                              SynchronizationContext mainThreadContext) {
            webSocket = new WebSocket($"ws://{host}:{port}/");
            notify = grayBlueNotify;
            context = mainThreadContext;
        }

        public void Open() {
            webSocket.OnOpen += OnWebSocketOpen;
            webSocket.OnMessage += OnWebSocketMessageReceive;
            webSocket.OnError += OnWebSocketError;
            webSocket.OnClose += OnWebSocketClose;
            webSocket.Connect();
        }

        public void Close() {
            webSocket.OnOpen -= OnWebSocketOpen;
            webSocket.OnMessage -= OnWebSocketMessageReceive;
            webSocket.OnError -= OnWebSocketError;
            webSocket.OnClose -= OnWebSocketClose;
            webSocket.Close();

        }

        // websocket callback

        private void OnWebSocketOpen(object sender, EventArgs e) {
            Debug.Log($"OnWebSocketOpen");
        }

        private void OnWebSocketMessageReceive(object sender, MessageEventArgs e) {
            Debug.Log($"OnWebSocketMessageReceive {e.Data}");
            context?.Post(_ => {
            }, null);
        }

        private void OnWebSocketError(object sender, ErrorEventArgs e) {
            Debug.LogWarning($"OnWebSocketError {e.Message}");
        }

        private void OnWebSocketClose(object sender, CloseEventArgs e) {
            Debug.LogWarning($"OnWebSocketClose {e.Code} {e.Reason}");
        }
    }
}

