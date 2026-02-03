using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Katpatat.Networking.Utils
{
    public static class NetworkMessageUtil {
        private static AuthMessage authMessage;
        
        // ----- SWIMMING GAME ----- //
        public static event Action<string, float, float> OnSwimLocation;    // id, x, y
        public static event Action<string> OnPlayerRemove;              // id
        public static event Action<string, string> OnSwimPlayerAction;      // id, actionName

        // ----- MOTOR GAME ----- //
        // public static event Action<string, float> OnRideLateralPosition;    // id, lateral position (0.0->1.0)
        // public static event Action<string, float> OnRideTrackProgress;      // id, progress along track (%1.0, loops)
        // public static event Action<string, float> OnRideSpeed;              // id, speed (increments progressOnTrack over time, 0.0 for rider not moving automatically)
        public static event Action<string, float, float> OnRiderPosition;   // id, progress, lateral
        public static event Action<string> OnRiderExplosion;   // id
        public static event Action<string> OnRiderJoined;   // id
        public static event Action<string> OnRiderLeft;   // id
        
        // ----- BOSS FIGHT GAME ----- //
        public static event Action<string, int, int> OnThrowObject;   // id, time between press and release (ms), index object type

        public static void HandleMessage(NormalMessage message)
        {
            switch (message.header) {
                // ----- SWIMMING GAME ----- //
                case "swimming-player-move":
                    OnSwimLocation?.DynamicInvoke(ConvertArguments(OnSwimLocation, message.args));
                    break;
                case "swimming-player-remove":
                    OnPlayerRemove?.DynamicInvoke(ConvertArguments(OnPlayerRemove, message.args));
                    break;                
                case "swimming-player-action":
                    OnSwimPlayerAction?.DynamicInvoke(ConvertArguments(OnSwimPlayerAction, message.args));
                    break;
                // ----- SWIMMING GAME ----- //
                
                // ----- MOTOR GAME ----- //
                case "rider-player-position":
                    OnRiderPosition?.DynamicInvoke(ConvertArguments(OnRiderPosition, message.args));
                    break;
                case "rider-player-explosion":
                    OnRiderExplosion?.DynamicInvoke(ConvertArguments(OnRiderExplosion, message.args));
                    break;
                case "rider-player-joined":
                    OnRiderJoined?.DynamicInvoke(ConvertArguments(OnRiderJoined, message.args));
                    break;
                case "rider-player-left":
                    OnRiderLeft?.DynamicInvoke(ConvertArguments(OnRiderLeft, message.args));
                    break;
                // ----- MOTOR GAME ----- //
                
                // ----- BOSS FIGHT GAME ----- //
                case "boss-player-throw":
                    OnThrowObject?.DynamicInvoke(ConvertArguments(OnThrowObject, message.args));
                    break;
                // ----- BOSS FIGHT GAME ----- //
                default:
                    Debug.LogWarning($"Unhandled header: {message.header}");
                    break;
            }
        }
        
        public static AuthMessage GetAuthMessage(string authJson) {
            return authMessage ??= JsonConvert.DeserializeObject<AuthMessage>(authJson);
        }

        private static object[] ConvertArguments(Delegate del, JArray args) {
            var paramInfo = del.Method.GetParameters();
            var finalArgs = new object[paramInfo.Length];
            
            if (finalArgs.Length != args.Count) {
                Debug.LogWarning("Invalid packet arguments, not calling the function");
                return null;
            }
            
            for (var i = 0; i < paramInfo.Length; i++)
            {
                var targetType = paramInfo[i].ParameterType;
                object raw = args[i];
            
                finalArgs[i] = ConvertJsonArg(raw, targetType);
            }
            
            return finalArgs;
        }
        
        private static object ConvertJsonArg(object raw, Type targetType) {
            return raw switch {
                JObject jObj => jObj.ToObject(targetType),
                JArray jArr => jArr.ToObject(targetType),
                _ => Convert.ChangeType(raw, targetType)
            };
        }
    }
}