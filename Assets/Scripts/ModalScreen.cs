using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
	public string url;

	public static ModalScreen Instance;

	CanvasGroup canvasGroup;

	void Awake() {
		if (Instance == null)
			Instance = this;

		canvasGroup = GetComponent<CanvasGroup> ();
		Close ();
	}

	// Use this for initialization
	void Start () {	
	}
	
	// Update is called once per frame
	void Update () {	
	}

	public void Show(ScriptableSatellite sat) {
		if (sat == null)
			return;

		canvasGroup.alpha = 1f;
		canvasGroup.interactable = true;
		canvasGroup.blocksRaycasts = true;

		Excerpt.text 		= sat.Excerpt;
		Description.text	= sat.Description;
		NameImage.sprite	= sat.NameImage;
		Picture.sprite 		= sat.Picture;
		Country.text 		= sat.Country;
		Date.text 			= sat.Date;
		Type.text 			= sat.GetTypeString ();
		MassAtLaunch.text 	= sat.MassAtLaunch;
		url 				= sat.URL;
	}

	public void Close() {
		canvasGroup.alpha = 0f;
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
	}

	public void GoToSatelliteLink() {
		Application.OpenURL (url);
	}
}
