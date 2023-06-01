using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBar : MonoBehaviour
{
    MeshRenderer meshRenderer;
    Material material;

    [SerializeField] private Color backColor = Color.black;
    [SerializeField] private Color frontColor = Color.green;

    [SerializeField] private bool isShow;

    [SerializeField, Range(0.0f, 1.0f)] float fillRate;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.material;
        SetBackColor(backColor);
        SetFrontColor(frontColor);
        SetActiveProgressBar(false);
    }

    private void Update()
    {
        if (!isShow) return;

        transform.LookAt(Camera.main.transform.position, Vector3.down);
    }
    

    public void SetActiveProgressBar(bool active)
    {
        isShow = active;
        meshRenderer.enabled = isShow;
    }


    public void SetBackColor(Color color)
    {
        backColor = color;
        material.SetColor("_BackColor", backColor);
    }
    public void SetFrontColor(Color color)
    {
        frontColor = color;
        material.SetColor("_FrontColor", frontColor);
    }

    public void SetProgressBar(float fillRate)
    {
        // 0 to 1
        this.fillRate = fillRate;
        material.SetFloat("_FillRate", fillRate);
    }

}
