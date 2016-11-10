using UnityEngine;
using System.Collections;

public class LineToTarget : MonoBehaviour {

	public Transform target;
	public Color startColor;
	public Color endColor;

	private LineRenderer LR;

	void Awake() {
		
		if (target == null)
			target = transform.GetChild (0);

		gameObject.AddComponent<LineRenderer> ();

		LR = GetComponent<LineRenderer>();
		LR.SetWidth(0.002f, 0.002f);
		LR.material.shader = Shader.Find ("Sprites/Default"); //"Mobile/Unlit (Supports Lightmap)";
		LR.SetColors (startColor, endColor);
		LR.SetVertexCount (2);

	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		LR.SetPosition (0, transform.position);
		LR.SetPosition (1, target.position);
	}
}
