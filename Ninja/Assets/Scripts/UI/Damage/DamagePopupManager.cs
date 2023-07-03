using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePopupManager : MonoBehaviour
{
    #region Singleton

    public static DamagePopupManager instance;

    private void Awake() 
    {
        if(instance != null)
        {
            Debug.LogWarning("More than 1 instance of DamagePopupManager. Destroying " + gameObject.name);
            Destroy(this);
            return;
        }

        instance = this;
    }

    #endregion

    [SerializeField] private List<DamagePopup> popups;

    [SerializeField] private AnimationCurve popupXCurve;
    [SerializeField] private AnimationCurve popupYCurve;

    [SerializeField] private float xVariation;
    [SerializeField] private float yVariation;

    [SerializeField] private float maxFallDuration;

    public void Damage(int softDamage, int hardDamage, Vector3 position)
    {
        if(popups == null) return;

        DamagePopup popup = popups[0];
        popups.RemoveAt(0);

        popup.gameObject.SetActive(true);
        popup.transform.position = position;
        popup.Display(-softDamage);

        StartCoroutine(AnimatePopup(popup));
    }

    private IEnumerator AnimatePopup(DamagePopup popup)
    {
        float startTime = Time.time;
        Vector3 startPos = popup.transform.position;

        float xVar = Random.Range(0f, xVariation);
        float yVar = Random.Range(0f, yVariation);

        while(Time.time - startTime < maxFallDuration)
        {
            Vector3 position = popup.transform.position;
            float t = (Time.time - startTime) / maxFallDuration;

            position.x = startPos.x + popupXCurve.Evaluate(t) + 1 * xVar;
            position.y = startPos.y + popupYCurve.Evaluate(t) + 1 * yVar;

            popup.transform.position = position;

            yield return null;
        }

        popup.gameObject.SetActive(false);
        popups.Add(popup);
    }
}
