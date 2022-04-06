using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScorePopup : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] float fadeTime;
    [SerializeField] Color startColor;
    Color endColor;

    private void Awake()
    {
        endColor = new Color(startColor.r, startColor.g, startColor.b, 0);
    }

    public void Set(int num)
    {
        string t = num.ToString();
        if (t.Length == 1) t = "0" + t;
        else if (t.Length == 0) t = "00";

        Set(t);
    }
    public void Set(string t)
    {
        text.text = t;
    }

    public void Pop()
    {
        StartCoroutine("Anim");
    }

    IEnumerator Anim()
    {
        LeanTween.moveLocalY(gameObject, transform.position.y + 1, fadeTime);
        LeanTween.value(gameObject, e => text.color = e, startColor, endColor, fadeTime);
        yield return new WaitForSeconds(fadeTime);
    }
}
