using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class TerrainObjectManager : MonoBehaviour
{
    [SerializeField]
    Terrain terrain;
    TerrainData terrainData;

    [SerializeField, Tooltip("total seven Objects = {Cone, Sequoia, Shpere, Rock1, Rock2, Copper1, Copper2}")]
    GameObject[] prefabObjects = new GameObject[7];
    GameObject objectParent;

    private void Awake()
    {
        terrainData = terrain.terrainData;
        objectParent = new GameObject("TerrainObjects");
        LoadData();
    }

    private void LoadData()
    {
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
        BinaryReader reader;
        GameObject obj;
        Vector3 pos;
        Quaternion rot;
        for (int i = 0; i < fileName.Length; i++)
        {
            string path = filePath + fileName[i];
            fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            reader = new BinaryReader(fileStream, Encoding.UTF8, false);

            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                float x = reader.ReadSingle() * terrainData.size.x - (terrainData.size.x * 0.5f);
                float y = reader.ReadSingle();
                float z = reader.ReadSingle() * terrainData.size.z - (terrainData.size.z * 0.5f);

                pos = new Vector3(x, y, z);
                rot = Quaternion.Euler(0.0f, UnityEngine.Random.Range(0.0f, 360.0f), 0);
                obj = Instantiate(prefabObjects[i], pos, rot, objectParent.transform);

            }
            
            //tempCube[i].transform.position = new Vector3(x, y, z);
            //reader.Read()
            //for (int j = 0; j < instances.Length; j++)
            //{
            //    if (instances[j].prototypeIndex != i)
            //        continue;
            //
            //    writer.Write(instances[j].position.x);
            //    writer.Write(instances[j].position.y);
            //    writer.Write(instances[j].position.z);
            //}
            //writer.Close();
            fileStream.Close();
        }
    }
}
