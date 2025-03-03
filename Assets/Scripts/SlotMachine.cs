using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotMachineManager : MonoBehaviour
{
    public ReelController reel1, reel2, reel3;
    public WinAnimation winAnimation;

    public TMP_Text balanceDisplay, resultDisplay, betAmountDisplay;
    public Button spinButton;

    private int balance = 1000;
    private int betAmount = 10;

    private int freeSpinCounter = 0;
    private const int maxFreeSpins = 3;



    private void Start()
    {
        spinButton.onClick.AddListener(Spin);
        UpdateUI();
    }

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

    private IEnumerator SpinReels()
    {
        spinButton.interactable = false;

        reel1.StartSpin();
        reel2.StartSpin();
        reel3.StartSpin();

        resultDisplay.text = "Spinning...";

        yield return new WaitUntil(() => !reel1.IsSpinning() && !reel2.IsSpinning() && !reel3.IsSpinning());

        yield return new WaitForSeconds(0.5f);

        CheckWin();
    }

    private void CheckWin()
    {
        string firstSymbol = reel1.GetMiddleSymbol().name;
        string secondSymbol = reel2.GetMiddleSymbol().name;
        string thirdSymbol = reel3.GetMiddleSymbol().name;

        Sprite firstSymbolSprite = reel1.GetMiddleSymbol();
        Sprite secondSymbolSprite = reel2.GetMiddleSymbol();
        Sprite thirdSymbolSprite = reel3.GetMiddleSymbol();

        int winnings = 0;
        int scatterCount = 0;
        string winReason = "";

        bool isFirstWild = firstSymbol.Contains("Wild");
        bool isSecondWild = secondSymbol.Contains("Wild");
        bool isThirdWild = thirdSymbol.Contains("Wild");

        bool isFirstScatter = firstSymbol.Contains("Scatter");
        bool isSecondScatter = secondSymbol.Contains("Scatter");
        bool isThirdScatter = thirdSymbol.Contains("Scatter");

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

        if (winnings > 0)
        {
            winAnimation.PlayWinAnimation(firstSymbolSprite, secondSymbolSprite, thirdSymbolSprite);

            Delay(3.5f, nameof(EnableSpinButton));
        }
        else
        {
            spinButton.interactable = true;
        }

        balance += winnings;
        UpdateUI();
        resultDisplay.text = winnings > 0 ? $"You won: ${winnings}! {winReason}" : "Try again!";

    }
    private void Delay(float delay, string methodName) 
    { 
        Invoke(methodName, delay); 
    }

    private void EnableSpinButton()
    {
        spinButton.interactable = true;
    }

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
        Spin();
    }
    public void IncreaseBet()
    {
        betAmount += 10;
        UpdateUI();
    }

    public void DecreaseBet()
    {
        if (betAmount > 10)
        {
            betAmount -= 10;
            UpdateUI();
        }
    }

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
