using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Mining Tile", menuName = "Tiles/Mining Tile")]
public class MiningTile : Tile
{
    [Header("Mining Settings")]
    public string gemstoneName;      // Gemstone name
    public int maxHealth = 3;         // Number of mining hits required
    public int hardness = 1;          // Base stamina cost hardness for each mining hit
    public GameObject dropPrefab;    // Physical item prefab dropped after successful mining

    public float staminaCostMultiplier = 1f;
    [Header("UX And Mechanics")]
    public int requiredToolLevel = 1;  // Minimum tool level required for mining
    public ParticleSystem hitParticlePrefab; // Particle burst played when the tile is hit
    
    // Optional crack sprites for different damage stages.
    // The controller currently darkens the tile color, leaving this for future sprite-based damage visuals.
    public Sprite[] crackSprites; 
}
