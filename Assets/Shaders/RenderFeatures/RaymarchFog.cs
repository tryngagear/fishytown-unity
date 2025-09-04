using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RaymarchFog : ScriptableRendererFeature 
{
        public float intensity = 1;
        public float scattering = 0;
        public float steps = 25;
        public float maxDistance=75;
        public float jitter = 250;

    [System.Serializable]
    public class Settings{
        public Material material;
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    }

    public Settings settings = new Settings();

    class FogPass : ScriptableRenderPass
    {
        public Settings settings;
        private RenderTargetIdentifier source;
        RenderTargetHandle tempText;

        private string profilerTag;
        public void Setup(RenderTargetIdentifier source)
        {
            this.source = source;
        }

        public FogPass(string profilerTag){
            this.profilerTag = profilerTag;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            //R8 has noticeable banding
            cameraTextureDescriptor.colorFormat = RenderTextureFormat.R16;
            //we dont need to resolve AA in every single Blit
            cameraTextureDescriptor.msaaSamples = 1;

            cmd.GetTemporaryRT(tempText.id, cameraTextureDescriptor);
            ConfigureTarget(tempText.Identifier());
            ConfigureClear(ClearFlag.All, Color.black);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(profilerTag);
            cmd.Clear();

            //it is very important that if something fails our code still calls 
            //CommandBufferPool.Release(cmd) or we will have a HUGE memory leak
            try
            {
                //here we set out material properties
                //...

                //never use a Blit from source to source, as it only works with MSAA
                // enabled and the scene view doesnt have MSAA,
                // so the scene view will be pure black
                //settings.material.SetFloat("_Scattering", settings.scattering);
                //settings.material.SetFloat("_Steps", settings.steps);
                //settings.material.SetFloat("_JitterVolumetric", settings.jitter);
                //settings.material.SetFloat("_MaxDistance", settings.maxDistance);                
                //settings.material.SetFloat("_Intensity", settings.intensity);            
                cmd.Blit(source, tempText.Identifier());
                cmd.Blit(tempText.Identifier(), source, settings.material, 0);

                context.ExecuteCommandBuffer(cmd);
            }
            catch
            {
                Debug.LogError("Error");
            }
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
    }

    FogPass fogPass;
    RenderTargetHandle renderTextureHandle;

    public override void Create()
    {
        fogPass = new FogPass("Volumetric Lights");
        name = "Volumetric Light";
        fogPass.settings = settings;
        fogPass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData){
        var cameraColorTargetIdent = renderer.cameraColorTarget;
        fogPass.Setup(cameraColorTargetIdent);
        renderer.EnqueuePass(fogPass);
    }
}
