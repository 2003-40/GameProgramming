using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Serializable map-hall level entry containing button, node position, target scene, and ticket cost.
/// </summary>
[System.Serializable]
public class MapHallLevelOption
{
    [SerializeField] private string displayName;
    [SerializeField] private string sceneName;
    [SerializeField] private int ticketCost;
    [SerializeField] private RectTransform nodeTransform;
    [SerializeField] private Button button;

    public MapHallLevelOption(string displayName, string sceneName, int ticketCost, RectTransform nodeTransform, Button button)
    {
        this.displayName = displayName;
        this.sceneName = sceneName;
        this.ticketCost = ticketCost;
        this.nodeTransform = nodeTransform;
        this.button = button;
    }

    public string DisplayName => displayName;
    public string SceneName => sceneName;
    public int TicketCost => ticketCost;
    public RectTransform NodeTransform => nodeTransform;
    public Button Button => button;
    public bool CanEnter => !string.IsNullOrWhiteSpace(SceneName);
}
