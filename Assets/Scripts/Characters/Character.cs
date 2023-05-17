using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum CharacterType
{
    CITIZEN = 1,
    ARMY,
    ENMEY
}

public enum CharacterState
{
    IDLE, MOVE, ATTACK, DEAD
}

public class Character : MonoBehaviour
{
    protected Rigidbody rigidBody;
    protected Animator animator;
    protected NavMeshAgent nvAgent;

    protected GameObject selectedObject;

    protected float hp;
    protected float maxHp;
    protected Vector3 desPos;

    private Dictionary<int, CharacterData> characterDatas = new Dictionary<int, CharacterData>();

    protected void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        nvAgent = GetComponent<NavMeshAgent>();
    }

    protected void Start()
    {
        characterDatas = DataManager.instance.GetCharacterDatas();
    }

    protected void Update()
    {
        Move();
    }

    protected void Move()
    {
        if (Input.GetMouseButtonDown(0)) // ��Ŭ��
        {
            Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if(hit.collider.gameObject.tag == "Army" || hit.collider.gameObject.tag == "Citizen")
                    selectedObject = hit.collider.gameObject;
            }
        }
        else if (Input.GetMouseButtonDown(1)) // ��Ŭ��
        {
            if (selectedObject != null)
            {
                Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);
                Ray ray = Camera.main.ScreenPointToRay(mousePos);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    desPos = new Vector3(hit.point.x, 0, hit.point.z);
                    animator.SetFloat("MoveSpeed", 3.0f);
                    nvAgent.destination = desPos;
                }
            }
        }
        float distance = Vector3.Distance(gameObject.transform.position, nvAgent.destination);
        if (distance < 1.0f)
            animator.SetFloat("MoveSpeed", 0.0f);
    }
}