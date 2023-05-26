using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    GameObject workPlace = null;     // �۾���
    GameObject storeHouse = null;    // �����

    int product;

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
                Vector3 desPos = new Vector3(hit.point.x, 0, hit.point.z);

                Debug.Log(hit.transform.name);
                MoveDestination(desPos);
                //nvAgent.destination = desPos;
                //selectedObject.transform.position = desPos;
            }
        }
        // workState �˻��ؼ� �ൿ ��ƾ �ݺ�
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

    void MoveDestination(Vector3 destination)
    {
        nvAgent.destination = destination;

        SetCitizenWorkState(CitizenState.None);
    }

    // ���� ���
    void ProductionOrder(GameObject obj)
    {
        workPlace = obj;
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
    void BuildingOrder(GameObject building)
    {
        // ���� ������Ʈ�� �̵�
        // �ǹ� ���� �ִϸ��̼� ����

        //SetCitizenWorkState(CitizenState.Fix);
        //SetCitizenWorkState(CitizenState.Building);
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
}
