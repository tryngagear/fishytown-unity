using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LightManager : MonoBehaviour
{
    [SerializeField] private Light dirLight;
    [SerializeField] private LightingPresets lightingPresets;
    [SerializeField, Range(0,24)] private float TimeOfDay;
    private float TimeRatio = 1;

    private void Update() {
        if(lightingPresets == null)
            return;
        //if(Application.isPlaying){
            TimeOfDay += Time.deltaTime * TimeRatio;
            TimeOfDay %= 24;
            UpdateLighting(TimeOfDay/24.0f);
            Shader.SetGlobalVector("_SunDirection", dirLight.transform.forward);
            Shader.SetGlobalVector("_SunMoonColor", dirLight.color);
        //}
    }
    private void UpdateLighting(float timePercent){
        RenderSettings.ambientLight = lightingPresets.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = lightingPresets.FogColor.Evaluate(timePercent);
        if(dirLight != null){
            dirLight.color = lightingPresets.SunColor.Evaluate(timePercent);
            dirLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 260) - 90f, 170f, 0));
        }
    }

    private void OnValidate() {
        if(dirLight != null)
            return;
        if(RenderSettings.sun != null)
        {
            dirLight = RenderSettings.sun;
        }
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach(Light light in lights){
                if(light.type == LightType.Directional){
                    dirLight = light;
                }
            }
        }
    }


}
