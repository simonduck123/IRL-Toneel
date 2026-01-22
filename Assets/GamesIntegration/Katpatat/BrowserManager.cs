using System;
using System.IO;
using UnityEngine;
using VoltstroStudios.UnityWebBrowser.Core;

namespace GamesIntegration.Katpatat {
	public class BrowserManager : MonoBehaviour {
		[SerializeField] private BaseUwbClientManager baseUwbClientManager;

		private void Awake() {
			var randomCacheDir = Path.Combine(
				Application.temporaryCachePath,
				"UWB_" + Guid.NewGuid().ToString("N")
			);

			Directory.CreateDirectory(randomCacheDir);
		
			baseUwbClientManager.browserClient.CachePath = new FileInfo(randomCacheDir);
		}
	}
}
