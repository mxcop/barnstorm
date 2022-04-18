using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { Up, Right, Down, Left }

public class MapNavigator : MonoBehaviour
{
    [SerializeField] private Camera mapCam;
    [SerializeField] private Vector3 camOffset;
    [SerializeField] private MapLevel selectedLevel;

    public static event Action OnNavigate;
    public static event Action<MapLevel> OnDestination;

    private void Start()
    {
        mapCam.transform.position = selectedLevel.transform.position + camOffset;

        OnNavigate = () => { };
        OnDestination = (MapLevel _) => { };
    }

    /// <summary>
    /// Navigate in a given direction.
    /// </summary>
    /// <returns>The time the animation will take.</returns>
    public float Navigate(Direction dir)
    {
        OnNavigate.Invoke();

        switch (dir)
        {
            case Direction.Up:
                if (!(selectedLevel.surroundings.top is null))
                    selectedLevel = selectedLevel.surroundings.top; break;
            case Direction.Right:
                if (!(selectedLevel.surroundings.right is null))
                    selectedLevel = selectedLevel.surroundings.right; break;
            case Direction.Down:
                if (!(selectedLevel.surroundings.bottom is null))
                    selectedLevel = selectedLevel.surroundings.bottom; break;
            case Direction.Left:
                if (!(selectedLevel.surroundings.left is null))
                    selectedLevel = selectedLevel.surroundings.left; break;
            default: break;
        }

        // Animation:
        LeanTween.cancel(mapCam.gameObject);
        LeanTween.move(
            mapCam.gameObject, 
            selectedLevel.transform.position + camOffset, 
            Vector2.Distance(mapCam.transform.position, selectedLevel.transform.position) / 6.0f)
        .setOnComplete(() => OnDestination.Invoke(selectedLevel));

        return Vector2.Distance(mapCam.transform.position, selectedLevel.transform.position + camOffset) / 6.0f;
    }

    public void StartLevel()
    {
        selectedLevel.StartLevel();
    }
}
