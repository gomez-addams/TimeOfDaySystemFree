using UnityEngine;
using UnityEngine.UI;
//using System.Collections;

namespace AC.TimeOfDaySystemFree
{

	public class DisplayTime : MonoBehaviour 
	{

		// UI text component.
		public Text timeText;

		// Time of day manager.
		private TimeOfDayManager m_TODManager = null;


		private void Start()
		{
			// Get Time Of Day Manager.
			m_TODManager = GetComponent<TimeOfDayManager> ();
		}
			

		private void Update()
		{

			if (timeText != null || m_TODManager != null) 
			{
				timeText.text = GetTimeString (); 
			}
		}
			

		public string GetTimeString()
		{
			string h   = m_TODManager.Hour   < 10 ? "0" + m_TODManager.Hour.ToString()   : m_TODManager.Hour.ToString();
			string m   = m_TODManager.Minute < 10 ? "0" + m_TODManager.Minute.ToString() : m_TODManager.Minute.ToString();
			//----------------------------------------------------------------------------------------------------------------

			return h   + ":" + m;
		}

		/*
		public string GetTimeString(float hour, float minute)
		{
			string h   = hour   < 10 ? "0" + hour.ToString()   : hour.ToString();
			string m   = minute < 10 ? "0" + minute.ToString() : minute.ToString();
			//----------------------------------------------------------------------

			return h   + ":" + m;
		}*/
	}


}
