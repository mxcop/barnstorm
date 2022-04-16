using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { Up, Right, Down, Left }

public class MapNavigator : MonoBehaviour
{
    [SerializeField] private Camera mapCam;
    [SerializeField] private MapLevel selectedLevel;

    private void Start()
    {
        mapCam.transform.position = selectedLevel.transform.position;
    }

    public float Navigate(Direction dir)
    {
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

		LeanTween.cancel(mapCam.gameObject);
        LeanTween.move(mapCam.gameObject, selectedLevel.transform.position, Vector2.Distance(mapCam.transform.position, selectedLevel.transform.position) / 3.0f);
        return Vector2.Distance(mapCam.transform.position, selectedLevel.transform.position) / 3.0f;
        //mapCam.transform.position = selectedLevel.transform.position;
    }
}
