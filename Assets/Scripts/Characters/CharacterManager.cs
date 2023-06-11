using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public enum CharacterKey
{
    //Army
    KNIGHT = 1001,
    DOGNIGHT,
    SPEARMAN,
    WIZARD,
    GRUNT,
    //Enemy
    TURTLE,
    SLIME,
    MUSHROOM,
    CACTUS,
    MIMIC,
    BEHOLDER,
    //Boss
    GOLEM,
    USURPER
}

public class CharacterManager : MonoBehaviour
{
    static public CharacterManager instance;   

    private GameObject character;

    private Dictionary<int, CharacterData> characterDatas = new Dictionary<int, CharacterData>();
    private Dictionary<CharacterKey, GameObject> characterPrefabs = new Dictionary<CharacterKey, GameObject>();
    private Dictionary<CharacterKey, List<GameObject>> characterPools = new Dictionary<CharacterKey, List<GameObject>>();

    private Vector3[] spawnPos = new Vector3[4];

    private int dCount = 0;
    private bool isEvnentOn = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        spawnPos[0] = new Vector3(-70, 0, -70);
        spawnPos[1] = new Vector3(-70, 0, +70);
        spawnPos[2] = new Vector3(+70, 0, -70);
        spawnPos[3] = new Vector3(+70, 0, +70);


        characterDatas = DataManager.instance.GetCharacterDatas();

        character = new GameObject("Character");

        foreach (KeyValuePair<int, CharacterData> data in characterDatas)
        {
            GameObject characterPrefab = data.Value.prefab;

            characterPrefabs.Add((CharacterKey)data.Value.key, characterPrefab);//���⼭ DataManager�� ���� key��, prefab �ְ��ְ�
        }

        CreateCharacter(CharacterKey.KNIGHT,4);
        CreateCharacter(CharacterKey.DOGNIGHT, 2);
        CreateCharacter(CharacterKey.SPEARMAN, 2);
        CreateCharacter(CharacterKey.WIZARD, 3);
        CreateCharacter(CharacterKey.GRUNT, 1);

        //CreateCharacter(CharacterKey.TURTLE, 3);
        //CreateCharacter(CharacterKey.SLIME, 12);
        //CreateCharacter(CharacterKey.MUSHROOM, 8);
        //CreateCharacter(CharacterKey.CACTUS, 8);
        //CreateCharacter(CharacterKey.BEHOLDER, 8);
        //CreateCharacter(CharacterKey.GOLEM, 16);
        //CreateCharacter(CharacterKey.USURPER, 1);

