using UnityEngine;
using System.Collections;

namespace AC.TimeOfDaySystemFree
{

	public abstract class TimeOfDay : MonoBehaviour 
	{


		#region Components.

		// Sun and moon light.
		[SerializeField] protected Light m_DirectionalLight = null;

		// Directional light transform.
		protected Transform m_DirectionalLightTransform = null;

		// Sun transform.
		[SerializeField] protected Transform m_SunTransform = null;

		// Moon transform.
		[SerializeField] protected Transform m_MoonTransform = null;
		//---------------------------------------------------------------------------

		#endregion

		#region World And Time

		public bool playTime;
		//-------------------------------------------------------------------------------------

		// Longitude.
		[SerializeField]
		[Range(-180f,180f)] protected float m_WorldLongitude = 25f;

		public bool  useWorldLongitudeCurve = false;

		public AnimationCurve worldLongitudeCurve = AnimationCurve.Linear(0, 25f, 1f, 25f);

		public float WorldLongitude
		{ 
			get{ return m_WorldLongitude;  } 
			set{ m_WorldLongitude = value; }
		}
		//--------------------------------------------------------------------------------------

		public Vector3 WorldRotation{ get; private set; }

		// Day in seconds. 
		public float dayInSeconds = 900f; 

	
		// Current time.
		public float timeline = 7f; 

		// Day duration in the earth.
		protected const float k_DayDuration = 24f;

		public float  Hour{ get; private set; }
		public float  Minute{ get; private set; }
		//----------------------------------------------------------------------------------------

		// Used to evaluate the curves and gradients.
		public float CGTime{get{return (timeline / k_DayDuration);}}


		// Get sun direction.
	    public Vector3 SunDirection { get { return -m_SunTransform.forward; } }
		//---------------------------------------------------------------------------------------

		// Const degrees.
		protected const int k_RightAngle = 90;
		//protected const int k_270deg     = 270; 
		//----------------------------------------------------------------------------------------


		// Day states.
		public bool IsDay{ get; private set; }
		public bool IsNight{ get; private set; }

		// Directional light states.
		public bool IsSunLight{ get; private set; }
		public bool IsMoonLight{ get; private set; }
		//----------------------------------------------------------------------------------------


		#endregion

		#region Sun


		[SerializeField] protected Color m_SunColor = new Color (1f, .851f, .722f, 1f);

		public bool useSunColorGradient = true;

		public Gradient sunColorGradient = new Gradient()
		{
			colorKeys = new GradientColorKey[]
			{
				new GradientColorKey(new Color(1f, .639f, .482f, 1f), .25f),
				new GradientColorKey(new Color(1f, .725f, .482f, 1f), .30f),
				new GradientColorKey(new Color(1f, .851f, .722f, 1f), .50f),
				new GradientColorKey(new Color(1f, .725f, .482f, 1f), .70f),
				new GradientColorKey(new Color(1f, .639f, .482f, 1f), .75f)
			},
			alphaKeys = new GradientAlphaKey[] 
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(1f, 1f)
			}
		};

		// Get sun color.
		public Color SunColor
		{
			get{ return m_SunColor;  }
			set{ m_SunColor = value; }
		}
		//---------------------------------------------------------------------------------------

		[SerializeField]
		[Range(0,8f)] private float m_SunLightIntensity = 0f;

		public bool useSunLightIntensityCurve = true; 

		public AnimationCurve sunLightIntensityCurve = new AnimationCurve()
		{
			keys = new Keyframe[]
			{
				new Keyframe(  0f, 0f), 
				new Keyframe(.25f, 0f), 
				new Keyframe(.30f, 1f), 
				new Keyframe(.70f, 1f), 
				new Keyframe(.75f, 0f), 
				new Keyframe(  1f, 0f) 
			}

		};

		// Get sun light intensity.
		public float SunLightIntensity
		{
			get{ return m_SunLightIntensity;  }
			set{ m_SunLightIntensity = value; }
		} 
		//----------------------------------------------------------------------------------------


		public Matrix4x4 SunMatrix { get{ return (m_SunTransform != null) ? m_SunTransform.worldToLocalMatrix : Matrix4x4.identity;}}

		#endregion

