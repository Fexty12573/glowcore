Shader "Hidden/GlowCore/HoverMaskWrite"
{
	// Writes a solid white silhouette of whatever renderer it is drawn with.
	// Used by HoverMaskRendererFeature to build the screen-space _HoverMask texture
	// that EdgeHighlight samples to tint the hovered object's outline white.
	SubShader
	{
		Tags
		{
			"RenderPipeline" = "UniversalPipeline"
		}

		Pass
		{
			Name "HoverMaskWrite"
			ZWrite Off
			ZTest Always
			Cull Back
			Blend One Zero

			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			struct Attributes
			{
				float4 positionOS : POSITION;
			};

			struct Varyings
			{
				float4 positionHCS : SV_POSITION;
			};

			Varyings Vert(Attributes input)
			{
				Varyings output;
				output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
				return output;
			}

			half4 Frag(Varyings input) : SV_Target
			{
				return half4(1, 1, 1, 1);
			}

			ENDHLSL
		}
	}
}
