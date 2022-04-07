using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Barn : MonoBehaviour
{
    [Header("Config")]

    [Tooltip("The hitpoints of the barn")]
    [SerializeField] private int storedFood;

    [Header("Prefabs")]

    [SerializeField] private Sprite[] stages;

    private SpriteRenderer sp;
    private int startFood;

    private void Awake()
    {
        sp = GetComponent<SpriteRenderer>();
        startFood = storedFood;
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (!(coll.gameObject is null) && coll.gameObject.CompareTag("Enemy"))
        {
            // Get the enemy and subtract their hunger.
            EnemyBase enemy = coll.gameObject.GetComponent<EnemyBase>();
            storedFood -= Mathf.FloorToInt(enemy.hunger);
            enemy.hunger = 0;
            enemy.state = EnemyBase.EnemyState.eating;
            UpdateSprite();

            // Gameover.
            if (storedFood <= 0) Gameover();
        }
    }

    /// <summary>
    /// Called when the game is lost.
    /// </summary>
    private void Gameover()
    {
        LobbyManager.players.Clear();
        SceneManager.LoadScene(0);
        Debug.Log("!! GAME OVER !!");
    }

    /// <summary>
    /// Update the sprite of the barn based on the amount of stored food.
    /// </summary>
    private void UpdateSprite()
    {
        sp.sprite = stages[Mathf.Max(0, Mathf.RoundToInt((float)storedFood / startFood * (stages.Length - 1)))];
    }
}
