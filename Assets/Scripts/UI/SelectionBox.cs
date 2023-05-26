using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectionBox : MonoBehaviour
{
    public static SelectionBox instance;

    Image selectionBox;

    public Dictionary<CharacterKey, List<GameObject>> selectedObjects = new Dictionary<CharacterKey, List<GameObject>>(); //현재 list 구조라서 GameObject가 아닌 Character 구조에다가 데이터를 넣어서 키정보가없음.

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
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.collider.gameObject.tag == "Army")
                            SelectObject(hit.collider.gameObject);

                        if(hit.collider.gameObject.tag == "Enemy")
                                SelectObject(hit.collider.gameObject);
                    }
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
        foreach (CharacterKey key in selectedObjects.Keys)
        {
            foreach (GameObject obj in selectedObjects[key])
            {
                Character character = obj.GetComponent<Character>();
                character.SetSelectOption(false);
            }
        }
        selectedObjects.Clear();
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
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (obj.transform.tag == "Army" && selectionRect.Contains(Camera.main.WorldToScreenPoint(obj.transform.position)))
            {
                SelectObject(obj);
            }
        }
    }

    private void SelectObject(GameObject gameObject)
    {
        List<GameObject> temp = new List<GameObject>();

        Character character = gameObject.GetComponent<Character>();
        
        if (selectedObjects.ContainsKey(character.key))
        {
            int i = selectedObjects[character.key].Count;
            selectedObjects[character.key].Insert(i, gameObject);
        }
        else 
        {
            temp.Add(gameObject);
            selectedObjects.Add(character.key, temp);
        }
    }

    private void Move()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (selectedObjects != null)
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

        foreach (CharacterKey key in selectedObjects.Keys)
        {
            keys[idx] = (int)key;
            counts[idx] = selectedObjects[key].Count;
            idx++;

            foreach (GameObject obj in selectedObjects[key])
            {
                //if (obj.gameObject.transform.tag == "Army")
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
        List<GameObject> objs = selectedObjects[(CharacterKey)keyValue];
        ClearSelection();
        selectedObjects.Add((CharacterKey)keyValue, objs);
        DataReturn();
    }


    private void MoveSelectedObjects(Vector3 destPos)
    {
        int verticalCount = -15;
        int horizontalCount = 0;

        foreach (CharacterKey key in selectedObjects.Keys)
        {
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