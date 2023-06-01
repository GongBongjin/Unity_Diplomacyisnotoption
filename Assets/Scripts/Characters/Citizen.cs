using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class Citizen : MonoBehaviour
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
        Mining     // ä��
    }
    CitizenState animState = CitizenState.Idle;
    CitizenState workState = CitizenState.None;
    Animator animator;
    NavMeshAgent nvAgent;
    
    [SerializeField, Tooltip("RightHand-JointItemR")] 
    Transform rightHand;
    GameObject[] tools = new GameObject[4]; // ����, ��ġ, ����, ���

    GameObject selectCircle;

    HpBar hpBar;

    GameObject workTarget = null;     // �۾���
    GameObject storageHouse = null;    // �����

    Building targetBuilding = null;         // �Ǽ�, ����
    ProductObject targetProduct = null;     // ���, ����, ä��

    int product;

    bool isMove = false;
    float workSpeed = 1.0f;     // �۾� �ӵ�
    bool isBuild = false;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        nvAgent = GetComponent<NavMeshAgent>();
        hpBar = transform.Find("HpBar").GetComponent<HpBar>();

        if (rightHand == null)
            Debug.Log("<color=red>RightHand is Null</color>");

        tools[0] = rightHand.Find("Axe").gameObject;
        tools[1] = rightHand.Find("Hammer").gameObject;
        tools[2] = rightHand.Find("Hoe").gameObject;
        tools[3] = rightHand.Find("PickAxe").gameObject;

        selectCircle = transform.Find("Circle").gameObject;
    }

    void Start()
    {
        SetSelectObject(false);
    }

    void Update()
    {
        //if(Input.GetMouseButtonDown(1))
        //{
        //    // ��Ŭ���� �̵�
        //    Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);
        //    Ray ray = Camera.main.ScreenPointToRay(mousePos);
        //
        //    if (Physics.Raycast(ray, out RaycastHit hit))
        //    {
        //        Vector3 destPos = new Vector3(hit.point.x, 0, hit.point.z);
        //
        //        MoveDestination(destPos);
        //    }
        //}

        if (isMove)
        {
            if (nvAgent.remainingDistance <= 0.0f)
            {
                isMove = false;
                animator.SetBool("Move", isMove);
            }
        }
        // workState �˻��ؼ� �ൿ ��ƾ �ݺ�

        WorkRoutine();
    }

    public void SetSelectObject(bool isSelected) 
    { 
        selectCircle.SetActive(isSelected);
        hpBar.SetActiveProgressBar(isSelected);
    }

    void WorkRoutine()
    {
        if (!workTarget) return;

        switch (workState)
        {
            case CitizenState.Felling:
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
                    SetCitizenWorkState(CitizenState.Idle);
                }
                break;
            case CitizenState.Fix:
                targetBuilding.RepairBuilding(workSpeed);
                break;
            case CitizenState.Hoeing:
                break;
            case CitizenState.Mining:
                break;
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
    }

    // �۾��� ���� �� ���� üũ
    void CheckWorkState()
    {
        Debug.Log("WorkState Check : " + workState);
        switch (workState)
        {
            case CitizenState.Felling:
                break;
            case CitizenState.Building:
                SetCitizenAnimationState(CitizenState.Building);
                isBuild = true;
                animator.SetBool("Build", isBuild);
                break;
            case CitizenState.Fix:
                Debug.Log("Citizen Work Fix");
                SetCitizenAnimationState(CitizenState.Fix);
                animator.SetBool("Fix", true);
                break;
            case CitizenState.Hoeing:
                break;
            case CitizenState.Mining:
                break;
        }
    }

    public void MoveDestination(Vector3 destination)
    {
        nvAgent.destination = destination;
        SetCitizenAnimationState(CitizenState.Move);
        isMove = true;
        animator.SetBool("Move", isMove);
    }

    // ���� ���
    void ProductionOrder(GameObject obj)
    {
        workTarget = obj;
        Product product = obj.GetComponent<ProductObject>().GetProductName();
        switch(product)
        {
            case Product.FOOD:
                SetCitizenWorkState(CitizenState.Hoeing);
                break;
            case Product.WOOD:
                SetCitizenWorkState(CitizenState.Felling);
                break;
            case Product.STONE:
                SetCitizenWorkState(CitizenState.Mining);
                break;
            case Product.COPPER:
                SetCitizenWorkState(CitizenState.Mining);
                break;
        }
        // �̵�
        // �� ������Ʈ �޾Ƽ� ����?
        // �� ������Ʈ�� ���� ��ǰ ���� ������ �޾Ƽ� �����ϸ� �� �� ������
        // ����, ��, ���� �� ��ġ < - > �����
        FindStoreHouse();
    }

    // �Ǽ� �� ����
    public void BuildingOrder(GameObject obj)
    {
        workTarget = obj;
        targetBuilding = obj.GetComponent<Building>();
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
        // �۾� �ϴ� �� �ֺ����� ���� ����� ��
        // �۾��� ���������� �˻��ϴ°� ���� ��
        // �ǹ� ���鼭 ���� ����� ������Ʈ ã��
        // ������ ��û����
        // storeHouse = 
    }

    // �۾��ϴ� �� �̾ �۾�
    private void FindWolkSpace()
    {
        // �۾� �ϴ� �� �ֺ����� ���� ����� ��
        // ������Ʈ ���鼭 �۾��� ��ġ ã��
        // workPlace = 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.Equals(workTarget))
        {
            Debug.Log("Citizen Work Target : " + workTarget.name);
            isMove = false;
            animator.SetBool("Move", isMove);
            CheckWorkState();
            nvAgent.ResetPath();
        }
    }
}
