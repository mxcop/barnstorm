using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ObjectivesPanel : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject primaryObjective;
    [SerializeField] private GameObject secondaryObjective;

    [Header("References")]
    [SerializeField] private RectTransform objectivesGrid;

    private Transform[] objectives;

    private static ObjectivesPanel instance;

    private void Awake() 
    {
        instance = this;
    }

    /// <summary>
    /// Initialize the GUI.
    /// </summary>
    public static void InitUI(LevelSettings.Objective[] objs)
    {
        instance.objectives = new Transform[objs.Length];
        
        for (int i = 0; i < objs.Length; i++)
        {
            // Instantiate the correct object.
            Transform objective;
            if (objs[i].isSecondary == false) {
                objective = Instantiate(instance.primaryObjective, instance.objectivesGrid).transform;
            } else {
                objective = Instantiate(instance.secondaryObjective, instance.objectivesGrid).transform;
            }

            // Set the icon and values.
            Image icon = objective.Find("Icon").GetComponent<Image>();
            icon.sprite = objs[i].icon;
            SetTargetAndCurrent(objective, objs[i].quantity);

            // Add it to the list.
            instance.objectives[i] = objective;
        }
    }

    /// <summary>
    /// Update the objectives GUI.
    /// </summary>
    /// <param name="index">Index of this objective in the level settings array.</param>
    /// <param name="obj">The level settings objective object.</param>
    /// <param name="num">The new quantity of items.</param>
    public static void UpdateUI(int index, LevelSettings.Objective obj, int num) 
    {
        SetTargetAndCurrent(instance.objectives[index], obj.quantity, num);
    }

    /// <summary>
    /// Set the target and current values on the GUI of an objective.
    /// </summary>
    private static void SetTargetAndCurrent(Transform objective, int target, int current = 0) 
    {
        // Get the UI components.
        TextMeshProUGUI targetUI = objective.Find("Status").Find("Target").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI currentUI = objective.Find("Status").Find("Current").GetComponent<TextMeshProUGUI>();

        // Set the text.
        targetUI.text = target.ToString();
        currentUI.text = current.ToString();
    }
}
