using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.GameContent.Mechanics;

namespace axleinblocks;

public sealed class AxleInBlocksMod : ModSystem
{
    public override void Start(ICoreAPI api)
    {
        api.RegisterBlockClass("AxleInBlocks", typeof(AxleInBlocks));
        api.RegisterBlockEntityClass("AxleInBlocksEntity", typeof(BEAxleInBlocks));
        api.RegisterBlockEntityBehaviorClass("AxleInBlocksBehavior", typeof(BEBehaviorAxleInBlocks));

        api.RegisterBlockClass("BlockGearsCrate", typeof(BlockGearsCrate));
        api.RegisterBlockEntityBehaviorClass("BEBehaviorMPGearsCrate", typeof(BEBehaviorMPGearsCrate));

        api.RegisterBlockClass("BlockTransmissionCovered", typeof(BlockTransmissionCovered));
        api.RegisterBlockEntityBehaviorClass(
            "BEBehaviorMPTransmissionCovered",
            typeof(BEBehaviorMPTransmissionCovered));
    }

    public override void AssetsFinalize(ICoreAPI api)
    {
        foreach (BlockLargeGear3m largeGear in api.World.Blocks.OfType<BlockLargeGear3m>())
        {
            if (largeGear.BlockBehaviors?.Any(
                    behavior => behavior is BlockBehaviorGearCrateCleanup) == true)
            {
                continue;
            }

            largeGear.BlockBehaviors = (largeGear.BlockBehaviors ?? [])
                .Append(new BlockBehaviorGearCrateCleanup(largeGear))
                .ToArray();
        }
    }
}
