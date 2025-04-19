using System.Linq;
using UnityEngine;

public class GameLevel : MonoBehaviour, Services.IRegistrable
{
    [SerializeField] MapGenerator _mapGenerator;

    void Awake()
    {
        ServiceLocator.Register(this);
    }

    void Start()
    {
        
    }

    public WaterBlock GetWaterBlockAtZ(float playerZ)
    {
        var waterBlock = _mapGenerator.WaterBlocks.FirstOrDefault(w => w.IsPlayerOnBlock(playerZ));
        if (waterBlock == null)
        {
            // can happen if bots are too far away from the player
            Debug.LogWarning($"No water block found for player at Z: {playerZ}");
            return null;
        }
        return waterBlock;
    }

    void Update()
    {
        _mapGenerator.UpdateMap();
    }
}
