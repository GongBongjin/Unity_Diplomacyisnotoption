using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public struct ProductData
{
    public int key;
    public string name;
    public int maxAmount;
    public int productTime;
    public int output;
    public Sprite icon;
    public GameObject prefab;
    public string description;
}

public class TerrainObjectManager : MonoBehaviour
{
    private static TerrainObjectManager instance;
    public static TerrainObjectManager Instance
    {
        get
        {
            return instance;
        }
    }


    [SerializeField]
    Terrain terrain;
    TerrainData terrainData;

    Dictionary<int, ProductData> productDatas = new Dictionary<int, ProductData>();

    GameObject objectParent;

    private void Awake()
    {
        instance = this;

        terrainData = terrain.terrainData;
        objectParent = new GameObject("TerrainObjects");
        LoadProductData();
        LoadData();
    }

    public ProductData GetProductData(int key)
    {
        return productDatas[key];
    }

    public void LoadProductData()
    {
        List<Dictionary<string, object>> reader = CSVReader.Read("TextData/ProductData");

        for (int i = 0; i < reader.Count; i++)
        {
            ProductData productData;
            productData.key = int.Parse(reader[i]["Key"].ToString());
            productData.name = reader[i]["Name"].ToString();
            productData.maxAmount = int.Parse(reader[i]["MaxAmount"].ToString());
            productData.productTime = int.Parse(reader[i]["ProductTime"].ToString());
            productData.output = int.Parse(reader[i]["Output"].ToString());
            productData.icon = Resources.Load<Sprite>(reader[i]["Icon"].ToString());
            productData.prefab = Resources.Load<GameObject>(reader[i]["Prefab"].ToString());
            productData.description = reader[i]["Description"].ToString();
            productDatas.Add(productData.key, productData);
        }
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
        //string filePath = Application.dataPath + "/Resources/TerrainData/";
        string filePath = Application.streamingAssetsPath + "/TerrainData/";
        FileStream fileStream;
        BinaryReader reader;
        GameObject obj;
        Vector3 pos;
        Quaternion rot;
        int baseKey = 3001;
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

                int key = baseKey + i;

                obj = Instantiate(productDatas[key].prefab, pos, rot, objectParent.transform);
                obj.GetComponent<ProductObject>().SetProductProperty(productDatas[key]);

            }
            fileStream.Close();
        }
    }
}
