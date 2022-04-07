using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager current;

    int _Score;
    public int Score
    {
        get { return _Score; }
        set
        {
            _Score = value;
            string s = value.ToString();
            s = s.PadLeft(Mathf.Max(0, 9 - Mathf.FloorToInt(s.Length * 0.5f)), '0');
            tmp_totalScore.text = s;
        }
    }

    [SerializeField] GameObject scoreGainPopup;
    [SerializeField] int objectPoolSize;
    [SerializeField] TextMeshProUGUI tmp_totalScore;
    GameObject canvas;

    List<ScorePopup> objectPool = new List<ScorePopup>();
    int objectPoolSelected;

    private void Awake()
    {
        current = this;
        canvas = GameObject.FindGameObjectWithTag("WorldCanvas");

        for(int i = 0; i < objectPoolSize; i++)
        {
            objectPool.Add(Instantiate(scoreGainPopup, canvas.transform).GetComponent<ScorePopup>());
        }
    }

    public void AddScore(float amount, Vector2 pos) => AddScore(Mathf.FloorToInt(amount), pos);
    public void AddScore(int amount, Vector2 pos)
    {
        if (amount > 0)
        {
            Score += amount;
            ScorePopup p = objectPool[objectPoolSelected];
            p.transform.position = pos;
            p.Set(amount);
            p.Pop();

            //increment
            objectPoolSelected = Mathf.FloorToInt(Mathf.Repeat(objectPoolSelected + 1, objectPool.Count));
        }
    }
}
