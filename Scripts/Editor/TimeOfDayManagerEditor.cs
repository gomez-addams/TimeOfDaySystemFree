using UnityEngine;
using System.Collections;

using UnityEditor;
using AC.TimeOfDaySystemFree;
using AC.CustomEditor;


[CustomEditor(typeof(TimeOfDayManager))] 
public class TimeOfDayManagerEditor : AC_CustomEditor
{

	SerializedObject serObject; 
	TimeOfDayManager timeOfDayManager;
	//-----------------------------------------------------

	#region SerializeProperties

	// Resources.
	SerializedProperty autoAssignSky;
	SerializedProperty skyMaterial;

	SerializedProperty directionalLight;
	SerializedProperty sunTransform;
	SerializedProperty moonTransform;

	SerializedProperty moonTexture;
	SerializedProperty starsCubemap;
	SerializedProperty starsNoiseCubemap;
	//-----------------------------------------------------


	// World and time.
	SerializedProperty playTime;
	SerializedProperty worldLongitude;
	SerializedProperty useWorldLongitudeCurve;
	SerializedProperty worldLongitudeCurve;
	SerializedProperty dayInSeconds;
	SerializedProperty timeline;
	//-----------------------------------------------------


	// Sun.

	SerializedProperty sunType;

	SerializedProperty useSunColorGradient;
	SerializedProperty sunColor;
	SerializedProperty sunColorGradient;
	SerializedProperty useSunSizeCurve;
	SerializedProperty sunSize;
	SerializedProperty sunSizeCurve;
	SerializedProperty useSunLightIntensityCurve;
	SerializedProperty sunLightIntensity;
	SerializedProperty sunLightIntensityCurve;
	//-----------------------------------------------------

	// Atmosphere
	SerializedProperty useSkyTintGradient;
	SerializedProperty skyTint;
	SerializedProperty skyTintGradient;

	SerializedProperty useAtmosphereThicknessCurve;
	SerializedProperty atmosphereThickness;
	SerializedProperty atmosphereThicknessCurve;

	SerializedProperty groundColor;

	SerializedProperty useNightColor;
	SerializedProperty nightColorType;
	SerializedProperty useNightColorGradient;
	SerializedProperty nightColor;
	SerializedProperty nightColorGradient;

	SerializedProperty useHorizonFade;
	SerializedProperty horizonFade;
	//-----------------------------------------------------

	// Moon.
	SerializedProperty useMoon;

	SerializedProperty moonRotationMode;

	SerializedProperty moonYaw;
	SerializedProperty useMoonYawCurve;
	SerializedProperty moonYawCurve;

	SerializedProperty moonPitch;
	SerializedProperty useMoonPitchCurve;
	SerializedProperty moonPitchCurve;

	SerializedProperty useMoonLightColorGradient;
	SerializedProperty moonLightColor;
	SerializedProperty moonLightColorGradient;
	SerializedProperty useMoonLightIntensityCurve;
	SerializedProperty moonLightIntensity;
	SerializedProperty moonLightIntensityCurve;

	SerializedProperty useMoonColorGradient;
	SerializedProperty moonColor;
	SerializedProperty moonColorGradient;

	SerializedProperty useMoonIntensityCurve;
	SerializedProperty moonIntensity;
	SerializedProperty moonIntensityCurve;

	SerializedProperty useMoonSizeCurve;
	SerializedProperty moonSize;
	SerializedProperty moonSizeCurve;


	//------------------------------------------------
	SerializedProperty useMoonHalo;
	SerializedProperty useMoonHaloColorGradient;
	SerializedProperty moonHaloColor;
	SerializedProperty moonHaloColorGradient;

	SerializedProperty useMoonHaloSizeCurve;
	SerializedProperty moonHaloSize;
	SerializedProperty moonHaloSizeCurve;

	SerializedProperty useMoonHaloIntensityCurve;
	SerializedProperty moonHaloIntensity;
	SerializedProperty moonHaloIntensityCurve;
	//-----------------------------------------------------

	// Stars.
	SerializedProperty useStars;
	SerializedProperty starsRotationMode;
	SerializedProperty starsOffsets;
	SerializedProperty useStarsColorGradient;
	SerializedProperty starsColor;
	SerializedProperty starsColorGradient;
	SerializedProperty useStarsIntensityCurve;

	SerializedProperty starsIntensity;
	SerializedProperty starsIntensityCurve;

	SerializedProperty useStarsTwinkle;
	SerializedProperty useStarsTwinkleCurve;
	SerializedProperty starsTwinkle;
	SerializedProperty starsTwinkleCurve;
	SerializedProperty useStarsTwinkleSpeedCurve;
	SerializedProperty starsTwinkleSpeed;
	SerializedProperty starsTwinkleSpeedCurve;
	//-----------------------------------------------------


	// Ambient.
	SerializedProperty ambientMode;
	SerializedProperty useAmbientSkyColorGradient;
	SerializedProperty ambientSkyColor;
	SerializedProperty ambientSkyColorGradient;
	SerializedProperty useAmbientEquatorColorGradient;
	SerializedProperty ambientEquatorColor;
	SerializedProperty ambientEquatorColorGradient;

	SerializedProperty useAmbientGroundColorGradient;
	SerializedProperty ambientGroundColor;
	SerializedProperty ambientGroundColorGradient;
	SerializedProperty useAmbientIntensityCurve;

	SerializedProperty ambientIntensity;
	SerializedProperty ambientIntensityCurve;
	//-----------------------------------------------------


	// fog.
	SerializedProperty fogType;
	SerializedProperty fogMode;
	SerializedProperty useRenderSettingsFog;
	SerializedProperty useFogDensityCurve;

