void MainLight_half(float3 WorldPos, out half3 Direction, out half3 Color, out half DistanceAtten, out half ShadowAtten)
{
	//#if SHADERGRAPH_PREVIEW
	//    Direction = half3(0.5, 0.5, 0);
	//    Color = 1;
	//    DistanceAtten = 1;
	//    ShadowAtten = 1;
	//#else
	//    #if SHADOWS_SCREEN
	//        half4 clipPos = TransformWorldToHClip(WorldPos);
	//        half4 shadowCoord = ComputeScreenPos(clipPos);
	//    #else
	//        half4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
	//    #endif
	//        DirectionalLightData light = _DirectionalLightDatas[0];
	//        i_lightDir = -light.forward.xyz;
	//        i_color = light.color;
	//
	//        Light mainLight = GetMainLight(shadowCoord);
	//        Direction = -light.forward.xyz;
	//        Color = light.color;
	//        //DistanceAtten = mainLight.distanceAttenuation;
	//        DistanceAtten = 1;
	//        ShadowAtten = light.shadowDimmer;
	//    #if !defined(_MAIN_LIGHT_SHADOWS) || defined(_RECEIVE_SHADOWS_OFF)
	//        ShadowAtten = 1.0h;
	//    #endif
	//
	//    #if SHADOWS_SCREEN
	//        ShadowAtten = SampleScreenSpaceShadowmap(shadowCoord);
	//    #else
	//        ShadowSamplingData shadowSamplingData = GetMainLightShadowSamlingData();
	//        half shadowStrength = GetMainLightShadowStrenght();
	//        ShadowAtten = SampleShadowmap(shadowCoord, TEXTURE2D_ARGS(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture), shadowSamplingData, shadowStrength, false);
	//    #endif
	//#endif
	Direction = half3(0.5, 0.5, 0);
	Color = 1;
	DistanceAtten = 1;
	ShadowAtten = 1;
}