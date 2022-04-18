using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MapTerminal : MonoBehaviour, Interactable, IPlayerInputActions
{
    [Header("Prefabs")]
    [SerializeField] private GameObject objectiveGUI;
    [SerializeField] private GameObject dividerGUI;
    [SerializeField] private GameObject animalGUI;

    [Header("Config")]
    [SerializeField] private float width;
    [SerializeField] private float height;

    [Header("References")]
    [SerializeField] private RectTransform terminal;
    [SerializeField] private RectTransform levelInfoPanel;

    [HideInInspector] public MapLevel selectedLevel;

    [SerializeField] byte controlLayer;
    [SerializeField] MapNavigator nav;

    PersistentPlayer currentController;
    DeviceProfileSprites currentDeviceProfile;

    public InteractButton interactButton { get => InteractButton.South; }
    public bool inUse { get; set; }
    private bool isHidden = true;
    public static bool hasSelected = false;

    private void Start()
    {
        // Make the terminal 0 wide and 6 pixels tall.
        terminal.sizeDelta = new Vector2(0, 6.0f / 16.0f);
    
        // Hide the level info panel on start.
        CanvasGroup group = levelInfoPanel.GetComponent<CanvasGroup>();
        levelInfoPanel.localScale = Vector3.zero;
        group.alpha = 0;

        MapNavigator.OnNavigate += HideLevelInfo;
        MapNavigator.OnDestination += ShowLevelInfo;
    }

    /// <summary>
    /// Animate in the map terminal.
    /// </summary>
    private void AnimateIn()
    {
        // Make the terminal 0 wide and 6 pixels tall.
        terminal.sizeDelta = new Vector2(0, 6.0f / 16.0f);

        // Scale the width of the terminal window.
        LeanTween.value(terminal.gameObject, w => terminal.sizeDelta = new Vector2(w, terminal.sizeDelta.y), terminal.sizeDelta.x, width, 0.3f).setEaseOutBack().setOnComplete(() =>
        {
            // Scale the height of the terminal window afterwards.
            LeanTween.value(terminal.gameObject, h => terminal.sizeDelta = new Vector2(terminal.sizeDelta.x, h), terminal.sizeDelta.y, height, 0.6f).setEaseOutBack();
        });

        isHidden = false;
    }

    /// <summary>
    /// Hides the level info panel.
    /// </summary>
    private void HideLevelInfo()
    {
        CanvasGroup group = levelInfoPanel.GetComponent<CanvasGroup>();

        LeanTween.value(levelInfoPanel.gameObject, a => group.alpha = a, group.alpha, 0.0f, 0.4f);
        LeanTween.scale(levelInfoPanel.gameObject, Vector3.zero, 0.4f).setEaseInBack();
    }

    /// <summary>
    /// Updates the info on the level info panel and then shows it with an animation.
    /// </summary>
    /// <param name="level">The currently selected level.</param>
    private void ShowLevelInfo(MapLevel level)
    {
        // Find all the children to update.
        TextMeshProUGUI levelName = levelInfoPanel.Find("Level Name").GetComponent<TextMeshProUGUI>();
        Transform objectives = levelInfoPanel.Find("Objectives");
        Transform animals = levelInfoPanel.Find("Animals");

        // Destroy the old children.
        for (int o = 0; o < objectives.childCount; o++)
            Destroy(objectives.GetChild(o).gameObject);
        for (int a = 0; a < animals.childCount; a++)
            Destroy(animals.GetChild(a).gameObject);

        // Update the children.
        levelName.text = level.levelSettings.l_displayName;

        bool divider = false;
        for (int o = 0; o < level.levelSettings.l_objectives.Length; o++)
        {
            LevelSettings.Objective objective = level.levelSettings.l_objectives[o];

            if (divider == false && objective.isSecondary) // Add a divider between the primary and secondary objectives.
            { Instantiate(dividerGUI, objectives); divider = true; } 
            
            Transform obj = Instantiate(objectiveGUI, objectives).transform;
            obj.Find("Icon").GetComponent<Image>().sprite = objective.icon;
            obj.Find("Amount").GetComponent<TextMeshProUGUI>().text = objective.quantity.ToString();
        }

        for (int a = 0; a < level.levelSettings.l_animals.Length; a++)
        {
            LevelSettings.Animal animal = level.levelSettings.l_animals[a];

            Transform ani = Instantiate(animalGUI, animals).transform;
            ani.Find("Icon").GetComponent<Image>().sprite = animal.icon;
        }

        // Animate in the panel.
        CanvasGroup group = levelInfoPanel.GetComponent<CanvasGroup>();

        LeanTween.value(levelInfoPanel.gameObject, a => group.alpha = a, group.alpha, 1.0f, 0.4f);
        LeanTween.scale(levelInfoPanel.gameObject, Vector3.one, 0.4f).setEaseOutBack();
    }

    /// <summary>
    /// Select/Deselect the current level.
    /// </summary>
    private void ToggleCurrentLevel()
    {
        hasSelected = !hasSelected;

        Image border = levelInfoPanel.Find("Border").GetComponent<Image>();
        TextMeshProUGUI buttonText = levelInfoPanel.Find("Button").Find("Label").GetComponent<TextMeshProUGUI>();

        // Make the button hint jump.
        ButtonHintBop();

        // Change the button text.
        buttonText.text = hasSelected ? "cancel" : "travel";

        // Animate the border in/out.
        LeanTween.cancel(border.gameObject);
        if (hasSelected) LeanTween.value(border.gameObject, a => border.color = new Color(border.color.r, border.color.g, border.color.b, a), border.color.a, 1.0f, 0.3f);
        else LeanTween.value(border.gameObject, a => border.color = new Color(border.color.r, border.color.g, border.color.b, a), border.color.a, 0.0f, 0.3f);
    }

    /// <summary>
    /// Update the button hint to match the current device profile.
    /// </summary>
    private void UpdateButtonHint()
    {
        if (!(currentDeviceProfile is null))
        {
            Image buttonHint = levelInfoPanel.Find("Button").Find("Hint").GetComponent<Image>();
            if (currentDeviceProfile.CompactWest == null)
                buttonHint.sprite = currentDeviceProfile.West;
            else
                buttonHint.sprite = currentDeviceProfile.CompactWest;
        }
    }

    /// <summary>
    /// Make the button hint bop.
    /// </summary>
    private void ButtonHintBop()
    {
        Transform buttonHint = levelInfoPanel.Find("Button").Find("Hint");
        LeanTween.scale(buttonHint.gameObject, Vector3.one * 1.3f, 0.1f).setEaseOutBack().setOnComplete(() => LeanTween.scale(buttonHint.gameObject, Vector3.one, 0.1f));
    }

    public void BreakInteraction() => inUse = false;

    public bool Interact(Player player)
    {
        if (currentController == null)
        {
            inUse = true;
            if (isHidden) AnimateIn();
            if (PersistentPlayerManager.main.TryGetPlayer(player.playerID, out PersistentPlayer _c))
            {
                currentController = _c;
                currentController.SetControlLayer(this, controlLayer);

                currentDeviceProfile = player.profile;
                UpdateButtonHint();

                inUse = true;
                return true;
            }
        }

        return false;        
    }

    private void RelinquishControl()
    {
        if (currentController != null)
        {
            currentController.BreakControlLayer(controlLayer);
            currentController = null;
            Debug.Log("Relinquished control of mapterminal");
        }
    }

    #region IPlayerInputActions
    public void Initialize()
    {
        Debug.Log("Player " + currentController.playerID + " Opened the map terminal");
    }

    public void DeInitialize()
    {

    }

    public void Input_BEast(InputAction.CallbackContext c)
    {
        //if (c.canceled) RelinquishControl();
    }

    public void Input_BNorth(InputAction.CallbackContext c)
    {
        //if (c.canceled) RelinquishControl();
    }

    public void Input_BSouth(InputAction.CallbackContext c)
    {
        if (c.canceled) RelinquishControl();
    }

    public void Input_BWest(InputAction.CallbackContext c)
    {
        // if (c.performed) RelinquishControl();

        if (c.canceled) ToggleCurrentLevel();
    }

    public void Input_DEast(InputAction.CallbackContext c) { }

    public void Input_DNorth(InputAction.CallbackContext c) { }

    public void Input_DSouth(InputAction.CallbackContext c) { }

    public void Input_DWest(InputAction.CallbackContext c) { }

    public void Input_LStick(InputAction.CallbackContext c)
    {
        if (c.phase != InputActionPhase.Performed) return;

        // Indicate to the player that they first have to cancel the level.
        if (hasSelected == true) { ButtonHintBop(); return; }

        Vector2 norm = c.ReadValue<Vector2>().normalized;
        Direction? d = null;

        if (norm.y <= -0.5f) d = Direction.Down;
        else if (norm.y >= 0.5f) d = Direction.Up;
        else if (norm.x <= -0.5f) d = Direction.Left;
        else if (norm.x >= 0.5f) d = Direction.Right;

        if (d != null) nav.Navigate((Direction)d);
    }

    public void Input_NumberSelect(int num)
    {
    }

    public void Input_RStick(InputAction.CallbackContext c)
    {
    }

    public void Input_ShoulderL(InputAction.CallbackContext c)
    {
    }

    public void Input_ShoulderR(InputAction.CallbackContext c)
    {
    }
    #endregion
}