	SerializedProperty fogDensity;
	SerializedProperty fogDensityCurve;
	SerializedProperty useFogStartDistanceCurve;
	SerializedProperty fogStartDistance;

	SerializedProperty fogStartDistanceCurve;

	SerializedProperty useFogEndDistanceCurve;

	SerializedProperty fogEndDistance;
	SerializedProperty fogEndDistanceCurve;

	SerializedProperty useFogColorGradient;
	SerializedProperty fogColor;
	SerializedProperty fogColorGradient;
	//-----------------------------------------------------


	// Other Settings.
	SerializedProperty exposure;
	SerializedProperty useExposureCurve;
	SerializedProperty exposureCurve;
	//-----------------------------------------------------

	#endregion

	#region foldouts
	//-----------------------------------------
	bool m_ResourcesFoldout;
	bool m_WorldAndTimeFoldout;
	bool m_SunFoldout;
	bool m_AtmosphereFoldout;
	bool m_MoonFoldout;
	bool m_StarsFoldout;
	bool m_AmbientFoldout;
	bool m_FogFoldout;
	bool m_OtherSettingsFoldout;
	//_________________________________________
	#endregion

	GUIStyle textTitleStyle
	{

		get 
		{

			GUIStyle style = new GUIStyle (EditorStyles.label); 
			style.fontStyle = FontStyle.Bold;
			style.fontSize = 12;

			return style;
		}
	}

	GUIStyle miniTextStyle
	{

		get 
		{
			GUIStyle style = new GUIStyle(EditorStyles.label); 
			style.fontStyle = FontStyle.Bold;
			style.fontSize = 8;

			return style;
		}
	}
		
//	Color WhiteColor { get { return Color.white; } }

