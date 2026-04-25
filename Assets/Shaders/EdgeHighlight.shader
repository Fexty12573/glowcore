Shader "Custom/Edge Highlight"
{
	Properties
	{
		g_highlightColor ("Highlight Color", Color) = (1, 1, 1, 1)
		g_depthThreshold ("Depth Threshold", Float) = 0.002
		g_normalThreshold ("Normal Threshold", Float) = 0.2
		g_depthStrength ("Depth Strength", Float) = 1.0
		g_normalStrength ("Normal Strength", Float) = 1.0
		g_blendStrength ("Blend Strength", Float) = 1.0
	}
	
	SubShader
	{
		Tags
		{
			"RenderType" = "Opaque"
			"RenderPipeline" = "UniversalPipeline"
		}
		
		Pass
		{
			Name "EdgeHighlight"
			ZTest Always
			ZWrite Off
			Cull Off
			Blend One Zero
			
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"
			#include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

			SAMPLER(sampler_BlitTexture);

			float4 g_highlightColor;
			float g_depthThreshold;
			float g_normalThreshold;
			float g_depthStrength;
			float g_normalStrength;
			float g_blendStrength;

			float SampleLinearDepth(float2 uv) {
				// Some platforms use a reversed Z buffer so we have to handle that explicitly.
				#if UNITY_REVERSED_Z
				float rawDepth = SampleSceneDepth(uv);
				#else
				float rawDepth = lerp(UNITY_NEAR_CLIP_VALUE, 1.0, SampleSceneDepth(uv));
				#endif

				return Linear01Depth(rawDepth, _ZBufferParams);
			}

			float3 SampleNormal(float2 uv) {
				// Could switch to reconstructed normals (from the depth buffer) if this sucks
				return normalize(SampleSceneNormals(uv));
			}

			float DepthEdge(float centerDepth, float neighborDepth) {
				return saturate(abs(neighborDepth - centerDepth) / max(g_depthThreshold, 1e-5));
			}

			float NormalEdge(float3 centerNormal, float3 neighborNormal) {
				float invDot = 1.0 - saturate(dot(centerNormal, neighborNormal));
				return saturate(invDot / max(g_normalThreshold, 1e-5));
			}

			half4 Frag(Varyings input) : SV_Target {
				float2 uv = input.texcoord;
				float2 texel = _BlitTexture_TexelSize.xy;

				half4 baseColor = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, uv);

				float centerDepth = SampleLinearDepth(uv);
				float3 centerNormal = SampleNormal(uv);

				float2 uvOffsets[4] = {
					float2(-1,  0),
                    float2( 1,  0),
                    float2( 0, -1),
                    float2( 0,  1),
				};

				float depthEdgeTotal = 0.0;
				float normalEdgeTotal = 0.0;

				UNITY_UNROLL
				for (int i = 0; i < 4; i++) {
					float2 neighborUv = uv + uvOffsets[i] * texel;

					float neighborDepth = SampleLinearDepth(neighborUv);
					float3 neighborNormal = SampleNormal(neighborUv);

					depthEdgeTotal += DepthEdge(centerDepth, neighborDepth);
					normalEdgeTotal += NormalEdge(centerNormal, neighborNormal);
				}

				float depthEdge = saturate(depthEdgeTotal / 4.0) * g_depthStrength;
				float normalEdge = saturate(normalEdgeTotal / 4.0) * g_normalStrength;

				float edge = saturate(max(depthEdge, normalEdge));
			    float edgeHard = step(0.5, edge * g_blendStrength);
			    float3 outColor = lerp(baseColor.rgb, g_highlightColor.rgb, edgeHard);

				return half4(outColor, baseColor.a);
			}

			ENDHLSL
		}
	}
}