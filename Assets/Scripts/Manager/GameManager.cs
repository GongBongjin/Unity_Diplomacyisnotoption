using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1920, 1080, true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0))
            UIManager.Instance.IncreaseMaxPopulation(10);
        if (Input.GetKeyDown(KeyCode.Keypad1))
            UIManager.Instance.IncreaseMaxStorage(100);
        if (Input.GetKeyDown(KeyCode.Keypad2))
            UIManager.Instance.IncreasesResources(Product.FOOD, 10);
        if (Input.GetKeyDown(KeyCode.Keypad3))
            UIManager.Instance.IncreasesResources(Product.WOOD, 10);
        if (Input.GetKeyDown(KeyCode.Keypad4))
            UIManager.Instance.IncreasesResources(Product.STONE, 10);
        if (Input.GetKeyDown(KeyCode.Keypad5))
            UIManager.Instance.IncreasesResources(Product.COPPER, 10);
    }
}
