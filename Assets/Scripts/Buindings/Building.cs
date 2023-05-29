using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    float maxBuildTime = 30;
    float buildTime = 0;

    int matrixSize;    // build matrix size

    float maxHP;
    float curHP;

    // 유닛
    bool isCompletion = false;  // 건물 완공 상태
    bool isRepairDone = false;  // 수리필요?

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool GetIsCompletion()
    {
        return isCompletion;
    }    
    public bool GetIsRepairDone()
    {
        return true;
    }

    public void BuildUpBuilding(float value)
    {
        buildTime += value;
        // 완성
        if(buildTime >= maxBuildTime)
        {
            buildTime = maxBuildTime;
            isCompletion = true;
            return true;
        }
        return false;
    }

    public bool RepairBuilding(float value)
    {
        curHP += value;
        if(curHP > maxHP) 
        { 
            curHP = maxHP;
            return true;
        }
        return false;
    }


    public void BeAttacked(float damage)
    {
        curHP -= damage;
        if(curHP <= 0)
        {
            curHP = 0;
            // destroy
        }
    }
}
