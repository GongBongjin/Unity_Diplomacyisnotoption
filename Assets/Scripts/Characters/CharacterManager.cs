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
        
    }
}
