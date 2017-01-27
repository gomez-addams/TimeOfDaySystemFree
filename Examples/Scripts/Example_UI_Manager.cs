using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using AC.TimeOfDaySystemFree;

public class Example_UI_Manager : MonoBehaviour 
{


	public TimeOfDayManager TOD_Manager;
	public GameObject UIObject;

	public Slider worldLongitude;
	public Toggle playTime;
	public Slider dayInSeconds;
	public Slider timeLine;


	private bool enableUI = true;


	void Start()
	{

		worldLongitude.value = TOD_Manager.WorldLongitude;
		playTime.isOn        =  TOD_Manager.playTime;
		timeLine.value       =  TOD_Manager.timeline;
		dayInSeconds.value   =  TOD_Manager.dayInSeconds;
	}

	void Update()
	{

		if (Input.GetKeyDown (KeyCode.G))
			enableUI = !enableUI;

		UIObject.SetActive (enableUI);

		TOD_Manager.WorldLongitude = worldLongitude.value;
		TOD_Manager.playTime = playTime.isOn;


		if (! TOD_Manager.playTime) 
		{
			TOD_Manager.timeline = timeLine.value;
		} 
		else 
		{

			TOD_Manager.dayInSeconds = dayInSeconds.value;
			timeLine.value           = TOD_Manager.timeline;
		}
	}

}
