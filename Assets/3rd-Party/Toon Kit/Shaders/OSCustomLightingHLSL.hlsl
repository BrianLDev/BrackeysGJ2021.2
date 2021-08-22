#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

void MainLight_float(float3 WorldPos, out float3 Direction, out float3 Color, out float DistanceAtten, out float ShadowAtten)
{
#ifdef SHADERGRAPH_PREVIEW
    Direction = float3(0.5, 0.5, 0);
    Color = 1;
    DistanceAtten = 1;
    ShadowAtten = 1;
#else
	float4 shadowCoord = TransformWorldToShadowCoord(WorldPos);

    Light mainLight = GetMainLight(shadowCoord);
    Direction = normalize(mainLight.direction);
    Color = mainLight.color;
    DistanceAtten = mainLight.distanceAttenuation;

	#if !defined(_MAIN_LIGHT_SHADOWS) || defined(_RECEIVE_SHADOWS_OFF)
		ShadowAtten = 1.0h;
	#else
        ShadowSamplingData shadowSamplingData = GetMainLightShadowSamplingData();
        float shadowStrength = GetMainLightShadowStrength();
        ShadowAtten = SampleShadowmap(shadowCoord, TEXTURE2D_ARGS(_MainLightShadowmapTexture,
        sampler_MainLightShadowmapTexture),
        shadowSamplingData, shadowStrength, false);
    #endif
#endif
}

void AdditionalLights_float(float3 WorldPosition, float3 WorldNormal, float3 WorldView, out float3 Diffuse)
{
    float3 diffuseColor = 0;
#ifdef SHADERGRAPH_PREVIEW
	Diffuse = 0;
#else
    WorldNormal = normalize(WorldNormal);
    WorldView = SafeNormalize(WorldView);
    int pixelLightCount = GetAdditionalLightsCount();
    for (int i = 0; i < pixelLightCount; ++i)
    {
        Light light = GetAdditionalLight(i, WorldPosition);
        half3 attenuatedLightColor = light.color * (light.distanceAttenuation * light.shadowAttenuation);
        diffuseColor += LightingLambert(attenuatedLightColor, light.direction, WorldNormal);
    }
#endif
    Diffuse = diffuseColor;
}
void GetAO_half(float2 InputUV, out half IndirectAO, out half DirectAO) {
#ifdef SHADERGRAPH_PREVIEW
	IndirectAO = 1;
	DirectAO = 1;
	#else
	#if defined(_SCREEN_SPACE_OCCLUSION)
		AmbientOcclusionFactor aoFactor = GetScreenSpaceAmbientOcclusion(InputUV);
		IndirectAO = aoFactor.indirectAmbientOcclusion;
		DirectAO = aoFactor.directAmbientOcclusion;
	#else
		IndirectAO = 1;
		DirectAO = 1;
	#endif
#endif
}

#endif