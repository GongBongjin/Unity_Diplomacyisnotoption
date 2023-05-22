using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CharacterManager;

public class EnemyManager : MonoBehaviour
{
    static public EnemyManager instance;

    private Vector3 destPos;

    private Enemy enmy;

    private Dictionary<CharacterKey, List<GameObject>> characterPools = new Dictionary<CharacterKey, List<GameObject>>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        characterPools = CharacterManager.instance.GetEnemyData();
        SetDestination();
    }

    // Update is called once per frame
    void Update()
    {
        Attack();
    }

    private void SetDestination()
    {
        foreach(CharacterKey key in characterPools.Keys)
        { 
            foreach (GameObject obj in characterPools[key])
            {
                destPos = new Vector3(10, 0, 10);
                enmy = obj.GetComponent<Enemy>();
                enmy.Move(destPos);
            }
        }
    }

    private void Attack()
    {
        foreach (CharacterKey key in characterPools.Keys)
        {
            foreach (GameObject obj in characterPools[key])
            {
                enmy = obj.GetComponent<Enemy>();

                if(Vector3.Distance(obj.transform.position, destPos) <2.0f)
                    enmy.Attack();
            }
        }
    }
}
