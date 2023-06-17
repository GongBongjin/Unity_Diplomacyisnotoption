
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Citizen : Objects
{
    const int gridSize = 5;
    enum CitizenState
    {
        None,
        Idle,
        Move,
        Die,
        Felling,    // 벌목
        Building,   // 건설
        Fix,        // 수리
        Hoeing,     // 농사
        Mining      // 채광
    }
    CitizenState animState = CitizenState.Idle;
    CitizenState workState = CitizenState.None;
    Animator animator;
    NavMeshAgent nvAgent;
    
    [SerializeField, Tooltip("RightHand-JointItemR")] 
    Transform rightHand;
    GameObject[] tools = new GameObject[4]; // 도끼, 망치, 괭이, 곡괭이

    GameObject workTarget = null;     // 작업장
    GameObject storageHouse = null;    // 저장소

    Building targetBuilding = null;         // 건설, 수리
    ProductObject targetProduct = null;     // 농사, 벌목, 채광

    Product product;
    int output;

    int primaryKey;
    float maxHP;
    float curHP;
    float workSpeed = 1.0f;     // 작업 속도

    bool isProduction = false;
    bool isCarry = false;
    //bool isMove = false;
    float moveSpeed = 0.0f;
    bool isBuild = false;

    float workSpaceRadius = 50.0f;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        nvAgent = GetComponent<NavMeshAgent>();

        selectCircle = transform.Find("Circle").gameObject;
        hpBar = transform.Find("HpBar").GetComponent<HpBar>();
        selectCircle.SetActive(false);

        if (rightHand == null)
            Debug.Log("<color=red>RightHand is Null</color>");

        tools[0] = rightHand.Find("Axe").gameObject;
        tools[1] = rightHand.Find("Hammer").gameObject;
        tools[2] = rightHand.Find("Hoe").gameObject;
        tools[3] = rightHand.Find("PickAxe").gameObject;
    }

    void Start()
    {
    }

    void Update()
    {

        moveSpeed = nvAgent.velocity.magnitude / nvAgent.speed;
        animator.SetFloat("MoveSpeed", moveSpeed);
        WorkRoutine();
    }

    public int GetKey()
    {
        return CitizenManager.Instance.GetCitizenKey();
    }

    public void SetCitizenProperty(int key, float maxHP, float workSpeed)
    {
        this.primaryKey = key;
        this.maxHP = maxHP;
        curHP = maxHP;
        this.workSpeed = workSpeed;
    }

    void WorkRoutine()
    {
        switch (workState)
        {
            case CitizenState.Felling:
                Production();
                break;
            case CitizenState.Hoeing:
                Production();
                break;
            case CitizenState.Mining:
                Production();
                break;
            case CitizenState.Building:
                if(isBuild)
                {
                    // 건설 중일 때 빌딩에게 건설중임을 알린다.
                    // 빌딩에서 점차 완공을한다.
                    targetBuilding.BuildUpBuilding(workSpeed);
                }
                if(targetBuilding.GetIsCompletion())
                {
                    isBuild = false;
                    animator.SetBool("Build", isBuild);
                    int key = targetBuilding.GetKey();

                    SetCitizenWorkState(CitizenState.None);
                }
                break;
            case CitizenState.Fix:
                targetBuilding.RepairBuilding(workSpeed);
                break;
        }
    }

    private void Production()
    {
        if(!workTarget)
        {
            if (FindWorkSpace())
            {
                if (!isCarry)
                    ProductionOrder(workTarget);
            }
            else
                SetCitizenWorkState(CitizenState.None);
        }

        if (isProduction)
        {
            output = targetProduct.Production(workSpeed);
        }
        if (output < 0)
        {
            // Product 고갈 & 파괴
            Debug.Log("Product 고갈");
            output = (-output) + 1;

            if (!FindWorkSpace()) 
                SetCitizenWorkState(CitizenState.None);
        }
        if (!isCarry && output > 0)
        {
            isCarry = true;
            isProduction = false;
            animator.SetBool("Felling", false);
            animator.SetBool("Mining", false);
            FindStoreHouse();
        }
    }

    void SetCitizenAnimationState(CitizenState state)
    {
        if (animState == state) return;

        animState = state;
    }
    void SetCitizenWorkState(CitizenState state)
    {
        if (workState == state) return;
        workState = state;
        if (workState == CitizenState.None)
        {
            //Debug.Log("None");
            SetCitizenWorkState(CitizenState.None);
            workTarget = null;
            isCarry = false;
            isBuild = false;
            isProduction = false;
            animator.SetBool("Felling", false);
            animator.SetBool("Build", false);
            animator.SetBool("Fix", false);
            animator.SetBool("Mining", false);
        }
    }

    // 작업장 도착 후 내용 체크
    void CheckWorkState()
    {
        //Debug.Log("WorkState Check : " + workState);
        switch (workState)
        {
            case CitizenState.Felling:
                SetCitizenAnimationState(CitizenState.Felling);
                isProduction = true;
                animator.SetBool("Felling", isProduction);
                break;
            case CitizenState.Building:
                SetCitizenAnimationState(CitizenState.Building);
                isBuild = true;
                animator.SetBool("Build", isBuild);
                break;
            case CitizenState.Fix:
                SetCitizenAnimationState(CitizenState.Fix);
                animator.SetBool("Fix", true);
                break;
            case CitizenState.Hoeing:
                SetCitizenAnimationState(CitizenState.Hoeing);
                isProduction = true;
                animator.SetBool("Mining", isProduction);
                break;
            case CitizenState.Mining:
                SetCitizenAnimationState(CitizenState.Mining);
                isProduction = true;
                animator.SetBool("Mining", isProduction);
                break;
        }
    }

    public void StopWork()
    {
        Debug.Log("Stop");
        workTarget = null;
        switch (workState)
        {
            case CitizenState.Felling:
                isProduction = false;
                animator.SetBool("Felling", isProduction);
                break;
            case CitizenState.Building:
                isBuild = false;
                animator.SetBool("Build", isBuild);
                break;
            case CitizenState.Fix:
                animator.SetBool("Fix", false);
                break;
            case CitizenState.Hoeing:
                isProduction = false;
                animator.SetBool("Mining", isProduction);
                break;
            case CitizenState.Mining:
                isProduction = false;
                animator.SetBool("Mining", isProduction);
                break;
        }
        SetCitizenWorkState(CitizenState.None);
    }

    public void MoveDestination(Vector3 destination)
    {
        nvAgent.velocity = Vector3.zero;
        nvAgent.destination = destination;
        SetCitizenAnimationState(CitizenState.Move);
    }

    // 생산 명령
    public void ProductionOrder(GameObject obj)
    {
        StopWork();
        workTarget = obj;
        targetProduct = obj.GetComponent<ProductObject>();
        product = targetProduct.GetProductName();
        output = 0;
        switch(product)
        {
            case Product.FOOD:
                SetCitizenWorkState(CitizenState.Hoeing);
                SelectTools(2);
                break;
            case Product.WOOD:
                SetCitizenWorkState(CitizenState.Felling);
                SelectTools(0);
                break;
            case Product.STONE:
                SetCitizenWorkState(CitizenState.Mining);
                SelectTools(3);
                break;
            case Product.COPPER:
                SetCitizenWorkState(CitizenState.Mining);
                SelectTools(3);
                break;
        }
        MoveDestination(workTarget.transform.position);
    }

    // 건설 및 수리
    public void BuildingOrder(GameObject obj)
    {
        StopWork();
        //isBuild = false;
        workTarget = obj;
        targetBuilding = obj.GetComponent<Building>();
        SelectTools(1);
        // 완공상태 검사

        if (targetBuilding.GetIsCompletion())
        {
            // 수리
            //Debug.Log("Citizen Fix Order");
            SetCitizenWorkState(CitizenState.Fix);
        }
        else
        {
            // 건설
            //Debug.Log("Citizen Building Order");
            SetCitizenWorkState(CitizenState.Building);

        }

        Vector3 dir = (transform.position - obj.transform.position).normalized;
        // maxrixSize * gridSize * 0.5f 
        // 이부분 수정
        Vector3 offsetPos = obj.transform.position + dir * (targetBuilding.GetMatrixSize() * gridSize * 0.4f);
        MoveDestination(offsetPos);
    }

    // 생산시 가까운 저장소 찾기
    private void FindStoreHouse()
    {
        storageHouse = BuildManager.Instance.GetNearestStorage(transform.position);
        Vector3 dir = (transform.position - storageHouse.transform.position).normalized;
        Vector3 offsetPos = storageHouse.transform.position + dir * (storageHouse.GetComponent<Building>().GetMatrixSize() * gridSize * 0.4f);
        MoveDestination(offsetPos);
    }

    // 작업하던 것 이어서 작업
    private bool FindWorkSpace()
    {
        Vector3 pos = transform.position + Vector3.up;
        Collider[] hits = Physics.OverlapSphere(pos, workSpaceRadius);
        GameObject tempTarget = null;
        float minDistance = workSpaceRadius * 2;
        ProductObject productObj;
        for (int i = 0; i < hits.Length; i++)
        {
            // 생산품목인지 검사
            if (!hits[i].tag.Equals("Product"))
                continue;

            if (hits[i].gameObject.Equals(workTarget))
                continue;
            // 생산하던 것과 똑같은 품목인지 검사
            productObj = hits[i].GetComponent<ProductObject>();
            if (!productObj.GetProductName().Equals(product))
                continue;

            // 최소 작업거리 검사
            float dist = Vector3.Distance(transform.position, hits[i].transform.position);
            if(dist < minDistance)
            {
                tempTarget = hits[i].gameObject;
                minDistance = dist;
            }
        }

        if (targetProduct.GetKey() == 3000)
            BuildManager.Instance.DestroyBuilding(2002, workTarget);
        else
            Destroy(workTarget);
        // 작업거리 안에 같은 작업이 있으면 이어서 진행
        if (tempTarget != null)
        {
            workTarget = tempTarget;
            targetProduct = tempTarget.GetComponent<ProductObject>();
            return true;
        }
        else
        {
            // 없으면 작업 중지
            //Debug.Log("못찾음?");
            workTarget = null;
            targetProduct = null;
            return false;
        }
    }

    public void SelectTools(int idx)
    {
        for(int i = 0; i < tools.Length; i++)
        {
            tools[i].SetActive(false);
            if(i == idx)
                tools[i].SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(workTarget != null)
        {
            if (other.gameObject.Equals(workTarget))
            {
                //isMove = false;
                //animator.SetBool("Move", isMove);
                CheckWorkState();
                //nvAgent.ResetPath();
                MoveDestination(transform.position);
            }
        }

        if(isCarry)
        {
            if(other.gameObject.Equals(storageHouse))
            {
                nvAgent.velocity = Vector3.zero;
                isCarry = false;
                UIManager.Instance.IncreasesResources(product, output);
                output = 0;
                if (workTarget != null)
                {
                    //Debug.Log("work : " + workState.ToString() + ", workTarget :" + workTarget.name);
                    MoveDestination(workTarget.transform.position);
                }
                else
                {
                    SetCitizenWorkState(CitizenState.None);
                }

            }
        }
    }
}
