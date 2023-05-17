using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;

public class JsonFileManager
{
    public string ReadJsonFile(string path)
    {
        string json = File.ReadAllText(Application.dataPath + path);

        return json;
    }
}
