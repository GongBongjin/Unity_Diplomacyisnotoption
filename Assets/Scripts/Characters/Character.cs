using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEditor.Build;
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
    [HideInInspector] protected Animator animator;
    [HideInInspector] protected NavMeshAgent nvAgent;
    [HideInInspector] protected Rigidbody rigidBody;
    [HideInInspector] protected CapsuleCollider bodyCollider;
    [HideInInspector] protected SphereCollider atkCollider;
    [HideInInspector] protected GameObject selectCircle;


    [SerializeField] protected float hp;
    [SerializeField] protected float maxHp;
    [SerializeField] protected bool isAttacking = false;
    [SerializeField] protected bool isDying = false;
    [SerializeField] protected bool isSelected = false;

    protected CharacterData data;
    public CharacterKey key;
    protected CharacterType characterType;

    protected void Awake()
    {
        animator = GetComponent<Animator>();
        nvAgent = GetComponent<NavMeshAgent>();
        rigidBody = GetComponent<Rigidbody>();
        bodyCollider = GetComponent<CapsuleCollider>();
        atkCollider = GetComponent<SphereCollider>();
        atkCollider.enabled = false;
        selectCircle = transform.Find("Circle").gameObject;
        selectCircle.SetActive(false);
    }

    protected virtual void Start()
    {
        bodyCollider.enabled = true;
        atkCollider.enabled = false;
    }

    protected virtual void Update()
    {
        ShowSelectionCircle();

        ApporachDestination();
    }

    public void SetData(CharacterData characterData)
    {
        data = characterData;
        key = (CharacterKey)data.key;
        characterType = data.characterType;
        maxHp = data.maxHp;
        hp = maxHp;
    }

    private void ShowSelectionCircle()
    {
        if(isSelected)
            selectCircle.SetActive(true);
        else
            selectCircle.SetActive(false);
    }

    private void ApporachDestination()
    {
        float distance = Vector3.Distance(gameObject.transform.position, nvAgent.destination);
        if (distance < 1.0f || isAttacking)
            animator.SetFloat("MoveSpeed", 0.0f);
    }

    public CharacterData GetCharacterData() { return  data; }

    public void SetSelectOption(bool isSelected) { this.isSelected = isSelected; }

    //Animation Event Function

    private void StartAttack()
    {
        atkCollider.enabled = true;
    }
    private void EndAttack()
    {
        isAttacking = false;

        atkCollider.enabled = false;
    }

    private void StartDead()
    {
        bodyCollider.enabled = false;
        atkCollider.enabled = false;
    }

    private void EndDead()
    {
        gameObject.SetActive(false);
    }

    //Children class Function
    public virtual void Move(Vector3 destPos) { }
    public virtual void Attack() { }

    //Collision Function
    public virtual void OnTriggerEnter(Collider other) { }





    /*
     army & enmey 공통부분

    이동시에 적군을 만나면 공격하는 함수
    아군 - 히트당하면
    적군 - range 안에 들어오면

    target 위치받아서 이동 -> 공격함수 실행
     
     */
}