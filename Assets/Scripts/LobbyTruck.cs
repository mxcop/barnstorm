using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LobbyTruck : MonoBehaviour
{
    [SerializeField] private GameObject cover;
    private bool coverOn = true;
    private List<GameObject> players = new List<GameObject>();
    public UnityEvent OnAllPlayersReady;


    private void FixedUpdate()
    {
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

    private void Update()
    {
        if(players.Count > 0)
        {            

            if(players.Count >= PersistentPlayerManager.main.PlayerCount)
            {
                // start fast countdown

                //do it instantly for now
                OnAllPlayersReady?.Invoke();
            }
            else
            {
                // start slow countdown
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) {
            players.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) {
            players.Remove(collision.gameObject);
        }
    }
}
