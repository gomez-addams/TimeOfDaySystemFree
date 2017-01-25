
///*************************************************************
/// Custom procedural skybox.
/// Added moon, night  color and stars.
/// Notice: This shader is property of the Unity Tecnologuies.
///*************************************************************

Shader "AC/Time Of Day System Free/Unity Custom/Skybox/Procedural"  
{

Properties
{

	[HideInInspector]_SunSize("Sun Size", Range(0,1)) = 0.04
	[HideInInspector]_AtmosphereThickness("Atmoshpere Thickness", Range(0,5)) = 1.0
	[HideInInspector]_SkyTint("Sky Tint", Color) = (.5, .5, .5, 1)
	[HideInInspector]_GroundColor("Ground", Color) = (.369, .349, .341, 1)
	[HideInInspector]_Exposure("Exposure", Range(0, 8)) = 1.3
}

SubShader 
{

	Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
	Cull Off ZWrite Off

	Pass 
	{

		CGPROGRAM
		#pragma glsl
		#pragma vertex vert
		#pragma fragment frag
		//#pragma target 3.0
//      ------------------------------------------------ 
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#include "SkyboxProceduralInclude.cginc"

		// Color space.
		//----------------------------------------------
		#pragma multi_compile __ UNITY_COLORSPACE_GAMMA
		//______________________________________________

		// Sun model.
		//----------------------------------------------
		#pragma multi_compile __ MIEPHASE
		//______________________________________________

		// Night sky.
		//----------------------------------------------
		#pragma multi_compile __ ATMOSPHERICNIGHTCOLOR
		#pragma multi_compile __ SIMPLENIGHTCOLOR
		#pragma multi_compile __ HORIZONFADE
		//______________________________________________

		// Moon.
		//----------------------------------------------
		#pragma multi_compile __ MOON
		#pragma multi_compile __ MOONHALO
		//______________________________________________


		// Stars.
		//----------------------------------------------
		#pragma multi_compile __ STARS
		#pragma multi_compile __ STARSTWINKLE
		//______________________________________________

//      =================================================================================================================================================================

		// Sun.
		//----------------------------------------------
		uniform half     _SunSize;
		uniform float3   _SunDir;
		uniform half4    _SunColor;
		uniform float4x4 _SunMatrix;
		//______________________________________________

		// Atmosphere.
		//----------------------------------------------
		uniform half3  _SkyTint;
		uniform half   _AtmosphereThickness;
		uniform half3  _GroundColor;
		//______________________________________________


		// Moon Halo.
		uniform float     _MoonHaloSize;
		uniform float4    _MoonHaloColor;
		uniform half      _MoonHaloIntensity;
		//______________________________________________

		// HDR exposure
		//----------------------------------------------
		uniform half  _Exposure;	
		//______________________________________________

//      =================================================================================================================================================================


        // Color space.
        //----------------------------------------------------------------------------------------------------------------
		#if defined(UNITY_COLORSPACE_GAMMA)
			#define GAMMA 2
			#define COLOR_2_GAMMA(color) color
			#define COLOR_2_LINEAR(color) color*color
			#define LINEAR_2_OUTPUT(color) sqrt(color)
		#else
			#define GAMMA 2.2
			// HACK: to get gfx-tests in Gamma mode to agree until UNITY_ACTIVE_COLORSPACE_IS_GAMMA is working properly
			#define COLOR_2_GAMMA(color) ((unity_ColorSpaceDouble.r>2.0) ? pow(color,1.0/GAMMA) : color)
			#define COLOR_2_LINEAR(color) color
			#define LINEAR_2_LINEAR(color) color
		#endif
		//_________________________________________________________________________________________________________________



		//
		//-----------------------------------------------------------------------------------------------------------------

		// RGB wavelengths
		// .35 (.62=158), .43 (.68=174), .525 (.75=190)
		static const float3 kDefaultScatteringWavelength = float3(.65, .57, .475);
		static const float3 kVariableRangeForScatteringWavelength = float3(.15, .15, .15);

		#define OUTER_RADIUS 1.025
		static const float kOuterRadius = OUTER_RADIUS;
		static const float kOuterRadius2 = OUTER_RADIUS*OUTER_RADIUS;
		static const float kInnerRadius = 1.0;
		static const float kInnerRadius2 = 1.0;

		static const float kCameraHeight = 0.0001;

		#define kRAYLEIGH (lerp(0, 0.0025, pow(_AtmosphereThickness,2.5)))		// Rayleigh constant
		#define kMIE 0.0010      		// Mie constant
		#define kSUN_BRIGHTNESS 20.0 	// Sun brightness

		#define kMAX_SCATTER 50.0 // Maximum scattering value, to prevent math overflows on Adrenos

		static const half  kSunScale = 400.0 * kSUN_BRIGHTNESS;
		static const float kKmESun = kMIE * kSUN_BRIGHTNESS;
		static const float kKm4PI = kMIE * 4.0 * 3.14159265;
		static const float kScale = 1.0 / (OUTER_RADIUS - 1.0);
		static const float kScaleDepth = 0.25;
		static const float kScaleOverScaleDepth = (1.0 / (OUTER_RADIUS - 1.0)) / 0.25;
		static const float kSamples = 2.0; // THIS IS UNROLLED MANUALLY, DON'T TOUCH

		#define MIE_G (-0.990)
		#define MIE_G2 0.9801

		#define SKY_GROUND_THRESHOLD 0.02
		//_________________________________________________________________________________________________________________

		//----------------------------------------------------------------------------------------------------------------
		#ifndef SKYBOX_COLOR_IN_TARGET_COLOR_SPACE
			#if defined(SHADER_API_MOBILE)
				#define SKYBOX_COLOR_IN_TARGET_COLOR_SPACE 1
			#else
				#define SKYBOX_COLOR_IN_TARGET_COLOR_SPACE 0
			#endif
		#endif
		//_________________________________________________________________________________________________________________


//      =================================================================================================================================================================


		// Calculates the Rayleigh phase function.
		//----------------------------------------------------------------------------------------------------------------
		half getRayleighPhase(half eyeCos2)
		{
			return 0.75 + 0.75*eyeCos2;
		}
		half getRayleighPhase(half3 light, half3 ray)
		{
			half eyeCos	= dot(light, ray);
			return getRayleighPhase(eyeCos * eyeCos);
		}
		//_________________________________________________________________________________________________________________


		//----------------------------------------------------------------------------------------------------------------
		float scale(float inCos)
		{
			float x = 1.0 - inCos;

			#if defined(SHADER_API_N3DS)
				// The polynomial expansion here generates too many swizzle instructions for the 3DS vertex assembler
				// Approximate by removing x^1 and x^2
				return 0.25 * exp(-0.00287 + x*x*x*(-6.80 + x*5.25));
			#else
				return 0.25 * exp(-0.00287 + x*(0.459 + x*(3.83 + x*(-6.80 + x*5.25))));
			#endif
		}
		//_________________________________________________________________________________________________________________


		// Calculates the Mie phase function.
		//----------------------------------------------------------------------------------------------------------------
		half getMiePhase(half eyeCos)
		{


			half eyeCos2 = eyeCos * eyeCos;
			half temp = 1.0 + MIE_G2 - 2.0 * MIE_G * eyeCos;
			temp = pow(temp, pow(_SunSize,0.65) * 10);
			temp = max(temp,1.0e-4); // prevent division by zero, esp. in half precision
			temp = 1.5 * ((1.0 - MIE_G2) / (2.0 + MIE_G2)) * (1.0 + eyeCos2) / temp;
			#if defined(UNITY_COLORSPACE_GAMMA) && SKYBOX_COLOR_IN_TARGET_COLOR_SPACE
				temp = pow(temp, .454545);
			#endif
			return temp;
		}
		//_________________________________________________________________________________________________________________

		// Sun disk.
		//----------------------------------------------------------------------------------------------------------------
		half calcSunSpot(half3 delta)
		{
			//half3 delta = vec1 - vec2;
			half  dist  = length(delta);
			half  spot  = 1.0 - smoothstep(0, _SunSize, dist);
			return kSunScale * spot * spot;
		}
		//_________________________________________________________________________________________________________________

		// Moon Halo.
		//----------------------------------------------------------------------------------------------------------------
		half3 moonHalo(half3 delta)
		{

			//half3 delta  = vec1 - vec2;
			float dir    = length(delta) * _MoonHaloSize;
			float spot   = 1-smoothstep(-3, 1, dir);
			return  spot * ( _MoonHaloColor.rgb * _MoonHaloIntensity * 3);
		}
		//_________________________________________________________________________________________________________________


		struct appdata_t
		{
			float4 vertex : POSITION;
		};

		struct v2f
		{

			float4	pos				   : SV_POSITION;

			float3 worldPos            : TEXCOORD0;

			// calculate sky colors in vprog
			half3	groundColor		   : TEXCOORD1;
			half3	skyColor		   : TEXCOORD2;
			half3	sunColor		   : TEXCOORD3;

			#if defined(MOON) 
			float4  moonCoords         : TEXCOORD4;
			#endif

			#if defined(STARS) 
			float3  starsCoords        : TEXCOORD5;

			#if defined(STARSTWINKLE) 
			float3  starsNoiseCoords   : TEXCOORD6;
			#endif

			#endif

		};



		v2f vert (appdata_t v)
		{
			v2f OUT;
			OUT.pos = UnityObjectToClipPos(v.vertex);


			OUT.worldPos = mul((float3x3)unity_ObjectToWorld, v.vertex.xyz);

			// Get the ray from the camera to the vertex and its length (which is the far point of the ray passing through the atmosphere)
			float3 eyeRay = normalize(OUT.worldPos);


			// Moon coords.
			//-----------------------------------------------------------------------------------------------------------------------------
			#if defined(MOON)

			OUT.moonCoords.xyz = mul((float3x3)_MoonMatrix, v.vertex.xyz) / _MoonSize + 0.5;
			OUT.moonCoords.w = saturate(dot(_MoonDir.xyz, OUT.worldPos));

			#endif
			//______________________________________________________________________________________________________________________________


			// Stars coords.
			//-----------------------------------------------------------------------------------------------------------------------------
			#if defined(STARS) 

			float3 sunCoords = mul((float3x3)_SunMatrix, v.vertex.xyz);
			OUT.starsCoords  = mul((float3x3)_StarsMatrix, sunCoords.xyz);

			#if defined(STARSTWINKLE)
			OUT.starsNoiseCoords = mul((float3x3)_StarsNoiseMatrix, sunCoords.xyz);
			#endif

			#endif
			//-----------------------------------------------------------------------------------------------------------------------------


			// Atmospheric Scattering.
			//------------------------------------------------------------------------------------------------------------------------------

			// Convert tint from Linear back to Gamma.
			float3 kSkyTintInGammaSpace = COLOR_2_GAMMA(_SkyTint); 

			float3 kScatteringWavelength = lerp 
			(
				kDefaultScatteringWavelength - kVariableRangeForScatteringWavelength,
				kDefaultScatteringWavelength + kVariableRangeForScatteringWavelength,

				// Using Tint in sRGB gamma allows for more visually linear interpolation and to keep (.5) at (128, gray in sRGB) point
				half3(1,1,1)                 - kSkyTintInGammaSpace
			); 

			float3 kInvWavelength = 1.0 / pow(kScatteringWavelength, 4);
			float kKrESun         = kRAYLEIGH * kSUN_BRIGHTNESS;
			float kKr4PI          = kRAYLEIGH * 4.0 * 3.14159265;

			// The camera's current position.
			float3 cameraPos      = float3(0,kInnerRadius + kCameraHeight,0); 	

			float far = 0.0; half3 cIn, cOut; 

			#if defined(ATMOSPHERICNIGHTCOLOR)
				float3 nColor = float3(0.0, 0.0, 0.0);
			#endif


			if(eyeRay.y >= 0.0)
			{
				// Sky
				//------------------------------------------------------------------------------------------------------------
				// Calculate the length of the "atmosphere"
				far = sqrt(kOuterRadius2 + kInnerRadius2 * eyeRay.y * eyeRay.y - kInnerRadius2) - kInnerRadius * eyeRay.y;

				float3 pos = cameraPos + far * eyeRay;

				// Calculate the ray's starting position, then calculate its scattering offset
				float height      = kInnerRadius + kCameraHeight;
				float depth       = exp(kScaleOverScaleDepth * (-kCameraHeight));
				float startAngle  = dot(eyeRay, cameraPos) / height;
				float startOffset = depth*scale(startAngle);


				// Initialize the scattering loop variables
				float  sampleLength = far / kSamples;
				float  scaledLength = sampleLength * kScale;
				float3 sampleRay    = eyeRay * sampleLength;
				float3 samplePoint  = cameraPos + sampleRay * 0.5;

				// Now loop through the sample rays
				float3 frontColor = float3(0.0, 0.0, 0.0);


				// Weird workaround: WP8 and desktop FL_9_1 do not like the for loop here
				// (but an almost identical loop is perfectly fine in the ground calculations below)
				// Just unrolling this manually seems to make everything fine again.
//				for(int i=0; i<int(kSamples); i++)
				{
					float  height      = length(samplePoint);
					float  depth       = exp(kScaleOverScaleDepth * (kInnerRadius - height));
					float  lightAngle  = dot(_SunDir.xyz, samplePoint) / height;
					float  cameraAngle = dot(eyeRay, samplePoint) / height;
					float  scatter     = (startOffset + depth*(scale(lightAngle) - scale(cameraAngle)));
					float3 attenuate   = exp(-clamp(scatter, 0.0, kMAX_SCATTER) * (kInvWavelength * kKr4PI + kKm4PI));

					frontColor  += (attenuate * (depth * scaledLength));
					samplePoint += sampleRay;

					// Night color.
					//------------------------------------------------------------------------------------------------
					#if defined(ATMOSPHERICNIGHTCOLOR)
					float nScatter = (startOffset + depth * (scale(-lightAngle) - scale(cameraAngle)));
					float nAtten   = exp(-clamp(nScatter, 0.0, kMAX_SCATTER) *  (kInvWavelength * kKr4PI + kKm4PI));
					nColor        += (nAtten * (depth * scaledLength));
					#endif
					//________________________________________________________________________________________________

				}
				{
					float  height      = length(samplePoint);
					float  depth       = exp(kScaleOverScaleDepth * (kInnerRadius - height));
					float  lightAngle  = dot(_SunDir.xyz, samplePoint) / height;
					float  cameraAngle = dot(eyeRay, samplePoint) / height;
					float  scatter     = (startOffset + depth*(scale(lightAngle) - scale(cameraAngle)));
					float3 attenuate   = exp(-clamp(scatter, 0.0, kMAX_SCATTER) * (kInvWavelength * kKr4PI + kKm4PI));

					frontColor  += attenuate * (depth * scaledLength);
					samplePoint += sampleRay;

					// Night color.
					//------------------------------------------------------------------------------------------------
					#if defined(ATMOSPHERICNIGHTCOLOR)
					float nScatter = (startOffset + depth * (scale(-lightAngle) - scale(cameraAngle)));
					float nAtten   = exp(-clamp(nScatter, 0.0, kMAX_SCATTER) *  (kInvWavelength * kKr4PI + kKm4PI));
					nColor        += (nAtten * (depth * scaledLength));
					#endif
					//________________________________________________________________________________________________
				}

				// Finally, scale the Mie and Rayleigh colors and set up the varying variables for the pixel shader
				cIn  = (frontColor * (kInvWavelength * kKrESun));
				cOut = frontColor * kKmESun;

				// Night color.
				//------------------------------------------------------------------------------------------------
				#if defined(ATMOSPHERICNIGHTCOLOR)
				cIn += (nColor * _NightColor) * (kInvWavelength * kKrESun) * 0.25;
				#endif
				//________________________________________________________________________________________________

			}
			else
			{
				// Ground
				//------------------------------------------------------------------------------------------------------------

				far = (-kCameraHeight) / (min(-0.001, eyeRay.y));

				float3 pos = cameraPos + far * eyeRay;

				// Calculate the ray's starting position, then calculate its scattering offset
				float depth        = exp((-kCameraHeight) * (1.0/kScaleDepth));
				float cameraAngle  = dot(-eyeRay, pos);
				float lightAngle   = dot(_SunDir.xyz, pos);
				float cameraScale  = scale(cameraAngle);
				float lightScale   = scale(lightAngle);
				float cameraOffset = depth*cameraScale;
				float temp         = (lightScale + cameraScale);

				// Initialize the scattering loop variables
				float  sampleLength = far / kSamples;
				float  scaledLength = sampleLength * kScale;
				float3 sampleRay    = eyeRay * sampleLength;
				float3 samplePoint  = cameraPos + sampleRay * 0.5;

				// Now loop through the sample rays
				float3 frontColor = float3(0.0, 0.0, 0.0);
				float3 attenuate;

//				for(int i=0; i<int(kSamples); i++) // Loop removed because we kept hitting SM2.0 temp variable limits. Doesn't affect the image too much.
				{
					float height  = length(samplePoint);
					float depth   = exp(kScaleOverScaleDepth * (kInnerRadius - height));
					float scatter = depth*temp - cameraOffset;
					attenuate     = exp(-clamp(scatter, 0.0, kMAX_SCATTER) * (kInvWavelength * kKr4PI + kKm4PI));
					frontColor   += attenuate * (depth * scaledLength);
					samplePoint  += sampleRay;
				}

				cIn  = frontColor * (kInvWavelength * kKrESun + kKmESun);
				cOut = clamp(attenuate, 0.0, 1.0);
			}


			// if we want to calculate color in vprog:
			// 1. in case of linear: multiply by _Exposure in here (even in case of lerp it will be common multiplier, so we can skip mul in fshader)
			// 2. in case of gamma and SKYBOX_COLOR_IN_TARGET_COLOR_SPACE: do sqrt right away instead of doing that in fshader

			OUT.groundColor	= _Exposure * (cIn + COLOR_2_LINEAR(_GroundColor) * cOut);

			OUT.skyColor	= _Exposure * (cIn * getRayleighPhase(_SunDir.xyz, -eyeRay));

			#if defined(SIMPLENIGHTCOLOR)

			OUT.skyColor += SimpleNightColor(eyeRay.y);

			#endif


			OUT.sunColor	= _Exposure * (cOut *_SunColor.xyz);

			#if defined(UNITY_COLORSPACE_GAMMA) && SKYBOX_COLOR_IN_TARGET_COLOR_SPACE
			OUT.groundColor	= sqrt(OUT.groundColor);
			OUT.skyColor	= sqrt(OUT.skyColor);
			OUT.sunColor    = sqrt(OUT.sunColor);
			#endif

			//_______________________________________________________________________________________________________________________________

			return OUT;
		}



		half4 frag (v2f IN) : SV_Target
		{
			half3 col = half3(0.0, 0.0, 0.0);

			half3 ray = normalize(IN.worldPos.xyz);
			half  y   = -ray.y / SKY_GROUND_THRESHOLD;

			// Horizon fade.
			//---------------------------------------------------------------------------------------------------------------------------------------
			#if defined (HORIZONFADE)
			half horizonFade = saturate(HorizonFade(ray.y/0.1));
			//IN.skyColor     += horizonFade; // debug.
			#endif
			//---------------------------------------------------------------------------------------------------------------------------------------

			// Moon.
			//---------------------------------------------------------------------------------------------------------------------------------------

			#if defined(MOON)  

			//IN.skyColor   +=IN.moonCoords.w; // debug.

			half4 moonColor  = MoonColor(IN.moonCoords);

			//IN.skyColor   += moonColor.a; // debug.

			#if defined (HORIZONFADE)
			moonColor *= horizonFade;
			#endif


			#if defined(MOONHALO)
			moonColor.rgb += moonHalo(_MoonDir - ray);
			#endif

			IN.skyColor   += _Exposure * moonColor.rgb;
			#endif
			//________________________________________________________________________________________________________________________________________


			// Stars.
			//----------------------------------------------------------------------------------------------------------------------------------------
			#if defined(STARS) 
			half3  starsColor  = StarsColor(IN.starsCoords);

			#if defined(MOON) 
			starsColor.rgb  *=   moonColor.a;
			#endif
	
			#if defined(STARSTWINKLE)
			half3 starsScintillation = StarsScintillation(IN.starsNoiseCoords);
			starsColor.rgb  *=  starsScintillation;
			#endif

			//IN.skyColor     += starsScintillation; // debug.


			#if defined (HORIZONFADE)
			starsColor.rgb *= horizonFade;
			#endif

			IN.skyColor     += _Exposure * starsColor.rgb;

			#endif
			//_________________________________________________________________________________________________________________________________________

			// if we did precalculate color in vprog: just do lerp between them
			col = lerp(IN.skyColor, IN.groundColor, saturate(y));

			half mie = 0;

			#if defined(MIEPHASE)
			{
				mie = getMiePhase(-dot(_SunDir.xyz,ray));
			}
			#else
			{
				mie =  calcSunSpot(_SunDir.xyz - ray);
			}
			#endif




			if(y<0.0)
				col += mie * IN.sunColor;

			#if defined(UNITY_COLORSPACE_GAMMA) && !SKYBOX_COLOR_IN_TARGET_COLOR_SPACE
			col = LINEAR_2_OUTPUT(col);
			#endif

			return half4(col,1);
		}
		ENDCG
	}
}

Fallback Off

}
