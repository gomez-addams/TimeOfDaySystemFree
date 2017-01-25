using UnityEngine;
using System.Collections;


namespace AC.TimeOfDaySystemFree
{
	[ExecuteInEditMode]
	public class TimeOfDayManager : TimeOfDay
	{


		#region Resources.

		// Autoassign sky material?.
		[SerializeField] protected bool m_AutoAssignSky = true;

		// Sky material.
		public Material skyMaterial = null;

		// Moon.
		public Texture2D  moonTexture = null;
		//----------------------------------------------------------------------------------------

		// Stars cubemap.
		public  Cubemap starsCubemap = null;

		// Stars noise cubemap.
		public  Cubemap starsNoiseCubemap = null;
		//----------------------------------------------------------------------------------------

		#endregion

		#region Atmosphere.

		public bool useSkyTintGradient = false;

		[SerializeField]
		protected Color m_SkyTint = new Color(.5f, .5f, .5f, 1f); 

		public Gradient skyTintGradient = new Gradient()
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

		// Get sky tint.
		public Color SkyTint
		{
			get{ return m_SkyTint;  }
			set{ m_SkyTint = value; }
		}
		//----------------------------------------------------------------------------------------

		public bool useAtmosphereThicknessCurve = false;

		[SerializeField]
		[Range(0,5f)]private float m_AtmosphereThickness = 1;

		public AnimationCurve atmosphereThicknessCurve = AnimationCurve.Linear(0, 1f, 1f, 1f); 

		// Get atmosphere thickness.
		public float AtmosphereThickness
		{
			get{ return m_AtmosphereThickness;  }
			set{ m_AtmosphereThickness = value; }
		}
		//----------------------------------------------------------------------------------------


		// Ground skybox color.
		public Color groundColor = new Color(.412f, .384f, .365f, 1f); 
		//----------------------------------------------------------------------------------------

		// Night Color Type.
		public enum NightColorType{Simple, Atmospheric}
		public NightColorType nightColorType = NightColorType.Atmospheric;


		// Night color.
		public bool useNightColor = true;

		public bool useNightColorGradient = false;

		[SerializeField]
		protected Color m_NightColor = new Color(.0f, .070f, .167f, 1f);

		public Gradient nightColorGradient = new Gradient ()
		{
			colorKeys = new GradientColorKey[]
			{
				new GradientColorKey(new Color(.0f, .070f, .167f, 1f), .20f),
				new GradientColorKey(new Color( 0f,    0f,    0f, 1f), .30f),
				new GradientColorKey(new Color( 0f,    0f,    0f, 1f), .70f),
				new GradientColorKey(new Color(.0f, .070f, .167f, 1f), .80f)
			},
			alphaKeys = new GradientAlphaKey[] 
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(1f, 1f)
			}
		};

		// Get night horizon color.
		public Color NightColor
		{
			get{ return m_NightColor;  }
			set{ m_NightColor = value; }
		}
		//----------------------------------------------------------------------------------------

		public bool useHorizonFade = true;

		[SerializeField]
		[Range(.03f,2f)]private float m_HorizonFade = 1.5f;

		public float HorizonFade
		{
			get{ return m_HorizonFade;  }  
			set{ m_HorizonFade = value; } 
		}
		#endregion

		#region Sun

		public enum SunType{MiePhase, Spot}
		public SunType sunType = SunType.MiePhase;

		[SerializeField]
		[Range(0,1f)] private float m_SunSize = .045f;

		public bool useSunSizeCurve = false;

		public AnimationCurve sunSizeCurve = AnimationCurve.Linear(0, .07f, 1f, .07f);

		// Get sun size.
		public float SunSize
		{
			get{ return m_SunSize;  }
			set{ m_SunSize = value; }
		}
		//----------------------------------------------------------------------------------------

		#endregion

		#region Moon

		public bool useMoonColorGradient = false;

		[SerializeField]
		protected Color m_MoonColor = Color.white;

