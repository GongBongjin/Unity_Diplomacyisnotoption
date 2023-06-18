using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;
using UnityEngine;
using UnityEngine.AI;

public class Building : Objects
{
    int primaryKey;

    Material material;
    float maxBuildTime = 30;
    float buildTime = 0;
    int matrixSize;    // build matrix size

    float maxHP;
    float curHP;

    bool isCompletion = false;  // 건물 완공 상태
    bool isRepairDone = false;  // 수리필요?

    int maxProduction = 7;
    bool isProduction = false;
    List<int> productObject;
    int characterKey;
    float maxProductTime = 10f;
    float productTime = 0;

    const float destroyTime = 5.0f;
    bool isDestroy = false;

    const float createOffset = 5.0f;
    Vector3 rallyPoint = Vector3.zero;

    const int maxParticleCount = 10;
    GameObject[] fireParticle;

    private void Awake()
    {
        productObject = new List<int>();
        material = transform.GetChild(0).GetComponent<MeshRenderer>().material;

        selectCircle = transform.Find("Circle").gameObject;
        hpBar = transform.Find("HpBar").GetComponent<HpBar>();
        selectCircle.SetActive(false);

        fireParticle = new GameObject[maxParticleCount];
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ProductionUnit();
        if (isSelected && Input.GetMouseButtonDown(1))
        {
            Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit hit;
        
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 destPos = new Vector3(hit.point.x, 0, hit.point.z);
                rallyPoint = destPos;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            BeAttacked(10);
        }
    }

    public void SetBuildingProperty(int key, int matrixSize, int maxHP, int buildTime)
    {
        this.primaryKey = key;
        this.matrixSize = matrixSize;
        this.maxHP = maxHP;
        this.maxBuildTime = buildTime;
        hpBar.SetProgressBar(curHP/ maxHP);
        material.SetFloat("_GridStepValue", 0.5f);
    }

    public int GetKey()
    {
        return primaryKey;
    }

    public int GetMatrixSize()
    {
        return matrixSize;
    }

    public bool GetIsCompletion()
    {
        return isCompletion;
    }    

    public void SetIsCompletion(bool isComplete)
    {
        isCompletion = isComplete;
    }
    public bool GetIsRepairDone()
    {
        return true;
    }

    public bool GetIsDestroy()
    {
        return isDestroy;
    }

    public void ResetHP()
    {
        curHP = maxHP;
        hpBar.SetProgressBar(curHP / maxHP);
    }

