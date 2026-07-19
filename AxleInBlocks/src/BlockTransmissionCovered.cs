using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent.Mechanics;

namespace axleinblocks;

public sealed class BlockTransmissionCovered : BlockTransmission
{
    public override bool TryPlaceBlock(
        IWorldAccessor world,
        IPlayer byPlayer,
        ItemStack itemstack,
        BlockSelection blockSel,
        ref string failureCode)
    {
        if (!CanPlaceBlock(world, byPlayer, blockSel, ref failureCode))
        {
            return false;
        }

        bool foundConnector = false;

        foreach (BlockFacing face in BlockFacing.HORIZONTALS)
        {
            BlockPos neighbourPos = blockSel.Position.AddCopy(face);
            if (world.BlockAccessor.GetBlock(neighbourPos) is not IMechanicalPowerBlock neighbour)
            {
                continue;
            }

            BlockFacing neighbourFace = face.Opposite;
            if (!neighbour.HasMechPowerConnectorAt(world, neighbourPos, neighbourFace, this))
            {
                continue;
            }

            foundConnector = true;
            string orientation = face.Axis == EnumAxis.X ? "we" : "ns";
            Block? toPlace = world.GetBlock(CodeWithVariant("orientation", orientation));
            if (toPlace == null)
            {
                failureCode = "missingorientationvariant";
                continue;
            }

            if (!toPlace.DoPlaceBlock(world, byPlayer, blockSel, itemstack))
            {
                continue;
            }

            neighbour.DidConnectAt(world, neighbourPos, neighbourFace);
            WasPlaced(world, blockSel.Position, face);

            BlockPos oppositePos = blockSel.Position.AddCopy(neighbourFace);
            if (world.BlockAccessor.GetBlock(oppositePos) is IMechanicalPowerBlock opposite &&
                opposite.HasMechPowerConnectorAt(world, oppositePos, face, this))
            {
                opposite.DidConnectAt(world, oppositePos, face);
                WasPlaced(world, blockSel.Position, neighbourFace);
            }

            return true;
        }

        // With no adjacent connector, vanilla handles the NWOrientable behavior and default placement.
        return !foundConnector && base.TryPlaceBlock(world, byPlayer, itemstack, blockSel, ref failureCode);
    }
}
