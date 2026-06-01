using TMPro;
using UnityEngine;

public class GoldDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private string label = "Gold";

    private void Awake()
    {
        if (goldText == null)
        {
            goldText = GetComponent<TMP_Text>();
        }
    }

    private void OnEnable()
    {
        GoldManager.Instance.GoldChanged += UpdateGoldText;
        UpdateGoldText(GoldManager.Instance.Gold);
    }

    private void OnDisable()
    {
        if (GoldManager.HasInstance)
        {
            GoldManager.Instance.GoldChanged -= UpdateGoldText;
        }
    }

    private void UpdateGoldText(int currentGold)
    {
        if (goldText == null)
        {
            return;
        }

        goldText.text = $"{label}: {currentGold}";
    }
}
