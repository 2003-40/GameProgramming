using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StaminaDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text staminaText;
    [SerializeField] private Image staminaFill;
    [SerializeField] private string label = "Stamina";

    private void Awake()
    {
        if (staminaText == null)
        {
            staminaText = GetComponentInChildren<TMP_Text>();
        }
    }

    private void OnEnable()
    {
        StaminaManager.Instance.StaminaChanged += UpdateStamina;
        UpdateStamina(StaminaManager.Instance.CurrentStamina, StaminaManager.Instance.MaxStamina);
    }

    private void OnDisable()
    {
        if (StaminaManager.HasInstance)
        {
            StaminaManager.Instance.StaminaChanged -= UpdateStamina;
        }
    }

    public void Initialize(TMP_Text text, Image fill)
    {
        staminaText = text;
        staminaFill = fill;
        UpdateStamina(StaminaManager.Instance.CurrentStamina, StaminaManager.Instance.MaxStamina);
    }

    private void UpdateStamina(int current, int max)
    {
        float staminaPercent = max > 0 ? Mathf.Clamp01((float)current / max) : 0f;

        if (staminaText != null)
        {
            staminaText.text = $"{current}/{max}";
        }

        if (staminaFill != null)
        {
            staminaFill.fillAmount = staminaPercent;
            staminaFill.rectTransform.localScale = new Vector3(staminaPercent, 1f, 1f);
            staminaFill.color = Color.Lerp(new Color(0.9f, 0.15f, 0.1f, 1f), new Color(0.3f, 0.85f, 0.35f, 1f), staminaPercent);
        }
    }
}
