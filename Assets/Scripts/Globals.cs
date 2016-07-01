using UnityEngine;
using System.Collections;

public class Globals : MonoBehaviour {

	public enum ControllerEnum { LEFT, RIGHT};

	public static bool [] handleEntered = new bool[2];
	public static bool [] gripping = new bool[2];
	public static GameObject [] grippedHandles = new GameObject[2];
	public static GameObject [] handles = new GameObject[2];
	public static Vector3 [] handleOffsets = new Vector3[2];
	public static Vector3 steerOrigin;
	public static float origDist; //distance from steering column origin to a handle
	public static float halfHandleDist; //half the distance between the steering column handles
	public static GameObject steeringColumn;
	public static GameObject seat;
	public static Camera mainCam;
	public static GameObject rootCam;
	public static GameObject [] hands = new GameObject[2];
	public static GameObject turret;
}
