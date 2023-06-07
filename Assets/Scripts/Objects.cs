using System.Collections;
using UnityEngine;

public class Objects : MonoBehaviour
{
    protected bool isSelected = false;
    [HideInInspector] protected GameObject selectCircle;
    protected HpBar hpBar;

    //public void SetSelectOption (bool isSelected) { this.isSelected = isSelected; }

    public void SetSelectObject(bool isSelected)
    {
        this.isSelected = isSelected;
        selectCircle.SetActive(isSelected);
        hpBar.SetActiveProgressBar(isSelected);
    }

    public void SelectObjectFlicker()
    {
        StartCoroutine(Flicker());
    }

    IEnumerator Flicker()
    {
        SetSelectObject(true);
        yield return new WaitForSeconds(0.5f);
        SetSelectObject(false);
    }

    protected void Update()
    {
        hpBar.transform.LookAt(Camera.main.transform.forward);
        
    }
}
