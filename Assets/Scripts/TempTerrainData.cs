using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempTerrainData : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    Terrain terrain;
    TerrainData td;

    [SerializeField]
    Image image2;

    [SerializeField]
    RawImage image;

    [SerializeField]
    public int index;

    void Start()
    {
        td = terrain.terrainData;
    }

    // Update is called once per frame
    void Update()
    {
        //terrain.terrainData.texture
        image.material.mainTexture = td.GetAlphamapTexture(index);
        image2.material.mainTexture = td.GetAlphamapTexture(index);
    }
}
