using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Objects : MonoBehaviour
{
    protected bool isSelected = false;
    [HideInInspector] protected GameObject selectCircle;
    protected HpBar hpBar;

    //public void SetSelectOption (bool isSelected) { this.isSelected = isSelected; }

    public void SetSelectObject(bool isSelected)
    {
        selectCircle.SetActive(isSelected);
        hpBar.SetActiveProgressBar(isSelected);
    }

    protected void Update()
    {
        hpBar.transform.LookAt(Camera.main.transform.forward);
        
    }
}
