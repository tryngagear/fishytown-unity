using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NoiseGen : MonoBehaviour
{
    const int computeThreadGroupSize = 8;
    public const string detailNoiseName = "DetailNoise";
    public const string shapeNoiseName = "ShapeNoise";

    public enum CloudNoiseType { Shape, Detail }
    public enum TextureChannel { R, G, B, A }

    [Header ("Editor Settings")]
    public CloudNoiseType activeTextureType;
    public TextureChannel activeChannel;
    public bool autoUpdate;
    public bool logComputeTime;

    [Header ("Noise Settings")]
    public int shapeResolution = 132;
    public int detailResolution = 32;

    public WorleyNoiseSettings[] shapeSettings;
    public WorleyNoiseSettings[] detailSettings;
    public ComputeShader noiseCompute;
    public ComputeShader copy;

    [Header("Viewer Settings")]
    public bool viewerEnabled;
    public bool viewerGreyscale = true;
    public bool viewerShowAllChannels;
    [Range (0, 1)]
    public float viewerSliceDepth;
    [Range (1, 5)]
    public float viewerTileAmount = 1;
    [Range (0, 1)]
    public float viewerSize = 1;

    List<ComputeBuffer> buffersToRelease;
    bool updateNoise;

    [HideInInspector]
    public bool showSettingsEditor = true;
    [SerializeField, HideInInspector]
    public RenderTexture shapeTexture;
    [SerializeField, HideInInspector]
    public RenderTexture detailTexture;
    
    public void CreateWorleyNoise(){
        //Texture2D texture;
    }
    public void UpdateNoise(){
        ValidateParamaters();
        CreateTexture(ref shapeTexture, shapeResolution, shapeNoiseName);
        CreateTexture(ref detailTexture, detailResolution, detailNoiseName);

        if (updateNoise && noiseCompute){
            var timer = System.Diagnostics.Stopwatch.StartNew();
            updateNoise = false;
            WorleyNoiseSettings activeSettings = ActiveSettings;
            if(activeSettings == null){
                return;
            }

            buffersToRelease = new List<ComputeBuffer>();

            int activeTextureResolution = ActiveTexture.width;

            //set val
            noiseCompute.SetFloat("persistence", activeSettings.persistence);
            noiseCompute.SetInt("resolution", activeTextureResolution);
            noiseCompute.SetVector("channelMask", ChannelMask);
            //set noise gen kernel
            noiseCompute.SetTexture(0, "Result", ActiveTexture);
            var minMaxBuffer = CreateBuffer(new int[] {int.MaxValue, 0}, sizeof (int), "minMax", 0);
            UpdateWorley(ActiveSettings);
            noiseCompute.SetTexture(0, "Result", ActiveTexture);

            int numThreadGroups = Mathf.CeilToInt(activeTextureResolution/(float) computeThreadGroupSize);
            noiseCompute.Dispatch(1,numThreadGroups, numThreadGroups, numThreadGroups);

            if(logComputeTime){
                var minMax = new int[2];
                minMaxBuffer.GetData(minMax);

                Debug.Log ($"Noise Generation: {timer.ElapsedMilliseconds}ms");
            }
            foreach(var buffer in buffersToRelease){
                buffer.Release();
            }
        }
    }

    void UpdateWorley(WorleyNoiseSettings settings){
        var prng = new System.Random(settings.seed);
        CreateWorleyPointsBuffer(prng, settings.numDivisionsA, "pointsA"); 
        CreateWorleyPointsBuffer(prng, settings.numDivisionsB, "pointsB");
        CreateWorleyPointsBuffer(prng, settings.numDivisionsC, "pointsC");

        noiseCompute.SetInt("numcellsA", settings.numDivisionsA); 
        noiseCompute.SetInt("numcellsB", settings.numDivisionsB);
        noiseCompute.SetInt("numcellsC", settings.numDivisionsC);
        noiseCompute.SetBool("invertNoise", settings.invert);
        noiseCompute.SetInt("tile", settings.tile); 
    }
    public void Load (string saveName, RenderTexture target) {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name;
        saveName = sceneName + "_" + saveName;
        Texture2D savedTex = (Texture2D) Resources.Load (saveName);
        if (savedTex != null && savedTex.width == target.width) {
            copy.SetTexture (0, "tex", savedTex);
            copy.SetTexture (0, "renderTex", target);
            int numThreadGroups = Mathf.CeilToInt (savedTex.width / 8f);
            copy.Dispatch (0, numThreadGroups, numThreadGroups, numThreadGroups);
        }
    }

    ComputeBuffer CreateBuffer(System.Array data, int stride, string bufferName ,int kernel = 0){
        var buffer = new ComputeBuffer(data.Length, stride, ComputeBufferType.Raw);
        buffersToRelease.Add(buffer);
        buffer.SetData(data);
        noiseCompute.SetBuffer(kernel, bufferName, buffer);
        return buffer;
    }

    void CreateTexture (ref RenderTexture texture, int resolution, string name) {
        var format = UnityEngine.Experimental.Rendering.GraphicsFormat.R16G16B16A16_UNorm;
        if (texture == null || !texture.IsCreated () || texture.width != resolution || texture.height != resolution || texture.volumeDepth != resolution || texture.graphicsFormat != format) {
            //Debug.Log ("Create tex: update noise: " + updateNoise);
            if (texture != null) {
                texture.Release ();
            }
            texture = new RenderTexture (resolution, resolution, 0);
            texture.graphicsFormat = format;
            texture.volumeDepth = resolution;
            texture.enableRandomWrite = true;
            texture.dimension = UnityEngine.Rendering.TextureDimension.Tex3D;
            texture.name = name;

            texture.Create ();
            Load (name, texture);
        }
        texture.wrapMode = TextureWrapMode.Repeat;
        texture.filterMode = FilterMode.Bilinear;
    }

    void CreateWorleyPointsBuffer(System.Random prng ,int numCellsPerAxis, string bufferName){
        var points = new Vector2[numCellsPerAxis*numCellsPerAxis];
        float cellSize = 1f/numCellsPerAxis;

        for(int x = 0; x < numCellsPerAxis; x++){
            for(int y = 0; y < numCellsPerAxis; y++){
                for(int z = 0; z < numCellsPerAxis; z++){
                    //var randomOffset = new Vector2(Random.value,Random.value);
                    //Vector2 cellCorner = new Vector2(x,y);
                    //var pos = (new Vector2(x,y) + randomOffset) * cellSize;
                    //int index = x+numCellsPerAxis * (y+numCellsPerAxis);
                    //points[index] = cellCorner + randomOffset;
                    float randomX = (float) prng.NextDouble(); 
                    float randomY = (float) prng.NextDouble(); 
                    float randomZ = (float) prng.NextDouble(); 
                    Vector3 randomOffset = new Vector3(randomX,randomY,randomZ)*cellSize;
                    Vector3 cellCorner = new Vector3(x,y,z) * cellSize;

                    int index = x + numCellsPerAxis * (y+z*numCellsPerAxis);
                    points[index] = cellCorner + randomOffset;
                }
            }
        }
        CreateBuffer(points, sizeof(float)*3, bufferName);
    }

    public RenderTexture ActiveTexture{
        get{
            return(activeTextureType == CloudNoiseType.Shape) ? shapeTexture : detailTexture;
        }
    }
    public WorleyNoiseSettings ActiveSettings {
        get {
            WorleyNoiseSettings[] settings = (activeTextureType == CloudNoiseType.Shape) ? shapeSettings : detailSettings;
            int activeChannelIndex = (int) activeChannel;
            if (activeChannelIndex >= settings.Length) {
                return null;
            }
            return settings[activeChannelIndex];
        }
    }

    public Vector4 ChannelMask{
        get{
            Vector4 channelWeight = new Vector4(
                (activeChannel == NoiseGen.TextureChannel.R)?1:0,
                (activeChannel == NoiseGen.TextureChannel.G)?1:0,
                (activeChannel == NoiseGen.TextureChannel.B)?1:0,
                (activeChannel == NoiseGen.TextureChannel.A)?1:0
                );
            return channelWeight;
        }
    }
    void ValidateParamaters () {
        detailResolution = Mathf.Max (1, detailResolution);
        shapeResolution = Mathf.Max (1, shapeResolution);
    }

    public void ManualUpdate () {
        updateNoise = true;
        UpdateNoise ();
    }

    public void ActiveNoiseSettingsChanged(){
        if(autoUpdate){
            updateNoise = true;
        }
    }
}