    public void PlacementComplete(bool placement)
    {
        transform.GetComponent<Collider>().enabled = placement;
        transform.GetComponent<NavMeshObstacle>().enabled = placement;
    }
    public void BuildUpBuilding(float value)
    {
        if (isCompletion) return;
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
            if (primaryKey == 2001)
                UIManager.Instance.IncreaseMaxPopulation(10);
            if (primaryKey == 2003)
                UIManager.Instance.IncreaseMaxStorage(100);
        }
        hpBar.SetProgressBar(curHP / maxHP);
    }

    public void RepairBuilding(float value)
    {
        curHP += value;
        if(curHP > maxHP) 
        { 
            curHP = maxHP;
            isRepairDone = false;
        }
        hpBar.SetProgressBar(curHP / maxHP);
        int count = 10 - (int)(curHP / maxHP * 10);
        ActiveBuildingFireParticle(count);
    }


    public void BeAttacked(float damage)
    {
        if (isDestroy) return;
        curHP -= damage;
        isRepairDone = true;
        if (curHP <= 0)
        {
            isDestroy = true;
            curHP = 0;
            hpBar.SetProgressBar(0);
            ActiveBuildingFireParticle(0);
            Vector3 size = new Vector3(matrixSize * GridManager.GRID_SIZE, 1, matrixSize * GridManager.GRID_SIZE);
            SmokeParticleManager.Instance.ActiveParticle(transform.position, size);
            StartCoroutine(DestroyMoving());
            //GetComponent<Animator>().SetBool("Destroy", isDestroy);
        }
        else
        {
            hpBar.SetProgressBar(curHP / maxHP);
            int count = 10 - (int)(curHP / maxHP * 10);
            ActiveBuildingFireParticle(count);
        }
    }

    IEnumerator DestroyMoving()
    {
        float time = 0;
        float destroyValue = maxBuildTime / destroyTime;
        Debug.Log(maxBuildTime + " : " + destroyTime + ", = " + destroyValue);

        while (time < destroyTime)
        {
            yield return null;
            time += Time.deltaTime;
            transform.position += Vector3.down * destroyValue * Time.deltaTime;
        }
        EndDestroy();
    }

    public void EndDestroy()
    {
        if (primaryKey == 2001)
            UIManager.Instance.IncreaseMaxPopulation(-10);
        if (primaryKey == 2003)
            UIManager.Instance.IncreaseMaxStorage(-100);
        BuildManager.Instance.DestroyBuilding(primaryKey, gameObject);
    }

    private void ActiveBuildingFireParticle(int count)
    {
        for(int i = 0; i < maxParticleCount; i++)
        {
            if(i < count)
            {
                if (fireParticle[i] == null)
                {
                    //float basicSize = matrixSize * GridManager.GRID_SIZE * 0.5f;
                    Mesh _Mesh = transform.GetChild(0).GetComponent<MeshFilter>().mesh;
                    Vector3 pos = transform.position + _Mesh.vertices[UnityEngine.Random.Range(0, _Mesh.vertices.Length)];
                    fireParticle[i] = FireParticleManager.Instance.ActiveParticle(pos);
                }
            }
            else
            {
                if (fireParticle[i] != null)
                {
                    fireParticle[i].SetActive(false);
                    fireParticle[i] = null;
                }
            }
        }
    }

    public bool GetIsProduction()
    {
        return isProduction;
    }

    public int[] GetProductionKeys()
    {
        int[] keys = new int[maxProduction];
        for(int i = 0; i < maxProduction; i++)
        {
            if(i < productObject.Count)
                keys[i] = productObject[i];
            else
                keys[i] = 0;
        }
        return keys;
    }

    public float GetProductionProgress()
    {
        return productTime / maxProductTime;
    }

    private void ProductionUnit()
    {
        if (!isProduction)
        {
            if(productObject.Count > 0)
            {
                characterKey = productObject[0];
                isProduction = true;
            }
            else
            {
                return;
            }
        }

        productTime += Time.deltaTime;
        //if(productionTime >= characterData.productionTime)
        if (productTime >= maxProductTime)
        {
            Production(characterKey);
            productObject.RemoveAt(0);
            productTime = 0;
            isProduction = false;
        }
    }

    public void AddUnitProduct(int key)
    {
        if (!isCompletion) 
            return;

        if (productObject.Count >= maxProduction)
            return;

        int population = 1;
        int food = 0;
        int wood = 0;
        int stone = 0;
        int copper = 0;
        if(key == 1000)
        {
            CitizenData data = CitizenManager.Instance.GetCitizenData();
            food = data.food;
            wood = data.wood;
            stone = data.stone;
            copper = data.copper;
        }
        else
        {
            CharacterData data = CharacterManager.instance.GetCharacterData(key);
            food = data.food;
            wood = data.wood;
            stone = data.stone;
            copper = data.copper;
        }
        // 리소스가 충분한지 확인
        if (UIManager.Instance.CheckRemainingResources(population, food, wood, stone, copper))
        {
            UIManager.Instance.SpendResources(population, food, wood, stone, copper);
            productObject.Add(key);
        }
    }

    public void Production(int key)
    {
        Vector3 pos = FindUnitPlacement();
        if(key < 1000)
        { }
        else if (key == 1000)
            CitizenManager.Instance.CreateCharacter(pos, rallyPoint);
        else
            CharacterManager.instance.CreateCharacter(key, FindUnitPlacement(), rallyPoint);
    }

    private Vector3 FindUnitPlacement()
    {
        float baseLength = matrixSize * GridManager.GRID_SIZE * 0.5f;
        Vector3 direction = Vector3.forward;
        Vector3 basePos = transform.position + Vector3.up;
        float dist = baseLength;
        for (int i = 0; i > -360; i-=3)
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
            direction = rot * Vector3.forward;

            RaycastHit[] hits = Physics.RaycastAll(basePos, direction, dist + createOffset);
            
            if (hits.Length < 1)
                break;
        }

        return transform.position + (direction * (dist + createOffset));
    }
}
