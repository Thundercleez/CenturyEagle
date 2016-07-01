using UnityEngine;
using System.Collections;

public class ShowAxis : MonoBehaviour {

	public float dist;

	void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(gameObject.transform.position, gameObject.transform.position + gameObject.transform.forward * dist);

		Gizmos.color = Color.red;
		Gizmos.DrawLine(gameObject.transform.position, gameObject.transform.position + gameObject.transform.right * dist);

		Gizmos.color = Color.green;
		Gizmos.DrawLine(gameObject.transform.position, gameObject.transform.position + gameObject.transform.up * dist);
	}
}
