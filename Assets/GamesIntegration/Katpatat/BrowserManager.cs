using System;
using System.IO;
using Katpatat.Networking;
using UnityEngine;
using VoltstroStudios.UnityWebBrowser.Core;

namespace GamesIntegration.Katpatat {
	public class BrowserManager : MonoBehaviour {
		[SerializeField] private BaseUwbClientManager baseUwbClientManager;

		[SerializeField] private BrowserScreen browserScreen;
		
		private void Awake() {
			var randomCacheDir = Path.Combine(
				Application.temporaryCachePath,
				"UWB_" + Guid.NewGuid().ToString("N")
			);

			Directory.CreateDirectory(randomCacheDir);
		
			baseUwbClientManager.browserClient.CachePath = new FileInfo(randomCacheDir);
			
			if (NetworkClient.config == null)
				NetworkClient.OnConfigLoaded += ConfigLoaded;
			else
				ConfigLoaded(NetworkClient.config);
		}

		private void ConfigLoaded(Config config) {
			Debug.Log("Config loaded set browser data");
			
			switch (browserScreen) {
				case BrowserScreen.Left:
					baseUwbClientManager.browserClient.initialUrl = config.serverConfig.leftScreenUrl;
					baseUwbClientManager.browserClient.LoadUrl(config.serverConfig.leftScreenUrl);
					break;
				case BrowserScreen.Right:
					baseUwbClientManager.browserClient.initialUrl = config.serverConfig.rightScreenUrl;
					baseUwbClientManager.browserClient.LoadUrl(config.serverConfig.rightScreenUrl);
					break;
				default:
					Debug.LogError("Unknown browser screen");
					break;
			}
		}

		private void OnDestroy() {
			NetworkClient.OnConfigLoaded -= ConfigLoaded;
		}
	}

	public enum BrowserScreen {
		Left,
		Right
	}
}
