using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[CreateAssetMenu (fileName = "Satellite", menuName = "New Satellite")]
public class ScriptableSatellite : ScriptableObject {

	public enum InfoType {
		OPTICAL,
		WEATHER_CONTROL,
		WAR
	}
		
	public string Excerpt;
	public string Description;

	[Header("Images")]
	public Sprite NameImage;
	public Sprite Picture;	

	[Header("Info")]
	public string Country;
	public string Date;
	public InfoType Type;
	public string MassAtLaunch;

	[ContextMenu("log")]
	string GetTypeString() {
		Debug.Log( Type.ToString ().Replace("_", " "));
		return Type.ToString ().Replace("_", " ");
	}
}