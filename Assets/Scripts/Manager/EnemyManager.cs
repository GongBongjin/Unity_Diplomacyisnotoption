//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CharacterManager;

public class EnemyManager : MonoBehaviour
{
    static public EnemyManager instance;

    private Vector3 destPos;

    private Enemy enmy;

    private Dictionary<CharacterKey, List<GameObject>> enmiesPools = new Dictionary<CharacterKey, List<GameObject>>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        enmiesPools = CharacterManager.instance.GetEnemyData();   
    }

    void Update()
    {
        //SetDestination();
        //
        //Attack();
    }
    public void SetMinimapPosition()
    {
        foreach (CharacterKey key in enmiesPools.Keys)
        {
            foreach (GameObject obj in enmiesPools[key])
            {
                if (obj.activeSelf)
                {
                    MiniMap.Instance.AddPosition(obj.transform.position, false);
                }
            }
        }
    }

    private void SetDestination()
    {
        foreach (CharacterKey key in enmiesPools.Keys)
        { 
            foreach (GameObject obj in enmiesPools[key])
            {
                destPos = CharacterManager.instance.GetArmyPos(obj);
                enmy = obj.GetComponent<Enemy>();
                enmy.Move(destPos);
            }
        }
    }

    private void Attack()
    {
        foreach (CharacterKey key in enmiesPools.Keys)
        {
            foreach (GameObject obj in enmiesPools[key])
            {
                enmy = obj.GetComponent<Enemy>();
    
                if (Vector3.Distance(enmy.gameObject.transform.position, destPos) < 1.0f)
                    enmy.Attack();
            }
        }
    }
}
