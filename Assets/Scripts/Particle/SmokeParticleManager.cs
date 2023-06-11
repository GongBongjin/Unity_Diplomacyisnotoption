using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeParticleManager : MonoBehaviour
{
    private static SmokeParticleManager instance;
    public static SmokeParticleManager Instance
    {
        get
        {
            if (instance == null)
                instance = new SmokeParticleManager();
            return instance;
        }
    }

    const int addPoolCount = 10;

    [SerializeField] GameObject smokeParticlePrefab;
    List<GameObject> smokeParticles = new List<GameObject>();

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
            obj = Instantiate(smokeParticlePrefab, transform);
            smokeParticles.Add(obj);
            obj.SetActive(false);
        }
    }

    public GameObject ActiveParticle(Vector3 pos, Vector3 size)
    {
        int index = -1;
        GameObject particle = null;
        for (int i = 0; i < smokeParticles.Count; i++)
        {
            if (smokeParticles[i].activeSelf)
                continue;

            index = i;
            break;
        }
        if (index < 0)
        {
            index = smokeParticles.Count;
            CreateParticlePool(addPoolCount);
        }
        smokeParticles[index].SetActive(true);
        smokeParticles[index].transform.position = pos;
        var shape = smokeParticles[index].GetComponent<ParticleSystem>().shape;
        shape.scale = size;
        smokeParticles[index].GetComponent<ParticleSystem>().Play();
        particle = smokeParticles[index];

        return particle;
    }
}
