using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ReelController : MonoBehaviour
{
    public Image[] reelImages; // The symbols in the reel (Top, Middle, Bottom)
    public RectTransform reelTransform; // Parent container that moves
    public float maxScrollSpeed = 1000f; // Fast initial scrolling speed
    public float minScrollSpeed = 50f; // Slower speed near stopping
    public float spinDuration = 3f; // Total spin time before stopping
    public float symbolHeight = 200f; // Height of each symbol

    public Sprite[] symbolSprites; // Different symbols for the reel

    private bool isSpinning = false;
    private float resetPositionY;
    private int finalSymbolIndex; // Store the final symbol to stop at

    private void Start()
    {
        resetPositionY = reelTransform.anchoredPosition.y; // Store original position
        RandomizeSymbols(); // Ensure random symbols at start
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
            // **Decrease speed near the end of spinning**
            if (elapsedTime > spinDuration * 0.7f) // Slow down in the last 30% of spin time
            {
                currentSpeed = Mathf.Lerp(maxScrollSpeed, minScrollSpeed, (elapsedTime - spinDuration * 0.7f) / (spinDuration * 0.3f));
            }

            reelTransform.anchoredPosition += new Vector2(0, currentSpeed * Time.deltaTime);

            // Reset position when it goes too high
            if (reelTransform.anchoredPosition.y >= resetPositionY + symbolHeight)
            {
                reelTransform.anchoredPosition = new Vector2(reelTransform.anchoredPosition.x, resetPositionY);
                RandomizeSymbols(); // Keep changing symbols while spinning
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // **Pick a final stopping symbol**
        finalSymbolIndex = Random.Range(0, symbolSprites.Length);

        // **Update all three symbols based on finalSymbolIndex**
        UpdateFinalSymbols(finalSymbolIndex);

        yield return StartCoroutine(SnapToNearestSymbol()); // Smooth stop
    }

    private IEnumerator SnapToNearestSymbol()
    {
        float targetY = Mathf.Round(reelTransform.anchoredPosition.y / symbolHeight) * symbolHeight;
        float duration = 0.3f; // Smooth snapping time
        float elapsed = 0f;

        Vector2 startPos = reelTransform.anchoredPosition;
        Vector2 endPos = new Vector2(startPos.x, targetY);

        while (elapsed < duration)
        {
            reelTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        reelTransform.anchoredPosition = endPos; // Ensure exact positioning
        StopSpin();
    }

    private void StopSpin()
    {
        isSpinning = false;
    }

    private void RandomizeSymbols()
    {
        // **Randomize only top and bottom symbols, NOT middle**
        reelImages[0].sprite = symbolSprites[Random.Range(0, symbolSprites.Length)]; // Top
        reelImages[2].sprite = symbolSprites[Random.Range(0, symbolSprites.Length)]; // Bottom
    }

    private void UpdateFinalSymbols(int symbolIndex)
    {
        // **Ensuring final symbol alignment when stopping**
        reelImages[1].sprite = symbolSprites[symbolIndex]; // Middle stays fixed
        reelImages[0].sprite = symbolSprites[(symbolIndex + symbolSprites.Length - 1) % symbolSprites.Length]; // Top
        reelImages[2].sprite = symbolSprites[(symbolIndex + 1) % symbolSprites.Length]; // Bottom
    }

    public Sprite GetMiddleSymbol()
    {
        return reelImages[1].sprite; // Return middle symbol for win checking
    }
}
