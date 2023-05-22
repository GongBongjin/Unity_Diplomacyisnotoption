using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    private Vector3 targetPos;

    protected override void Start()
    {
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Move(Vector3 destPos)
    {
        if (isAttacking) return;

        animator.SetFloat("MoveSpeed", 3.0f);
        nvAgent.SetDestination(destPos);

        //atkCollider�� �浹���� ��, ���� �ִϸ��̼� ����
    }

    public override void Attack()
    {
        if (isAttacking) return;

        isAttacking = true;
        nvAgent.isStopped = true;
        nvAgent.ResetPath();
        animator.SetTrigger("Attack");
    }
}
