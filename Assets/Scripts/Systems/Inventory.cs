using System.Collections;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject GUI;

    [Header("Config")]
    [SerializeField] private Vector2 offset;
    [SerializeField] private int size = 3;

    [HideInInspector] public Container<Item> container;
    private InventoryGUI gui;

    private void Start()
    {
        // Initialize the container of the inventory.
        container = new Container<Item>(size);

        // Create the GUI of the inventory.
        gui = Instantiate(GUI, GameObject.FindWithTag("MainCanvas").transform).GetComponent<InventoryGUI>();
        gui.Initialize(size, ref container);

        StartCoroutine(nameof(Test));
    }

    IEnumerator Test()
    {
        yield return new WaitForSeconds(1.0f);

        Open();

        yield return new WaitForSeconds(2.0f);

        Close();
    }

    /// <summary>
    /// Open the inventory GUI.
    /// </summary>
    public void Open()
    {
        CanvasGroup canvas = gui.GetComponent<CanvasGroup>();
        gui.gameObject.SetActive(true);
        LeanTween.cancel(gui.gameObject);
        LeanTween.value(gui.gameObject, a => canvas.alpha = a, canvas.alpha, 1.0f, 0.1f);
    }

    /// <summary>
    /// Close the inventory GUI.
    /// </summary>
    public void Close()
    {
        CanvasGroup canvas = gui.GetComponent<CanvasGroup>();
        LeanTween.cancel(gui.gameObject);
        LeanTween.value(gui.gameObject, a => canvas.alpha = a, canvas.alpha, 0.0f, 0.1f)
            .setOnComplete(() => gui.gameObject.SetActive(false));
    }
}
