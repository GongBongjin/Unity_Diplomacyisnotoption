using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Army : Character
{   
    protected override void Start()
    {
        
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Move(Vector3 destPos)
    {
        if (isAttacking)
            return;

        animator.SetFloat("MoveSpeed", 3.0f);
        nvAgent.SetDestination(destPos);
    }

    public override void Attack()
    {
        
    }
}
