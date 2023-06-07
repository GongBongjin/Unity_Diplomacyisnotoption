using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    static public UIManager Instance
    {
        get
        {
            return instance;
        }
    }

    Texture2D defaultCursor;
    Texture2D targetCursor;

    Sprite defaultSprite;

    [Header("Timer")]
    [SerializeField, Tooltip("Time Object in TimePanel")]
    private RectTransform timeCircle;
    [SerializeField, Tooltip("Day Text in TimePanel")]
    private Text text_Day;

    ResourcesManager resourceManager;

    [SerializeField] SingleInformation singleInfo;
    [SerializeField] MultiInformation multiInfo;
    [SerializeField] CommandSlot commandSlot;


    private void Awake()
    {
        instance = this;


        LoadCursor();
        LoadDefaultSpriteIcon();
        resourceManager = GetComponent<ResourcesManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        singleInfo.gameObject.SetActive(false);
        multiInfo.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LoadCursor()
    {
        defaultCursor = Resources.Load<Texture2D>("CursorIcon/default");
        targetCursor = Resources.Load<Texture2D>("CursorIcon/target");

        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
    }

    private void LoadDefaultSpriteIcon()
    {
        defaultSprite = Resources.Load<Sprite>("Icons/default");
    }

    public Sprite GetDefaultSpriteIcon()
    {
        return defaultSprite;
    }

    // 자원 관리용 오브젝트 설정
    
    // 0.0f ~ 23.99f
    public void ChangeTime(float hour)
    {
        timeCircle.rotation = Quaternion.Euler(0, 0, hour + 135.0f);
    }

    public void UpdateDay(int day)
    {
        CharacterManager.instance.SetAttackCityHall(day);
        text_Day.text = day.ToString();
    }

    public bool CheckRemainingResources(int population = 0, int food = 0, int wood = 0, int stone = 0, int copper = 0)
    {
        bool isEnough = true;
        // 인구 비교, 자원 비교
        if (resourceManager.GetPopulation() + population > resourceManager.GetMaxPopulation())
            isEnough = false;

        if(!(resourceManager.GetCheckResourceCount(Product.FOOD, food) && resourceManager.GetCheckResourceCount(Product.WOOD, wood) &&
            resourceManager.GetCheckResourceCount(Product.STONE, stone) && resourceManager.GetCheckResourceCount(Product.COPPER, copper)))
            isEnough = false;

        return isEnough;
    }

    public void SpendResources(int population = 0, int food = 0, int wood = 0, int stone = 0, int copper = 0)
    {
        IncreasesResources(Product.POPULATION, population);
        DecreasesResources(Product.FOOD, food);
        DecreasesResources(Product.WOOD, wood);
        DecreasesResources(Product.STONE, stone);
        DecreasesResources(Product.COPPER, copper);
    }

    public void IncreaseMaxPopulation(int value)
    {
        resourceManager.UpdateMaxPopulation(value);
    }
    public void IncreaseMaxStorage(int value)
    {
        resourceManager.UpdateMaxStorage(value);
    }

    public void IncreasesResources(Product product, int qty)
    {
        resourceManager.IncreasesResources(product, qty);
    }
    public void DecreasesResources(Product product, int qty)
    {
        resourceManager.DecreasesResources(product, qty);
    }

    public void SetProductionBuilding(Building building)
    {
        singleInfo.SetProductionBuilding(building);
    }

    public void ShowInformation(int[] keys, int[] count)
    {
        singleInfo.gameObject.SetActive(false);
        multiInfo.gameObject.SetActive(false);

        if (keys.Length == 0)
        {
            //clear
            commandSlot.SetDefaultCommandSlot();
        }
        else if (keys.Length == 1)
        {
            singleInfo.gameObject.SetActive(true);
            singleInfo.ShowInformation(keys[0], count[0]);
            commandSlot.SetCommandSlot(keys[0]);
        }
        else
        {
            multiInfo.gameObject.SetActive(true);
            multiInfo.ShowInformation(keys, count);
            commandSlot.SetCommandSlot(keys[0]);
        }
    }

}
