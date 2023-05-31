using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    int key;

    Material material;
    float maxBuildTime = 30;
    float buildTime = 0;
    int matrixSize;    // build matrix size

    float maxHP;
    float curHP;

    // 유닛
    bool isCompletion = false;  // 건물 완공 상태
    //bool isRepairDone = false;  // 수리필요?

    GameObject selectCircle;
    const float createOffset = 3.0f;
    Vector3 rallyPoint = Vector3.zero;

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
        // 완성
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

    public void CreateUnit(int key)
    {
        Vector3 pos = FindUnitPlacement();
        CitizenManager.Instance.CreateCharacter(pos, rallyPoint);
    }

    private Vector3 FindUnitPlacement()
    {
        float baseLength = matrixSize * GridManager.GRID_SIZE * 0.5f;
        Vector3 direction = Vector3.back;
        Vector3 basePos = transform.position + Vector3.up;
        float dist = baseLength;
        for (int i = 0; i > -360; i-=2)
        {
            // 건물 주변 사각형
            float angle = Mathf.Abs(i) % 45;
            if((i / 45) % 2 == 0)
                angle = angle * (Mathf.PI / 180);
            else
                angle = (46 - angle) * (Mathf.PI / 180);

            // 거리
            dist = baseLength / Mathf.Cos(angle);

            // 방향
            Quaternion rot = Quaternion.Euler(0, i, 0);
            direction = rot * Vector3.back;

            RaycastHit[] hits = Physics.RaycastAll(basePos, direction, dist + createOffset);
            
            if (hits.Length < 1)
                break;
        }

        return transform.position + (direction * (dist + createOffset));
    }
}
