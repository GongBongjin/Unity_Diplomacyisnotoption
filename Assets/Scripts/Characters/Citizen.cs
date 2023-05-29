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
        Felling,    // 벌목
        Building,   // 건설
        Fix,        // 수리
        Hoeing,     // 농사
        Mining     // 채광
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

    int product;

    float workSpeed = 0.1f;     // 작업 속도




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
            // 우클릭시 이동
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
        // workState 검사해서 행동 루틴 반복

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

    // 작업 내용 체크
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

    // 생산 명령
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
        // 이동
        // 겜 오브젝트 받아서 생산?
        // 겜 오브젝트에 생산 물품 정보 있으니 받아서 설정하면 될 것 같은뎅
        // 나무, 돌, 광석 등 위치 < - > 저장소
        FindStoreHouse();
    }

    // 건설 및 수리
    public void BuildingOrder(GameObject obj)
    {
        workTarget = obj;
        targetBuilding = obj.GetComponent<Building>();
        // 완공상태 검사
        if (targetBuilding.GetIsCompletion())
        {
            // 수리
            SetCitizenWorkState(CitizenState.Fix);
        }
        else
        {
            // 건설
            SetCitizenWorkState(CitizenState.Building);
        }

        Vector3 dir = (transform.position - obj.transform.position).normalized;
        // maxrixSize * gridSize * 0.5f 
        // 이부분 수정
        Vector3 offsetPos = obj.transform.position + dir * (3 * 5 * 0.5f);
        MoveDestination(offsetPos);
    }

    // 생산시 가까운 저장소 찾기
    private void FindStoreHouse()
    {
        // 작업 하던 곳 주변에서 가장 가까운 곳
        // 작업이 끝날때마다 검사하는게 좋을 듯
        // 건물 돌면서 가장 가까운 오브젝트 찾기
        // 없으면 시청이지
        // storeHouse = 
    }

    // 작업하던 것 이어서 작업
    private void FindWolkSpace()
    {
        // 작업 하던 곳 주변에서 가장 가까운 곳
        // 오브젝트 돌면서 작업할 위치 찾기
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
