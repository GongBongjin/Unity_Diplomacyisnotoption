using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectionBox : MonoBehaviour
{
    public static SelectionBox instance;

    Image selectionBox;

    private bool isMoveable = true;
    // CharacterKey-> int �� ��ȯ
    //public Dictionary<CharacterKey, List<GameObject>> selectedObjects = new Dictionary<CharacterKey, List<GameObject>>(); //���� list ������ GameObject�� �ƴ� Character �������ٰ� �����͸� �־ Ű����������.
    public Dictionary<int, List<GameObject>> selectedObjects = new Dictionary<int, List<GameObject>>();

    //Drag
    private Vector3 dragStartPos;
    private bool isDragging = false;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        selectionBox = GetComponent<Image>();
        selectionBox.enabled = false;
    }

    void Update()
    {
        Select();
        Move();
    }

    private void Select()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                dragStartPos = Input.mousePosition;
                ClearSelection();
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {

            if (!EventSystem.current.IsPointerOverGameObject())
            {

                if (!isDragging)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    // RayCast All�� ��ȯ
                    // selectedObjects ��� singleSelectObject �� ���� ����
                    RaycastHit[] hits = Physics.RaycastAll(ray);
                    bool isSelected = false;
                    GameObject obj;
                    int key;
                    // �� �κ� �ٲ�����ϴ�.
                    // Charcter�� ������ �ù�, ����, ������Ʈ ���� �����ϵ��� �ӽ���ġ�س����
                    for (int i = 0; i < hits.Length; i++)
                    {
                        string tag = hits[i].transform.tag;
                        obj = hits[i].transform.gameObject;
                        switch (tag)
                        {
                            case "Army":
                                isSelected = true;
                                key = (int)obj.GetComponent<Character>().key;
                                AddSelectObject(key, obj);
                                break;
                            case "Enemy":
                                isMoveable = false;
                                isSelected = true;
                                key = (int)obj.GetComponent<Character>().key;
                                AddSelectObject(key, obj);
                                break;
                            case "Citizen":
                                isSelected = true;
                                Citizen citizen = obj.GetComponent<Citizen>();
                                citizen.SetSelectObject(true);
                                AddSelectObject(1000, obj);
                                break;
                            case "Building":
                                isMoveable = false;
                                isSelected = true;
                                Building building = obj.GetComponent<Building>();
                                building.SetSelectObject(true);
                                key = building.GetKey();
                                AddSelectObject(key, obj);
                                break;
                            case "Product":
                                isMoveable = false;
                                isSelected = true;
                                // ����, ���� �� �ӽ���ġ
                                break;
                        }
                        if (isSelected) break;
                    }
                    //RaycastHit hit;
                    //
                    //if (Physics.Raycast(ray, out hit))
                    //{
                    //    if (hit.collider.gameObject.tag == "Army")
                    //        SelectObject(hit.collider.gameObject);
                    //}

                }

                isDragging = false;
                selectionBox.enabled = false;
                DataReturn();
            }
        }
        if (Input.GetMouseButton(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (!isDragging && Vector3.Distance(Input.mousePosition, dragStartPos) > 5.0f)
                    isDragging = true;
                
                if (isDragging)
                    UpdateDragBox(Input.mousePosition);
            }
        }
    }

    private void ClearSelection()
    {
        foreach (int key in selectedObjects.Keys)
        {
            foreach (GameObject obj in selectedObjects[key])
            {
                string tag = obj.transform.tag;
                //if (obj.gameObject.transform.tag == "Army")
                //{
                //    army = obj.GetComponent<Army>();
                //    army.SetSelectOption(false);
                //}
                switch (tag)
                {
                    case "Army":
                        obj.GetComponent<Character>().SetSelectOption(false);
                        break;
                    case "Enemy":
                        obj.GetComponent<Character>().SetSelectOption(false);
                        break;
                    case "Citizen":
                        obj.GetComponent<Citizen>().SetSelectObject(false);
                        break;
                    case "Building":
                        obj.GetComponent<Building>().SetSelectObject(false);
                        break;
                    case "Product":
                        //obj.GetComponent<ProductObject>().SetSelectObject(false);
                        break;
                }
            }
        }
        selectedObjects.Clear();
        isMoveable = true;
    }

    private void UpdateDragBox(Vector3 currentMousePos)
    {
        Vector3 lowerLeft = Vector3.Min(dragStartPos, currentMousePos);
        Vector3 upperRight = Vector3.Max(dragStartPos, currentMousePos);
        Rect selectionRect = new Rect(lowerLeft.x, lowerLeft.y, upperRight.x - lowerLeft.x, upperRight.y - lowerLeft.y);

        selectionBox.enabled = true;
        selectionBox.transform.position = selectionRect.center;
        selectionBox.rectTransform.sizeDelta = selectionRect.size;

        selectedObjects.Clear();
        CharacterManager.instance.GetObjectsContainedInRect(selectionRect);
        // -------------------------------------------------
        CitizenManager.Instance.GetObjectsContainedInRect(selectionRect);
    }

    // �Ʒ� �Լ� ����
    public void AddSelectObject(int key, GameObject gameObject)
    {
        if (selectedObjects.ContainsKey(key))
        {
            selectedObjects[key].Add(gameObject);
        }
        else
        {
            selectedObjects.Add(key, new List<GameObject>());
            selectedObjects[key].Add(gameObject);
        }
    }

    private void SelectObject(GameObject gameObject)
    {
        List<GameObject> temp = new List<GameObject>();

        Character character = gameObject.GetComponent<Character>();
        
        if (selectedObjects.ContainsKey((int)character.key))
        {
            int i = selectedObjects[(int)character.key].Count;
            selectedObjects[(int)character.key].Insert(i, gameObject);
        }
        else 
        {
            temp.Add(gameObject);
            selectedObjects.Add((int)character.key, temp);
        }
    }

    public void InputCommandKey(char hotKey)
    {
        switch (hotKey)
        {
            case 'M':
                // true�϶� ��Ŭ���� �̵�
                break;
            case 'H':
                // Stop
                break;
            case 'F':
                // ���� ��
                break;
            case 'P':
                // ����
                break;
        }
    }

    private void Move()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (!isMoveable) return;
            if (selectedObjects.Count != 0)
            {
                Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);
                Ray ray = Camera.main.ScreenPointToRay(mousePos);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    Vector3 destPos = new Vector3(hit.point.x, 0, hit.point.z);
                    MoveSelectedObjects(destPos);
                }
            }
        }
    }

    private void DataReturn()
    {
        int[] keys = new int[selectedObjects.Count];
        int[] counts = new int[selectedObjects.Count];
        int idx = 0;

        foreach (int key in selectedObjects.Keys)
        {
            keys[idx] = (int)key;
            counts[idx] = selectedObjects[key].Count;
            idx++;

            foreach (GameObject obj in selectedObjects[key])
            {
                if(obj.tag.Equals("Army") || obj.tag.Equals("Enemy"))
                {
                    Character character = obj.GetComponent<Character>();
                    character.SetSelectOption(true);
                }
            }
        }
        UIManager.Instance.ShowInformation(keys, counts);
    }


    public void SelectCharacterKey(int keyValue)
    {
        List<GameObject> objs = selectedObjects[keyValue];
        ClearSelection();
        selectedObjects.Add(keyValue, objs);
        DataReturn();
    }


    private void MoveSelectedObjects(Vector3 destPos)
    {
        int verticalCount = -15;
        int horizontalCount = 0;

        foreach (int key in selectedObjects.Keys)
        {
            if(key == 1000)
            {
                foreach (GameObject obj in selectedObjects[key])
                {
                    obj.GetComponent<Citizen>().MoveDestination(destPos);
                }
                continue;
            }
            foreach (GameObject obj in selectedObjects[key])
            {
                //verticalCount+=6;
                Army army = obj.GetComponent<Army>();

                //if (verticalCount > 15)
                //{
                //    verticalCount = -15;
                //    horizontalCount+=6;
                //}
                //
                //destPos = SetDestPos(verticalCount, horizontalCount, destPos);

                army.Move(destPos);
            }
        }
    }


    private Vector3 SetDestPos(int verticalCount, int horizontalCount, Vector3 mousePos)
    {
        Vector3 pos = new Vector3(mousePos.x + verticalCount, 0, mousePos.z + horizontalCount);

        return pos;
    }
}