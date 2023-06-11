using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    static public LightManager instance;

    private Light light;
    private float rotAngle = 30.0f;
    private int dayCount = 1;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        light = GetComponent<Light>();
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateLight();
    }

    private void UpdateLight()
    {
        rotAngle += Time.deltaTime;// * 3.0f;

        light.transform.rotation = Quaternion.Euler(rotAngle, 90, 0);

        UIManager.Instance.ChangeTime(rotAngle);

        if (rotAngle > 225.0f)
        {
            rotAngle = 30.0f;
            dayCount++;
            UIManager.Instance.UpdateDay(dayCount);
        }

        if(light.transform.rotation.x > 30)
        {
            Debug.Log(rotAngle);
            return;
        }
    }
}
