// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced '_ProjectorClip' with 'unity_ProjectorClip'

Shader "Projector/Target" {
	Properties {
		_Color("Main Color", Color) = (1,1,1,1)
		_TopLeft("_TopLeft", Color) = (1,1,1,1)
		_TopRight("_TopRight", Color) = (1,1,1,1)
		_BottomLeft("_BottomLeft", Color) = (1,1,1,1)
		_BottomRight("_BottomRight", Color) = (1,1,1,1)
		_ShadowTex ("Cookie", 2D) = "" {}
		_FalloffTex ("FallOff", 2D) = "" {}
	}
	
	Subshader {
		Tags {"Queue"="Transparent"}
		Pass {
			ZWrite Off
			ColorMask RGB
			Blend DstColor One
			Offset -1, -1
	
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#include "UnityCG.cginc"
			
			struct v2f {
				float4 uvShadow : TEXCOORD0;
				float4 uvFalloff : TEXCOORD1;
				UNITY_FOG_COORDS(2)
				float4 pos : SV_POSITION;
			};
			
			float4x4 unity_Projector;
			float4x4 unity_ProjectorClip;
			
			v2f vert (float4 vertex : POSITION)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(vertex);
				o.uvShadow = mul (unity_Projector, vertex);
				o.uvFalloff = mul (unity_ProjectorClip, vertex);
				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}
			
			fixed4 _Color;
			sampler2D _ShadowTex;
			sampler2D _FalloffTex;
			float4 _TopLeft;
			float4 _TopRight;
			float4 _BottomLeft;
			float4 _BottomRight;
			
			fixed4 frag(v2f i) : SV_Target
			{
				float2 uv = i.uvShadow.xy;

				float angle = _Time * 5.0;
				float s = sin(angle);
				float c = cos(angle);

				float2x2 rotationMatrix = float2x2(c, s,-s,  c);

				float2 pivot = float2(0.5, 0.5);
				uv = mul(rotationMatrix, (uv - pivot)) + pivot;

				i.uvShadow.xy = uv;
				fixed4 texS = tex2Dproj (_ShadowTex, UNITY_PROJ_COORD(i.uvShadow));
				
				float3 colorTopLeft = _TopLeft.rgb * step(uv.x, 0.5f) * step(uv.y * -1, -0.5f);
				float3 colorTopRight = _TopRight.rgb * step(uv.x * -1, -0.5f) * step(uv.y * -1, -0.5f);
				float3 colorBottomLeft = _BottomLeft.rgb * step(uv.x, 0.5f)* step(uv.y, 0.5f);
				float3 colorBottomRight = _BottomRight.rgb * step(uv.x * -1, -0.5f) * step(uv.y, 0.5f);
				
				clamp(colorTopLeft, 0, 1);
				clamp(colorTopRight, 0, 1);
				clamp(colorBottomLeft, 0, 1);
				clamp(colorBottomRight, 0, 1);

				texS.rgb *= colorTopLeft + colorTopRight + colorBottomLeft + colorBottomRight;

				texS.a = 1.0 - texS.a;
				
				fixed4 texF = tex2Dproj (_FalloffTex, UNITY_PROJ_COORD(i.uvFalloff));
				fixed4 res = texS * texF.a;

				UNITY_APPLY_FOG_COLOR(i.fogCoord, res, fixed4(0,0,0,0));
				return res;
			}
			ENDCG
		}
	}
}
