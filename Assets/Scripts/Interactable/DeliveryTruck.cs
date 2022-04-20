using System.Collections;
using System.Collections.Generic;
using Systems.Inventory;
using UnityEngine.SceneManagement;
using UnityEngine;

public class DeliveryTruck : MonoBehaviour
{
    [SerializeField] private GameObject colliderObject;
    [SerializeField] private GameObject cover;
    private bool coverOn = false;
    private bool arrived = false;

    [SerializeField] private List<GameObject> players = new List<GameObject>();
    private Animator animator;

    private void Start() {
        animator = GetComponent<Animator>();
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
        }
    }

    void SetPlayerInsideTruck(GameObject player, bool val)
    {
        player.GetComponent<Player>().isInBuilding = val;
    }

    public void HasArived() {
        arrived = true;
        colliderObject.SetActive(false);
    }
}