using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class SelectionBox : MonoBehaviour
{
    Image selectionBox;

    private Army army;
    public Dictionary<CharacterKey, List<GameObject>> selectedObjects = new Dictionary<CharacterKey, List<GameObject>>(); //���� list ������ GameObject�� �ƴ� Character �������ٰ� �����͸� �־ Ű����������.

    //Drag
    private Vector3 dragStartPos;
    private bool isDragging = false;

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
            dragStartPos = Input.mousePosition;
            ClearSelection();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (!isDragging)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject.tag == "Army")
                        SelectObject(hit.collider.gameObject);
                }
            }

            isDragging = false;
            selectionBox.enabled = false;
        }
        if (Input.GetMouseButton(0))
        {
            if (!isDragging && Vector3.Distance(Input.mousePosition, dragStartPos) > 5.0f)
                isDragging = true;

            if (isDragging)
                UpdateDragBox(Input.mousePosition);

        }

        DataReturn();
    }

    private void ClearSelection()
    {
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

        army = gameObject.AddComponent<Army>();

        if(selectedObjects.ContainsKey(army.key))
        {
            int i = selectedObjects[army.key].Count;
            selectedObjects[army.key].Insert(i, gameObject);
        }
        else 
        {
            temp.Add(gameObject);
            selectedObjects.Add(army.key, temp);
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
        foreach (CharacterKey key in selectedObjects.Keys)
        {
            foreach (GameObject obj in selectedObjects[key])
            {
                if (obj.gameObject.transform.tag == "Army")
                {
                    Army army = obj.GetComponent<Army>();
                    CharacterData data;

                    data = army.GetCharacterData();

                    //UIManager.instance.GetData() = data;
                }

                //
                //if(obj.gameObject.transform.tag == "Building")
                //{
                //    Building building = obj.GetComponent<Building>();
                //    BuildingData data;
                //    data = building.GetBuildingData();
                //
                //    UIManager.instance.GetData() = data;
                //}
            }
        }
    }


    private void MoveSelectedObjects(Vector3 destPos)
    {
        foreach (CharacterKey key in selectedObjects.Keys)
        {
            foreach (GameObject obj in selectedObjects[key])
            {
                army = obj.GetComponent<Army>();
                army.Move(destPos);
            }
        }
    }
}

/*UIManager
 * List<CharacterData> list;
 * 
 * public CharacterData GetData(CharacterData data)
 * {
 *
 *      list.Add(data);
        ShowUI();
        
        Sprite sprite = Resources<Sprite>(data.value.sprite);
        
        
 * }
 * 
 * 
 * public BuildingData GetData()
 */

//Update���ȿ� ������

/*private void ShowUI()
 * {
 * }
 
 */