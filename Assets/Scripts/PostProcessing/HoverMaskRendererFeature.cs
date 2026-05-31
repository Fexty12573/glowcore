using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace GlowCore.Rendering
{
    /// <summary>
    /// Renders the currently-hovered object(s) (see <see cref="HoverMaskRegistry"/>) into a screen-space
    /// mask texture published globally as <c>_HoverMask</c>. The EdgeHighlight full-screen pass samples this
    /// mask so the hovered object's outline is drawn white instead of the default highlight color.
    /// </summary>
    public class HoverMaskRendererFeature : ScriptableRendererFeature
    {
        private sealed class HoverMaskPass : ScriptableRenderPass
        {
            private static readonly int s_hoverMaskId = Shader.PropertyToID("_HoverMask");

            private readonly Material m_maskMaterial;
            private readonly List<Renderer> m_renderers = new();

            public HoverMaskPass(Material maskMaterial)
            {
                m_maskMaterial = maskMaterial;
                // Run after opaques so geometry exists; the global survives to the post-process edge pass.
                renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
            }

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
                if (cameraData.cameraType != CameraType.Game && cameraData.cameraType != CameraType.SceneView)
                    return;

                RenderTextureDescriptor desc = cameraData.cameraTargetDescriptor;
                desc.msaaSamples = 1;
                desc.depthBufferBits = 0;
                desc.colorFormat = RenderTextureFormat.R8;

                TextureHandle mask = UniversalRenderer.CreateRenderGraphTexture(
                    renderGraph, desc, "_HoverMask", false, FilterMode.Point);

                using var builder = renderGraph.AddUnsafePass<PassData>("Hover Mask", out PassData passData);

                m_renderers.Clear();
                IReadOnlyList<Renderer> source = HoverMaskRegistry.Renderers;
                for (int i = 0; i < source.Count; i++)
                    m_renderers.Add(source[i]);

                passData.Material = m_maskMaterial;
                passData.Renderers = m_renderers;
                passData.Target = mask;

                builder.UseTexture(mask, AccessFlags.Write);
                builder.AllowPassCulling(false);
                builder.AllowGlobalStateModification(true);
                builder.SetGlobalTextureAfterPass(mask, s_hoverMaskId);

                builder.SetRenderFunc(static (PassData data, UnsafeGraphContext context) => ExecutePass(data, context));
            }

            private static void ExecutePass(PassData data, UnsafeGraphContext context)
            {
                CommandBuffer cmd = CommandBufferHelpers.GetNativeCommandBuffer(context.cmd);

                context.cmd.SetRenderTarget(data.Target);
                cmd.ClearRenderTarget(false, true, Color.clear);

                if (data.Material == null)
                    return;

                for (int i = 0; i < data.Renderers.Count; i++)
                {
                    Renderer renderer = data.Renderers[i];
                    if (renderer == null || !renderer.enabled || !renderer.gameObject.activeInHierarchy)
                        continue;

                    int submeshCount = Mathf.Max(1, renderer.sharedMaterials.Length);
                    for (int submesh = 0; submesh < submeshCount; submesh++)
                        cmd.DrawRenderer(renderer, data.Material, submesh, 0);
                }
            }

            private sealed class PassData
            {
                public Material Material;
                public List<Renderer> Renderers;
                public TextureHandle Target;
            }
        }

        [Tooltip("Unlit shader that writes a white silhouette. Defaults to Hidden/GlowCore/HoverMaskWrite.")]
        [SerializeField] private Shader m_maskShader;

        private Material m_maskMaterial;
        private HoverMaskPass m_pass;

        public override void Create()
        {
            Shader shader = m_maskShader != null ? m_maskShader : Shader.Find("Hidden/GlowCore/HoverMaskWrite");
            if (shader == null)
                return;

            m_maskMaterial = CoreUtils.CreateEngineMaterial(shader);
            m_pass = new HoverMaskPass(m_maskMaterial);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (m_pass == null || m_maskMaterial == null)
                return;

            renderer.EnqueuePass(m_pass);
        }

        protected override void Dispose(bool disposing)
        {
            CoreUtils.Destroy(m_maskMaterial);
            m_maskMaterial = null;
        }
    }
}
