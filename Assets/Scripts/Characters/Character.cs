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

    public CharacterData data;

    [SerializeField] protected float hp;
    [SerializeField] protected float maxHp;
    [SerializeField] protected bool isAttacking = false;
    [SerializeField] protected bool isDying = false;
    protected CharacterType characterType;

    protected void Awake()
    {
        animator = GetComponent<Animator>();
        nvAgent = GetComponent<NavMeshAgent>();
        rigidBody = GetComponent<Rigidbody>();
        bodyCollider = GetComponent<CapsuleCollider>();
        atkCollider = GetComponent<SphereCollider>();
    }

    protected virtual void Start()
    {
        bodyCollider.enabled = true;
        atkCollider.enabled = false;
    }

    protected virtual void Update()
    {
        ApporachDestination();
    }

    public void SetData(CharacterData characterData)
    {
        data = characterData;
        maxHp = data.maxHp;
        hp = maxHp;
        characterType = data.characterType;
    }

    private void ApporachDestination()
    {
        float distance = Vector3.Distance(gameObject.transform.position, nvAgent.destination);
        if (distance < 1.0f)
            animator.SetFloat("MoveSpeed", 0.0f);
    }

    //Animation Event Function

    private void StatAttack()
    {
        atkCollider.enabled = true;
    }
    private void EndAttack()
    {
        isAttacking = false;

        //atkCollider.enabled = false;
    }

    private void EndDead()
    {
        bodyCollider.enabled = false;
        atkCollider.enabled = false;
        gameObject.SetActive(false);
    }

    //Children class Function
    public virtual void Move(Vector3 destPos) { }
    public virtual void Attack() { }

    //Collision Function
    public virtual void OnTriggerEnter(Collider other) { }
}