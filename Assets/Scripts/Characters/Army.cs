using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Army : Character
{
    private Enemy target;
    private Vector3 patrolCurrentPos;
    private Vector3 patrolPos;

    private bool isTest = true;
    public bool isPatrol;
    public bool isTargeting;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        PatrolMove();

        ApporachDestination();

        base.Update();
    }

    public override void Move(Vector3 destPos)
    {
        if (!isTargeting)
        {
            if (isAttacking) return;
        }

        if (isDying) return;

        animator.SetFloat("MoveSpeed", 3.0f);
        nvAgent.SetDestination(destPos);

        //Debug.Log("cibla" + nvAgent.destination);
        //Debug.Log(nvAgent.SetDestination(destPos));

        //float distance = Vector3.Distance(transform.position, destPos);
        //if (Vector3.Distance(transform.position, destPos) < 1.0f || isAttacking)
        //{
        //    animator.SetFloat("MoveSpeed", 0.0f);
        //    nvAgent.isStopped = true;
        //    nvAgent.ResetPath();
        //}

        isTargeting = false;
    }

    private void ApporachDestination()
    {
        if (isDying) return;

        float distance = Vector3.Distance(transform.position, nvAgent.destination);
        if (distance < 1.0f || isAttacking)
        {
            animator.SetFloat("MoveSpeed", 0.0f);
            nvAgent.isStopped = true;
            nvAgent.ResetPath();
        }
    }

    public override void Attack()
    {
        if (isAttacking) return;
        if (isDying) return;

        nvAgent.isStopped = true;
        nvAgent.ResetPath();
        animator.SetTrigger("Attack");
    }

    public override void OnTriggerEnter(Collider other)
    {
        if(isDying) return;

        target = other.gameObject.GetComponent<Enemy>();

        if (other.gameObject.transform.tag == "Enemy")
        {
            if (isPatrol) 
                isPatrol = false;
            if (other.GetType() == typeof(SphereCollider))
            {
                //if(target.GetAttacking())//
                    hp -= target.GetDmg() - (target.GetDmg()*(def/100));

                if (hp <= 0)
                {
                    isDying = true;
                    animator.SetTrigger("Dead");
                }
                isHitted = true;
                if (isHitted)
                    Move(target.transform.position);

            }

            else if(other.GetType() == typeof(CapsuleCollider))
            {
                isHitted = false;
                transform.LookAt(target.transform.position);
                Attack();
            }
        }
    }

    //SelectionBox Command Functions
    public void StopCommand()
    {
        if (isDying) return;

        nvAgent.isStopped = true;
        nvAgent.ResetPath();
    }

    public void AttackCommand(Enemy enemy)
    {
        target = enemy;

        Move(target.transform.position);
    }

    public void PatrolCommand(Vector3 nextPos)
    {
        isPatrol = true;

        patrolCurrentPos = transform.position;
        patrolPos = nextPos;
    }

    private void PatrolMove()
    {
        if(isDying) return;

        if(isPatrol)
        {
            if(isTest)
            {     
                nvAgent.SetDestination(patrolPos);

                animator.SetFloat("MoveSpeed", 3.0f);

                if (Vector3.Distance(transform.position, patrolPos) < 1.0f)
                {
                    isTest = false;

                    animator.SetFloat("MoveSpeed", 0.0f);

                    //nvAgent.ResetPath();
                }
            }

            else
            {
                nvAgent.SetDestination(patrolCurrentPos);

                animator.SetFloat("MoveSpeed", 3.0f);

                if (Vector3.Distance(transform.position, patrolCurrentPos) < 1.0f)
                {
                    isTest = true;

                    animator.SetFloat("MoveSpeed", 0.0f);

                    //nvAgent.ResetPath();
                }
            }
        }
    }
}
