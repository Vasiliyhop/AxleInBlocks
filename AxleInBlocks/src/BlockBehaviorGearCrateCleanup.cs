using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace axleinblocks;

/// <summary>
/// Vanilla large gears reduce attached angled cage gears to peg gears before
/// removal. This behavior performs the same conversion for mod-domain crates
/// before vanilla's game-domain-only lookup runs.
/// </summary>
public sealed class BlockBehaviorGearCrateCleanup : BlockBehavior
{
    public BlockBehaviorGearCrateCleanup(Block block) : base(block)
    {
    }

    public override bool DoPlaceBlock(
        IWorldAccessor world,
        IPlayer byPlayer,
        BlockSelection blockSel,
        ItemStack byItemStack,
        ref EnumHandling handling)
    {
        foreach (BlockFacing facing in BlockFacing.HORIZONTALS)
        {
            if (world.BlockAccessor.GetBlock(blockSel.Position.AddCopy(facing))
                is not BlockGearsCrate)
            {
                continue;
            }

            handling = EnumHandling.PreventSubsequent;
            (byPlayer as IServerPlayer)?.SendIngameError(
                "axleinblocks-largegear-coveredcrate",
                "Remove covered angled gears beside the large gear before placing it.");
            return false;
        }

        return true;
    }

    public override void OnBlockRemoved(
        IWorldAccessor world,
        BlockPos pos,
        ref EnumHandling handling)
    {
        foreach (BlockFacing facing in BlockFacing.HORIZONTALS)
        {
            BlockPos gearPos = pos.AddCopy(facing);
            if (world.BlockAccessor.GetBlock(gearPos) is BlockGearsCrate gearCrate)
            {
                gearCrate.ConvertToPegGear(world, gearPos);
            }
        }

        handling = EnumHandling.PassThrough;
    }
}
