using Vintagestory.API.Common;
using Vintagestory.GameContent.Mechanics;

namespace axleinblocks;

public sealed class BEBehaviorAxleInBlocks : BEBehaviorMPAxle
{
    public BEBehaviorAxleInBlocks(BlockEntity blockEntity) : base(blockEntity)
    {
    }

    protected override bool AddStands => false;

    protected override CompositeShape GetShape()
    {
        CompositeShape shape = new()
        {
            Base = new AssetLocation("game:shapes/block/wood/mechanics/axle.json")
        };

        switch (Block.Variant["rotation"])
        {
            case "ns":
                shape.rotateY = 90;
                break;
            case "ud":
                shape.rotateZ = 90;
                break;
        }

        return shape;
    }
}
