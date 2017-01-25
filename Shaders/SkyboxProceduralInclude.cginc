
#ifndef SKYBOX_PROCEDURAL_INCLUDED
#define SKYBOX_PROCEDURAL_INCLUDED

#include "UnityCG.cginc"


// Atmosphere.
uniform half4  _NightColor;
uniform float  _HorizonFade;
//--------------------------------------


// Moon.
uniform sampler2D _MoonTexture;
uniform half4     _MoonColor;
uniform half      _MoonIntensity;
//--------------------------------------
uniform float3    _MoonDir;
uniform float4x4  _MoonMatrix;
uniform half      _MoonSize;
//--------------------------------------

// Stars.
uniform samplerCUBE _StarsCubemap;
uniform samplerCUBE _StarsNoiseCubemap;
//--------------------------------------
uniform half        _StarsIntensity;
uniform half4       _StarsColor;
uniform float       _StarsTwinkle;
half4               _StarsCubemap_HDR;
//--------------------------------------
uniform float4x4    _StarsMatrix;
uniform float4x4    _StarsNoiseMatrix;
//--------------------------------------


// Moon coords.
inline float4 MoonCoords(float3 worldPos, float3 vertex)
{

	float3 coords = mul((float3x3)_MoonMatrix, vertex.xyz) / _MoonSize + 0.5;
	float mask    = saturate(dot(_MoonDir.xyz, worldPos));

	return float4(coords, mask);
}

inline float3 StarsCoords(float3 sunCoords)
{
	return mul((float3x3)_StarsMatrix, sunCoords.xyz);
}

inline float3 StarsNoiseCoords(float3 sunCoords)
{
	return mul((float3x3)_StarsNoiseMatrix, sunCoords.xyz);
}

//---------------------------------------------------------------------------



// Horizon fade.
inline half HorizonFade(float dir)
{
	return pow(max(0, dir), _HorizonFade);
}


// Simple night color.
inline half3 SimpleNightColor(float dir)
{
	return (saturate(1-dot(dir, 0.5)) * _NightColor.rgb) ;
}


// Moon color.
inline half4 MoonColor(float4 coords)
{
	half4 color = (tex2D(_MoonTexture, coords.xy) * _MoonColor) * coords.w;
	color.rgb  *= _MoonIntensity;

	// Mask
	half mask   = (1.0 - color.a);
	color      *= 1.0 - mask; // for no black backgrouds.

	// moon color = rgb, mask = a.
	return half4(color.rgb, mask);
}

// Stars.
inline half3 StarsColor(float3 coords)
{
	half4 cube  = texCUBE(_StarsCubemap, coords);
	half3 color = DecodeHDR(cube,  _StarsCubemap_HDR)* _StarsColor.rgb;
	color.rgb  *= (unity_ColorSpaceDouble.rgb) * _StarsIntensity;
	return color;
}

inline half3 StarsScintillation(float3 coords)
{
	half4 noise = texCUBE(_StarsNoiseCubemap, coords);
	return lerp(1, 2 * noise.rgb, _StarsTwinkle);
}


#endif