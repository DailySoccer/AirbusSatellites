using UnityEngine;
using System.Collections;

public class Autorotate : MonoBehaviour {

	public float rotationSpeed = 1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate( 0, -Time.deltaTime * rotationSpeed, 0);
	}
}
