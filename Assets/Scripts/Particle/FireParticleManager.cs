using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireParticleManager : MonoBehaviour
{
    private static FireParticleManager instance;
    public static FireParticleManager Instance
    {
        get 
        { 
            if (instance == null)
                instance = new FireParticleManager();
            return instance; 
        }
    }

    const int addPoolCount = 10;

    [SerializeField] GameObject fireParticlePrefab;
    List<GameObject> fireParticles = new List<GameObject>();

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        CreateParticlePool(addPoolCount);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CreateParticlePool(int poolCount)
    {
        GameObject obj;
        for (int i = 0; i < poolCount; i++)
        {
            obj = Instantiate(fireParticlePrefab, transform);
            fireParticles.Add(obj);
            obj.SetActive(false);
        }
    }

    public GameObject ActiveParticle(Vector3 pos)
    {
        int index = -1;
        GameObject particle = null;
        for(int i = 0; i < fireParticles.Count; i++) 
        {
            if (fireParticles[i].activeSelf)
                continue;

            index = i;
            break;
        }
        if (index < 0)
        {
            index = fireParticles.Count;
            CreateParticlePool(addPoolCount);
        }
        fireParticles[index].SetActive(true);
        fireParticles[index].transform.position = pos;
        particle = fireParticles[index];

        return particle;
    }
}