	void OnEnable()
	{

		serObject = new SerializedObject (target);
		//timeOfDayManager = (TimeOfDayManager)target;
		//---------------------------------------------------------------------------------------------------

		// Resources and components.

		autoAssignSky     = serObject.FindProperty ("m_AutoAssignSky");
		skyMaterial       = serObject.FindProperty ("skyMaterial");
		directionalLight  = serObject.FindProperty ("m_DirectionalLight");
		sunTransform      = serObject.FindProperty ("m_SunTransform");
		moonTransform     = serObject.FindProperty ("m_MoonTransform");

		moonTexture       = serObject.FindProperty ("moonTexture");
		starsCubemap      = serObject.FindProperty ("starsCubemap");
		starsNoiseCubemap = serObject.FindProperty ("starsNoiseCubemap");
		//---------------------------------------------------------------------------------------------------

		// World and time.
		playTime               = serObject.FindProperty ("playTime");
		worldLongitude         = serObject.FindProperty ("m_WorldLongitude");
		useWorldLongitudeCurve = serObject.FindProperty ("useWorldLongitudeCurve");
		worldLongitudeCurve    = serObject.FindProperty ("worldLongitudeCurve");
		dayInSeconds           = serObject.FindProperty ("dayInSeconds");
		timeline               = serObject.FindProperty ("timeline");
		//---------------------------------------------------------------------------------------------------

		// Sun.
		sunType                       = serObject.FindProperty ("sunType");
		useSunColorGradient           = serObject.FindProperty ("useSunColorGradient");
		sunColor                      = serObject.FindProperty ("m_SunColor");
		sunColorGradient              = serObject.FindProperty ("sunColorGradient");
		useSunSizeCurve               = serObject.FindProperty ("useSunSizeCurve");
		sunSize                       = serObject.FindProperty ("m_SunSize");
		sunSizeCurve                  = serObject.FindProperty ("sunSizeCurve");
		useSunLightIntensityCurve     = serObject.FindProperty ("useSunLightIntensityCurve");
		sunLightIntensity             = serObject.FindProperty ("m_SunLightIntensity");
		sunLightIntensityCurve        = serObject.FindProperty ("sunLightIntensityCurve");
		//---------------------------------------------------------------------------------------------------

		// Atmosphere.
		useSkyTintGradient                 = serObject.FindProperty ("useSkyTintGradient");
		skyTint                            = serObject.FindProperty ("m_SkyTint");
		skyTintGradient                    = serObject.FindProperty ("skyTintGradient");
		useAtmosphereThicknessCurve        = serObject.FindProperty ("useAtmosphereThicknessCurve");
		atmosphereThickness                = serObject.FindProperty ("m_AtmosphereThickness");
		atmosphereThicknessCurve           = serObject.FindProperty ("atmosphereThicknessCurve");
		groundColor                        = serObject.FindProperty ("groundColor");

		useNightColor                      = serObject.FindProperty ("useNightColor");
		nightColorType 					   = serObject.FindProperty ("nightColorType");
		useNightColorGradient              = serObject.FindProperty ("useNightColorGradient");
		nightColor                         = serObject.FindProperty ("m_NightColor");
		nightColorGradient                 = serObject.FindProperty ("nightColorGradient");
		useHorizonFade                     = serObject.FindProperty ("useHorizonFade");
		horizonFade                        = serObject.FindProperty ("m_HorizonFade");
		//---------------------------------------------------------------------------------------------------

		// Use moon.
		useMoon                       = serObject.FindProperty ("useMoon");
		moonRotationMode              = serObject.FindProperty ("moonRotationMode");

		moonYaw                    = serObject.FindProperty ("m_MoonYaw");
		useMoonYawCurve            = serObject.FindProperty ("useMoonYawCurve");
		moonYawCurve               = serObject.FindProperty ("moonYawCurve");

		moonPitch                  = serObject.FindProperty ("m_MoonPitch");
		useMoonPitchCurve          = serObject.FindProperty ("useMoonPitchCurve");
		moonPitchCurve             = serObject.FindProperty ("moonPitchCurve");

		useMoonLightColorGradient     = serObject.FindProperty ("useMoonLightColorGradient");
		moonLightColor                = serObject.FindProperty ("m_MoonLightColor");
		moonLightColorGradient        = serObject.FindProperty ("moonLightColorGradient");
		useMoonLightIntensityCurve    = serObject.FindProperty ("useMoonLightIntensityCurve");
		moonLightIntensity            = serObject.FindProperty ("m_MoonLightIntensity");
		moonLightIntensityCurve       = serObject.FindProperty ("moonLightIntensityCurve");
		useMoonColorGradient          = serObject.FindProperty ("useMoonColorGradient");
		moonColor                     = serObject.FindProperty ("m_MoonColor");
		moonColorGradient             = serObject.FindProperty ("moonColorGradient");
		useMoonIntensityCurve         = serObject.FindProperty ("useMoonIntensityCurve");
		moonIntensity                 = serObject.FindProperty ("m_MoonIntensity");
		moonIntensityCurve            = serObject.FindProperty ("moonIntensityCurve");
		useMoonSizeCurve              = serObject.FindProperty ("useMoonSizeCurve");
		moonSize                      = serObject.FindProperty ("m_MoonSize");
		moonSizeCurve                 = serObject.FindProperty ("moonSizeCurve");

	
		useMoonHalo                   = serObject.FindProperty ("useMoonHalo");
		useMoonHaloColorGradient      = serObject.FindProperty ("useMoonHaloColorGradient");
		moonHaloColor                 = serObject.FindProperty ("m_MoonHaloColor");
		moonHaloColorGradient         = serObject.FindProperty ("moonHaloColorGradient");
		useMoonHaloSizeCurve          = serObject.FindProperty ("useMoonHaloSizeCurve");
		moonHaloSize                  = serObject.FindProperty ("m_MoonHaloSize");
		moonHaloSizeCurve             = serObject.FindProperty ("moonHaloSizeCurve");
		useMoonHaloIntensityCurve     = serObject.FindProperty ("useMoonHaloIntensityCurve");
		moonHaloIntensity             = serObject.FindProperty ("m_MoonHaloIntensity");
		moonHaloIntensityCurve        = serObject.FindProperty ("moonHaloIntensityCurve");
		//---------------------------------------------------------------------------------------------------

		// Stars.
		useStars                  = serObject.FindProperty ("useStars");
		starsRotationMode         = serObject.FindProperty ("starsRotationMode");
		starsOffsets              = serObject.FindProperty ("starsOffsets");
		useStarsColorGradient     = serObject.FindProperty ("useStarsColorGradient");
		starsColor                = serObject.FindProperty ("m_StarsColor");
		starsColorGradient        = serObject.FindProperty ("starsColorGradient");
		useStarsIntensityCurve    = serObject.FindProperty ("useStarsIntensityCurve");
		useStarsTwinkle           = serObject.FindProperty ("useStarsTwinkle");
		starsIntensity            = serObject.FindProperty ("m_StarsIntensity");
		starsIntensityCurve       = serObject.FindProperty ("starsIntensityCurve");
		useStarsTwinkleCurve      = serObject.FindProperty ("useStarsTwinkleCurve");
		starsTwinkle              = serObject.FindProperty ("m_StarsTwinkle");
		starsTwinkleCurve         = serObject.FindProperty ("starsTwinkleCurve");
		useStarsTwinkleSpeedCurve = serObject.FindProperty ("useStarsTwinkleSpeedCurve");
		starsTwinkleSpeed         = serObject.FindProperty ("m_StarsTwinkleSpeed");
		starsTwinkleSpeedCurve    = serObject.FindProperty ("starsTwinkleSpeedCurve");
		//---------------------------------------------------------------------------------------------------

		// Ambient.
		ambientMode                      = serObject.FindProperty ("m_AmbientMode");
		useAmbientSkyColorGradient       = serObject.FindProperty ("useAmbientSkyColorGradient");
		ambientSkyColor                  = serObject.FindProperty ("m_AmbientSkyColor");
		ambientSkyColorGradient          = serObject.FindProperty ("ambientSkyColorGradient");
		useAmbientEquatorColorGradient   = serObject.FindProperty ("useAmbientEquatorColorGradient");
		ambientEquatorColor              = serObject.FindProperty ("m_AmbientEquatorColor");
		ambientEquatorColorGradient      = serObject.FindProperty ("ambientEquatorColorGradient");
		useAmbientGroundColorGradient    = serObject.FindProperty ("useAmbientGroundColorGradient");
		ambientGroundColor               = serObject.FindProperty ("m_AmbientGroundColor");
		ambientGroundColorGradient       = serObject.FindProperty ("ambientGroundColorGradient");
		useAmbientIntensityCurve         = serObject.FindProperty ("useAmbientIntensityCurve");
		ambientIntensity                 = serObject.FindProperty ("m_AmbientIntensity");
		ambientIntensityCurve            = serObject.FindProperty ("ambientIntensityCurve");
		//---------------------------------------------------------------------------------------------------

		// fog.
		fogType                  = serObject.FindProperty ("fogType");
		fogMode                  = serObject.FindProperty ("fogMode");
		useRenderSettingsFog     = serObject.FindProperty ("useRenderSettingsFog");
		useFogDensityCurve       = serObject.FindProperty ("useFogDensityCurve");
		fogDensity               = serObject.FindProperty ("m_FogDensity");
		fogDensityCurve          = serObject.FindProperty ("fogDensityCurve");
		useFogStartDistanceCurve = serObject.FindProperty ("useFogStartDistanceCurve");
		fogStartDistance         = serObject.FindProperty ("m_FogStartDistance");
		fogStartDistanceCurve    = serObject.FindProperty ("fogStartDistanceCurve");
		useFogEndDistanceCurve   = serObject.FindProperty ("useFogEndDistanceCurve");
		fogEndDistance           = serObject.FindProperty ("m_FogEndDistance");
		fogEndDistanceCurve      = serObject.FindProperty ("fogEndDistanceCurve");
		useFogColorGradient      = serObject.FindProperty ("useFogColorGradient");
		fogColor                 = serObject.FindProperty ("m_FogColor");
		fogColorGradient         = serObject.FindProperty ("fogColorGradient");
		//---------------------------------------------------------------------------------------------------

		// Other settings.
		exposure             = serObject.FindProperty ("m_Exposure");
		useExposureCurve     = serObject.FindProperty ("useExposureCurve");
		exposureCurve        = serObject.FindProperty ("exposureCurve");
		//---------------------------------------------------------------------------------------------------
	}

