using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeliveryTruck : MonoBehaviour
{
    [SerializeField] private GameObject colliderObject;
    [SerializeField] private GameObject cover;
    [SerializeField] private float bounceStrength;
    [SerializeField] private ControlIndicator crate;

    [SerializeField] private int storedFood;
    private int startFood;

    private bool coverOn = false;
    private bool arrived = false;
    private bool gameoverTrigged = false;

    private List<GameObject> players = new List<GameObject>();
    private Animator animator;

    private void Start() {
        crate.enabled = false;
        animator = GetComponent<Animator>();
        startFood = storedFood;

        // Transition in.
        CameraTransitions.CircleTransitionIn(transform);
    }

    private void FixedUpdate() {
        if (players.Count > 0 && coverOn)
        {
            // Fade out
            LeanTween.alpha(cover, 0f, 0.15f);
            coverOn = false;
        }
        else if (players.Count <= 0 && !coverOn)
        {
            // Fade in
            LeanTween.alpha(cover, 1f, 0.15f);
            coverOn = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            collision.transform.SetParent(transform);
            players.Add(collision.gameObject);
            SetPlayerInsideTruck(collision.gameObject, true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            collision.transform.SetParent(null);
            players.Remove(collision.gameObject);
            SetPlayerInsideTruck(collision.gameObject, false);
            collision.transform.rotation = Quaternion.identity;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy")) {
            GameObject collObj = collision.gameObject;

            collObj.GetComponent<Rigidbody2D>().AddForceAtPosition((collision.transform.position - transform.position).normalized * bounceStrength, collision.contacts[0].point);
            CowEnemy enemy = collObj.GetComponent<CowEnemy>();
            storedFood -= Mathf.FloorToInt(enemy.hunger);

            // Truck shake
            LeanTween.cancel(gameObject);
            LeanTween.rotateZ(gameObject, Random.Range(-3.5f, 3.5f), 0.1f).setRepeat(4).setOnComplete(() => transform.rotation = Quaternion.identity);

            // Gameover.
            if (storedFood <= 0 && !gameoverTrigged) {
                gameoverTrigged = true;
                Transform[] players = GameObject.FindGameObjectsWithTag("Player").Select(p => p.transform).ToArray();
                CameraTransitions.CircleTransitionOut(players).setOnComplete(() => {
                    LevelLoader.main.ExitLevel();
                });
            }
        }
    }

    void SetPlayerInsideTruck(GameObject player, bool val)
    {
        player.GetComponent<Player>().isInBuilding = val;
    }

    /// <summary>
    /// Called from the animator when the truck has arrived.
    /// </summary>
    public void Arrived() {
        crate.enabled = true;
        arrived = true;
        colliderObject.SetActive(false);
    }
}