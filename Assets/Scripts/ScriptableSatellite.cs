using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[CreateAssetMenu (fileName = "Satellite", menuName = "New Satellite")]
public class ScriptableSatellite : ScriptableObject {

	public enum InfoType {
		EMPTY,
		OPTICAL,
		WEATHER_CONTROL
	}
	public string SatelliteName;	
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

	public string URL;

	//[ContextMenu("log")]
	public string GetTypeString() {
		return Type.ToString ().Replace("_", " ");
	}
}