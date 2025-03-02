using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ReelController : MonoBehaviour
{
    // 🎨 Reel Symbols (Top, Middle, Bottom)
    public Image[] reelImages;
    public RectTransform reelTransform; 

    // 🔄 Spin Parameters
    public float maxScrollSpeed = 1000f; 
    public float minScrollSpeed = 50f; 
    public float spinDuration = 3f; 
    public float symbolHeight = 200f; 

    // 🎰 Available Symbols
    public Sprite[] symbolSprites;

    // 🔧 Private Variables
    private bool isSpinning = false;
    private float resetPositionY;
    private int finalSymbolIndex;

    private void Start()
    {
        resetPositionY = reelTransform.anchoredPosition.y;
        RandomizeSymbols(); 
    }

    // 🎯 Start Spinning the Reel
    public void StartSpin()
    {
        if (!isSpinning)
        {
            StartCoroutine(SpinReel());
        }
    }

    // 🔄 Handle Reel Spinning
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

            // 🔄 Reset reel position when it moves too high
            if (reelTransform.anchoredPosition.y >= resetPositionY + symbolHeight)
            {
                reelTransform.anchoredPosition = new Vector2(reelTransform.anchoredPosition.x, resetPositionY);
                RandomizeSymbols();
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 🎰 Choose Final Stopping Symbol
        finalSymbolIndex = Random.Range(0, symbolSprites.Length);
        UpdateFinalSymbols(finalSymbolIndex);

        // 🛑 Smooth Snap to Final Position
        yield return SnapToFinalPosition();
    }

    // 🎯 Smoothly Align Reel to Final Position
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

    // 🔄 Randomize All Symbols
    private void RandomizeSymbols()
    {
        for (int i = 0; i < reelImages.Length; i++)
        {
            reelImages[i].sprite = symbolSprites[Random.Range(0, symbolSprites.Length)];
        }
    }

    // 🎰 Align Final Symbols Correctly
    private void UpdateFinalSymbols(int symbolIndex)
    {
        reelImages[1].sprite = symbolSprites[symbolIndex]; // Middle stays fixed
        reelImages[0].sprite = symbolSprites[(symbolIndex - 1 + symbolSprites.Length) % symbolSprites.Length]; // Top
        reelImages[2].sprite = symbolSprites[(symbolIndex + 1) % symbolSprites.Length]; // Bottom
    }

    // 🚀 Check if Reel is Still Spinning
    public bool IsSpinning()
    {
        return isSpinning;
    }

    // 🎯 Get Middle Symbol for Win Checking
    public Sprite GetMiddleSymbol()
    {
        return reelImages[1].sprite;
    }
}
