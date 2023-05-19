using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    static public CharacterManager instance;   

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

    private GameObject army;

    private Dictionary<int, CharacterData> characterDatas = new Dictionary<int, CharacterData>();
    private Dictionary<CharacterKey, GameObject> characterPrefabs = new Dictionary<CharacterKey, GameObject>();
    private Dictionary<CharacterKey, List<GameObject>> characterPools = new Dictionary<CharacterKey, List<GameObject>>();

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        characterDatas = DataManager.instance.GetCharacterDatas();

        army = new GameObject("ArmyCharacter");

        foreach (KeyValuePair<int, CharacterData> data in characterDatas)
        {
            GameObject characterPrefab = Resources.Load<GameObject>(data.Value.prefab);

            characterPrefabs.Add((CharacterKey)data.Value.key, characterPrefab);
        }

        CreateArmies(CharacterKey.KNIGHT,1);
        CreateArmies(CharacterKey.DOGNIGHT, 1);
        CreateArmies(CharacterKey.SPEARMAN, 1);
        CreateArmies(CharacterKey.WIZARD, 1);
        CreateArmies(CharacterKey.GRUNT,1);

    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void CreateArmies(CharacterKey key, int poolCount)
    {
        List<GameObject> temp = new List<GameObject>();

        for (int i = 0; i < poolCount; i++)
        {
            GameObject obj = Instantiate(characterPrefabs[key], army.transform);

            Character character = obj.GetComponent<Character>();

            character.SetData(characterDatas[(int)key]);
            obj.transform.position = new Vector3(Random.Range(-20f, 10f), 0, Random.Range(-10f, 20f));
            obj.SetActive(true);

            temp.Add(obj);
        }
        characterPools.Add(key, temp);
    }
   
}