
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectionBox : MonoBehaviour
{
    public static SelectionBox instance;

    Image selectionBox;

    private bool isMoveable = true;
    private bool isCommandM;
    private bool isCommandP;
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
        CommandControl();
    }

    private void Select()
    {
        if (BuildManager.Instance.GetIsBuild())
            return;

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
                    // RayCast All로 변환
                    // selectedObjects 대신 singleSelectObject 로 따로 설정
                    RaycastHit[] hits = Physics.RaycastAll(ray);
                    bool isSelected = false;
                    GameObject obj;
                    int key;
                    // 이 부분 바뀌었습니다.
                    // Charcter를 포함한 시민, 빌딩, 오브젝트 선택 가능하도록 임시조치해놨어요
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
                                key = obj.GetComponent<Citizen>().GetKey();
                                AddSelectObject(key, obj);
                                break;
                            case "Building":
                                isMoveable = false;
                                isSelected = true;
                                Building building = obj.GetComponent<Building>();
                                key = building.GetKey();
                                UIManager.Instance.SetProductionBuilding(building);
                                AddSelectObject(key, obj);
                                break;
                            case "Product":
                                isMoveable = false;
                                isSelected = true;
                                key = obj.GetComponent<ProductObject>().GetKey();
                                AddSelectObject(key, obj);
                                // 나무, 광석 등 임시조치
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
                Objects objects = obj.GetComponent<Objects>();
                objects.SetSelectObject(false);
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
        CitizenManager.Instance.GetObjectsContainedInRect(selectionRect);
    }

    // 아래 함수 변형
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

    private void CommandControl()
    {
        //if (selectedObjects.Count == 0 && !isCommandM) return;

        Move();

        //if(Input.anyKey)
        //{
        //    string inputString = Input.inputString;
        //    if(!string.IsNullOrEmpty(inputString))
        //    {
        //        char hotKey = inputString[0];
        //        int ascii = 'a' - 'A';
        //        if (hotKey >= 'a')
        //            hotKey = (char)(hotKey - ascii);
        //
        //        InputCommandKey(hotKey);
        //    }
        //}
    }

    public void InputCommandKey(char hotKey)
    {
        switch (hotKey)
        {
            case 'M':
                // true일때 좌클릭시 이동
                isCommandM = true;
                Move();
                break;
            case 'H':
                StopSelectedObjects();
                break;
            case 'F':
                // 어택 땅
                AttackSelectedObjects();
                break;
            case 'P':
                // 순찰
                PatrolSelectedObjects();
                break;
        }
    }

    private void Move()
    {
        if (Input.GetMouseButtonDown(1)|| isCommandM)
        {
            if (!isMoveable) return;

            Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 destPos = new Vector3(hit.point.x, 0, hit.point.z);
                MoveSelectedObjects(destPos, hit);

            }
        }
        isCommandM = false;
    }

    private void CitizenProductionOrder(GameObject product)
    {
        int citizenKey = CitizenManager.Instance.GetCitizenKey();
        if (selectedObjects[citizenKey].Count == 0)
            return;

        foreach (GameObject obj in selectedObjects[citizenKey])
        {
            Citizen citizen = obj.GetComponent<Citizen>();
            citizen.ProductionOrder(product);
        }
    }
    private void CitizenBuildingOrder(GameObject building)
    {
        int citizenKey = CitizenManager.Instance.GetCitizenKey();
        if (selectedObjects[citizenKey].Count == 0)
            return;

        foreach (GameObject obj in selectedObjects[citizenKey])
        {
            Citizen citizen = obj.GetComponent<Citizen>();
            citizen.BuildingOrder(building);
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
                Objects objects = obj.GetComponent<Objects>();
                objects.SetSelectObject(true);
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


    private void MoveSelectedObjects(Vector3 destPos, RaycastHit hit)
    {
        foreach (int key in selectedObjects.Keys)
        {
            if (key == 1000)
            {
                if (hit.transform.tag.Equals("Product"))
                {
                    CitizenProductionOrder(hit.transform.gameObject);
                    hit.transform.GetComponent<Objects>().SelectObjectFlicker();
                }
                else if (hit.transform.tag.Equals("Building"))
                {
                    CitizenBuildingOrder(hit.transform.gameObject);
                    hit.transform.GetComponent<Objects>().SelectObjectFlicker();
                }
                else
                {
                    foreach (GameObject obj in selectedObjects[key])
                    {
                        obj.GetComponent<Citizen>().StopWork();
                        obj.GetComponent<Citizen>().MoveDestination(destPos);
                    }
                }
                continue;
            }
            foreach (GameObject obj in selectedObjects[key])
            {
                Army army = obj.GetComponent<Army>();

                if(army.isPatrol)
                    army.isPatrol = false;

                army.isTargeting = true;
                army.Move(destPos);
            }
        }
    }

    private void StopSelectedObjects()
    {
        foreach (int key in selectedObjects.Keys)
        {
            if(key == 1000)
            {
                continue;
            }
            foreach (GameObject obj in selectedObjects[key])
            {
                Army army = obj.GetComponent<Army>();

                if (army.isPatrol)
                    army.isPatrol = false;

                army.StopCommand();
            }
        }
    }

    private void AttackSelectedObjects()
    {
        Enemy enmy = null;

        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject gameObject = hit.collider.gameObject;
            enmy = gameObject.GetComponent<Enemy>();
        }

        foreach (int key in selectedObjects.Keys)
        {
            foreach (GameObject obj in selectedObjects[key])
            {
                Army army = obj.GetComponent<Army>();

                if (army.isPatrol)
                    army.isPatrol = false;

                army.AttackCommand(enmy);
            }
        }
    }

    private void PatrolSelectedObjects()
    {
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if(selectedObjects.Count!=0)
        {
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 destPos = new Vector3(hit.point.x, 0, hit.point.z);

                foreach (int key in selectedObjects.Keys)
                {
                    foreach (GameObject obj in selectedObjects[key])
                    {
                        Army army = obj.GetComponent<Army>();

                        army.PatrolCommand(destPos);
                    }
                }
            }
        }
    }

    public void CreateUnit(int buildingKey, int key)
    {
        selectedObjects[buildingKey][0].GetComponent<Building>().AddUnitProduct(key);
    }

    public void BuildingOrder(int citizenKey, GameObject building)
    {
        for(int i = 0; i < selectedObjects[citizenKey].Count; i++)
        {
            selectedObjects[citizenKey][i].GetComponent<Citizen>().BuildingOrder(building);
        }
    }
}