using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class DescriptionPositionedBillboard : MonoBehaviour
{
	public Camera referenceCamera;
	public float earthRadius;
	public float distanceRadius;
	public Color GizmoColor;

	void  Awake ()
	{
		// if no camera referenced, grab the main camera
		if (!referenceCamera)
			referenceCamera = Camera.main; 
	}

	void Update()
	{
		transform.LookAt(transform.position + referenceCamera.transform.rotation * Vector3.forward, referenceCamera.transform.rotation * Vector3.up);

		if (Vector3.Distance (transform.position, transform.parent.position) != distanceRadius) {
			Vector3 vN = (transform.position - transform.parent.position).normalized;
			transform.localPosition = (vN * distanceRadius);
		}

	}

	void OnDrawGizmos() {
		Gizmos.color = GizmoColor;
		Gizmos.DrawLine(transform.position, transform.parent.position);
	}

	[ContextMenu("Positcionar")]
	public void Reposition ()
	{
		//Transform ZeroPoint = GameObject.FindGameObjectWithTag("Tierra").transform;
		Transform parentPosition = transform.parent;


		Vector3 aN = parentPosition.localPosition.normalized;

		parentPosition.localPosition = aN * earthRadius;

		transform.localPosition = (aN * distanceRadius);
	}
}