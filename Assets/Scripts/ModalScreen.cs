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
		_sats = GameObject.FindGameObjectsWithTag("Satellite");
	}

	// Use this for initialization
	void Start () {
		_earth = GameObject.FindGameObjectWithTag("Tierra");
		_lastCameraTransitionTime = SmoothCameraOrbit.Instance.cameraTransitionTime;
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

		SetOnlySatelliteVisible (sender);
		sender.GetComponent<Button> ().Select ();

		SmoothCameraOrbit.Instance.ChangeCameraSettings (sender.transform, CameraState.Fixed);

		while (SmoothCameraOrbit.Instance.GetTimeLeftToTarget() > 0) {
			yield return null;
		}

		SetModalInfo(sat);

		SmoothCameraOrbit.Instance.cameraTransitionTime = 0.5f;
		SmoothCameraOrbit.Instance.ChangeCameraSettings (sender.transform, CameraState.Fixed, 0.05f, _earth.transform);
	}

	void SetOnlySatelliteVisible(GameObject sat) {
		SetAllSatellitesVisibility(false);
		if (sat != null) 
			sat.SetActive(true);
	}

	void SetAllSatellitesVisibility(bool visibility) {
		foreach(GameObject go in _sats) {
			go.SetActive(visibility);
		}
	}

	public void Close() {
		canvasGroup.alpha = 0f;
		canvasGroup.interactable = false; 
		canvasGroup.blocksRaycasts = false;
		SetAllSatellitesVisibility(true);
		SmoothCameraOrbit.Instance.cameraTransitionTime = _lastCameraTransitionTime;
		SmoothCameraOrbit.Instance.ChangeCameraSettings(null, CameraState.FreeRotation);
	}

	public void GoToSatelliteLink() {
		Application.OpenURL (url);
	}


	GameObject _earth;
	//float cameraDist;
	GameObject[] _sats;
	float _lastCameraTransitionTime;
}
