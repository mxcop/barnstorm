using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    [SerializeField] CropData[] stageData;
    CropData currentStageData;
    int currentStage;
    float lastGrowthT;

    [SerializeField] private GameObject particlePrefab;

    [Header("Settings")]
    [SerializeField] bool autoRespawn;
    [SerializeField] float growthDelayMultiplier;
    [SerializeField] int startingGrowth;

    SpriteRenderer spr;
    bool isHarvested;
    bool hasSpawnedParticle;

    private void Start()
    {
        Vector3Int pos = new Vector3Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y), 0);
        transform.position = (Vector3)pos + new Vector3(0.5f, 0.5f, 0);

        spr = GetComponent<SpriteRenderer>();
        SetGrowthStage(startingGrowth);

        if (!CropManager.current.AddToCrops(pos, this)) Harvest();
    }

    private void Update()
    {
        if (lastGrowthT + currentStageData.growthDelay * growthDelayMultiplier < Time.time) SetGrowthStage(currentStage + 1);
    }

    void SetGrowthStage(int i)
    {
        lastGrowthT = Time.time;

        currentStage = Mathf.Clamp(i, 0, stageData.Length - 1);
        currentStageData = stageData[currentStage];
        spr.sprite = currentStageData.sprite;

        // Bitch, Deze shit is kut
        if (!hasSpawnedParticle && currentStage == stageData.Length - 1) {
            Instantiate(particlePrefab, transform.position, Quaternion.identity);
            hasSpawnedParticle = true;
        }
            
    }

    public CropData? Harvest()
    {
        if (!isHarvested)
        {
            isHarvested = true;

            ScoreManager.current?.AddScore(currentStageData.scoreFromHarvest, transform.position);
            if (!autoRespawn) Destroy(gameObject);
            else
            {
                SetGrowthStage(0);
                isHarvested = false;
                SFXManager.PlayClip("till");
            }

            return stageData[currentStage];
        }
        else return null;
    }
}

[System.Serializable]
public struct CropData
{
    public Sprite sprite;
    public float growthDelay;    
    [Space]
    [SerializeField] GameObject _item;
    public Item item => _item.GetComponent<Item>();
    public Vector2 amount;
    public int scoreFromHarvest;
}