	public override void OnInspectorGUI()
	{

		serObject.Update ();

		Separator (WhiteColor, 2);
		Text("Time of Day Manager", textTitleStyle, true);
		Separator (WhiteColor, 2);

		//---------------------------------------------
		ResourcesAndComponents(); 
		WorldAndTime();
		Atmosphere();
		Sun(); 
		Moon();
		Stars();   
		Ambient();  
		Fog();
		OtherSettings();
		//---------------------------------------------

		serObject.ApplyModifiedProperties();

	}
		
	void ResourcesAndComponents()
	{

		m_ResourcesFoldout = EditorGUILayout.Foldout (m_ResourcesFoldout, "Resources");

		if (m_ResourcesFoldout) 
		{

			Separator (WhiteColor, 2);
			Text ("Resources And Components", textTitleStyle, true);
			Separator (WhiteColor, 2);
			//---------------------------------------------------------------------------------------------------

			//EditorGUILayout.PropertyField(autoAssignSky, new GUIContent("Auto Assign Sky?"));

			autoAssignSky.boolValue = EditorGUILayout.Toggle ("Auto Assign Sky", autoAssignSky.boolValue, EditorStyles.radioButton);

			EditorGUILayout.PropertyField(skyMaterial, new GUIContent("Sky Material"));

			if (skyMaterial.objectReferenceValue == null)
			{
				EditorGUILayout.HelpBox ("Please Assign Sky Material", MessageType.Warning);
			}
			//---------------------------------------------------------------------------------------------------

			EditorGUILayout.PropertyField(sunTransform, new GUIContent("Sun Transform"));
			if (sunTransform.objectReferenceValue == null) 
			{
				EditorGUILayout.HelpBox ("Please Assign Sun Transform", MessageType.Warning);
			} 

			EditorGUILayout.PropertyField(moonTransform, new GUIContent("Moon Transform"));
			if (moonTransform.objectReferenceValue == null) 
			{
				EditorGUILayout.HelpBox ("Please Assign Moon Light", MessageType.Warning);
			} 

			EditorGUILayout.PropertyField(directionalLight, new GUIContent("Directional Light"));
			//---------------------------------------------------------------------------------------------------

			Separator (WhiteColor, 2);


			EditorGUILayout.BeginVertical(EditorStyles.textField);
			{


				moonTexture.objectReferenceValue = (Texture2D)EditorGUILayout.ObjectField ("Moon Texture", moonTexture.objectReferenceValue, typeof(Texture2D), true);

				if (moonTexture.objectReferenceValue == null) 
				{
					EditorGUILayout.HelpBox ("Please Assign Moon Texture", MessageType.Warning);
				} 

				//---------------------------------------------------------------------------------------------------

				//AC_UtilityEditor.Separator (WhiteColor, 2);

				starsCubemap.objectReferenceValue = (Cubemap)EditorGUILayout.ObjectField("Stars Cubemap", starsCubemap.objectReferenceValue, typeof(Cubemap),true);
				if (starsCubemap.objectReferenceValue == null) 
				{
					EditorGUILayout.HelpBox ("Please Assign Stars Cubemap", MessageType.Warning);
				}

				starsNoiseCubemap.objectReferenceValue = (Cubemap)EditorGUILayout.ObjectField("Stars Noise Cubemap", starsNoiseCubemap.objectReferenceValue, typeof(Cubemap),true);
				if (starsNoiseCubemap.objectReferenceValue == null) 
				{
					EditorGUILayout.HelpBox ("Please Assign Stars Noise Cubemap", MessageType.Warning);
				}
				//---------------------------------------------------------------------------------------------------
			}
			EditorGUILayout.EndVertical();

			Separator (WhiteColor, 2);
		}
	}

	void WorldAndTime()
	{

		m_WorldAndTimeFoldout = EditorGUILayout.Foldout (m_WorldAndTimeFoldout , "World And Time");
		if(m_WorldAndTimeFoldout)
		{

			Separator (WhiteColor, 2);
			Text ("World And Time", textTitleStyle, true);
			Separator (WhiteColor, 2);
			//---------------------------------------------------------------------------------------------------

			// World Longitude.
			EditorGUILayout.Separator ();
			EditorGUILayout.BeginHorizontal ();
			{

				if (useWorldLongitudeCurve.boolValue)
					CurveField ("Longitude", worldLongitudeCurve, Color.white, new Rect (0, 0, 1, 360f), 75);
				else
					EditorGUILayout.PropertyField (worldLongitude, new GUIContent ("Longitude"));

				ToggleButton (useWorldLongitudeCurve, "C");

			}
			EditorGUILayout.EndHorizontal ();
			//---------------------------------------------------------------------------------------------------

			// Play time.
			EditorGUILayout.Separator ();
			EditorGUILayout.BeginHorizontal ();
			{
				playTime.boolValue = EditorGUILayout.Toggle ("Play Time", playTime.boolValue, EditorStyles.radioButton);
				GUI.enabled = playTime.boolValue;

				Text ("Day in seconds", miniTextStyle, false, 75);
				EditorGUILayout.PropertyField (dayInSeconds, new GUIContent (""), GUILayout.Width (50));

				GUI.enabled = true;
			}
			EditorGUILayout.EndHorizontal ();
			//---------------------------------------------------------------------------------------------------

			EditorGUILayout.Separator ();
			EditorGUILayout.PropertyField(timeline, new GUIContent("Timeline"));

			//---------------------------------------------------------------------------------------------------
			Separator (WhiteColor, 2);
		}
	}

