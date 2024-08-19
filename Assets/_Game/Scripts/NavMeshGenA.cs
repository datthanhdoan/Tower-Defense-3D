using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshSurface))]
public class NavMeshGenA : MonoBehaviour
{
    Terrain terrain;
    TerrainData terrainData;
    Vector3 terrainPos;

    public String NavAgentLayer = "Default";
    public String defaultarea = "Walkable";
    public bool includeTrees;
    public float timeLimitInSecs = 20;
    public int step = 1;
    public float padding = 1.0f; // New padding field
    public List<string> areaID;

    [SerializeField] bool _destroyTempObjects;
    [SerializeField] bool _break;

    void Start()
    {
    }

    public void Build()
    {
        EditorCoroutineUtility.StartCoroutine(GenMeshes(), this);
    }

    IEnumerator GenMeshes()
    {
        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;
        terrainPos = terrain.transform.position;

        Vector3 size = terrain.terrainData.size;
        Vector3 tpos = terrain.GetPosition();
        float minX = tpos.x;
        float maxX = minX + size.x;
        float minZ = tpos.z;
        float maxZ = minZ + size.z;

        GameObject attachParent;
        Transform childA = terrain.transform.Find("Delete me");

        if (childA != null)
        {
            attachParent = childA.gameObject;
        }
        else
        {
            attachParent = new GameObject();
            attachParent.name = "Delete me";
            attachParent.transform.SetParent(terrain.transform);
            attachParent.transform.localPosition = Vector3.zero;
        }

        yield return null;

        int terrainLayer = LayerMask.NameToLayer(NavAgentLayer);
        int defaultWalkableArea = NavMesh.GetAreaFromName(defaultarea);
        Debug.Log("terrain pos:" + tpos);
        Debug.Log("terrain size:" + size);
        Debug.Log("minX:" + minX + ", maxX:" + maxX + ", minZ:" + minZ + ", maxZ:" + maxZ);

        float[,,] splatmapData = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);
        Debug.Log("alpha h width:" + terrainData.alphamapWidth + ", height:" + terrainData.alphamapHeight + ", resolution:" + terrainData.alphamapResolution);

        float alphaWidth = terrainData.alphamapWidth;
        float alphaHeight = terrainData.alphamapHeight;
        float tWidth = terrainData.size.x;
        float tHeight = terrainData.size.z;
        float startTime = Time.realtimeSinceStartup;
        float xStepsize = (tWidth / alphaWidth) * (1 + padding); // Apply padding
        float zStepsize = (tHeight / alphaHeight) * (1 + padding); // Apply padding

        Debug.Log("xStepSize:" + xStepsize);
        Debug.Log("Total Iterations = " + ((alphaWidth / step) * (alphaHeight / step)) + ", step:" + step);

        for (int dx = 0; dx <= alphaWidth; dx += step)
        {
            Debug.Log("Waiting a frame)");
            yield return null;

            float xOff = tWidth * ((float)dx / alphaWidth) - padding * xStepsize;
            for (int dz = 0; dz <= alphaHeight; dz += step)
            {
                float zOff = tHeight * ((float)dz / alphaHeight) - padding * zStepsize;
                {
                    if (_break)
                    {
                        Debug.Log("Breaking");
                        yield break;
                    }
                    int surface = GetMainTextureA(dz, dx, ref splatmapData);

                    if (!areaID.Contains(surface.ToString()))
                        continue;

                    if (Time.realtimeSinceStartup > startTime + timeLimitInSecs)
                    {
                        Debug.Log("Time limit exceeded");
                        goto escape;
                    }

                    Vector3 pos = new Vector3(minX + xOff, 0, minZ + zOff);
                    GameObject obj = new GameObject();
                    Transform objT = obj.transform;
                    objT.SetParent(attachParent.transform);
                    objT.localScale = Vector3.one;
                    float height = terrain.SampleHeight(pos);
                    objT.position = new Vector3(pos.x, height, pos.z);
                    NavMeshModifierVolume nmmv = obj.AddComponent<NavMeshModifierVolume>();
                    nmmv.size = new Vector3(xStepsize * step, 1, zStepsize * step);
                    nmmv.center = Vector3.zero;
                    nmmv.area = 0;
                }
            }
        }
    escape:

        if (includeTrees)
        {
            Debug.Log("Now doing trees");
            TreeInstance[] instances = terrainData.treeInstances;
            TreePrototype[] prototypes = terrainData.treePrototypes;
            Vector3 tsize = terrainData.size;

            foreach (TreeInstance inst in instances)
            {
                TreePrototype prototype = prototypes[inst.prototypeIndex];
                Vector3 pos = (Vector3.Scale(inst.position, tsize));
                float rotY = inst.rotation;
                float hscale = inst.heightScale;
                float wscale = inst.widthScale;

                GameObject tree = GameObject.Instantiate(prototype.prefab);
                Transform objT = tree.transform;
                objT.SetParent(attachParent.transform);
                objT.position = new Vector3(pos.x, pos.y, pos.z);
                objT.localRotation = Quaternion.Euler(0, Mathf.Deg2Rad * rotY, 0);
                objT.localScale = new Vector3(wscale, hscale, wscale);
                objT.gameObject.layer = terrainLayer;
                tree.isStatic = true;
            }
        }
        Debug.Log("Done prep, build nav mesh");
        foreach (NavMeshSurface nsurface in GetComponents<NavMeshSurface>())
        {
            nsurface.BuildNavMesh();
            yield return null;
        }

        if (!_destroyTempObjects)
            yield break;
        Debug.Log($"Finished, destroy our {attachParent.transform.childCount} temp objects");
        GameObject.DestroyImmediate(attachParent.gameObject);
    }

    void destroyChildren(Transform attachParent)
    {
        while (attachParent.childCount > 0)
            GameObject.DestroyImmediate(attachParent.GetChild(0).gameObject);
    }

    private float[] GetTextureMixA(int x, int z, ref float[,,] splatmapData)
    {
        float[] cellMix = new float[splatmapData.GetUpperBound(2) + 1];
        try
        {
            for (int n = 0; n < cellMix.Length; n++)
            {
                cellMix[n] = splatmapData[x, z, n];
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        return cellMix;
    }

    private int GetMainTextureA(int x, int z, ref float[,,] splatmapData)
    {
        float[] mix = GetTextureMixA(x, z, ref splatmapData);
        float maxMix = 0;
        int maxIndex = 0;
        for (int n = 0; n < mix.Length; n++)
        {
            if (mix[n] > maxMix)
            {
                maxIndex = n;
                maxMix = mix[n];
            }
        }
        return maxIndex;
    }
}
