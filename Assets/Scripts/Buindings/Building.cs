using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    int key;

    Material material;

    // Building Base Height
    float maxBuildTime = 30;
    float buildTime = 0;

    int matrixSize;    // build matrix size

    float maxHP;
    float curHP;

    // ����
    bool isCompletion = false;  // �ǹ� �ϰ� ����
    bool isRepairDone = false;  // �����ʿ�?


    GameObject selectCircle;

    private void Awake()
    {
        material = transform.GetChild(0).GetComponent<MeshRenderer>().material;

        selectCircle = transform.Find("Circle").gameObject;
        SetSelectObject(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetBuildingProperty(int key, int matrixSize, int maxHP, int buildTime)
    {
        this.key = key;
        this.matrixSize = matrixSize;
        this.maxHP = maxHP;
        this.maxBuildTime = buildTime;
        material.SetFloat("_GridStepValue", 0.5f);
    }

    public int GetKey()
    {
        return key;
    }

    public int GetMatrixSize()
    {
        return matrixSize;
    }

    public bool GetIsCompletion()
    {
        return isCompletion;
    }    
    public bool GetIsRepairDone()
    {
        return true;
    }

    public void SetSelectObject(bool isSelected)
    {
        selectCircle.SetActive(isSelected);
    }

    public void BuildUpBuilding(float value)
    {
        float increaseValue = value * Time.deltaTime;
        buildTime += increaseValue;
        curHP += increaseValue / maxBuildTime * maxHP;
        material.SetFloat("_GridStepValue", buildTime);
        // �ϼ�
        if(buildTime >= maxBuildTime)
        {
            buildTime = maxBuildTime;
            if(curHP > maxHP)
                curHP = maxHP;
            isCompletion = true;
        }
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