	void Atmosphere()
	{

		m_AtmosphereFoldout = EditorGUILayout.Foldout (m_AtmosphereFoldout, "Atmosphere");
		if (m_AtmosphereFoldout) 
		{

			Separator (WhiteColor, 2);
			Text ("Atmosphere", textTitleStyle, true);
			Separator (WhiteColor, 2);
			//---------------------------------------------------------------------------------------------------

			// Sky Tint.
			EditorGUILayout.Separator ();
			EditorGUILayout.BeginHorizontal ();
			{

				if (useSkyTintGradient.boolValue)
					ColorField (skyTintGradient, "Sky Tint", 75);
				else
					ColorField (skyTint, "Sky Tint", 75);

				ToggleButton (useSkyTintGradient, "G");
			}
			EditorGUILayout.EndHorizontal ();
			//---------------------------------------------------------------------------------------------------


			// Atmosphere Thickness.
			EditorGUILayout.BeginHorizontal ();
			{

				if (useAtmosphereThicknessCurve.boolValue)
					CurveField ("Atmosphere Thickness", atmosphereThicknessCurve, Color.white, new Rect (0, 0, 1, 7f), 75);
				else
					EditorGUILayout.PropertyField (atmosphereThickness, new GUIContent ("Atmosphere Thickness"));

				ToggleButton (useAtmosphereThicknessCurve, "C");
			}
			EditorGUILayout.EndHorizontal ();
			//---------------------------------------------------------------------------------------------------



			// Night color.
			EditorGUILayout.Separator ();
			useNightColor.boolValue = EditorGUILayout.Toggle ("Night Color", useNightColor.boolValue, EditorStyles.radioButton);
			GUI.enabled = useNightColor.boolValue;
			{

				EditorGUILayout.PropertyField (nightColorType, new GUIContent ("Night Color Type"));
		
				EditorGUILayout.BeginHorizontal ();
				{
					if (useNightColorGradient.boolValue)
						ColorField (nightColorGradient, "Night Color", 75);
					else
						ColorField (nightColor, "Night Color", 75);

					ToggleButton (useNightColorGradient, "G");
				}
				EditorGUILayout.EndHorizontal ();

			}
			GUI.enabled = true;
			//---------------------------------------------------------------------------------------------------

			// Horizon fade.
			EditorGUILayout.Separator();
			EditorGUILayout.BeginHorizontal ();
			{
				useHorizonFade.boolValue = EditorGUILayout.Toggle ("Horizon Fade", useHorizonFade.boolValue, EditorStyles.radioButton);
				GUI.enabled = useHorizonFade.boolValue;

				EditorGUILayout.PropertyField (horizonFade, new GUIContent (""));
				GUI.enabled = true;
			}
			EditorGUILayout.EndHorizontal ();
			//---------------------------------------------------------------------------------------------------
			Separator (WhiteColor, 2);
		}
	}

	void Sun()
	{

		m_SunFoldout = EditorGUILayout.Foldout (m_SunFoldout, "Sun");
		if(m_SunFoldout)
		{

			Separator (WhiteColor, 2);
			Text ("Sun", textTitleStyle, true);
			Separator (WhiteColor, 2);
			//---------------------------------------------------------------------------------------------------

			EditorGUILayout.PropertyField(sunType, new GUIContent("Sun Type"));

			// Sun Color.
			EditorGUILayout.BeginHorizontal ();
			{

				if (useSunColorGradient.boolValue)
					ColorField (sunColorGradient, "Sun Color", 75);
				else
					ColorField (sunColor, "Sun Color", 75);

				ToggleButton (useSunColorGradient, "G");
			}
			EditorGUILayout.EndHorizontal ();
			//---------------------------------------------------------------------------------------------------

			// Sun Size.
			EditorGUILayout.BeginHorizontal ();
			{

				if (useSunSizeCurve.boolValue)
					CurveField ("Sun Size", sunSizeCurve, Color.white, new Rect (0, 0, 1, 0.3f), 75);
				else
					EditorGUILayout.PropertyField (sunSize, new GUIContent ("Sun Size"));

				ToggleButton (useSunSizeCurve, "C");
			}
			EditorGUILayout.EndHorizontal ();
			//---------------------------------------------------------------------------------------------------

			// Sun Light Intensity.
			EditorGUILayout.BeginHorizontal ();
			{

				if (useSunLightIntensityCurve.boolValue)
					CurveField ("Sun Light Intensity", sunLightIntensityCurve, Color.white, new Rect (0, 0, 1, 8f), 75);
				else
					EditorGUILayout.PropertyField (sunLightIntensity, new GUIContent ("Sun Light Intensity"));

				ToggleButton (useSunLightIntensityCurve, "C");
			}
			EditorGUILayout.EndHorizontal ();

			//---------------------------------------------------------------------------------------------------
			Separator (WhiteColor, 2);
		}

	}

