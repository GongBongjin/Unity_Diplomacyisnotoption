using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class TerrainObjectSave : MonoBehaviour
{
    [SerializeField]
    Terrain terrain;
    TerrainData terrainData;
    TreeInstance[] instances;


    // Start is called before the first frame update
    void Start()
    {
        SaveTerrainData();
    }

    private void SaveTerrainData()
    {
        terrainData = terrain.terrainData;
        instances = terrainData.treeInstances;

        Debug.Log(terrainData.treeInstanceCount);

        // 나무1, 2, 3
        // 돌 1, 2
        // 구리?광석 1, 2
        string[] fileName = new string[7];
        fileName[0] = "Cone.tree";
        fileName[1] = "Sequoia.tree";
        fileName[2] = "Sphere.tree";
        fileName[3] = "stone1.stone";
        fileName[4] = "stone2.stone";
        fileName[5] = "Copper1.stone";
        fileName[6] = "Copper2.stone";
        string filePath = Application.dataPath + "/Resources/TerrainData/";
        FileStream fileStream;
        BinaryWriter writer;
        for (int i = 0; i < fileName.Length; i++)
        {
            string path = filePath + fileName[i];
            fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
            writer = new BinaryWriter(fileStream, Encoding.UTF8, false);

            for (int j = 0; j < instances.Length; j++)
            {
                if (instances[j].prototypeIndex != i)
                    continue;
                writer.Write(instances[j].position.x);
                writer.Write(instances[j].position.y);
                writer.Write(instances[j].position.z);
            }
            writer.Close();
            fileStream.Close();
        }
    }
}
