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

        animator.SetTrigger("Attack");
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Enemy") return;
        if(isDying) return;

        if (other.gameObject.tag == "Enemy")
        {
            hp -= 10.0f;

            Debug.Log(characterType);
            if (hp <= 0)
            {
                isDying = true;
                //atkCollider.enabled = false;
                //bodyCollider.enabled = false;
                animator.SetTrigger("Dead");
            }
        }

    }

}
