using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    private Vector3 targetPos;
    private SphereCollider rangeCollider;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Move(Vector3 destPos)
    {
        if (isAttacking) return;
        if (isDying) return;

        animator.SetFloat("MoveSpeed", 3.0f);
        nvAgent.SetDestination(destPos);
    }

    public override void Attack()
    {
        if (isAttacking) return;
        if (isDying) return;

        isAttacking = true;
        nvAgent.isStopped = true;
        nvAgent.ResetPath();
        animator.SetTrigger("Attack");
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (isDying) return;
    }
}
