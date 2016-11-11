using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SatelliteTarget : MonoBehaviour {

	public float distanceRadius = 0.5f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	if (Vector3.Distance (transform.position, transform.parent.position) != distanceRadius) {
			Vector3 vN = (transform.position - transform.parent.position).normalized;
			transform.localPosition = (vN * distanceRadius);
		}
	}
}
