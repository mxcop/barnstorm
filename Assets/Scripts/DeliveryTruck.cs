using System.Collections;
using System.Collections.Generic;
using Systems.Inventory;
using UnityEngine;

public class DeliveryTruck : MonoBehaviour
{
    class Loot
    {
        public Item item;
        public float itemWeight;

        public Vector2 amountRange;
        public float amountWeight;

        public Vector2 dropRange;
    }

    [HideInInspector] public bool isFinished = false;
    [SerializeField] private Crate[] crates;
    [SerializeField] private GameObject colliderObject;

    [SerializeField] private GameObject cover;
    private bool coverOn = true;

    [SerializeField] private List<Item> items;
    private List<Loot> lootTable = new List<Loot>();
    private float totalWeight = 0;

    private List<GameObject> players = new List<GameObject>();
    private Animator animator;

    private void Start() {
        animator = GetComponent<Animator>();

        GeneratedDefaultValues();
        AssignTableWeights();

        // Fill every crate with a random item
        for (int i = 0; i < crates.Length; i++) {
            Loot loot = RandomLoot();
            crates[i].container.PushItem(loot.item, (int)Random.Range(loot.amountRange.x, loot.amountRange.y));
        }
    }

    private void FixedUpdate() {
        if(players.Count > 0 && coverOn) {
            // Fade out
            LeanTween.alpha(cover, 0f, 0.15f);
            coverOn = false;
        }
        else if (players.Count <= 0 && !coverOn) {
            // Fade in
            LeanTween.alpha(cover, 1f, 0.15f);
            coverOn = true;

            // If all the crates are empty start leaving
            if (AllCratesEmpty()) {
                animator.SetTrigger("Leave");
                colliderObject.SetActive(true);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            collision.transform.SetParent(transform);
            players.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            collision.transform.SetParent(null);
            players.Remove(collision.gameObject);
        }
    }

    private void GeneratedDefaultValues() {
        for (int i = 0; i < items.Count; i++) {
            Loot item = new Loot();

            switch (items[i].GetType().ToString()) {
                case "Potato":
                    item.itemWeight = 50;
                    item.amountRange = new Vector2(15, 80);
                    break;
                case "Carrot":
                    item.itemWeight = 20;
                    item.amountRange = new Vector2(20, 95);
                    break;
                case "Corn":
                    item.itemWeight = 15;
                    item.amountRange = new Vector2(10, 50);
                    break;
                case "Pepper":
                    item.itemWeight = 10;
                    item.amountRange = new Vector2(20, 60);
                    break;
                case "Pumpkin":
                    item.itemWeight = 5;
                    item.amountRange = new Vector2(5, 25);
                    break;
                default:
                    return;
            }
            item.item = items[i];
            lootTable.Add(item);
        }
    }

    private void AssignTableWeights() {
        totalWeight = 0;
        for (int i = 0; i < lootTable.Count; i++) {
            lootTable[i].dropRange.x = totalWeight;
            totalWeight += lootTable[i].itemWeight;
            lootTable[i].dropRange.y = totalWeight;
        }
    }

    private Loot RandomLoot() {
        int index = -1;
        float number = Random.Range(0, totalWeight);

        // Loop over table to find match
        for (int i = 0; i < lootTable.Count; i++){
            if (number >= lootTable[i].dropRange.x && number <= lootTable[i].dropRange.y)
                index = i;
        }

        return index < 0 ? null : lootTable[index];
    }

    bool AllCratesEmpty() {
        int emptyCount = 0;
        for (int i = 0; i < crates.Length; i++) {
            if (!crates[i].container.Peek(0, out Item _))
                emptyCount++;
        }

        return emptyCount >= crates.Length;
    }

    public void Finish() {
        isFinished = true;
        Destroy(gameObject);
    }
}