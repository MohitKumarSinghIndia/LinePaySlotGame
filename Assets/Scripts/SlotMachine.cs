using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class SlotMachine : MonoBehaviour
{
    public ReelController[] reels; // All reels in the slot machine
    public Button spinButton;
    public TMP_Text balanceText, betText, resultText; // UI elements

    public Sprite wildSymbol, scatterSymbol; // Wild and Scatter symbols

    private int playerBalance = 1000;
    private int betAmount = 10;
    private bool isSpinning = false; // Prevent multiple spins

    private void Start()
    {
        UpdateUI();
        spinButton.onClick.AddListener(SpinAllReels);
    }

    public void SpinAllReels()
    {
        if (isSpinning || playerBalance < betAmount)
            return; // Prevents multiple spins and checks balance

        isSpinning = true;
        resultText.text = "Spinning...";
        spinButton.interactable = false; // Disable button during spin

        playerBalance -= betAmount; // Deduct bet
        UpdateUI();

        // Start each reel with a delay for a natural spin effect
        for (int i = 0; i < reels.Length; i++)
        {
            StartCoroutine(SpinWithDelay(reels[i], i * 0.3f));
        }

        StartCoroutine(WaitForReelsToStop());
    }

    private IEnumerator SpinWithDelay(ReelController reel, float delay)
    {
        yield return new WaitForSeconds(delay);
        reel.StartSpin();
    }

    private IEnumerator WaitForReelsToStop()
    {
        yield return new WaitForSeconds(5f); // Wait for reels to stop spinning

        StartCoroutine(CheckSpinResult());
    }

    private IEnumerator CheckSpinResult()
    {
        yield return new WaitForSeconds(1f); // Wait for reels to stop

        (int winAmount, string winMessage) = CalculateWinnings();
        playerBalance += winAmount;

        resultText.text = winAmount > 0 ? $"You won: ${winAmount}! {winMessage}" : "No win, try again!";

        UpdateUI();
        isSpinning = false;
        spinButton.interactable = true; // Enable button
    }

    private (int, string) CalculateWinnings()
    {
        Sprite firstSymbol = reels[0].GetMiddleSymbol();
        Sprite secondSymbol = reels[1].GetMiddleSymbol();
        Sprite thirdSymbol = reels[2].GetMiddleSymbol();

        int winAmount = 0;
        string winMessage = "";

        int wildCount = 0, scatterCount = 0;

        // Count Wilds and Scatters
        if (firstSymbol == wildSymbol) wildCount++;
        if (secondSymbol == wildSymbol) wildCount++;
        if (thirdSymbol == wildSymbol) wildCount++;

        if (firstSymbol == scatterSymbol) scatterCount++;
        if (secondSymbol == scatterSymbol) scatterCount++;
        if (thirdSymbol == scatterSymbol) scatterCount++;

        // **SCATTER BONUS**
        if (scatterCount == 1)
        {
            winAmount += betAmount;
            winMessage += "(Single Scatter Bonus x1) ";
        }
        else if (scatterCount == 2)
        {
            winAmount += betAmount * 2;
            winMessage += "(Scatter Bonus x2) ";
        }
        else if (scatterCount == 3)
        {
            winAmount += betAmount * 5;
            winMessage += "(Scatter Jackpot x5) ";
        }

        // **MATCH CONDITIONS**
        bool firstTwoMatch = (firstSymbol == secondSymbol || firstSymbol == wildSymbol || secondSymbol == wildSymbol);
        bool lastTwoMatch = (secondSymbol == thirdSymbol || secondSymbol == wildSymbol || thirdSymbol == wildSymbol);
        bool allThreeMatch = (firstSymbol == secondSymbol && secondSymbol == thirdSymbol) || wildCount > 0;

        // **Winning Calculation**
        if (allThreeMatch)
        {
            winAmount += betAmount * 5;
            winMessage += "(Jackpot x5) ";
        }
        if (firstTwoMatch)
        {
            winAmount += betAmount * 2;
            winMessage += "(First Two Match x2) ";
        }
        if (lastTwoMatch)
        {
            winAmount += betAmount * 2;
            winMessage += "(Last Two Match x2) ";
        }

        // **✅ NEW: First & Last Match (Even if Scatter is in the Middle)**
        if (firstSymbol == thirdSymbol && firstSymbol != scatterSymbol)
        {
            winAmount += betAmount * 3;
            winMessage += "(First & Last Match x3) ";
        }

        return (winAmount, winMessage);
    }

    public void IncreaseBet()
    {
        betAmount = Mathf.Min(betAmount + 10, playerBalance);
        UpdateUI();
    }

    public void DecreaseBet()
    {
        betAmount = Mathf.Max(betAmount - 10, 10);
        UpdateUI();
    }

    private void UpdateUI()
    {
        balanceText.text = "Balance: $" + playerBalance;
        betText.text = "Bet: $" + betAmount;
    }
}
