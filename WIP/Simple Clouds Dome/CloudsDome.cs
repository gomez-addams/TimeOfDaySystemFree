using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.TimeOfDaySystemFree
{

	[ExecuteInEditMode]
	public class CloudsDome : MonoBehaviour 
	{


		// Time of day manager.
		public TimeOfDayManager TOD_Manager = null;

		// Player.
		public Transform player = null;

		// Material.
		public Material  material = null;

		// Clouds texture.
		public Texture2D cloudsTexture = null;

		// Clouds Size.
		public Vector2 cloudsSize = Vector2.one;

		// Clouds Offset.
		public Vector2 cloudsOffset = Vector2.one;

		// Clouds Speed.
		public Vector2 cloudsSpeed = Vector2.one;


		// Sharpness.
		[Range(0f,10f)]
		public float sharpness = 1;


		public Gradient cloudsColorGradient = new Gradient()
		{
			colorKeys = new GradientColorKey[]
			{
				new GradientColorKey(new Color(.5f, .5f, .5f, 1f), .25f),
				new GradientColorKey(new Color(.5f, .5f, .5f, 1f), .30f),
				new GradientColorKey(new Color(.5f, .5f, .5f, 1f), .70f),
				new GradientColorKey(new Color(.5f, .5f, .5f, 1f), .75f)
			},
			alphaKeys = new GradientAlphaKey[] 
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(1f, 1f)
			}
		};


		// Cloud density.
		[Range(1,10)]
		public float density = 1f;

		// Shadow multiplier.
		[Range(0,1)]
		public float cloudsShadow = 1;


		void Update()
		{

			if (TOD_Manager != null && player != null && material != null && cloudsTexture != null) 
			{

				// Folow player.
				this.transform.position = player.position;

				//this.transform.eulerAngles = new Vector3 (-90, TOD_Manager.WorldLongitude, 0);

				material.SetColor("_CloudsColor", cloudsColorGradient.Evaluate(TOD_Manager.CGTime)); 
				material.SetTexture("_CloudsTex", cloudsTexture);
				material.SetVector("_CloudsSize", cloudsSize);
				material.SetVector("_CloudsOffset", cloudsOffset);
				material.SetVector("_CloudsSpeed", cloudsSpeed);
				material.SetFloat ("_Sharpness", sharpness);
				material.SetFloat ("_Density", density);
				material.SetFloat("_CloudsShadow", cloudsShadow);

			}

		}


	}

}