using UnityEngine;
using System.Collections;

public class DamageHide : MonoBehaviour
{
    [Header("Time settings")]
    [SerializeField] private float visibleDuration = 3f;
    [SerializeField] private float invisibleDuration = 2f;
    [SerializeField] private float startDelay = 0f;

    [Header("Components")]
    [SerializeField] private GameObject visualModel;
    private Collider mainCollider;

    void Start()
    {
        mainCollider = GetComponent<Collider>();

        if (visualModel == null && transform.childCount > 0)
        {
            visualModel = transform.GetChild(0).gameObject;
        }

        StartCoroutine(ToggleCycle());
    }

    IEnumerator ToggleCycle()
    {
        if (startDelay > 0f)
        {
            yield return new WaitForSeconds(startDelay);
        }

        while (true)
        {
            SetObstacleState(true);
            yield return new WaitForSeconds(visibleDuration);

            SetObstacleState(false);
            yield return new WaitForSeconds(invisibleDuration);
        }
    }

    private void SetObstacleState(bool isActive)
    {
        if (mainCollider != null)
        {
            mainCollider.enabled = isActive;
        }

        if (visualModel != null)
        {
            visualModel.SetActive(isActive);
        }
    }
}