	void Moon()
	{

		m_MoonFoldout = EditorGUILayout.Foldout (m_MoonFoldout, "Moon");
		if (m_MoonFoldout)
		{
			Separator (WhiteColor, 2);
			Text ("Moon", textTitleStyle, true);
			Separator (WhiteColor, 2);
			//---------------------------------------------------------------------------------------------------

			useMoon.boolValue = EditorGUILayout.Toggle ("Use Moon", useMoon.boolValue, EditorStyles.radioButton);
			if(!useMoon.boolValue)return;

			EditorGUILayout.PropertyField (moonRotationMode, new GUIContent ("Moon Rotation Mode"));

			if (moonRotationMode.intValue != 0) 
			{

				// Moon Longitude.
				EditorGUILayout.BeginHorizontal ();
				{

					if (useMoonYawCurve.boolValue)
						CurveField ("Moon Yaw", moonYawCurve, Color.white, new Rect (0, 0, 1, 360f), 75);
					else
						EditorGUILayout.PropertyField (moonYaw, new GUIContent ("Moon Yaw"));

					ToggleButton (useMoonYawCurve, "C");

				}
				EditorGUILayout.EndHorizontal ();
				//---------------------------------------------------------------------------------------------------


				// Moon Latitude.
				EditorGUILayout.BeginHorizontal ();
				{

					if (useMoonPitchCurve.boolValue)
						CurveField ("Moon Pitch", moonPitchCurve, Color.white, new Rect (0, 0, 1, 360f), 75);
					else
						EditorGUILayout.PropertyField (moonPitch, new GUIContent ("Moon Pitch"));

					ToggleButton (useMoonPitchCurve, "C");

				}
				EditorGUILayout.EndHorizontal ();
				//---------------------------------------------------------------------------------------------------
			}
			//---------------------------------------------------------------------------------------------------


			// Moon Light Color.
			EditorGUILayout.Separator ();
			EditorGUILayout.BeginHorizontal ();
			{

				if (useMoonLightColorGradient.boolValue)
					ColorField (moonLightColorGradient, "Moon Light Color", 75);
				else
					ColorField (moonLightColor, "Moon Light Color", 75);

				ToggleButton (useMoonLightColorGradient, "G");

			}
			EditorGUILayout.EndHorizontal ();

			//---------------------------------------------------------------------------------------------------


			// Moon Light Intensity.
			EditorGUILayout.BeginHorizontal ();
			{

				if (useMoonLightIntensityCurve.boolValue)
					CurveField ("Moon Light Intensity", moonLightIntensityCurve, Color.white, new Rect (0, 0, 1, 1f), 75);
				else
					EditorGUILayout.PropertyField (moonLightIntensity, new GUIContent ("Moon Light Intensity"));

				ToggleButton (useMoonLightIntensityCurve, "C");

			}
			EditorGUILayout.EndHorizontal ();
			//---------------------------------------------------------------------------------------------------



			// Moon Color.
			EditorGUILayout.Separator ();
			EditorGUILayout.BeginHorizontal ();
			{

				if (useMoonColorGradient.boolValue)
					ColorField (moonColorGradient, "Moon Color (RGBA)", 75);
				else
					ColorField (moonColor, "Moon Color(RGBA)", 75);

				ToggleButton (useMoonColorGradient, "G");

			}
			EditorGUILayout.EndHorizontal ();
			//---------------------------------------------------------------------------------------------------


			// Moon Intensity.
			EditorGUILayout.BeginHorizontal ();
			{

				if (useMoonIntensityCurve.boolValue)
					CurveField ("Moon Intensity", moonIntensityCurve, Color.white, new Rect (0, 0, 1, 3f), 75);
				else
					EditorGUILayout.PropertyField (moonIntensity, new GUIContent ("Moon Intensity"));

				ToggleButton (useMoonIntensityCurve, "C");

			}
			EditorGUILayout.EndHorizontal ();
			//---------------------------------------------------------------------------------------------------


			// Moon Size.
			EditorGUILayout.BeginHorizontal ();
			{

				if (useMoonSizeCurve.boolValue)
					CurveField ("Moon Size", moonSizeCurve, Color.white, new Rect (0, 0, 1, 1f), 75);
				else
					EditorGUILayout.PropertyField (moonSize, new GUIContent ("Moon Size"));

				ToggleButton (useMoonSizeCurve, "C");

			}
			EditorGUILayout.EndHorizontal ();
			//---------------------------------------------------------------------------------------------------
			EditorGUILayout.Separator ();

			useMoonHalo.boolValue = EditorGUILayout.Toggle ("Use Moon Halo", useMoonHalo.boolValue, EditorStyles.radioButton);
			GUI.enabled = useMoonHalo.boolValue;
			{


				// Moon Halo Color.
				EditorGUILayout.BeginHorizontal ();
				{

					if (useMoonHaloColorGradient.boolValue)
						ColorField (moonHaloColorGradient, "Moon Halo Color", 75);
					else
						ColorField (moonHaloColor, "Moon Halo Color", 75);

					ToggleButton (useMoonHaloColorGradient, "G");

				}
				EditorGUILayout.EndHorizontal ();

				//---------------------------------------------------------------------------------------------------

				// Moon Halo Intensity.
				EditorGUILayout.BeginHorizontal ();
				{

					if (useMoonHaloIntensityCurve.boolValue)
						CurveField ("Moon Halo Intensity", moonHaloIntensityCurve, Color.white, new Rect (0, 0, 1, 5f), 75);
					else
						EditorGUILayout.PropertyField (moonHaloIntensity, new GUIContent ("Moon Halo Intensity"));

					ToggleButton (useMoonHaloIntensityCurve, "C");

				}
				EditorGUILayout.EndHorizontal ();
				//---------------------------------------------------------------------------------------------------


				// Moon Halo Size.
				EditorGUILayout.BeginHorizontal ();
				{

					if (useMoonHaloSizeCurve.boolValue)
						CurveField ("Moon Halo Size", moonHaloSizeCurve, Color.white, new Rect (0, 0, 1, 10f), 75);
					else
						EditorGUILayout.PropertyField (moonHaloSize, new GUIContent ("Moon Halo Size"));

					ToggleButton (useMoonHaloSizeCurve, "C");

				}
				EditorGUILayout.EndHorizontal ();
				//---------------------------------------------------------------------------------------------------

				GUI.enabled = true;
				Separator (WhiteColor, 2);
			}

		}

	}

