using UnityEngine;
using System.Collections;

public class Grabber : MonoBehaviour {

	float rotSpeed = 100;
	float maxSeatAngle = 60;
	float turretRotSpeed = 50;
	float maxTurretAngle = 15;

	SteamVR_ControllerManager ControllerManager;
	SteamVR_TrackedObject[] Controllers;
	SteamVR_Controller.Device[] Devices;

	TextMesh handleText;
	TextMesh handText;
	GameObject centerTracker;
	GameObject centerTracker2;

	float refaceThreshold = 10;
	GameObject seatTracker;

	void Start()
	{
		centerTracker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		centerTracker.transform.localScale *= .05f;
		centerTracker.GetComponent<Renderer>().material.color = Color.blue;

		centerTracker2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		centerTracker2.transform.localScale *= .05f;
		centerTracker2.GetComponent<Renderer>().material.color = Color.red;

		ControllerManager = GameObject.FindObjectOfType<SteamVR_ControllerManager>();
		Controllers = new SteamVR_TrackedObject[] { ControllerManager.left.GetComponent<SteamVR_TrackedObject>(), ControllerManager.right.GetComponent<SteamVR_TrackedObject>() };
		Devices = new SteamVR_Controller.Device[Controllers.Length];

		Globals.steeringColumn = GameObject.Find("SteeringColumn");
		Globals.steerOrigin = Globals.steeringColumn.transform.position;
		Globals.seat = GameObject.Find("Seat");
		seatTracker = Globals.seat.transform.FindChild("SeatTracker").gameObject;

		Globals.handles[0] = GameObject.Find("Cylinder.003");
		Globals.handles[1] = GameObject.Find("Cylinder.000");
		Globals.handles[0].tag = "handle";
		Globals.handles[1].tag = "handle";
		Globals.handles[0].AddComponent<BoxCollider>().isTrigger = true;
		Globals.handles[1].AddComponent<BoxCollider>().isTrigger = true;
		Globals.handles[0].transform.parent = null;
		Globals.handles[1].transform.parent = null;

		Globals.origDist = Mathf.Abs(Globals.handles[0].transform.position.y - Globals.steerOrigin.y);
		Globals.halfHandleDist = Mathf.Abs(Vector3.Distance(Globals.handles[0].transform.position, Globals.handles[1].transform.position))/2f;
		Globals.rootCam = GameObject.Find("[CameraRig]");
		Globals.mainCam = GameObject.Find("Camera (eye)").GetComponent<Camera>();
		Globals.turret = GameObject.Find("Turret");

		handleText = GameObject.Find("HandlePos").GetComponent<TextMesh>();
		handText = GameObject.Find("HandPos").GetComponent<TextMesh>();
	}

