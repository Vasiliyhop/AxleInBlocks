using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent.Mechanics;

namespace axleinblocks;

/// <summary>
/// A covered axle is self-supporting: the surrounding block is its support.
/// Placement and connector selection are inherited from the current vanilla axle.
/// </summary>
public sealed class AxleInBlocks : BlockAxle
{
    public override void OnNeighbourBlockChange(IWorldAccessor world, BlockPos pos, BlockPos neibpos)
    {
        EnumHandling handling = EnumHandling.PassThrough;

        foreach (BlockBehavior behavior in BlockBehaviors)
        {
            behavior.OnNeighbourBlockChange(world, pos, neibpos, ref handling);
            if (handling == EnumHandling.PreventSubsequent)
            {
                return;
            }
        }
    }
}