		#region Moon
		public bool useMoon = true; 

		public enum MoonRotationMode{Automatic, Custom}
		public MoonRotationMode moonRotationMode = MoonRotationMode.Automatic;
		//---------------------------------------------------------------------------------------


		[SerializeField]
		[Range(-180f,180f)] protected float m_MoonYaw = 0;

		public bool useMoonYawCurve  = false;

		public AnimationCurve moonYawCurve =  AnimationCurve.Linear (0, 1f, 1f, 1f); 

		public float MoonYaw
		{
			get { return m_MoonYaw; } 
			set { m_MoonYaw = value;} 
		}
		//---------------------------------------------------------------------------------------

		[Range(-180f,180f)]
		[SerializeField]
		protected float m_MoonPitch = 0;

		public bool useMoonPitchCurve  = false;

		public AnimationCurve moonPitchCurve =  AnimationCurve.Linear (0, 1f, 1f, 360f); 

		public float MoonPitch
		{
			get { return m_MoonPitch; } 
			set { m_MoonPitch = value;} 
		}
		//---------------------------------------------------------------------------------------


		[SerializeField]
		protected Color m_MoonLightColor = new Color (.345f, .459f, .533f, 1f);

		public bool useMoonLightColorGradient;

		public Gradient moonLightColorGradient = new Gradient()
		{
			colorKeys = new GradientColorKey[]
			{
				new GradientColorKey(new Color(.345f, .459f, .533f, 1f), 0f),
				new GradientColorKey(new Color(.345f, .459f, .533f, 1f), 1f)
			},
			alphaKeys = new GradientAlphaKey[] 
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(1f, 1f)
			}
		};

		// Get moon light color.
		public Color MoonLightColor
		{
			get{ return m_MoonLightColor;  }
			set{ m_MoonLightColor = value; }
		}
		//---------------------------------------------------------------------------------------

		public bool useMoonLightIntensityCurve = true;

		[SerializeField]
		[Range(0,5f)]protected float m_MoonLightIntensity = .2f;

		public AnimationCurve moonLightIntensityCurve = new AnimationCurve()
		{
			keys = new Keyframe[]
			{
				new Keyframe(  0f, .2f), new Keyframe(.20f, .2f), 
				new Keyframe(.22f,  0f), new Keyframe(.77f,  0f), 
				new Keyframe(.80f, .2f), new Keyframe( 1f, .2f) 
			}
		}; 

		// Get moon intensity .
		public float MoonLightIntensity 
		{ 
			get{ return m_MoonLightIntensity;  } 
			set{ m_MoonLightIntensity = value; } 
		}
		//--------------------------------------------------------------------------------------



		#endregion

		#region Ambient

		// Ambient Source.
		private enum AmbientMode{Color, Gradient, Skybox}

		[SerializeField]
		private AmbientMode m_AmbientMode = AmbientMode.Gradient;
		//----------------------------------------------------------------------------------

		// Gradient Ambient.
		public bool useAmbientSkyColorGradient = true;

		[SerializeField]
		protected Color m_AmbientSkyColor = new Color(.004f, .071f, .161f, 1f);

		public Gradient ambientSkyColorGradient = new Gradient () 
		{
			colorKeys = new GradientColorKey[]
			{
				new GradientColorKey(new Color(.004f, .071f, .161f, 1f), .22f),
				new GradientColorKey(new Color(.435f, .494f, .498f, 1f), .25f),
				new GradientColorKey(new Color(.463f, .576f, .769f, 1f), .30f),
				new GradientColorKey(new Color(.463f, .576f, .769f, 1f), .70f),
				new GradientColorKey(new Color(.435f, .494f, .498f, 1f), .75f),
				new GradientColorKey(new Color(.004f, .071f, .161f, 1f), .78f)
			},
			alphaKeys = new GradientAlphaKey[] 
			{
				new GradientAlphaKey(1f, .20f),
				new GradientAlphaKey(1f, .70f)
			}
		};

		public Color AmbientSkyColor
		{
			get{ return m_AmbientSkyColor;  } 
			set{ m_AmbientSkyColor = value; } 
		}
		//----------------------------------------------------------------------------------

		public bool useAmbientEquatorColorGradient = true;

		[SerializeField]
		protected Color m_AmbientEquatorColor = new Color(.008f, .125f, 0.275f, 1f);

		public Gradient ambientEquatorColorGradient = new Gradient ()
		{
			colorKeys = new GradientColorKey[]
			{
				new GradientColorKey(new Color(.008f, .125f, 0.275f,  1f), .22f),
				new GradientColorKey(new Color(.859f, .780f, .561f, 1f), .25f),
				new GradientColorKey(new Color(.698f, .843f,    1f, 1f), .30f),
				new GradientColorKey(new Color(.698f, .843f,    1f, 1f), .70f),
				new GradientColorKey(new Color(.859f, .780f, .561f, 1f), .75f),
				new GradientColorKey(new Color(.008f, .125f, 0.275f,  1f), .78f)
			},
			alphaKeys = new GradientAlphaKey[] 
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(1f, 1f)
			}
		};

		public Color AmbientEquatorColor
		{
			get{ return m_AmbientEquatorColor;  } 
			set{ m_AmbientEquatorColor = value; } 
		}
		//----------------------------------------------------------------------------------

		public bool useAmbientGroundColorGradient = true;

		[SerializeField]
		protected Color m_AmbientGroundColor = new Color (.467f, .435f, .416f, 1f);

		public Gradient ambientGroundColorGradient  = new Gradient ()
		{
			colorKeys = new GradientColorKey[]
			{
				new GradientColorKey(new Color(0f, 0f, 0f, 1f), .22f),
				new GradientColorKey(new Color(.227f, .157f, .102f, 1f), .25f),
				new GradientColorKey(new Color(.467f, .435f, .416f, 1f), .30f),
				new GradientColorKey(new Color(.467f, .435f, .416f, 1f), .70f),
				new GradientColorKey(new Color(.227f, .157f, .102f, 1f), .75f),
				new GradientColorKey(new Color(0f, 0f, 0f, 1f), .78f)
			},
			alphaKeys = new GradientAlphaKey[] 
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(1f, 1f)
			}
		};

		public Color AmbientGroundColor
		{
			get{ return m_AmbientGroundColor;  } 
			set{ m_AmbientGroundColor = value; } 
		}
		//----------------------------------------------------------------------------------

		public bool useAmbientIntensityCurve = false;

		[SerializeField]
		[Range(0,8f)] protected float m_AmbientIntensity = 1f;

		public AnimationCurve ambientIntensityCurve = AnimationCurve.Linear (0, 1f, 1f, 1f);  

		public float AmbientIntensity
		{
			get{ return m_AmbientIntensity;  } 
			set{ m_AmbientIntensity = value; } 
		}

		//----------------------------------------------------------------------------------

		#endregion

		#region Fog

		protected enum FogType{RenderSettings, EvaluateOnly, Off}
		[SerializeField]protected FogType fogType = FogType.RenderSettings;
		//--------------------------------------------------------------------------------------

		public FogMode fogMode = FogMode.ExponentialSquared;
		//--------------------------------------------------------------------------------------

		public bool useRenderSettingsFog = false;

		public bool useFogDensityCurve;

		[SerializeField]
		[Range(0,1f)]protected float m_FogDensity = 0.001f;

		public AnimationCurve fogDensityCurve = AnimationCurve.Linear(0, 0.0016f, 1f, 0.0016f);  

		public float FogDensity
		{
			get{ return m_FogDensity;  }  
			set{ m_FogDensity = value; } 
		}
		//--------------------------------------------------------------------------------------

		public bool useFogStartDistanceCurve;

		[SerializeField]
		protected float m_FogStartDistance = 0f;

		public AnimationCurve fogStartDistanceCurve = AnimationCurve.Linear(0, 0f, 1f, 0f);  

		public float FogStartDistance
		{
			get{ return m_FogStartDistance;  }  
			set{ m_FogStartDistance = value; } 
		}
		//--------------------------------------------------------------------------------------

		public bool useFogEndDistanceCurve;

		[SerializeField]
		protected float m_FogEndDistance = 300f;

		public AnimationCurve fogEndDistanceCurve = AnimationCurve.Linear(0, 300f, 1f, 300f);  

		public float FogEndDistance
		{
			get{ return m_FogEndDistance;  }  
			set{ m_FogEndDistance = value; } 
		}
		//--------------------------------------------------------------------------------------

		public bool useFogColorGradient = true;

		[SerializeField]
		protected Color m_FogColor = new Color(.008f, .117f, .259f, 1f); 

		public Gradient fogColorGradient  = new Gradient ()
		{
			colorKeys = new GradientColorKey[]
			{
				new GradientColorKey(new Color(.008f, .117f, .259f, 1f), .22f),
				new GradientColorKey(new Color(.682f, .655f, .584f, 1f), .25f),
				new GradientColorKey(new Color(.576f, .706f, .878f, 1f), .30f),
				new GradientColorKey(new Color(.576f, .706f, .878f, 1f), .70f),
				new GradientColorKey(new Color(.682f, .655f, .584f, 1f), .75f),
				new GradientColorKey(new Color(.008f, .117f, .259f, 1f), .78f)
			},
			alphaKeys = new GradientAlphaKey[] 
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(1f, 1f)
			}
		};

		public Color FogColor
		{
			get{ return m_FogColor;  }  
			set{ m_FogColor = value; } 
		}
		//--------------------------------------------------------------------------------------

		#endregion


		protected virtual void Init()
		{
			if(m_DirectionalLight != null)
				m_DirectionalLightTransform  = m_DirectionalLight.transform;

		}


		protected void UpdateTime()
		{

			// Prevent the current time exceeds the day duration.
			if (timeline > k_DayDuration) timeline = 0; 
			if (timeline < 0)          timeline = k_DayDuration; 
			//---------------------------------------------------------------------------------

			// Play time.
			if (playTime && Application.isPlaying) 
				timeline  += (Time.deltaTime / dayInSeconds) * k_DayDuration;
			//---------------------------------------------------------------------------------

			// Get hour and minutes.
			Hour   = Mathf.Floor (timeline);
			Minute = Mathf.Floor ((timeline - Hour)*60);
			//---------------------------------------------------------------------------------

			// Directional light state.
			IsSunLight  = (timeline <= 5.50f || timeline >= 18.50f) ? false : true;
			IsMoonLight = !IsSunLight;
			//---------------------------------------------------------------------------------

			// Day state.
			IsDay = (timeline <= 5.49f || timeline >= 18.49f) ? false : true;
			IsNight = !IsDay; 

			// Get world rotation.
			if(useWorldLongitudeCurve) 
				WorldLongitude = worldLongitudeCurve.Evaluate(CGTime) - k_RightAngle;

			WorldRotation = new Vector3 () 
			{
				x =	timeline * (360 / k_DayDuration) - k_RightAngle,
				y = WorldLongitude,
				z = 0
			};

		}



		protected virtual void Sun()
		{

			m_SunTransform.localEulerAngles = new Vector3(WorldRotation.x, WorldRotation.y, 0);

			// Evaluate sun curves and gradients.
			if (useSunLightIntensityCurve)SunLightIntensity = sunLightIntensityCurve.Evaluate(CGTime);
			if (useSunColorGradient) SunColor               = sunColorGradient.Evaluate(CGTime);
		}


		protected virtual void Moon()
		{


			// Evaluate moon curves and gradients.
			if (useMoonLightColorGradient)MoonLightColor      = moonLightColorGradient.Evaluate(CGTime);
			if (useMoonLightIntensityCurve)MoonLightIntensity = moonLightIntensityCurve.Evaluate(CGTime); 

			//---------------------------------------------------------------------------------------


			switch (moonRotationMode)
			{

			case MoonRotationMode.Automatic:

				m_MoonTransform.parent        = m_SunTransform;
				m_MoonTransform.localRotation = Quaternion.Euler(0, 180f, -180f);
				m_MoonTransform.localScale    = new Vector3(-1, 1, 1);

			break;

			case MoonRotationMode.Custom:

				if (useMoonYawCurve)MoonYaw      = moonYawCurve.Evaluate (CGTime);
				if (useMoonPitchCurve)MoonPitch  = moonPitchCurve.Evaluate (CGTime);

				m_MoonTransform.localEulerAngles = new Vector3(MoonPitch, MoonYaw, 0f);
				m_MoonTransform.localScale       = new Vector3(-1, 1, 1);
				m_MoonTransform.parent           = this.transform;

			break;

			}

			//--------------------------------------------------------------------------------------
		}


		protected void DirLight()
		{


			if(m_DirectionalLight == null) return;

			if (IsSunLight) 
			{
				m_DirectionalLightTransform.parent           = this.transform;
				m_DirectionalLightTransform.localEulerAngles = m_SunTransform.localEulerAngles;
				m_DirectionalLight.intensity                 = SunLightIntensity;
				m_DirectionalLight.color                     = SunColor;

			} 
			else if (useMoon) 
			{

				switch (moonRotationMode)
				{

				case MoonRotationMode.Automatic:

					m_DirectionalLightTransform.parent        = m_SunTransform;
					m_DirectionalLightTransform.localRotation = Quaternion.Euler(0, 180f, -180f);

				break;

				case MoonRotationMode.Custom:

					m_DirectionalLightTransform.localEulerAngles = m_MoonTransform.localEulerAngles;
					m_DirectionalLightTransform.parent           = this.transform;

				break;

				}


				m_DirectionalLight.intensity =  MoonLightIntensity;
				m_DirectionalLight.color     =  MoonLightColor;
			}
		}


		protected void UpdateAmbient()
		{


			// Ambient

			// Evaluate ambient curves and gradients.
			if (useAmbientSkyColorGradient) AmbientSkyColor         = ambientSkyColorGradient.Evaluate(CGTime);
			if (useAmbientEquatorColorGradient) AmbientEquatorColor = ambientEquatorColorGradient.Evaluate(CGTime);
			if (useAmbientGroundColorGradient) AmbientGroundColor   = ambientGroundColorGradient.Evaluate(CGTime);
			if (useAmbientIntensityCurve) AmbientIntensity          = ambientIntensityCurve.Evaluate(CGTime);
			//------------------------------------------------------------------------------------------------------

			switch (m_AmbientMode) 
			{

			case AmbientMode.Skybox:

				RenderSettings.ambientMode      = UnityEngine.Rendering.AmbientMode.Skybox;
				RenderSettings.ambientIntensity = AmbientIntensity;

			break;

			case AmbientMode.Color: 

				RenderSettings.ambientMode     = UnityEngine.Rendering.AmbientMode.Flat;
				RenderSettings.ambientSkyColor = AmbientSkyColor;

			break;

			case AmbientMode.Gradient:

				RenderSettings.ambientMode         = UnityEngine.Rendering.AmbientMode.Trilight;
				RenderSettings.ambientSkyColor     =  AmbientSkyColor; 
				RenderSettings.ambientEquatorColor =  AmbientEquatorColor;
				RenderSettings.ambientGroundColor  =  AmbientGroundColor;

			break;

			}


			// Fog

			if (fogType == FogType.Off && fogType != FogType.EvaluateOnly) return;
			//-----------------------------------------------------------------------------------------

			// Evaluate fog curves and gradients.
			if (useFogDensityCurve) FogDensity             = fogDensityCurve.Evaluate(CGTime);
			if (useFogStartDistanceCurve) FogStartDistance = fogStartDistanceCurve.Evaluate(CGTime);
			if (useFogEndDistanceCurve) FogEndDistance     = fogEndDistanceCurve.Evaluate(CGTime);
			if (useFogColorGradient) FogColor              = fogColorGradient.Evaluate(CGTime);
			//-----------------------------------------------------------------------------------------


			if (fogType == FogType.RenderSettings) 
			{

				RenderSettings.fog      = useRenderSettingsFog;
				RenderSettings.fogMode  = fogMode;
				RenderSettings.fogColor = FogColor;

				switch (fogMode) 
				{

				case FogMode.Exponential:
					RenderSettings.fogDensity = FogDensity;
				break;

				case FogMode.ExponentialSquared:
					RenderSettings.fogDensity = FogDensity;
				break;

				case FogMode.Linear:
					RenderSettings.fogStartDistance = FogStartDistance;
					RenderSettings.fogEndDistance = FogEndDistance;
				break;

				}
			}
		}



	}
}
