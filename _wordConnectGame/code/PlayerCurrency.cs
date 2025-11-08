using UnityEngine;
using TMPro;

public class PlayerCurrency : MonoBehaviour
{
    public int coins;
    public TextMeshProUGUI coinsText;
    //public static event Action<>

    public void AddCoins(int coins)
    { 
        this.coins += coins;
        coinsText.text = this.coins.ToString("N0");
    }

    public void SpendCoins(int coins)
    {
        this.coins -= coins;

        if (this.coins < 0)
        {
            this.coins = 0;
        }

        coinsText.text = this.coins.ToString("N0");

    }
}
