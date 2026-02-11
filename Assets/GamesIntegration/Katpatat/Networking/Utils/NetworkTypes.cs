using System;
using Newtonsoft.Json.Linq;

namespace Katpatat.Networking.Utils
{
    [Serializable]
    public class BaseWebsocketMessage {
        public string packet;
    }
    
    [Serializable]
    public class AuthMessage : BaseWebsocketMessage {
        public string channelId;
        public string[] moduleIds;
        public string apiKey;
    }
    
    [Serializable]
    public class NormalMessage : BaseWebsocketMessage {
        public string header;
        public string moduleId;
        public JArray args;
    }
    
    [Serializable]
    public class SystemMessage : BaseWebsocketMessage {
        public string type;
        public string id;
    }
    
    public enum DisconnectReason
    {
        Closed,
        Unavailable,
        Inactive
    }

    [Serializable]
    public class PlayerPosition {
        public string clientId;
        public float progress;
        public float lateral;
        public float distance;
        public string nickname;
    }
}