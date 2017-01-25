using UnityEngine;
using System.Collections;
using AC.TimeOfDaySystemFree;

[ExecuteInEditMode]
public class Example_TOD_Night_Particles : MonoBehaviour 
{

	TimeOfDayManager TOD { get { return GetComponent<TimeOfDayManager> (); } }

	public ParticleSystem _particleSystem = null;


	void Update()
	{

		if (TOD != null && _particleSystem != null)
		{
			

			ParticleSystem.EmissionModule emission = _particleSystem.emission;

			emission.enabled = TOD.IsNight ? true : false;
		}

	}



}
