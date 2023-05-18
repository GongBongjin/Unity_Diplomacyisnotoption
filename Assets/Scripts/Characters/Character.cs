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
    IDLE, MOVE, ATTACK, HITTED, DEAD
}

public class Character : MonoBehaviour
{ 
    [HideInInspector] public Animator animator;
    [HideInInspector] public NavMeshAgent nvAgent;

    public CharacterData data;

    public float hp;
    public float maxHp;
    public bool isAttacking = false;

    protected void Awake()
    {
        animator = GetComponent<Animator>();
        nvAgent = GetComponent<NavMeshAgent>();
    }

    protected void Start()
    {
    }

    protected void Update()
    {
        StopMove();

        AnimationTest();
    }

    private void AnimationTest()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            isAttacking = true;
            animator.SetTrigger("Attack");
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            animator.SetTrigger("Hitted");
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            animator.SetTrigger("Dead");
        }
    }

    public void SetData(CharacterData characterData)
    {
        data = characterData;
    }

    public void Move(Vector3 destPos)
    {
        if (isAttacking)
            return;

        animator.SetFloat("MoveSpeed", 3.0f);
        nvAgent.SetDestination(destPos);
    }

    private void StopMove()
    {
        float distance = Vector3.Distance(gameObject.transform.position, nvAgent.destination);
        if (distance < 1.0f)
            animator.SetFloat("MoveSpeed", 0.0f);
    }

    private void EndAttack()
    {
        isAttacking = false;
    }
}