        SpawnArmies();
        SpawnEnmies();
    }
    public void SetMinimapPosition()
    {
        foreach (CharacterKey key in characterPools.Keys)
        {
            if (key >= CharacterKey.TURTLE) return;

            foreach (GameObject obj in characterPools[key])
            {
                if (obj.activeSelf)
                {
                    MiniMap.Instance.AddPosition(obj.transform.position);
                }
            }
        }
    }

    // �߰�(����� �ʿ� �ڿ������;���)
    public CharacterData GetCharacterData(int key)
    {
        //if(characterDatas.ContainsKey(key))
        return characterDatas[key];
    }

    // �߰�
    public void CreateCharacter(int key, Vector3 position, Vector3 rallyPoint = new Vector3())
    {
        GameObject obj = Instantiate(characterPrefabs[(CharacterKey)key], this.character.transform);
        obj.transform.position = position;
        Character character = obj.GetComponent<Character>();
        character.SetData(characterDatas[(int)key]);
        obj.SetActive(true);
        characterPools[(CharacterKey)key].Add(obj);

        if (rallyPoint.Equals(Vector3.zero))
        {
            // not move
        }
    }

    private void CreateCharacter(CharacterKey key, int poolCount = 1)
    {
        List<GameObject> temp = new List<GameObject>();

        for (int i = 0; i < poolCount; i++)
        {
            GameObject obj = Instantiate(characterPrefabs[key], this.character.transform);

            Character character = obj.GetComponent<Character>();

            character.SetData(characterDatas[(int)key]);
            obj.SetActive(false);

            temp.Add(obj);
        }
        characterPools.Add(key, temp);
    }

    private void SpawnArmies()
    {
        foreach (CharacterKey key in characterPools.Keys)
        {
            if (key >= CharacterKey.TURTLE) return;

            foreach (GameObject obj in characterPools[key])
            {
                if (!obj.activeSelf)
                {
                    obj.transform.position = new Vector3(Random.Range(-15, 15), 0.0f,
                        Random.Range(25, 40));
                    obj.SetActive(true);
                }
            }
        }
    }


    private void SpawnEnmies()
    {
        int spawnCount = 0;

        foreach (CharacterKey key in characterPools.Keys)
        {
            if (key < CharacterKey.TURTLE) continue;

            foreach (GameObject obj in characterPools[key])
            {
                if (!obj.activeSelf)
                {
                    obj.transform.position = new Vector3(Random.Range(spawnPos[spawnCount].x - 10f, spawnPos[spawnCount].x + 10f), 0,
                        Random.Range(spawnPos[spawnCount].x - 10f, spawnPos[spawnCount].x + 10f));
                    obj.SetActive(true);
                    spawnCount++;
                    if (spawnCount > 3)
                        spawnCount = 0;
                }
            }
        }
    }

    public Dictionary<CharacterKey, List<GameObject>> GetCharacters() { return characterPools; }
    
    public Dictionary<CharacterKey, List<GameObject>> GetEnemyData()
    {
        Dictionary<CharacterKey, List<GameObject>> temp = new Dictionary<CharacterKey, List<GameObject>>();

        foreach (CharacterKey key in characterPools.Keys)
        {            
            if (key>=CharacterKey.TURTLE)
            {
                temp.Add(key, characterPools[key]);
            }
        }
        return temp;
    }

    public Vector3 GetArmyPos(GameObject enmy)
    {
        Vector3 pos = enmy.gameObject.transform.position;

        foreach (CharacterKey key in characterPools.Keys)
        {
            foreach (GameObject obj in characterPools[key])
            {
                if (Vector3.Distance(obj.gameObject.transform.position, enmy.gameObject.transform.position) < 10.0f)
                { 
                    return obj.gameObject.transform.position - new Vector3(1,0,1); 
                }
            }
        }
        return pos;
    }

    // �߰�
    public void GetObjectsContainedInRect(Rect rect)
    {
        foreach (CharacterKey key in characterPools.Keys)
        {
            if (key >= CharacterKey.TURTLE) continue;
            foreach (GameObject obj in characterPools[key])
            {
                // Ȱ��ȭ �Ǿ����� ���� ������Ʈ���� �˻� ���� ����
                if (!obj.activeSelf) 
                    continue;

                // �Ʊ���ü�� ���鼭 ������ �ڽ��� �߰�
                if(rect.Contains(Camera.main.WorldToScreenPoint(obj.transform.position)))
                {
                    SelectionBox.instance.AddSelectObject((int)key, obj);
                }
            }
        }
    }

    public void SetAttackCityHall(int dayCount)
    {
        if (dayCount % 3 == 0)      
            isEvnentOn = true;
        else
            isEvnentOn = false;

        if(isEvnentOn)
        {
            foreach (CharacterKey key in characterPools.Keys)
            {
                if (key < CharacterKey.TURTLE) continue;

                int count = characterPools[key].Count;

                for (int i = 0; i<count; i+=1)
                {
                    GameObject obj = characterPools[key][i];
                        
                    Enemy enemy = obj.GetComponent<Enemy>();
                    
                    if(!enemy.isMainEvent)
                        enemy.isMainEvent = true;

                    //enemy.Move(enemy.targetPos);
                }
            }
            dCount++;
        }
        
    }
}