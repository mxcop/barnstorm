using TMPro;
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

    public static bool gameIsOver;

    private void Awake()
    {
        sp = GetComponent<SpriteRenderer>();
        startFood = storedFood;
        //if(gameoverPanel != null) gameoverPanel.SetActive(false);

        gameIsOver = false;
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (!(coll.gameObject is null) && coll.gameObject.CompareTag("Enemy"))
        {
            // Get the enemy and subtract their hunger.
            CowEnemy enemy = coll.gameObject.GetComponent<CowEnemy>();
            storedFood -= Mathf.FloorToInt(enemy.hunger);
            enemy.hunger = 0;
            enemy.state = CowEnemy.EnemyState.eating;
            //UpdateSprite();

            // Gameover.
            if (storedFood <= 0) Gameover();
        }
    }

    /// <summary>
    /// Called when the game is lost.
    /// </summary>
    private void Gameover()
    {
        Debug.Log("!! GAME OVER !!");
        //gameoverPanel.SetActive(true);
        //gameoverPanel.transform.Find("Panel").Find("Text (Score)").GetComponent<TextMeshProUGUI>().text = "Score : " + ScoreManager.current.Score.ToString();

        gameIsOver = true;
        LobbyManager.players.Clear();
        LobbyManager.hasStarted = false;
    }

    /// <summary>
    /// Update the sprite of the barn based on the amount of stored food.
    /// </summary>
    private void UpdateSprite()
    {
        sp.sprite = stages[Mathf.Max(0, Mathf.RoundToInt((float)storedFood / startFood * (stages.Length - 1)))];
    }
}
