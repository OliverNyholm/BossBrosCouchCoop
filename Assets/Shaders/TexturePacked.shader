Shader "Custom/TexturePacked" {
	Properties{
		_MainTex("Diffuse", 2D) = "white" {}
		_Metallic("Material", 2D) = "white" {}
		_BumpMap("Normal", 2D) = "white" {}
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			#pragma surface surf Standard fullforwardshadows

			#pragma target 3.0

			sampler2D _MainTex;

			struct Input {
				float2 uv_MainTex;
				float2 uv_BumpMap;
				float2 uv_Metallic;
			};

			UNITY_INSTANCING_BUFFER_START(Props)
				// put more per-instance properties here
			UNITY_INSTANCING_BUFFER_END(Props)

			sampler2D _BumpMap;
			sampler2D _Metallic;

			void surf(Input IN, inout SurfaceOutputStandard o) 
			{
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
				o.Albedo = c.rgb;

				o.Metallic = tex2D(_Metallic, IN.uv_Metallic).r;
				o.Smoothness = 1 - tex2D(_Metallic, IN.uv_Metallic).g;
				o.Alpha = c.a;

				o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));

			}
			ENDCG
		}
			FallBack "Diffuse"
}
