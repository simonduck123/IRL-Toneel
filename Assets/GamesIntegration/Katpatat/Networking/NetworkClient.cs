using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Katpatat.Networking.Utils;
using NativeWebSocket;
using Newtonsoft.Json;
using UnityEngine;

namespace Katpatat.Networking
{
    public class NetworkClient : MonoBehaviour
    {
        public static NetworkClient Instance;

        private static Config config;

        public static event Action<string> OnHandleClientMessage;
        
        [SerializeField] private int maxReconnectAttempts = 5;
        private const int RECONNECT_INTERVAL = 5000;
        private int _reconnectAttempts;

        private static WebSocket webSocket;
        
        private Queue<Tuple<string, bool>> _clientConnectQueue;
        private Queue<Tuple<string, DisconnectReason>> _clientDisconnectQueue;
        public int messagesPerFrame = 200;
        private Queue<string> _messageQueue;

        private void OnEnable() {
            OnHandleClientMessage += HandleClientMessage;
        }
        
        private void OnDisable() {
            OnHandleClientMessage -= HandleClientMessage;
        }

        private void Awake()
        {
            if (Instance == null) 
            {
                Instance = this;
                LoadConfig();
            }
            else {
                Debug.LogError($"NetworkClient instance already exists removing new one");
                Destroy(gameObject);
            }
            
            DontDestroyOnLoad(this);
        }

        private void LoadConfig()
        {
            string pathToConfigFile = Path.Combine(
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),
                "MorphixProductions",
                "Unity",
                "config.json"
            );

            if (!File.Exists(pathToConfigFile))
            {
                Debug.LogWarning("Config file not found: " + pathToConfigFile);
                return;
            }

            string allText = File.ReadAllText(pathToConfigFile);
            config = JsonUtility.FromJson<Config>(allText);    
        }

        private async void Start()
        {
            if(config==null)
                return;

            _clientConnectQueue = new Queue<Tuple<string, bool>>();
            _clientDisconnectQueue = new Queue<Tuple<string, DisconnectReason>>();
            _messageQueue = new Queue<string>();
            
            webSocket = new WebSocket(config.server.useLocalServer ? config.server.localServerAddress : config.server.serverAddress);
            webSocket.OnOpen += OnOpen;
            webSocket.OnClose += OnClose;
            webSocket.OnMessage += OnMessage;
            webSocket.OnError += OnError;

            await webSocket.Connect();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                Application.Quit();
            
            if(config==null)
                return;

            // #if !UNITY_WEBGL || UNITY_EDITOR
            // webSocket.DispatchMessageQueue();
            // #endif

            int currentMessage = 0;
            while(currentMessage++<=messagesPerFrame)
            {
                if (_messageQueue.Count == 0)
                    break;
                
                var message = _messageQueue.Dequeue();
                if (message == null)
                    return;
                
                OnHandleClientMessage?.Invoke(message);
            }
        }

        private void OnOpen()
        {
            _reconnectAttempts = 0;
            
            var authMessage = JsonUtility.ToJson(NetworkMessageUtil.GetAuthMessage(JsonUtility.ToJson(config.auth)));

            SendWebSocketMessage(authMessage);
        }

        private void OnClose(WebSocketCloseCode closeCode)
        {
            Debug.Log($"Disconnected from server: {closeCode}");

            if (closeCode == WebSocketCloseCode.Abnormal) 
                TryReconnect();
        }

        private void OnMessage(string data) {
            Debug.Log($"Message received: {data}");
            
            try
            {
                var jsonMessage = JsonUtility.FromJson<BaseWebsocketMessage>(data);

                switch (jsonMessage.packet) 
                {
                    case "AUTH":
                        Debug.Log($"Auth message received: {data}");
                        break;
                    case "SYSTEM":
                        var systemMessage = JsonUtility.FromJson<SystemMessage>(data);
                        switch (systemMessage?.type) 
                        {
                            case "authOk":
                                Debug.Log($"Authentication was OK!");
                                break;
                            case "id":
                                Debug.Log($"Id received: {systemMessage.id}");
                                break;
                            default:
                                Debug.LogWarning($"Unhandled system message received: {data}");
                                break;
                        }
                        break;
                    case "NORMAL":
                        // Make the message handled by queue else it is not on the right thread
                        _messageQueue.Enqueue(data);
                        break;
                    case "ERROR":
                        Debug.LogError($"Error message received: {data}");
                        break;
                    default:
                        Debug.LogError($"Unknown packet: {jsonMessage.packet}");
                        break;
                }
            }
            catch (Exception exception)
            {
                Debug.Log($"Message unhandled: {exception.Message}");
            }
        }

        private void HandleClientMessage(string message)
        {
            NetworkMessageUtil.HandleMessage(JsonConvert.DeserializeObject<NormalMessage>(message));
        }
  
        public static async void SendWebSocketMessage(string message)
        {
            Debug.Log($"Sending websocket message: {message} | Websocket state: {webSocket.State}");
            
            if (webSocket.State == WebSocketState.Open) 
                await webSocket.SendText(message);
        }

        private async void TryReconnect()
        {
            if (!Application.isPlaying) return;

            if (_reconnectAttempts != maxReconnectAttempts)
            {
                _reconnectAttempts++;

                await Task.Delay(RECONNECT_INTERVAL);
                await webSocket.Connect();
            }
            else
            {
               Debug.LogError($"Unable to reconnect after {maxReconnectAttempts} attempts at an {RECONNECT_INTERVAL / 1000}s interval.");
            }
        }

        private void OnError(string errorMsg)
        {
           Debug.LogError($"Error: {errorMsg}");
        }

        private async void OnApplicationQuit()
        {
            try 
            {
                await webSocket.Close();
            }
            catch (Exception e) 
            {
                Debug.LogError($"There was an error when closing the websocket connection. Message: {e.Message}");
            }
        }
    }
}

[Serializable]
public class Config
{
    public AuthConfig auth;
    public ServerConfig server;
}

[Serializable]
public class AuthConfig
{
    public string packet;
    public string channelId;
    public string[] moduleIds;
    public string apiKey;
}

[Serializable]
public class ServerConfig
{
    public string serverAddress;
    public string localServerAddress;
    public bool useLocalServer;
    public string leftScreenUrl;
    public string rightScreenUrl;
}