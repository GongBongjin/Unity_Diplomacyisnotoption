using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class Citizen : MonoBehaviour
{
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

    GameObject workTarget = null;     // �۾���
    GameObject storageHouse = null;    // �����

    Building targetBuilding = null;         // �Ǽ�, ����
    ProductObject targetProduct = null;     // ���, ����, ä��

    int product;

    float workSpeed = 0.1f;     // �۾� �ӵ�




    private void Awake()
    {
        animator = GetComponent<Animator>();
        nvAgent = GetComponent<NavMeshAgent>();

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
        if(Input.GetMouseButtonDown(1))
        {
            // ��Ŭ���� �̵�
            Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);
            Ray ray = Camera.main.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 destPos = new Vector3(hit.point.x, 0, hit.point.z);

                Debug.Log(hit.transform.name);
                MoveDestination(destPos);
                //nvAgent.destination = desPos;
                //selectedObject.transform.position = desPos;
            }
        }
        // workState �˻��ؼ� �ൿ ��ƾ �ݺ�

        WorkRoutine();
    }
    
    void WorkRoutine()
    {
        if (!workTarget) return;

        switch (workState)
        {
            case CitizenState.Felling:
                break;
            case CitizenState.Building:
                targetBuilding.BuildUpBuilding(workSpeed);
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

    // �۾� ���� üũ
    void CheckWorkState()
    {
        switch (workState)
        {
            case CitizenState.Felling:
                break;
            case CitizenState.Building:
                SetCitizenAnimationState(CitizenState.Building);
                animator.SetBool("Build", true);
                break;
            case CitizenState.Fix:
                SetCitizenAnimationState(CitizenState.Fix);
                animator.SetBool("Fix", true);
                break;
            case CitizenState.Hoeing:
                break;
            case CitizenState.Mining:
                break;
        }
    }

    void MoveDestination(Vector3 destination)
    {
        nvAgent.destination = destination;
        SetCitizenAnimationState(CitizenState.Move);
        animator.SetBool("Move", true);
        SetCitizenWorkState(CitizenState.None);
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
            SetCitizenWorkState(CitizenState.Fix);
        }
        else
        {
            // �Ǽ�
            SetCitizenWorkState(CitizenState.Building);
        }

        Vector3 dir = (transform.position - obj.transform.position).normalized;
        // maxrixSize * gridSize * 0.5f 
        // �̺κ� ����
        Vector3 offsetPos = obj.transform.position + dir * (3 * 5 * 0.5f);
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
            CheckWorkState();
            nvAgent.ResetPath();
            animator.SetBool("Move", false);
        }
    }
}