	void Stars()
	{


		m_StarsFoldout = EditorGUILayout.Foldout (m_StarsFoldout, "Stars");
		if (m_StarsFoldout) 
		{

			Separator (WhiteColor, 2);
			Text ("Stars", textTitleStyle, true);
			Separator (WhiteColor, 2);
			//---------------------------------------------------------------------------------------------------

			useStars.boolValue = EditorGUILayout.Toggle ("Use Stars", useStars.boolValue, EditorStyles.radioButton);

			if (useStars.boolValue) 
			{

				EditorGUILayout.PropertyField (starsRotationMode, new GUIContent ("Stars Rotation Mode"));
				EditorGUILayout.PropertyField (starsOffsets, new GUIContent ("Stars Offsets"));
				//---------------------------------------------------------------------------------------------------


				// Stars Color.
				EditorGUILayout.Separator ();
				EditorGUILayout.BeginHorizontal ();
				{

					if (useStarsColorGradient.boolValue)
						ColorField (starsColorGradient, "Stars Color", 75);
					else
						ColorField (starsColor, "Stars Color", 75);

					ToggleButton (useStarsColorGradient, "G");

				}
				EditorGUILayout.EndHorizontal ();

				//---------------------------------------------------------------------------------------------------



				// Stars Intensity.
				EditorGUILayout.BeginHorizontal ();
				{

					if (useStarsIntensityCurve.boolValue)
						CurveField ("Stars Intensity", starsIntensityCurve, Color.white, new Rect (0, 0, 1, 5f), 75);
					else
						EditorGUILayout.PropertyField (starsIntensity, new GUIContent ("Stars Intensity"));

					ToggleButton (useStarsIntensityCurve, "C");
				}
				EditorGUILayout.EndHorizontal ();
				//---------------------------------------------------------------------------------------------------


				// Stars Twinkle.
				EditorGUILayout.Separator ();
				useStarsTwinkle.boolValue = EditorGUILayout.Toggle ("Use Stars Twinkle", useStarsTwinkle.boolValue, EditorStyles.radioButton);

				GUI.enabled = useStarsTwinkle.boolValue;

				EditorGUILayout.BeginHorizontal ();
				{

					if (useStarsTwinkleCurve.boolValue)
						CurveField ("Stars Twinkle", starsTwinkleCurve, Color.white, new Rect (0, 0, 1, 1f), 75);
					else
						EditorGUILayout.PropertyField (starsTwinkle, new GUIContent ("Stars Twinkle"));

					ToggleButton (useStarsTwinkleCurve, "C");

				}
				EditorGUILayout.EndHorizontal ();

				//---------------------------------------------------------------------------------------------------

				// Stars Twinkle Speed.
				EditorGUILayout.BeginHorizontal ();
				{

					if (useStarsTwinkleSpeedCurve.boolValue)
						CurveField ("Stars Twinkle Speed", starsTwinkleSpeedCurve, Color.white, new Rect (0, 0, 1, 10f), 75);
					else
						EditorGUILayout.PropertyField (starsTwinkleSpeed, new GUIContent ("Stars Twinkle Speed"));

					ToggleButton (useStarsTwinkleSpeedCurve, "C");
				}
				EditorGUILayout.EndHorizontal ();

				GUI.enabled = true;
					//---------------------------------------------------------------------------------------------------
				
				Separator (WhiteColor, 2);
			}

		}
	}

	void Ambient()
	{

		m_AmbientFoldout = EditorGUILayout.Foldout (m_AmbientFoldout, "Ambient");
		if (m_AmbientFoldout) 
		{

			Separator (WhiteColor, 2);
			Text("Ambient", textTitleStyle, true);
			Separator (WhiteColor, 2);
			//---------------------------------------------------------------------------------------------------

			EditorGUILayout.PropertyField (ambientMode, new GUIContent ("Ambient Mode"));
			//---------------------------------------------------------------------------------------------------

			string ambientColorName = (ambientMode.enumValueIndex == 0) ? "Ambient Color" : "Sky Color";

			if (ambientMode.enumValueIndex != 2) 
			{

				// Ambient Color.
				EditorGUILayout.BeginHorizontal ();
				{

					if (useAmbientSkyColorGradient.boolValue)
						ColorField (ambientSkyColorGradient, ambientColorName, 75);
					else
						ColorField (ambientSkyColor, ambientColorName, 75);

					ToggleButton (useAmbientSkyColorGradient, "G");

				}
				EditorGUILayout.EndHorizontal ();

				//---------------------------------------------------------------------------------------------------

			} 
			else 
			{


				// Ambient Intensity.
				EditorGUILayout.BeginHorizontal ();
				{

					if (useAmbientIntensityCurve.boolValue)
						CurveField ("Ambient Intensity", ambientIntensityCurve, Color.white, new Rect (0, 0, 1, 8f), 75);
					else
						EditorGUILayout.PropertyField (ambientIntensity, new GUIContent ("Ambient Intensity"));

					ToggleButton (useAmbientIntensityCurve, "C");
				}
				EditorGUILayout.EndHorizontal ();
				//---------------------------------------------------------------------------------------------------
			}


			if (ambientMode.enumValueIndex == 1) 
			{

				// Ambient Equator Color.
				EditorGUILayout.BeginHorizontal ();
				{
					
					if (useAmbientEquatorColorGradient.boolValue)
						ColorField (ambientEquatorColorGradient, "Ambient Equator Color", 75);
					else
						ColorField (ambientEquatorColor, "Ambient Equator Color", 75);

					ToggleButton (useAmbientEquatorColorGradient, "G");

				}
				EditorGUILayout.EndHorizontal ();

				//---------------------------------------------------------------------------------------------------


				// Ambient Ground Color.
				EditorGUILayout.BeginHorizontal ();
				{

					if (useAmbientGroundColorGradient.boolValue)
						ColorField (ambientGroundColorGradient, "Ambient Ground Color", 75);
					else
						ColorField (ambientGroundColor, "Ambient Ground Color", 75);

					ToggleButton (useAmbientGroundColorGradient, "G");

				}
				EditorGUILayout.EndHorizontal ();

				//---------------------------------------------------------------------------------------------------

			}
			Separator (WhiteColor, 2);
		}

	}

