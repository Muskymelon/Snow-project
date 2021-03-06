using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public RenderTexture tempTex, persistentTex, renderTemp;

    [SerializeField]
    Transform target;

    public Shader _drawShader;
    public Material _tempMaterial, _snowMaterial, _drawMaterial;
    public int timeInSecForSnowToGenerate = 20;
    public int regenPerSecond = 3;
    public float timer = 0;
    private float currentTime = 0;
    public int blurLevel = 1;
    public int edgeWidth = 1;

    public Camera cam;
    // Start is called before the first frame update

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //_tempMaterial.SetInteger("isRegening", (currentTime >= 1f / regenPerSecond) ? 1 : 0);
        //currentTime = (currentTime >= 1f / regenPerSecond) ? currentTime : 0;

        //********OLD********//
        /*Graphics.Blit(source, destination, _tempMaterial, 0);
        Graphics.Blit(tempTex, persistentTex);*/

        renderTemp = RenderTexture.GetTemporary(tempTex.width, tempTex.height);

        Graphics.Blit(source, destination, _tempMaterial, 0);
        Graphics.Blit(tempTex, persistentTex); 

        Graphics.Blit(persistentTex, renderTemp, _tempMaterial, 1);
        Graphics.Blit(renderTemp, destination, _tempMaterial, 2);

        RenderTexture.ReleaseTemporary(renderTemp);

    }

    void Awake()
    {

        

        // set camera mode
        cam = GetComponent<Camera>();
        cam.depthTextureMode = DepthTextureMode.DepthNormals;
        Shader.SetGlobalFloat("_OrthographicCamSize", cam.orthographicSize);
        cam.targetTexture = tempTex;

        // shader to make it red
        _tempMaterial = new Material(_drawShader);

        // set variables
        _tempMaterial.SetVector("_Color", Color.red);
        _tempMaterial.SetTexture("_ExistingTexture", persistentTex);
        _tempMaterial.SetFloat("snowIncrease", (float)System.Math.Round(1d / ((double)timeInSecForSnowToGenerate * (double)regenPerSecond), 3));

        timer = (_tempMaterial.GetFloat("snowIncrease"));
      
        // update the persistent texture
        Graphics.Blit(tempTex, persistentTex);

        // apply to the snow shader
        _snowMaterial = target.GetComponent<MeshRenderer>().material;
        _snowMaterial.SetTexture("_Splat", tempTex);
    }

    private void Update()
    {
        transform.position = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
        _tempMaterial.SetTexture("_snowIncrease", persistentTex);
    }

    private void FixedUpdate()
    {
        _tempMaterial.SetInt("_BlurLevel", blurLevel);
        _tempMaterial.SetInt("_EdgeWidth", edgeWidth);
        currentTime += Time.fixedDeltaTime;
        transform.position = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
        _tempMaterial.SetTexture("_snowIncrease", persistentTex);
    }

    private void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, 256, 256), tempTex, ScaleMode.ScaleToFit, false, 1);
        //GUI.DrawTexture(new Rect(512, 0, 256, 256), persistentTex, ScaleMode.ScaleToFit, false, 1);
    }
}