	void Update()
	{
		InitializeControllers();

		for(int i = 0; i < Devices.Length; i++)
		{
			if(Devices[i] != null)
			{
				//On the frame the grip is pressed, store the hand offset from the handle
				if(Globals.handleEntered[i] && Devices[i].GetPressDown(Valve.VR.EVRButtonId.k_EButton_Grip))
				{
					Debug.Log(Time.time + " gripped " + Globals.handles[i].name);
					Globals.gripping[i] = true;
					Globals.handleOffsets[i] = Globals.grippedHandles[i].transform.position - Globals.hands[i].gameObject.transform.position;
				}

				//While the grip is held down, position the grippedHandles
				if(Globals.handleEntered[i] && Globals.gripping[i] && Devices[i].GetPress(Valve.VR.EVRButtonId.k_EButton_Grip))
				{
					handleText.text = Globals.hands[i].transform.parent.parent.gameObject.name + " " + i;
					Globals.grippedHandles[i].transform.position = Globals.hands[i].gameObject.transform.position + Globals.handleOffsets[i];
					//Rotate handle to face steering origin
					Vector3 toOrig = Globals.steerOrigin - Globals.grippedHandles[i].transform.position;
					Globals.grippedHandles[i].transform.rotation = Quaternion.LookRotation(toOrig, Globals.grippedHandles[i].transform.forward);
				}
			}
		}

		//Find center point between handles
		if(Globals.gripping[(int)Globals.ControllerEnum.LEFT] || Globals.gripping[(int)Globals.ControllerEnum.RIGHT])
		{
			//constrain handle distances
			if(Globals.gripping[(int)Globals.ControllerEnum.LEFT])
			{
				//steering column right is negative due to object facing
				Globals.handles[(int)Globals.ControllerEnum.RIGHT].transform.position = Globals.handles[(int)Globals.ControllerEnum.LEFT].transform.position + -Globals.steeringColumn.transform.right * 2 * Globals.halfHandleDist;
				//Rotate handle to face steering origin
				Vector3 toOrig = Globals.steerOrigin - Globals.handles[(int)Globals.ControllerEnum.RIGHT].transform.position;
				Globals.handles[(int)Globals.ControllerEnum.RIGHT].transform.rotation = Quaternion.LookRotation(toOrig, Globals.handles[(int)Globals.ControllerEnum.RIGHT].transform.forward);
			}else if(Globals.gripping[(int)Globals.ControllerEnum.RIGHT])
			{
				//steering column right is positive due to object facing
				Globals.handles[(int)Globals.ControllerEnum.LEFT].transform.position = Globals.handles[(int)Globals.ControllerEnum.RIGHT].transform.position + Globals.steeringColumn.transform.right * 2 * Globals.halfHandleDist;
				//Rotate handle to face steering origin
				Vector3 toOrig = Globals.steerOrigin - Globals.handles[(int)Globals.ControllerEnum.LEFT].transform.position;
				Globals.handles[(int)Globals.ControllerEnum.LEFT].transform.rotation = Quaternion.LookRotation(toOrig, Globals.handles[(int)Globals.ControllerEnum.LEFT].transform.forward);
			}
				
			//Rotate steering column to face center point
			//steering column right is negative due to object facing
			Vector3 handleCenter = Globals.handles[(int)Globals.ControllerEnum.LEFT].transform.position + -Globals.steeringColumn.transform.right * Globals.halfHandleDist;
			Vector3 toCenter = handleCenter - Globals.steerOrigin;
			Globals.steeringColumn.transform.rotation = Quaternion.LookRotation(toCenter, Globals.steeringColumn.transform.up);
			centerTracker.transform.position = handleCenter;

			//Constrain handle height to be within steering column
			Globals.handles[(int)Globals.ControllerEnum.LEFT].transform.position = Globals.steerOrigin + Globals.steeringColumn.transform.forward * Globals.origDist + Globals.steeringColumn.transform.right * Globals.halfHandleDist;
			Globals.handles[(int)Globals.ControllerEnum.RIGHT].transform.position = Globals.steerOrigin + Globals.steeringColumn.transform.forward * Globals.origDist + -Globals.steeringColumn.transform.right * Globals.halfHandleDist;

			centerTracker2.transform.position = Globals.steerOrigin + Globals.steeringColumn.transform.forward * Globals.origDist;

			//Rotate seat if past dead zone
			Vector3 toCam = seatTracker.transform.position - Globals.steerOrigin;
			//toCam = new Vector3(toCam.x, 0, toCam.z);
			Vector3 steerUp = Globals.steeringColumn.transform.forward;
			//steerUp = new Vector3(steerUp.x, steerUp.y, 0);
			Vector3 steerRight = -Globals.steeringColumn.transform.right;
			steerRight = new Vector3(steerRight.x, 0, steerRight.z);
			float steerAngle = Mathf.Abs(90 - Vector3.Angle(steerUp, steerRight));

			Vector3 cross = Vector3.Cross(steerUp, steerRight);
			float seatAngle = (Vector3.Angle(Vector3.forward, toCam));
			handText.text = "steerAngle " + steerAngle + " seat angle " + seatAngle + "\ncross " + cross.x + "\n" + cross.y + "\n" + cross.z;
			if(steerAngle > 5 && seatAngle < maxSeatAngle)
			{
				//Debug.Log(Time.time + "cross " + Mathf.Sign(cross.x) + "," + Mathf.Sign(cross.y) + "," + Mathf.Sign(cross.z));
				//Debug.Log(Time.time + " steerRight " + steerRight + " steerUp " + steerUp);
				if(cross.y < 0)
				{
					Globals.seat.transform.Rotate(Vector3.up, rotSpeed * Time.deltaTime);
					toCam = seatTracker.transform.position - Globals.steerOrigin;
					//toCam = new Vector3(toCam.x, 0, toCam.z);
					seatAngle = (Vector3.Angle(Vector3.forward, toCam));
					if(seatAngle >= maxSeatAngle)
					{
						Debug.Log(Time.time + " reset seat angle 1 " + seatAngle);
						Globals.seat.transform.Rotate(Vector3.up, -rotSpeed * Time.deltaTime);
					}
				}else
				{
					Globals.seat.transform.Rotate(Vector3.up, -rotSpeed * Time.deltaTime);
					toCam = seatTracker.transform.position - Globals.steerOrigin;
					//toCam = new Vector3(toCam.x, 0, toCam.z);
					seatAngle = (Vector3.Angle(Vector3.forward, toCam));
					if(seatAngle >= maxSeatAngle)
					{
						Debug.Log(Time.time + " reset seat angle 2 " + seatAngle);
						Globals.seat.transform.Rotate(Vector3.up, rotSpeed * Time.deltaTime);
					}
				}
				//handleText.text = Time.time + " rotating " + Globals.seat.transform.rotation.eulerAngles;
			}

			//Globals.rootCam.gameObject.transform.position = Globals.seat.transform.position;
			//Globals.rootCam.gameObject.transform.LookAt(new Vector3(Globals.steerOrigin.x, Globals.mainCam.gameObject.transform.position.y, Globals.steerOrigin.z));

			//Rotate steering column to face seat
			Vector3 steeringColumnForward = -Globals.steeringColumn.transform.up;
			Vector3 toSeat = seatTracker.transform.position - Globals.steerOrigin;
			toSeat = new Vector3(toSeat.x, 0, toSeat.z);
			for(int i = 0; i < 3600; i++)
			{
				steeringColumnForward = -Globals.steeringColumn.transform.up;
				steeringColumnForward = new Vector3(steeringColumnForward.x, 0, steeringColumnForward.z);
				toSeat = seatTracker.transform.position - Globals.steerOrigin;
				toSeat = new Vector3(toSeat.x, 0, toSeat.z);

				float faceAngle = Vector3.Angle(toSeat, steeringColumnForward);
				if(faceAngle < 1)
				{
					break;
				}else
				{
					Vector3 prevRot = Globals.steeringColumn.transform.rotation.eulerAngles;
					Globals.steeringColumn.transform.rotation = Quaternion.Euler(prevRot.x, prevRot.y + .1f, prevRot.z);
				}
			}
			Globals.handles[(int)Globals.ControllerEnum.LEFT].transform.position = Globals.steerOrigin + Globals.steeringColumn.transform.forward * Globals.origDist + Globals.steeringColumn.transform.right * Globals.halfHandleDist;
			Globals.handles[(int)Globals.ControllerEnum.RIGHT].transform.position = Globals.steerOrigin + Globals.steeringColumn.transform.forward * Globals.origDist + -Globals.steeringColumn.transform.right * Globals.halfHandleDist;

			//rotate turret on Y
			Vector3 prevTurretRot = Globals.turret.transform.rotation.eulerAngles;
			Globals.turret.transform.rotation = Quaternion.Euler(new Vector3(prevTurretRot.x, Globals.seat.transform.rotation.eulerAngles.y, prevTurretRot.z));
			//rotate turret on X
			Vector3 tmp = -Globals.steeringColumn.transform.up;
			tmp = new Vector3(tmp.x, 0, tmp.z);
			steerAngle = Mathf.Abs(90 - Vector3.Angle(tmp, Globals.steeringColumn.transform.forward));
			cross = Vector3.Cross(Vector3.up, Globals.steeringColumn.transform.forward);
			Vector3 turretForward = new Vector3(0, Globals.turret.transform.forward.y, Globals.turret.transform.forward.z);
			float turretAngle = Vector3.Angle(Vector3.forward, turretForward);
			Vector3 cross2 = Vector3.Cross(Vector3.forward, turretForward);
			handText.text = "Turret turretAngle " + turretAngle;
			//cross2 down = -x
			//cross forward = -x
			if(steerAngle > 3 && 
				((Mathf.Sign(cross.x) != Mathf.Sign(cross2.x) && turretAngle >= maxTurretAngle) ||
					turretAngle < maxTurretAngle))
			{
				if(cross.x < 0)
				{
					prevTurretRot = Globals.turret.transform.rotation.eulerAngles;
					Globals.turret.transform.rotation = Quaternion.Euler(new Vector3(prevTurretRot.x - Time.deltaTime * turretRotSpeed, prevTurretRot.y, prevTurretRot.z));
				}else
				{
					prevTurretRot = Globals.turret.transform.rotation.eulerAngles;
					Globals.turret.transform.rotation = Quaternion.Euler(new Vector3(prevTurretRot.x + Time.deltaTime * turretRotSpeed, prevTurretRot.y, prevTurretRot.z));
				}
			}
		}
	}

	void InitializeControllers()
	{
		for (int index = 0; index < Controllers.Length; index++)
		{
			if (Controllers[index] != null && Controllers[index].index != SteamVR_TrackedObject.EIndex.None)
			{
				Devices[index] = SteamVR_Controller.Input((int)Controllers[index].index);
			}
			else
			{
				Devices[index] = null;
			}
		}
	}
}
