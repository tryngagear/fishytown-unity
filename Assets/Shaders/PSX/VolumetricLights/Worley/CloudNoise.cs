using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudNoise : MonoBehaviour
{
    public bool logComputeTime;

    [Header("Base Noise")]
    public bool updateBaseNoise = true;
    public ComputeShader noiseCompute;
    public RenderTexture noiseTexture;
    public int baseResolution = 128;

    public Vector3 noiseTestParams;

    public SimplexSettings simplexSettings;
    public WorleySettings worleySettings;
    [HideInInspector]
    //bool calculateWorley = true;

    [Header("Detail Noise")]
    public int detailNoiseResolution = 32;
    public RenderTexture detailNoiseTexture;
    public WorleySettings detailWorleySettings;

    //bool updateNoise = true;
}
