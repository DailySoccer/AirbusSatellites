using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class ModalScreen : MonoBehaviour {

	public Text Excerpt;
	public Text Description;
	
	[Header("Images")]
	public Image NameImage;
	public Image Picture;	
	
	[Header("Info")]
	public Text Country;
	public Text Date;
	public Text Type;
	public Text MassAtLaunch;
	public Text satName;
	public string url;

	public static ModalScreen Instance;

	CanvasGroup canvasGroup;

	void Awake() {
		if (Instance == null)
			Instance = this;

		canvasGroup = GetComponent<CanvasGroup> ();
	}

	// Use this for initialization
	void Start () {
		Earth = GameObject.FindGameObjectWithTag("Tierra");
		Close ();
	}
	
	// Update is called once per frame
	void Update () {	
	}

	public void Show(ScriptableSatellite sat) {
		if (sat == null)
			return;

		StartCoroutine(GotoToTarget(sat));
	}

	void SetModalInfo(ScriptableSatellite sat) {
		canvasGroup.alpha = 1f;
		canvasGroup.interactable = true;
		canvasGroup.blocksRaycasts = true;
		
		Excerpt.text 		= sat.Excerpt.ToUpper();
		Description.text	= sat.Description;
		NameImage.sprite	= sat.NameImage;
		Picture.sprite 		= sat.Picture;
		Country.text 		= sat.Country.ToUpper();
		Date.text 			= sat.Date.ToUpper();
		Type.text 			= sat.GetTypeString ().ToUpper();
		MassAtLaunch.text 	= sat.MassAtLaunch.ToUpper();
		satName.text		= ("MORE ABOUT " + sat.SatelliteName).ToUpper();
		url 				= sat.URL;
	}

	IEnumerator GotoToTarget(ScriptableSatellite sat) {
		GameObject sender = EventSystem.current.currentSelectedGameObject;
		cameraDist = SmoothCameraOrbit.Instance.distance;
		SmoothCameraOrbit.Instance.ChangeCameraSettings (sender.transform, CameraState.Fixed);
		while (SmoothCameraOrbit.Instance.GetTimeLeftToTarget() > 0) {
			yield return null;
		}
		SetModalInfo(sat);
	}

	public void Close() {
		canvasGroup.alpha = 0f;
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
		SmoothCameraOrbit.Instance.ChangeCameraSettings(null, CameraState.FreeRotation);
	}

	public void GoToSatelliteLink() {
		Application.OpenURL (url);
	}


	GameObject Earth;
	float cameraDist;
}
