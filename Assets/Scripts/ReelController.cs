using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ReelController : MonoBehaviour
{
    public Image[] reelImages;
    public RectTransform reelTransform; 
    public Sprite[] symbolSprites;

    public float maxScrollSpeed = 1000f; 
    public float minScrollSpeed = 50f; 
    public float spinDuration = 3f; 
    public float symbolHeight = 200f; 

    private bool isSpinning = false;
    private float resetPositionY;
    private int finalSymbolIndex;

    private void Start()
    {
        resetPositionY = reelTransform.anchoredPosition.y;
        RandomizeSymbols(); 
    }

    public void StartSpin()
    {
        if (!isSpinning)
        {
            StartCoroutine(SpinReel());
        }
    }

    private IEnumerator SpinReel()
    {
        isSpinning = true;
        float elapsedTime = 0f;
        float currentSpeed = maxScrollSpeed;

        while (elapsedTime < spinDuration)
        {
            if (elapsedTime > spinDuration * 0.7f) 
            {
                currentSpeed = Mathf.Lerp(maxScrollSpeed, minScrollSpeed, (elapsedTime - spinDuration * 0.7f) / (spinDuration * 0.3f));
            }

            reelTransform.anchoredPosition += new Vector2(0, currentSpeed * Time.deltaTime);

            if (reelTransform.anchoredPosition.y >= resetPositionY + symbolHeight)
            {
                reelTransform.anchoredPosition = new Vector2(reelTransform.anchoredPosition.x, resetPositionY);
                RandomizeSymbols();
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        finalSymbolIndex = Random.Range(0, symbolSprites.Length);
        UpdateFinalSymbols(finalSymbolIndex);

        yield return SnapToFinalPosition();
    }

    private IEnumerator SnapToFinalPosition()
    {
        float elapsedTime = 0f;
        float duration = 0.2f;

        Vector2 startPos = reelTransform.anchoredPosition;
        Vector2 endPos = new Vector2(startPos.x, resetPositionY);

        while (elapsedTime < duration)
        {
            reelTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        reelTransform.anchoredPosition = endPos;
        isSpinning = false;
    }

    private void RandomizeSymbols()
    {
        for (int i = 0; i < reelImages.Length; i++)
        {
            reelImages[i].sprite = symbolSprites[Random.Range(0, symbolSprites.Length)];
        }
    }

    private void UpdateFinalSymbols(int symbolIndex)
    {
        reelImages[1].sprite = symbolSprites[symbolIndex];
        reelImages[0].sprite = symbolSprites[(symbolIndex - 1 + symbolSprites.Length) % symbolSprites.Length];
        reelImages[2].sprite = symbolSprites[(symbolIndex + 1) % symbolSprites.Length];
    }

    public bool IsSpinning()
    {
        return isSpinning;
    }

    public Sprite GetMiddleSymbol()
    {
        return reelImages[1].sprite;
    }
}
