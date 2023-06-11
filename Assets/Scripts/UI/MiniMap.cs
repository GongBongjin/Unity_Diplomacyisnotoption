using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    private static MiniMap instance;
    public static MiniMap Instance
    { 
        get 
        { 
            if (instance == null)
                instance = new MiniMap();
            return instance; 
        } 
    }

    [SerializeField] RenderTexture _RenderTexture;
    [SerializeField] RawImage miniMap;
    Texture2D texture;

    Color red = Color.red;
    Color green = Color.green;

    List<Vector2> armyPos = new List<Vector2>();
    List<Vector2> enemyPos = new List<Vector2>();

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rot = Camera.main.transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, rot.y));
    }

    private void LateUpdate()
    {
        CharacterManager.instance.SetMinimapPosition();
        EnemyManager.instance.SetMinimapPosition();
        CitizenManager.Instance.SetMinimapPosition();
        if(texture != null)
            Destroy(texture);
        texture = new Texture2D(_RenderTexture.width, _RenderTexture.height, TextureFormat.ARGB32, false);
        RenderTexture.active = _RenderTexture;
        texture.ReadPixels(new Rect(0, 0, _RenderTexture.width, _RenderTexture.height), 0, 0);

        for (int i = 0; i < armyPos.Count; i++)
        {
            texture.SetPixel((int)armyPos[i].x, (int)armyPos[i].y, green);
        }
        for (int i = 0; i < enemyPos.Count; i++)
        {
            texture.SetPixel((int)enemyPos[i].x, (int)enemyPos[i].y, red);
        }
        texture.Apply();
        miniMap.texture = texture;

        armyPos.Clear();
        enemyPos.Clear();
    }

    public void AddPosition(Vector3 pos, bool isArmy = true)
    {
        int x = (int)((pos.x + 500) * 0.25f);
        int y = (int)((pos.z + 500) * 0.25f);
        Vector2 point = new Vector2(x, y);
        if(isArmy)
            armyPos.Add(point);
        else
            enemyPos.Add(point);
    }
}
