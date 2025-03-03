using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WinAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    public float popupDuration = 3f; // Time before hiding
    public float scaleUpSize = 1.2f; // First scale size
    public float scaleNormalSize = 1f; // Final size
    public float scaleSpeed = 0.5f; // Scale animation speed
    public float fadeSpeed = 0.5f; // Fade-in speed

    public GameObject winPopupPrefab; // Win prefab reference
    public Transform uiCanvas; // The parent UI canvas to instantiate under
    public float popupSpacing = 200f; // Distance between each win popup

    public void PlayWinAnimation(Sprite sprite1, Sprite sprite2, Sprite sprite3)
    {
        if (winPopupPrefab == null || uiCanvas == null) return;

        CreateWinPopup(sprite1, new Vector3(-popupSpacing, 0, 0));
        CreateWinPopup(sprite2, Vector3.zero);
        CreateWinPopup(sprite3, new Vector3(popupSpacing, 0, 0));
    }

    private void CreateWinPopup(Sprite sprite, Vector3 offset)
    {
        GameObject winPopup = Instantiate(winPopupPrefab, uiCanvas);
        winPopup.transform.localPosition += offset;

        Image symbolImage = winPopup.transform.GetChild(0).GetChild(0).GetComponent<Image>();
        CanvasGroup canvasGroup = winPopup.GetComponent<CanvasGroup>();

        symbolImage.sprite = sprite;

        winPopup.transform.localScale = Vector3.zero;
        LeanTween.scale(winPopup, Vector3.one * scaleUpSize, scaleSpeed)
            .setEaseOutBounce()
            .setOnComplete(() =>
                LeanTween.scale(winPopup, Vector3.one * scaleNormalSize, scaleSpeed)
            );

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0;
            LeanTween.alphaCanvas(canvasGroup, 1, fadeSpeed);
        }

        StartCoroutine(HideWinPopup(winPopup));
    }

    private IEnumerator HideWinPopup(GameObject winPopup)
    {
        yield return new WaitForSeconds(popupDuration);

        CanvasGroup canvasGroup = winPopup.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            LeanTween.alphaCanvas(canvasGroup, 0, fadeSpeed).setOnComplete(() => Destroy(winPopup));
        }
        else
        {
            Destroy(winPopup);
        }
    }
}