	void Fog()
	{

		m_FogFoldout = EditorGUILayout.Foldout (m_FogFoldout , "Fog");
		if (m_FogFoldout) 
		{
			Separator (WhiteColor, 2);
			Text ("Fog", textTitleStyle, true);
			Separator (WhiteColor, 2);
			//---------------------------------------------------------------------------------------------------

			EditorGUILayout.PropertyField (fogType, new GUIContent ("Fog Type"));

			if (fogType.intValue == 0) 
			{
				EditorGUILayout.BeginVertical (EditorStyles.textField);
				{
					useRenderSettingsFog.boolValue = EditorGUILayout.Toggle ("Use Fog", useRenderSettingsFog.boolValue, EditorStyles.radioButton);
					EditorGUILayout.HelpBox ("Render settings default fog", MessageType.Info);
				}
				EditorGUILayout.EndVertical();
			}

			//---------------------------------------------------------------------------------------------------

			if (fogType.intValue != 2) 
			{

				EditorGUILayout.PropertyField (fogMode, new GUIContent ("Fog Mode"));

				if (fogMode.enumValueIndex == 0)
				{

					// Fog Start Distance.
					EditorGUILayout.BeginHorizontal ();
					{

						if (useFogStartDistanceCurve.boolValue)
							CurveField ("Start Distance", fogStartDistanceCurve, Color.white, new Rect (0, 0, 1, 1000f), 75);
						else
							EditorGUILayout.PropertyField (fogStartDistance, new GUIContent ("Fog Start Distance"));

						ToggleButton (useFogStartDistanceCurve, "C");

					}
					EditorGUILayout.EndHorizontal ();
					//---------------------------------------------------------------------------------------------------

					// Fog End Distance.
					EditorGUILayout.BeginHorizontal ();
					{

						if (useFogEndDistanceCurve.boolValue)
							CurveField ("End Distance", fogEndDistanceCurve, Color.white, new Rect (0, 0, 1, 1000f), 75);
						else
							EditorGUILayout.PropertyField (fogEndDistance, new GUIContent ("Fog End Distance"));

						ToggleButton (useFogEndDistanceCurve, "C");

					}
					EditorGUILayout.EndHorizontal ();
					//---------------------------------------------------------------------------------------------------

				} 
				else 
				{

					// Fog Density.
					EditorGUILayout.BeginHorizontal ();
					{

						if (useFogDensityCurve.boolValue)
							CurveField ("Fog Density", fogDensityCurve, Color.white, new Rect (0, 0, 1, 1f), 75);
						else
							EditorGUILayout.PropertyField (fogDensity, new GUIContent ("Fog Density"));

						ToggleButton (useFogDensityCurve, "C");

					}
					EditorGUILayout.EndHorizontal ();
					//---------------------------------------------------------------------------------------------------
				}

				// Fog Color.
				EditorGUILayout.BeginHorizontal ();
				{

					if (useFogColorGradient.boolValue)
						ColorField (fogColorGradient, "Fog Color", 75);
					else
						ColorField (fogColor, "Fog Color", 75);

					ToggleButton (useFogColorGradient, "G");

				}
				EditorGUILayout.EndHorizontal ();

				//---------------------------------------------------------------------------------------------------
			}
			Separator (WhiteColor, 2);
		}
	}

	void OtherSettings()
	{

		m_OtherSettingsFoldout = EditorGUILayout.Foldout (m_OtherSettingsFoldout , "Other Settings");
		if (m_OtherSettingsFoldout) 
		{

			Separator (WhiteColor, 2);
			Text ("Other Settings",textTitleStyle, true);
			Separator (WhiteColor, 2);
			//---------------------------------------------------------------------------------------------------

			ColorField(groundColor, "Ground Color",99);
			//---------------------------------------------------------------------------------------------------

			// Exposure.
			EditorGUILayout.BeginHorizontal ();
			{

				if (useExposureCurve.boolValue)
					CurveField ("Exposure", exposureCurve, Color.white, new Rect (0, 0, 1, 5f), 75);
				else
					EditorGUILayout.PropertyField (exposure, new GUIContent ("Exposure"));

				ToggleButton (useExposureCurve, "C");

			}
			EditorGUILayout.EndHorizontal ();
			//---------------------------------------------------------------------------------------------------
			Separator (WhiteColor, 2);
		}
	}

}

