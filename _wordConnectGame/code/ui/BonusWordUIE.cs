using TMPro;
using UnityEngine;

public class BonusWordUIE : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshPro;

    public void SetElementText(string word)
    {
        textMeshPro.SetText(word);
    }
}
