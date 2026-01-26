using System.IO;
using UnityEditor;
using UnityEngine;

public class ConfigMenu : MonoBehaviour
{
	[MenuItem("Katpatat/Open Config Location")]
	private static void OpenConfigLocation()
	{
		string pathToConfigFile = Path.Combine(
			System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),
			"MorphixProductions",
			"Unity",
			"config.json"
		);
		
		EditorUtility.RevealInFinder(pathToConfigFile);
	}
}
