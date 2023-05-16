using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "LightingPreset", menuName = "ScriptableObjects/LightingPreset")]
public class LightingPresets : ScriptableObject
{
    public Gradient AmbientColor;
    public Gradient SunColor;
    public Gradient FogColor;
}
