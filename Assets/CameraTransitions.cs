using UnityEngine;

public class CameraTransitions : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject mask;

    [Header("References")]
    [SerializeField] private Transform maskHolder;

    private static CameraTransitions instance;

    private void Awake() 
    {
        instance = this;
    }

    public static void CircleTransitionIn(Transform truck) 
    {
        GameObject maskObj = Instantiate(instance.mask, instance.maskHolder);
        SpriteMask spriteMask = maskObj.GetComponent<SpriteMask>();

        LeanTween.value(maskObj, 1.0f, 0.65f, 1.0f).setEaseInExpo()
            .setOnUpdate((float f) => {
                spriteMask.alphaCutoff = f;
                maskObj.transform.position = truck.position;
            })
            .setOnComplete(() => {
                Destroy(maskObj);
            });
    }

    public static LTDescr CircleTransitionOut(Transform[] players) 
    {
        GameObject[] masks = new GameObject[players.Length];

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].localScale != Vector3.zero) {
                masks[i] = Instantiate(instance.mask, instance.maskHolder);
                SpriteMask spriteMask = masks[i].GetComponent<SpriteMask>();

                int ii = i;
                LeanTween.value(masks[i], 0.65f, 1.0f, 2.5f).setEaseOutExpo()
                    .setOnUpdate((float f) => {
                        spriteMask.alphaCutoff = f;
                        masks[ii].transform.position = players[ii].position;
                    })
                    .setOnComplete(() => {
                        Destroy(masks[ii]);
                    });
            }
        }

        return LeanTween.value(0, 0, 2.5f);
    }
}
