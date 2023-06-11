
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
        Felling,    // ����
        Building,   // �Ǽ�
        Fix,        // ����
        Hoeing,     // ���
        Mining      // ä��
    }
    CitizenState animState = CitizenState.Idle;
    CitizenState workState = CitizenState.None;
    Animator animator;
    NavMeshAgent nvAgent;
    
    [SerializeField, Tooltip("RightHand-JointItemR")] 
    Transform rightHand;
    GameObject[] tools = new GameObject[4]; // ����, ��ġ, ����, ���

    GameObject workTarget = null;     // �۾���
    GameObject storageHouse = null;    // �����

    Building targetBuilding = null;         // �Ǽ�, ����
    ProductObject targetProduct = null;     // ���, ����, ä��

    Product product;
    int output;

    bool isProduction = false;
    bool isCarry = false;
    //bool isMove = false;
    float moveSpeed = 0.0f;
    float workSpeed = 1.0f;     // �۾� �ӵ�
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

    void WorkRoutine()
    {
        switch (workState)
        {
            case CitizenState.Felling:
                Production();
                if (output > 0)
                {
                    animator.SetBool("Felling", false);
                }
                break;
            case CitizenState.Hoeing:
                Production();
                if (output > 0)
                {
                    animator.SetBool("Mining", false);
                }
                break;
            case CitizenState.Mining:
                Production();

                if (output > 0)
                {
                    animator.SetBool("Mining", false);
                }
                break;
            case CitizenState.Building:
                if(isBuild)
                {
                    // �Ǽ� ���� �� �������� �Ǽ������� �˸���.
                    // �������� ���� �ϰ����Ѵ�.
                    targetBuilding.BuildUpBuilding(workSpeed);
                }
                if(targetBuilding.GetIsCompletion())
                {
                    isBuild = false;
                    animator.SetBool("Build", isBuild);
                    int key = targetBuilding.GetKey();

                    if (key == 2001)
                        UIManager.Instance.IncreaseMaxPopulation(10);
                    if (key == 2003)
                        UIManager.Instance.IncreaseMaxStorage(100);
                    SetCitizenWorkState(CitizenState.Idle);
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
                SetCitizenWorkState(CitizenState.Idle);
        }

        if (isProduction)
        {
            output = targetProduct.Production(workSpeed);
        }
        if (output == 0)
            return;
        if (output < 0)
        {
            // Product �� & �ı�
            Debug.Log("Product ��");
            output = (-output) + 1;

            if (!FindWorkSpace())
                SetCitizenWorkState(CitizenState.Idle);
        }
        if (!isCarry && output > 0)
        {
            isCarry = true;
            isProduction = false;
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
        Debug.Log("WorkState : " + state);
        workState = state;
    }

    // �۾��� ���� �� ���� üũ
    void CheckWorkState()
    {
        Debug.Log("WorkState Check : " + workState);
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
        SetCitizenWorkState(CitizenState.Idle);
    }

    public void MoveDestination(Vector3 destination)
    {
        nvAgent.velocity = Vector3.zero;
        nvAgent.destination = destination;
        SetCitizenAnimationState(CitizenState.Move);
    }

    // ���� ���
    public void ProductionOrder(GameObject obj)
    {
        StopWork();
        workTarget = obj;
        targetProduct = obj.GetComponent<ProductObject>();
        product = targetProduct.GetProductName();
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

    // �Ǽ� �� ����
    public void BuildingOrder(GameObject obj)
    {
        StopWork();
        //isBuild = false;
        workTarget = obj;
        targetBuilding = obj.GetComponent<Building>();
        SelectTools(1);
        // �ϰ����� �˻�

        if (targetBuilding.GetIsCompletion())
        {
            // ����
            Debug.Log("Citizen Fix Order");
            SetCitizenWorkState(CitizenState.Fix);
        }
        else
        {
            // �Ǽ�
            Debug.Log("Citizen Building Order");
            SetCitizenWorkState(CitizenState.Building);

        }

        Vector3 dir = (transform.position - obj.transform.position).normalized;
        // maxrixSize * gridSize * 0.5f 
        // �̺κ� ����
        Vector3 offsetPos = obj.transform.position + dir * (targetBuilding.GetMatrixSize() * gridSize * 0.4f);
        MoveDestination(offsetPos);
    }

    // ����� ����� ����� ã��
    private void FindStoreHouse()
    {
        storageHouse = BuildManager.Instance.GetNearestStorage(workTarget.transform.position);
        Vector3 dir = (transform.position - storageHouse.transform.position).normalized;
        Vector3 offsetPos = storageHouse.transform.position + dir * (storageHouse.GetComponent<Building>().GetMatrixSize() * gridSize * 0.4f);
        MoveDestination(offsetPos);
    }

    // �۾��ϴ� �� �̾ �۾�
    private bool FindWorkSpace()
    {
        Vector3 pos = transform.position + Vector3.up;
        Collider[] hits = Physics.OverlapSphere(pos, workSpaceRadius);
        GameObject tempTarget = null;
        float minDistance = workSpaceRadius * 2;
        ProductObject productObj;
        for (int i = 0; i < hits.Length; i++)
        {
            // ����ǰ������ �˻�
            if (!hits[i].tag.Equals("Product"))
                continue;

            if (hits[i].gameObject.Equals(workTarget))
                continue;
            // �����ϴ� �Ͱ� �Ȱ������� �˻�
            productObj = hits[i].GetComponent<ProductObject>();
            if (!productObj.GetProductName().Equals(product))
                continue;

            //Debug.Log(hits[i].tag);
            // �ּ� �۾��Ÿ� �˻�
            float dist = Vector3.Distance(transform.position, hits[i].transform.position);
            if(dist < minDistance)
            {
                //Debug.Log(dist + " < minDistance");
                tempTarget = hits[i].gameObject;
                minDistance = dist;
            }
        }

        Destroy(workTarget);
        // �۾��Ÿ� �ȿ� ���� �۾��� ������ �̾ ����
        if (tempTarget != null)
        {
            workTarget = tempTarget;
            targetProduct = tempTarget.GetComponent<ProductObject>();
            return true;
        }
        else
        {
            // ������ �۾� ����
            Debug.Log("��ã��?");
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
                    Debug.Log("work : " + workState.ToString() + ", workTarget :" + workTarget.name);
                    MoveDestination(workTarget.transform.position);
                }
                else
                {
                    Debug.Log("�� Null?");
                }

            }
        }
    }
}
