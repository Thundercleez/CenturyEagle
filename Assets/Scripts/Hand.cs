using UnityEngine;
using System.Collections;

public class Hand : MonoBehaviour {

	float exitTimer;
	float exitTimeout = .25f;
	bool exiting;
	GameObject handle;

	void Update()
	{
		if(exiting &&
			Time.time - exitTimer >= exitTimeout)
		{
			exiting = false;
			Debug.Log(Time.time + " Trigger Exit " + handle.name);
			if(gameObject.transform.parent.parent.name.Contains("left"))
			{
				Globals.handleEntered[(int)Globals.ControllerEnum.LEFT] = false;
				Globals.grippedHandles[(int)Globals.ControllerEnum.LEFT] = null;
				Globals.gripping[(int)Globals.ControllerEnum.LEFT] = false;
			}else
			{
				Globals.handleEntered[(int)Globals.ControllerEnum.RIGHT] = false;
				Globals.grippedHandles[(int)Globals.ControllerEnum.RIGHT] = null;
				Globals.gripping[(int)Globals.ControllerEnum.RIGHT] = false;
			}
		}
	}

	void OnTriggerEnter(Collider col)
	{
		if(col.gameObject.tag.Equals("handle"))
		{
			exiting = false;
			Debug.Log(Time.time + " Trigger enter " + gameObject.transform.parent.parent.name);
			if(gameObject.transform.parent.parent.name.Contains("left"))
			{
				Debug.Log(Time.time + " Trigger Enter Left " + gameObject.name);
				Globals.handleEntered[(int)Globals.ControllerEnum.LEFT] = true;
				Globals.grippedHandles[(int)Globals.ControllerEnum.LEFT] = col.gameObject;
			}else
			{
				Debug.Log(Time.time + " Trigger Enter Right " + gameObject.name);
				Globals.handleEntered[(int)Globals.ControllerEnum.RIGHT] = true;
				Globals.grippedHandles[(int)Globals.ControllerEnum.RIGHT] = col.gameObject;
			}
		}
	}

	void OnTriggerExit(Collider col)
	{
		if(col.gameObject.tag.Equals("handle"))
		{
			exiting = true;
			exitTimer = Time.time;

			handle = col.gameObject;
		}
	}
}
