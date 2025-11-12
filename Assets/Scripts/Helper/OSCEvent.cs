using UnityEngine.Events;
using extOSC;

[System.Serializable] public class OSCMessageEvent : UnityEvent<OSCMessage> { }
[System.Serializable] public class FloatEvent : UnityEvent<float> { }
[System.Serializable] public class IntEvent : UnityEvent<int> { }
[System.Serializable] public class StringEvent : UnityEvent<string> { }
