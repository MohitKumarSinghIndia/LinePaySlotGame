using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotMachineManager : MonoBehaviour
{
    // Reel Controllers
    public ReelController reel1, reel2, reel3;

    // Betting & Balance UI
    public TMP_Text balanceDisplay, resultDisplay, betAmountDisplay;
    public Button spinButton;

    // Balance and Bet
    private int balance = 1000;
    private int betAmount = 10;

    // Free Spin Tracker
    private int freeSpinCounter = 0;
    private const int maxFreeSpins = 3;

    private void Start()
    {
        spinButton.onClick.AddListener(Spin);
        UpdateUI();
    }

    // Spin Button Action
    public void Spin()
    {
        if (balance < betAmount)
        {
            resultDisplay.text = "Not enough balance!";
            return;
        }

        balance -= betAmount;
        UpdateUI();
        StartCoroutine(SpinReels());
    }

    // Handle Full Spin Cycle
    private IEnumerator SpinReels()
    {
        spinButton.interactable = false;

        // Start Spinning All Reels
        reel1.StartSpin();
        reel2.StartSpin();
        reel3.StartSpin();

        resultDisplay.text = "Spinning...";

        // Wait until ALL reels stop spinning
        yield return new WaitUntil(() => !reel1.IsSpinning() && !reel2.IsSpinning() && !reel3.IsSpinning());

        // Add a short delay before checking the win condition
        yield return new WaitForSeconds(0.5f);

        // Now Check Win Condition
        CheckWin();

        spinButton.interactable = true;
    }

    private void CheckWin()
    {
        // Get Middle Symbol Image Names
        string firstSymbol = reel1.GetMiddleSymbol().name;
        string secondSymbol = reel2.GetMiddleSymbol().name;
        string thirdSymbol = reel3.GetMiddleSymbol().name;

        Debug.Log($"First Symbol: {firstSymbol}");
        Debug.Log($"Second Symbol: {secondSymbol}");
        Debug.Log($"Third Symbol: {thirdSymbol}");

        int winnings = 0;
        int scatterCount = 0;
        string winReason = "";

        // Identify Wild & Scatter Symbols
        bool isFirstWild = firstSymbol.Contains("Wild");
        bool isSecondWild = secondSymbol.Contains("Wild");
        bool isThirdWild = thirdSymbol.Contains("Wild");

        bool isFirstScatter = firstSymbol.Contains("Scatter");
        bool isSecondScatter = secondSymbol.Contains("Scatter");
        bool isThirdScatter = thirdSymbol.Contains("Scatter");

        // Count scatter symbols
        if (isFirstScatter) scatterCount++;
        if (isSecondScatter) scatterCount++;
        if (isThirdScatter) scatterCount++;

        // Scatter Wins
        if (scatterCount == 2)
        {
            winnings += betAmount * 2;
            winReason = "Two scatter symbols!";
        }
        else if (scatterCount == 3)
        {
            winnings += betAmount * 5;
            winReason = "Three scatter symbols! Free spin awarded!";
            StartCoroutine(FreeSpin());
        }

        // Regular Line Wins (including wilds)
        if (firstSymbol == secondSymbol && secondSymbol == thirdSymbol)
        {
            winnings += betAmount * 5;
            winReason = $"Three matching symbols: {firstSymbol}";
        }
        else if (isFirstWild && secondSymbol == thirdSymbol)
        {
            winnings += betAmount * 5;
            winReason = $"Wild + two matching symbols: {secondSymbol}";
        }
        else if (isSecondWild && firstSymbol == thirdSymbol)
        {
            winnings += betAmount * 5;
            winReason = $"Wild in middle + matching symbols: {firstSymbol}";
        }
        else if (isThirdWild && firstSymbol == secondSymbol)
        {
            winnings += betAmount * 5;
            winReason = $"Wild at end + two matching symbols: {firstSymbol}";
        }
        // Partial Wins (only if adjacent symbols match)
        else if ((firstSymbol == secondSymbol && thirdSymbol != firstSymbol) ||
                 (secondSymbol == thirdSymbol && firstSymbol != secondSymbol))
        {
            winnings += betAmount * 2;
            winReason = $"Nice! You got a partial win with {firstSymbol} and {secondSymbol}!";
        }

        // Special Case: Double Wild Bonus
        else if ((isFirstWild && isSecondWild) || (isSecondWild && isThirdWild))
        {
            winnings += betAmount * 3;
            winReason = "Double wild boost!";
        }

        // Update Balance & Display
        balance += winnings;
        UpdateUI();
        resultDisplay.text = winnings > 0 ? $"You won: ${winnings}! {winReason}" : "Try again!";
    }

    // Free Spin Functionality (limited to prevent infinite loops)
    private IEnumerator FreeSpin()
    {
        if (freeSpinCounter >= maxFreeSpins)
        {
            resultDisplay.text = "No more free spins!";
            yield break;
        }

        freeSpinCounter++;
        resultDisplay.text = "Free Spin Awarded!";
        yield return new WaitForSeconds(1.5f);
        Spin(); // Automatically trigger a free spin
    }

    // Increase Bet Amount
    public void IncreaseBet()
    {
        betAmount += 10;
        UpdateUI();
    }

    // Decrease Bet Amount
    public void DecreaseBet()
    {
        if (betAmount > 10)
        {
            betAmount -= 10;
            UpdateUI();
        }
    }

    // Update UI Elements
    private void UpdateUI()
    {
        balanceDisplay.text = $"${balance}";
        betAmountDisplay.text = $"${betAmount}";
    }

    private void OnDestroy()
    {
        spinButton.onClick.RemoveListener(Spin);
    }
}
