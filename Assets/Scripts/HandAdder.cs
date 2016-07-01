using UnityEngine;
using System.Collections;

public class HandAdder : MonoBehaviour {

	void Update()
	{
		if(gameObject.transform.FindChild("body") != null)
		{
			GameObject body = gameObject.transform.FindChild("body").gameObject;
			Rigidbody rbody = body.AddComponent<Rigidbody>();
			rbody.constraints = RigidbodyConstraints.FreezeAll;
			rbody.useGravity = false;
			body.AddComponent<BoxCollider>().isTrigger = true;
			body.AddComponent<Hand>();
			if(gameObject.transform.parent.name.Contains("left"))
			{
				Globals.hands[(int)Globals.ControllerEnum.LEFT] = body;
				body.GetComponent<Renderer>().material.color = Color.red;
			}else
			{
				Globals.hands[(int)Globals.ControllerEnum.RIGHT] = body;
				body.GetComponent<Renderer>().material.color = Color.green;
			}
			Destroy(this);
		}
	}
}
