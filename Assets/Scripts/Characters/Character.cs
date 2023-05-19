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
    [HideInInspector] protected Animator animator;
    [HideInInspector] protected NavMeshAgent nvAgent;
    [HideInInspector] protected CapsuleCollider bodyCollider;
    //[HideInInspector] protected CapsuleCollider atkCollider;

    public CharacterData data;

    protected float hp;
    protected float maxHp;
    protected bool isAttacking = false;
    protected CharacterType characterType;

    protected void Awake()
    {
        animator = GetComponent<Animator>();
        nvAgent = GetComponent<NavMeshAgent>();
        bodyCollider = GetComponent<CapsuleCollider>();
    }

    protected virtual void Start()
    {
        maxHp = data.maxHp;
        hp = maxHp;
        characterType = data.characterType;
    }

    protected virtual void Update()
    {
        ApporachDestination();

        AnimationTest();
    }

    public void SetData(CharacterData characterData)
    {
        data = characterData;
    }

    private void AnimationTest()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            //isAttacking = true;
            animator.SetTrigger("Attack");
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            animator.SetTrigger("Dead");
        }
    }

    private void ApporachDestination()
    {
        float distance = Vector3.Distance(gameObject.transform.position, nvAgent.destination);
        if (distance < 1.0f)
            animator.SetFloat("MoveSpeed", 0.0f);
    }

    //Collision Function


    //Animation Event Function
    private void EndAttack()
    {
        isAttacking = false;

        //atkCollider.enabled = false
    }

    private void EndDead()
    {
        gameObject.SetActive(false);
    }

    //Children class Function
    public virtual void Move(Vector3 destPos) { }
    public virtual void Attack() { }
}