		public Gradient moonColorGradient = new Gradient()
		{
			colorKeys = new GradientColorKey[]
			{
				new GradientColorKey(new Color(1f, 1f, 1f, 1f), 0f),
				new GradientColorKey(new Color(1f, 1f, 1f, 1f), 1f)
			},
			alphaKeys = new GradientAlphaKey[] 
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(1f, 1f)
			}
		};


		// Get moon color .
		public Color MoonColor
		{
			get{ return m_MoonColor;  }
			set{ m_MoonColor = value; }
		}
		//---------------------------------------------------------------------------------------


		public bool useMoonIntensityCurve = true;

		[SerializeField]
		[Range(0,5f)] protected float m_MoonIntensity = 1;

		public AnimationCurve moonIntensityCurve = new AnimationCurve()
		{
			keys = new Keyframe[]
			{
				new Keyframe(  0f, 1f),  new Keyframe(.15f, 1f), 
				new Keyframe(.30f, 0f),  new Keyframe(.70f, 0f), 
				new Keyframe(.85f, 1f),  new Keyframe(  1f, 1f), 
			}

		}; 

		// Get moon intensity .
		public float MoonIntensity 
		{ 
			get{ return m_MoonIntensity;  } 
			set{ m_MoonIntensity = value; } 
		}
		//---------------------------------------------------------------------------------------

		public bool useMoonSizeCurve = false;

		[SerializeField]
		[Range(0,1f)]protected float m_MoonSize = .096f;

		public AnimationCurve moonSizeCurve =  AnimationCurve.Linear (0, .096f, 1f, .096f); 

		// Get moon color .
		public float MoonSize
		{
			get{ return m_MoonSize;  }
			set{ m_MoonSize = value; }
		}
		//---------------------------------------------------------------------------------------

		public bool useMoonHalo = true;

		public bool useMoonHaloColorGradient;

		[SerializeField]
		protected Color m_MoonHaloColor = new Color (0.008f, 0.110f, .275f, 1f);

		public Gradient moonHaloColorGradient = new Gradient()
		{
			colorKeys = new GradientColorKey[]
			{
				new GradientColorKey(new Color(0.008f, 0.110f, .275f, 1f), .20f),
				new GradientColorKey(new Color(0.008f, 0.110f, .275f, 1f), .80f)
			},
			alphaKeys = new GradientAlphaKey[] 
			{
				new GradientAlphaKey(1f, .20f),
				new GradientAlphaKey(1f, .70f)
			}
		};

		// Get moon halo color.
		public Color MoonHaloColor
		{ 
			get{ return m_MoonHaloColor;  } 
			set{ m_MoonHaloColor = value; } 
		}
		//---------------------------------------------------------------------------------------

		public bool useMoonHaloSizeCurve;

		[SerializeField]
		[Range(0, 10f)]private float m_MoonHaloSize = 3f;

		public AnimationCurve moonHaloSizeCurve  =  AnimationCurve.Linear (0, 3f, 1f, 3f);

		// Get moon halo size.
		public float MoonHaloSize
		{ 
			get{ return m_MoonHaloSize;  } 
			set{ m_MoonHaloSize = value; } 
		}
		//---------------------------------------------------------------------------------------

		public bool useMoonHaloIntensityCurve = true;

		[SerializeField]
		[Range(0, 5f)]private float m_MoonHaloIntensity = 1f;

		public AnimationCurve moonHaloIntensityCurve = new AnimationCurve()
		{
			keys = new Keyframe[]
			{
				new Keyframe(  0f, 1f),  new Keyframe(.15f, 1f), 
				new Keyframe(.25f, 0f),  new Keyframe(.75f, 0f), 
				new Keyframe(.85f, 1f),  new Keyframe(  1f, 1f), 
			}

		};  

		// Get moon halo intensity.
		public float MoonHaloIntensity
		{ 
			get{ return m_MoonHaloIntensity;  } 
			set{ m_MoonHaloIntensity = value; } 
		}
		//---------------------------------------------------------------------------------------

		// Moon direction.
		public Vector3 MoonDirection{get{return -m_MoonTransform.forward;}}

		// Moon ligth state.
		public bool MoonLightEnable{ get; set; }
		//---------------------------------------------------------------------------------------

		#endregion

		#region Stars

		public bool useStars = true;

		public enum StarsRotationMode{Automatic, Static}
		public StarsRotationMode starsRotationMode = StarsRotationMode.Automatic;
		//--------------------------------------------------------------------------------------

		// Outer space offsets.
		public Vector3 starsOffsets = Vector3.zero;
		//--------------------------------------------------------------------------------------

		public bool useStarsColorGradient;

		[SerializeField]
		protected Color m_StarsColor = Color.white;

		public Gradient starsColorGradient = new Gradient()
		{
			colorKeys = new GradientColorKey[]
			{
				new GradientColorKey(new Color(1f, 1f, 1f, 1f), .20f),
				new GradientColorKey(new Color(1f, 1f, 1f, 1f), .80f)
			},
			alphaKeys = new GradientAlphaKey[] 
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(1f, 1f)
			}
		};

		// Get stars Color.
		public Color StarsColor
		{
			get{ return m_StarsColor;  } 
			set{ m_StarsColor = value; } 
		}
		//--------------------------------------------------------------------------------------

		public bool useStarsIntensityCurve = true;

		[SerializeField]
		[Range(0,5f)] private float m_StarsIntensity = 1;

		public AnimationCurve starsIntensityCurve = new AnimationCurve()
		{
			keys = new Keyframe[]
			{
				new Keyframe(  0f, 1f),  new Keyframe(.20f, 1f), 
				new Keyframe(.25f, 0f),  new Keyframe(.75f, 0f), 
				new Keyframe(.80f, 1f),  new Keyframe(  1f, 1f), 
			}
		};  

		public float StarsIntensity
		{
			get{ return m_StarsIntensity;  } 
			set{ m_StarsIntensity = value; } 
		}
		//--------------------------------------------------------------------------------------

		public bool useStarsTwinkle = true;

		public bool useStarsTwinkleCurve;

		[SerializeField]
		[Range(0,1f)]
		protected float m_StarsTwinkle = .4f;

		public AnimationCurve starsTwinkleCurve = AnimationCurve.Linear (0, .4f, 1f, .4f); 

		public float StarsTwinkle
		{
			get{ return m_StarsTwinkle;  } 
			set{ m_StarsTwinkle = value; } 
		}

		public bool useStarsTwinkleSpeedCurve;

		[SerializeField]
		[Range(0,10f)]
		protected float m_StarsTwinkleSpeed = 7;

		public AnimationCurve starsTwinkleSpeedCurve =  AnimationCurve.Linear (0, 71f, 1f, 7f);

		public float StarsTwinkleSpeed
		{
			get{ return m_StarsTwinkleSpeed;  } 
			set{ m_StarsTwinkleSpeed = value; } 
		}

		private float starsTwinkleSpeed;
		//--------------------------------------------------------------------------------------

		#endregion

		#region Other Settings

		//--------------------------------------------------------------------------------------
		[SerializeField]protected float m_Exposure = 1.3f;
		public  bool useExposureCurve = false;
		public  AnimationCurve exposureCurve = AnimationCurve.Linear(0, 1.3f, 0, 1.3f);

		public float Exposure
		{
			get{ return m_Exposure;  }  
			set{ m_Exposure = value; } 
		}
		//---------------------------------------------------------------------------------------
		#endregion




		void Start()
		{
			if (Application.isPlaying)
			{
				Init();
			}
		}


		void Update()
		{

			if (skyMaterial == null) return;

			#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				Init(); 
				timeline = Mathf.Clamp (timeline, 0 - .0001f, k_DayDuration + .0001f);
			}
			#endif

			UpdateTime ();

			Atmosphere ();

			Sun ();

			Moon ();

			DirLight ();

			UpdateAmbient();
	
			Stars ();

			// Exposure.
			if (useExposureCurve) Exposure = exposureCurve.Evaluate (CGTime);
			skyMaterial.SetFloat ("_Exposure", Exposure);
		

		}



		protected override void Init()
		{
			base.Init ();

			// Set sky material.
			if (m_AutoAssignSky)
				RenderSettings.skybox = skyMaterial;

		}
			



		void Atmosphere()
		{


			// Atmosphere.

			// Evaluate sky tint curves and gradients.
			if (useSkyTintGradient) SkyTint                      = skyTintGradient.Evaluate(CGTime);
			if (useAtmosphereThicknessCurve) AtmosphereThickness = atmosphereThicknessCurve.Evaluate(CGTime);
			//------------------------------------------------------------------------------------------------


			skyMaterial.SetColor("_SkyTint", SkyTint);
			skyMaterial.SetFloat("_AtmosphereThickness", AtmosphereThickness);
			skyMaterial.SetColor("_GroundColor", groundColor);
			//------------------------------------------------------------------------------------------------

			if (!useNightColor)
			{
				skyMaterial.DisableKeyword("ATMOSPHERICNIGHTCOLOR");
				skyMaterial.DisableKeyword("SIMPLENIGHTCOLOR");
			}
			else 
			{

				if (useNightColorGradient) NightColor = nightColorGradient.Evaluate(CGTime);
				//-------------------------------------------------------------------------------------------

				if (nightColorType == NightColorType.Atmospheric)
				{
					skyMaterial.DisableKeyword("SIMPLENIGHTCOLOR");
					skyMaterial.EnableKeyword ("ATMOSPHERICNIGHTCOLOR");
				} 
				else 
				{
					skyMaterial.DisableKeyword("ATMOSPHERICNIGHTCOLOR");
					skyMaterial.EnableKeyword ("SIMPLENIGHTCOLOR");
				}

				skyMaterial.SetColor("_NightColor", NightColor); 
			}

			if (!useHorizonFade) 
			{
				skyMaterial.DisableKeyword ("HORIZONFADE");
			}
			else
			{
				skyMaterial.EnableKeyword ("HORIZONFADE"); 
				skyMaterial.SetFloat("_HorizonFade", HorizonFade);
			}
		}


		protected override void Sun()
		{


			if (m_SunTransform != null) 
			{
				
				base.Sun ();

				if (useSunSizeCurve)
					SunSize = sunSizeCurve.Evaluate (CGTime);


				if (sunType == SunType.MiePhase)
					skyMaterial.EnableKeyword ("MIEPHASE");
				else
					skyMaterial.DisableKeyword ("MIEPHASE");

				skyMaterial.SetFloat ("_SunSize", SunSize);
				skyMaterial.SetColor ("_SunColor", SunColor); 
				skyMaterial.SetVector ("_SunDir", SunDirection);

			}
		}

		protected override void Moon()
		{

			if (!useMoon || m_MoonTransform == null || moonTexture == null)
			{
				
				skyMaterial.DisableKeyword("MOONHALO");
				skyMaterial.DisableKeyword("MOON");

				return;
			}

			base.Moon ();
			//------------------------------------------------------------------------------

			skyMaterial.EnableKeyword("MOON");

			// Evaluate moon curves and gradients.
			if(useMoonColorGradient) MoonColor      = moonColorGradient.Evaluate(CGTime);
			if(useMoonSizeCurve) MoonSize           = moonSizeCurve.Evaluate(CGTime);
			if(useMoonIntensityCurve) MoonIntensity = moonIntensityCurve.Evaluate(CGTime); 
			//------------------------------------------------------------------------------


			skyMaterial.SetMatrix ("_MoonMatrix",  m_MoonTransform.worldToLocalMatrix);
			skyMaterial.SetTexture ("_MoonTexture", moonTexture);
			skyMaterial.SetFloat ("_MoonSize", MoonSize);
			skyMaterial.SetColor ("_MoonColor", MoonColor);
			skyMaterial.SetFloat ("_MoonIntensity", MoonIntensity);
			//-------------------------------------------------------------------------------


			// Moon Halo.
			if (!useMoonHalo) 
			{
				skyMaterial.DisableKeyword("MOONHALO");

				return;
			}


			// Evaluate moon halo curves and gradients.
			if (useMoonHaloColorGradient) MoonHaloColor      = moonHaloColorGradient.Evaluate(CGTime);
			if (useMoonHaloSizeCurve) MoonHaloSize           = moonHaloSizeCurve.Evaluate(CGTime);
			if (useMoonHaloIntensityCurve) MoonHaloIntensity = moonHaloIntensityCurve.Evaluate(CGTime);


			skyMaterial.SetVector("_MoonDir", MoonDirection);
			skyMaterial.EnableKeyword("MOONHALO"); 
			skyMaterial.SetColor("_MoonHaloColor", MoonHaloColor); 
			skyMaterial.SetFloat("_MoonHaloSize", MoonHaloSize); 
			skyMaterial.SetFloat("_MoonHaloIntensity", MoonHaloIntensity); 

		}

		void Stars()
		{


			if ((starsCubemap == null && starsNoiseCubemap == null) || !useStars)
			{
				
				skyMaterial.DisableKeyword("STARS");
				skyMaterial.DisableKeyword("STARSTWINKLE");

				return;

			}


			skyMaterial.EnableKeyword ("STARS"); 

			// Get Matrix.
			Matrix4x4 sunMatrix = starsRotationMode == StarsRotationMode.Automatic ? SunMatrix : Matrix4x4.identity; 
			Matrix4x4 starsMatrix = Matrix4x4.TRS (Vector3.zero, Quaternion.Euler (starsOffsets), Vector3.one); 

			// Set Matrix.
			skyMaterial.SetMatrix ("_SunMatrix", sunMatrix);
			skyMaterial.SetMatrix ("_StarsMatrix", starsMatrix);


			// Evaluate stars curves and gradients.
			if (useStarsColorGradient) StarsColor            = starsColorGradient.Evaluate (CGTime); 
			if (useStarsIntensityCurve) StarsIntensity       = starsIntensityCurve.Evaluate (CGTime); 
			if (useStarsTwinkleCurve) StarsTwinkle           = starsTwinkleCurve.Evaluate (CGTime); 
			if (useStarsTwinkleSpeedCurve) StarsTwinkleSpeed = starsTwinkleSpeedCurve.Evaluate (CGTime);


			skyMaterial.SetTexture ("_StarsCubemap", starsCubemap);
			skyMaterial.SetColor ("_StarsColor", StarsColor);
			skyMaterial.SetFloat ("_StarsIntensity", StarsIntensity);
			//------------------------------------------------------------------------------------------------------------------


			// Stars twinkle. 
			if (!useStarsTwinkle || starsNoiseCubemap == null) 
			{
				
				skyMaterial.DisableKeyword ("STARSTWINKLE");

				return;
			}

			skyMaterial.EnableKeyword ("STARSTWINKLE"); 

			// Get stars twinkle speed.
			starsTwinkleSpeed += Time.deltaTime * StarsTwinkleSpeed; 

			// Get noise matrix.
			Matrix4x4 starsNoiseMatrix = Matrix4x4.TRS (Vector3.zero, Quaternion.Euler (starsTwinkleSpeed, 0, 0), Vector3.one); 


			skyMaterial.SetTexture ("_StarsNoiseCubemap", starsNoiseCubemap); 
			skyMaterial.SetMatrix ("_StarsNoiseMatrix", starsNoiseMatrix); 
			skyMaterial.SetFloat ("_StarsTwinkle", StarsTwinkle); 

		}
			
	}
}
