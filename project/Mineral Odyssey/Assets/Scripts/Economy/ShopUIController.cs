using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Controls the hub shop UI, including opening/closing panels, upgrade purchases, and runtime layout repair.
/// </summary>
public class ShopUIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject shopPanelRoot;
    [SerializeField] private Button shopButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text messageText;

    [Header("Upgrade Items")]
    [SerializeField] private ScrollRect upgradeScrollRect;
    [SerializeField] private RectTransform upgradeScrollContent;
    [SerializeField] private float upgradeItemHeight = 300f;
    [SerializeField] private Transform toolUpgradeItem;
    [SerializeField] private Transform weaponUpgradeItem;

    private TMP_Text toolNameText;
    private TMP_Text toolLevelText;
    private TMP_Text toolEffectText;
    private TMP_Text toolCostText;
    private Button toolUpgradeButton;
    private TMP_Text toolUpgradeButtonText;

    private TMP_Text weaponNameText;
    private TMP_Text weaponLevelText;
    private TMP_Text weaponEffectText;
    private TMP_Text weaponCostText;
    private Button weaponUpgradeButton;
    private TMP_Text weaponUpgradeButtonText;

    private void Awake()
    {
        BindSceneReferences();
    }

    private void OnEnable()
    {
        // Re-bind every time because the hub UI can be created manually or repaired at runtime.
        BindSceneReferences();

        if (shopButton != null)
        {
            shopButton.onClick.RemoveListener(OpenShop);
            shopButton.onClick.AddListener(OpenShop);
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(CloseShop);
            closeButton.onClick.AddListener(CloseShop);
        }

        if (toolUpgradeButton != null)
        {
            toolUpgradeButton.onClick.RemoveListener(BuyToolUpgrade);
            toolUpgradeButton.onClick.AddListener(BuyToolUpgrade);
        }

        if (weaponUpgradeButton != null)
        {
            weaponUpgradeButton.onClick.RemoveListener(BuyWeaponUpgrade);
            weaponUpgradeButton.onClick.AddListener(BuyWeaponUpgrade);
        }

        GoldManager.Instance.GoldChanged += OnGoldChanged;
        PlayerUpgradeState.UpgradesChanged += Refresh;
        ConfigureUpgradeScrollList();
        Refresh();
        CloseShop();
    }

    private void OnDisable()
    {
        if (shopButton != null)
        {
            shopButton.onClick.RemoveListener(OpenShop);
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(CloseShop);
        }

        if (toolUpgradeButton != null)
        {
            toolUpgradeButton.onClick.RemoveListener(BuyToolUpgrade);
        }

        if (weaponUpgradeButton != null)
        {
            weaponUpgradeButton.onClick.RemoveListener(BuyWeaponUpgrade);
        }

        if (GoldManager.HasInstance)
        {
            GoldManager.Instance.GoldChanged -= OnGoldChanged;
        }

        PlayerUpgradeState.UpgradesChanged -= Refresh;
    }

    private void OpenShop()
    {
        if (shopPanelRoot != null)
        {
            shopPanelRoot.SetActive(true);
        }

        SetMessage("Choose an upgrade.", Color.white);
        Refresh();
    }

    private void CloseShop()
    {
        if (shopPanelRoot != null)
        {
            shopPanelRoot.SetActive(false);
        }
    }

    private void BuyToolUpgrade()
    {
        // Tool upgrades improve ore access and mining stamina efficiency.
        if (PlayerUpgradeState.IsToolMaxed)
        {
            SetMessage("Mining tool is already max level.", WarningColor);
            Refresh();
            return;
        }

        int cost = PlayerUpgradeState.ToolUpgradeCost;
        if (!GoldManager.Instance.TrySpendGold(cost))
        {
            SetMessage($"Not enough gold. Need {cost - GoldManager.Instance.Gold} more.", FailureColor);
            Refresh();
            return;
        }

        PlayerUpgradeState.TryUpgradeTool(out _);
        SetMessage("Mining tool upgraded.", SuccessColor);
        Refresh();
    }

    private void BuyWeaponUpgrade()
    {
        // Weapon upgrades make monster pressure more manageable without adding complex combat.
        if (PlayerUpgradeState.IsWeaponMaxed)
        {
            SetMessage("Weapon is already max level.", WarningColor);
            Refresh();
            return;
        }

        int cost = PlayerUpgradeState.WeaponUpgradeCost;
        if (!GoldManager.Instance.TrySpendGold(cost))
        {
            SetMessage($"Not enough gold. Need {cost - GoldManager.Instance.Gold} more.", FailureColor);
            Refresh();
            return;
        }

        PlayerUpgradeState.TryUpgradeWeapon(out _);
        SetMessage("Weapon upgraded.", SuccessColor);
        Refresh();
    }

    private void OnGoldChanged(int currentGold)
    {
        Refresh();
    }

    private void Refresh()
    {
        if (goldText != null)
        {
            goldText.text = $"Gold:{GoldManager.Instance.Gold}";
        }

        UpdateToolItem();
        UpdateWeaponItem();
    }

    private void UpdateToolItem()
    {
        int level = PlayerUpgradeState.ToolLevel;
        bool isMaxed = PlayerUpgradeState.IsToolMaxed;

        SetText(toolNameText, "Mining Tool");
        SetText(toolLevelText, $"Level {level}");
        SetText(toolEffectText, $"effect: Mining power {PlayerUpgradeState.ToolMiningPower} | stamina x{PlayerUpgradeState.ToolStaminaMultiplier:0.##}");
        SetText(toolCostText, isMaxed ? "cost: Max" : $"cost: {PlayerUpgradeState.ToolUpgradeCost}");

        if (toolUpgradeButton != null)
        {
            toolUpgradeButton.interactable = !isMaxed;
        }

        SetText(toolUpgradeButtonText, isMaxed ? "Max" : "Upgrade");
    }

    private void UpdateWeaponItem()
    {
        int level = PlayerUpgradeState.WeaponLevel;
        bool isMaxed = PlayerUpgradeState.IsWeaponMaxed;

        SetText(weaponNameText, "Weapon");
        SetText(weaponLevelText, $"Level {level}");
        SetText(weaponEffectText, $"effect: Hit damage {PlayerUpgradeState.WeaponDamage} | monster contact -{level - 1}");
        SetText(weaponCostText, isMaxed ? "cost: Max" : $"cost: {PlayerUpgradeState.WeaponUpgradeCost}");

        if (weaponUpgradeButton != null)
        {
            weaponUpgradeButton.interactable = !isMaxed;
        }

        SetText(weaponUpgradeButtonText, isMaxed ? "Max" : "Upgrade");
    }

    private void BindSceneReferences()
    {
        // Name-based binding keeps the UI functional when scene references are lost during iteration.
        if (shopPanelRoot == null)
        {
            shopPanelRoot = FindSceneObject("ShopPanelRoot");
        }

        if (shopButton == null)
        {
            shopButton = FindSceneComponent<Button>("ShopButton");
        }

        if (closeButton == null)
        {
            closeButton = FindSceneComponent<Button>("CloseButton");
        }

        if (goldText == null)
        {
            goldText = FindSceneComponent<TMP_Text>("GoldText");
        }

        if (messageText == null)
        {
            messageText = FindSceneComponent<TMP_Text>("MessageText");
        }

        if (upgradeScrollRect == null)
        {
            GameObject scrollObject = FindSceneObject("UpgradeScrollList");
            if (scrollObject != null)
            {
                upgradeScrollRect = scrollObject.GetComponent<ScrollRect>();
                if (upgradeScrollRect == null)
                {
                    upgradeScrollRect = scrollObject.AddComponent<ScrollRect>();
                }
            }
        }

        if (upgradeScrollContent == null)
        {
            Transform content = upgradeScrollRect != null ? upgradeScrollRect.transform.Find("Viewport/Content") : null;
            if (content == null)
            {
                GameObject contentObject = FindSceneObject("Content");
                content = contentObject != null ? contentObject.transform : null;
            }

            upgradeScrollContent = content != null ? content.GetComponent<RectTransform>() : null;
        }

        if (toolUpgradeItem == null)
        {
            GameObject toolObject = FindSceneObject("ToolUpgradeItem");
            toolUpgradeItem = toolObject != null ? toolObject.transform : null;
        }

        if (weaponUpgradeItem == null)
        {
            GameObject weaponObject = FindBestUpgradeItem("WeaponUpgradeItem");
            weaponUpgradeItem = weaponObject != null ? weaponObject.transform : null;
        }

        BindUpgradeItem(toolUpgradeItem, out toolNameText, out toolLevelText, out toolEffectText, out toolCostText, out toolUpgradeButton, out toolUpgradeButtonText);
        BindUpgradeItem(weaponUpgradeItem, out weaponNameText, out weaponLevelText, out weaponEffectText, out weaponCostText, out weaponUpgradeButton, out weaponUpgradeButtonText);
    }

    private void ConfigureUpgradeScrollList()
    {
        // The shop list is repaired in code so upgrade entries stay readable in the submitted scene.
        if (upgradeScrollRect == null)
        {
            return;
        }

        upgradeScrollRect.horizontal = false;
        upgradeScrollRect.vertical = true;
        upgradeScrollRect.movementType = ScrollRect.MovementType.Clamped;
        upgradeScrollRect.inertia = true;

        if (upgradeScrollContent == null)
        {
            Transform content = upgradeScrollRect.transform.Find("Viewport/Content");
            upgradeScrollContent = content != null ? content.GetComponent<RectTransform>() : null;
        }

        if (upgradeScrollContent != null)
        {
            upgradeScrollRect.content = upgradeScrollContent;
            HideEmptyDuplicateUpgradeItems(upgradeScrollContent);
            ConfigureContentLayout(upgradeScrollContent);
        }

        if (upgradeScrollRect.viewport == null)
        {
            Transform viewport = upgradeScrollRect.transform.Find("Viewport");
            upgradeScrollRect.viewport = viewport != null ? viewport.GetComponent<RectTransform>() : null;
        }

        if (upgradeScrollRect.viewport != null && upgradeScrollRect.viewport.GetComponent<RectMask2D>() == null)
        {
            upgradeScrollRect.viewport.gameObject.AddComponent<RectMask2D>();
        }

        ConfigureUpgradeItemLayout(toolUpgradeItem, upgradeItemHeight);
        ConfigureUpgradeItemLayout(weaponUpgradeItem, upgradeItemHeight);

        Canvas.ForceUpdateCanvases();
        if (upgradeScrollContent != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(upgradeScrollContent);
        }

        upgradeScrollRect.verticalNormalizedPosition = 1f;
    }

    private static void ConfigureContentLayout(RectTransform content)
    {
        VerticalLayoutGroup layoutGroup = content.GetComponent<VerticalLayoutGroup>();
        if (layoutGroup == null)
        {
            layoutGroup = content.gameObject.AddComponent<VerticalLayoutGroup>();
        }

        layoutGroup.padding = new RectOffset(10, 10, 10, 10);
        layoutGroup.spacing = 12f;
        layoutGroup.childAlignment = TextAnchor.UpperCenter;
        layoutGroup.childControlWidth = true;
        layoutGroup.childControlHeight = true;
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childForceExpandHeight = false;

        ContentSizeFitter sizeFitter = content.GetComponent<ContentSizeFitter>();
        if (sizeFitter == null)
        {
            sizeFitter = content.gameObject.AddComponent<ContentSizeFitter>();
        }

        sizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    private static void ConfigureUpgradeItemLayout(Transform item, float preferredHeight)
    {
        if (item == null)
        {
            return;
        }

        LayoutElement layoutElement = item.GetComponent<LayoutElement>();
        if (layoutElement == null)
        {
            layoutElement = item.gameObject.AddComponent<LayoutElement>();
        }

        layoutElement.flexibleWidth = 1f;
        layoutElement.minHeight = Mathf.Max(layoutElement.minHeight, preferredHeight);
        layoutElement.preferredHeight = Mathf.Max(layoutElement.preferredHeight, preferredHeight);
    }

    private static Transform FindChildTransform(Transform root, string objectName)
    {
        if (root == null)
        {
            return null;
        }

        Transform[] children = root.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].name == objectName)
            {
                return children[i];
            }
        }

        return null;
    }

    private static void HideEmptyDuplicateUpgradeItems(RectTransform content)
    {
        // Some UI iterations left placeholder duplicates; hide only entries without usable shop fields.
        for (int i = 0; i < content.childCount; i++)
        {
            Transform child = content.GetChild(i);
            if (!child.name.StartsWith("WeaponUpgradeItem"))
            {
                continue;
            }

            bool hasShopFields = FindChildComponent<TMP_Text>(child, "NameText") != null
                || FindChildComponent<Button>(child, "UpgradeButton") != null;
            if (!hasShopFields)
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    private static void BindUpgradeItem(
        Transform item,
        out TMP_Text nameText,
        out TMP_Text levelText,
        out TMP_Text effectText,
        out TMP_Text costText,
        out Button upgradeButton,
        out TMP_Text upgradeButtonText)
    {
        nameText = FindChildComponent<TMP_Text>(item, "NameText");
        levelText = FindChildComponent<TMP_Text>(item, "LevelText");
        effectText = FindChildComponent<TMP_Text>(item, "EffectText");
        costText = FindChildComponent<TMP_Text>(item, "CostText");
        upgradeButton = FindChildComponent<Button>(item, "UpgradeButton");
        upgradeButtonText = upgradeButton != null ? upgradeButton.GetComponentInChildren<TMP_Text>(true) : null;
    }

    private void SetMessage(string message, Color color)
    {
        if (messageText == null)
        {
            return;
        }

        messageText.text = message;
        messageText.color = color;
    }

    private static void SetText(TMP_Text text, string value)
    {
        if (text != null)
        {
            text.text = value;
        }
    }

    private static T FindChildComponent<T>(Transform root, string objectName) where T : Component
    {
        if (root == null)
        {
            return null;
        }

        Transform[] children = root.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].name == objectName && children[i].TryGetComponent(out T component))
            {
                return component;
            }
        }

        return null;
    }

    private static T FindSceneComponent<T>(string objectName) where T : Component
    {
        GameObject target = FindSceneObject(objectName);
        if (target == null)
        {
            return null;
        }

        T component = target.GetComponent<T>();
        if (component != null)
        {
            return component;
        }

        return target.GetComponentInChildren<T>(true);
    }

    private static GameObject FindBestUpgradeItem(string objectName)
    {
        // Prefer the candidate that has the expected child labels/buttons, not just the matching name.
        Transform[] transforms = Resources.FindObjectsOfTypeAll<Transform>();
        GameObject fallback = null;
        for (int i = 0; i < transforms.Length; i++)
        {
            Transform current = transforms[i];
            if (current == null || !current.name.StartsWith(objectName))
            {
                continue;
            }

            GameObject gameObject = current.gameObject;
            if (!gameObject.scene.IsValid() || gameObject.scene != SceneManager.GetActiveScene())
            {
                continue;
            }

            if (fallback == null)
            {
                fallback = gameObject;
            }

            if (FindChildComponent<TMP_Text>(current, "NameText") != null
                && FindChildComponent<Button>(current, "UpgradeButton") != null)
            {
                return gameObject;
            }
        }

        return fallback;
    }

    public static GameObject FindSceneObject(string objectName)
    {
        Transform[] transforms = Resources.FindObjectsOfTypeAll<Transform>();
        for (int i = 0; i < transforms.Length; i++)
        {
            Transform current = transforms[i];
            if (current == null || current.name != objectName)
            {
                continue;
            }

            GameObject gameObject = current.gameObject;
            if (gameObject.scene.IsValid() && gameObject.scene == SceneManager.GetActiveScene())
            {
                return gameObject;
            }
        }

        return null;
    }

    private static Color SuccessColor => new Color(0.62f, 0.95f, 0.50f, 1f);
    private static Color FailureColor => new Color(1f, 0.45f, 0.32f, 1f);
    private static Color WarningColor => new Color(0.95f, 0.78f, 0.45f, 1f);
}
