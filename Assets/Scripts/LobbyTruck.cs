using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class LobbyTruck : MonoBehaviour
{
    [SerializeField] private int startTimer;
    [SerializeField] private Text timerText;
    private bool inCountdown;

    [SerializeField] private GameObject cover;
    private bool coverOn = true;
    private List<GameObject> players = new List<GameObject>();
    public UnityEvent OnAllPlayersReady;

    private void Start()
    {
        timerText.text = "";
    }

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
        if(players.Count > 0 && !inCountdown)
        {
            StartCoroutine(StartCountdown());
            /*
            if(players.Count >= PersistentPlayerManager.main.PlayerCount)
            {
                // start fast countdown

                //do it instantly for now
                OnAllPlayersReady?.Invoke();
            }
            else
            {
                // start slow countdown
            }*/
        }
    }

    private IEnumerator StartCountdown()
    {
        inCountdown = true;
        for (int i = 0; i < startTimer; i++) {
            if(players.Count >= PersistentPlayerManager.main.PlayerCount) {
                timerText.text = "Game starts in " + (startTimer - i);
                yield return new WaitForSeconds(1);
            }
            else {
                timerText.text = "";
                inCountdown = false;
                yield break;
            }
        }
        OnAllPlayersReady?.Invoke();
        timerText.text = "";
        inCountdown = false;
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
