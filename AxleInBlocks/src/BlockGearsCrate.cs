using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent.Mechanics;

#nullable disable

namespace axleinblocks;

/// <summary>
/// Vanilla angled gears assume that orientation is their only code variant.
/// Gear crates also vary by cover shape and material, so the three methods that
/// change orientation must preserve the complete mod-domain block code.
/// </summary>
public sealed class BlockGearsCrate : BlockAngledGears
{
    private Block GetGearBlock(
        IWorldAccessor world,
        bool cageGear,
        BlockFacing facing,
        BlockFacing adjacentFacing = null)
    {
        if (adjacentFacing == null)
        {
            char orientation = facing.Code[0];
            string code = cageGear ? $"{orientation}{orientation}" : orientation.ToString();
            return world.GetBlock(CodeWithVariant("orientation", code));
        }

        Block block = world.GetBlock(CodeWithVariant(
            "orientation",
            $"{adjacentFacing.Code[0]}{facing.Code[0]}"));

        return block ?? world.GetBlock(CodeWithVariant(
            "orientation",
            $"{facing.Code[0]}{adjacentFacing.Code[0]}"));
    }

    public override void DidConnectAt(IWorldAccessor world, BlockPos pos, BlockFacing face)
    {
        if (!IsDeadEnd())
        {
            return;
        }

        BlockFacing currentFace = BlockFacing.FromFirstLetter(Orientation[0]);
        if (!currentFace.IsAdjacent(face))
        {
            return;
        }

        Block replacement = GetGearBlock(world, false, Facings[0], face);
        if (replacement != null)
        {
            ExchangeBlockAt(world, pos, replacement);
        }
    }

    public override bool TryPlaceBlock(
        IWorldAccessor world,
        IPlayer byPlayer,
        ItemStack itemstack,
        BlockSelection blockSel,
        ref string failureCode)
    {
        Block existingBlock = world.BlockAccessor.GetBlock(blockSel.Position);
        if (!CanPlaceBlock(world, byPlayer, blockSel, ref failureCode, existingBlock))
        {
            return false;
        }

        BlockFacing firstFace = null;
        BlockFacing secondFace = null;
        BlockMPMultiblockGear largeGearEdge = existingBlock as BlockMPMultiblockGear;
        bool validLargeGear = false;

        if (largeGearEdge != null &&
            world.BlockAccessor.GetBlockEntity(blockSel.Position) is BEMPMultiblock multiblock)
        {
            validLargeGear = multiblock.Principal != null;
        }

        foreach (BlockFacing face in BlockFacing.ALLFACES)
        {
            if (validLargeGear && (face == BlockFacing.UP || face == BlockFacing.DOWN))
            {
                continue;
            }

            BlockPos neighbourPos = blockSel.Position.AddCopy(face);
            if (world.BlockAccessor.GetBlock(neighbourPos) is not IMechanicalPowerBlock neighbour ||
                !neighbour.HasMechPowerConnectorAt(world, neighbourPos, face.Opposite, this))
            {
                continue;
            }

            if (firstFace == null)
            {
                firstFace = face;
            }
            else if (face.IsAdjacent(firstFace))
            {
                secondFace = face;
                break;
            }
        }

        if (firstFace == null)
        {
            failureCode = "requiresaxle";
            return false;
        }

        BlockPos firstPos = blockSel.Position.AddCopy(firstFace);
        BlockEntity firstEntity = world.BlockAccessor.GetBlockEntity(firstPos);
        IMechanicalPowerBlock firstNeighbour = world.BlockAccessor.GetBlock(firstPos) as IMechanicalPowerBlock;

        if (IsUnsupportedAxle(world, firstEntity, firstNeighbour as Block, firstPos))
        {
            failureCode = "axlemusthavesupport";
            return false;
        }

        Block toPlace = GetGearBlock(world, validLargeGear, firstFace, secondFace);
        if (toPlace == null)
        {
            failureCode = "missingorientationvariant";
            return false;
        }

        BlockEntity largeGearEntity = validLargeGear
            ? largeGearEdge.GearPlaced(world, blockSel.Position)
            : null;

        world.BlockAccessor.SetBlock(toPlace.BlockId, blockSel.Position);

        if (secondFace != null)
        {
            BlockPos secondPos = blockSel.Position.AddCopy(secondFace);
            if (world.BlockAccessor.GetBlock(secondPos) is IMechanicalPowerBlock secondNeighbour)
            {
                secondNeighbour.DidConnectAt(world, secondPos, secondFace.Opposite);
            }
        }

        BEBehaviorMPGearsCrate gearBehavior = world.BlockAccessor
            .GetBlockEntity(blockSel.Position)?
            .GetBehavior<BEBehaviorMPGearsCrate>();

        // This matches vanilla behavior at the world height boundary, where no BE is created.
        if (gearBehavior == null)
        {
            return true;
        }

        if (largeGearEntity?.GetBehavior<BEBehaviorMPBase>() is BEBehaviorMPLargeGear3m largeGear)
        {
            gearBehavior.AddToLargeGearNetwork(largeGear, firstFace);
        }

        firstNeighbour?.DidConnectAt(world, firstPos, firstFace.Opposite);

        gearBehavior.newlyPlaced = true;
        try
        {
            if (!gearBehavior.tryConnect(firstFace) && secondFace != null)
            {
                gearBehavior.tryConnect(secondFace);
            }
        }
        finally
        {
            gearBehavior.newlyPlaced = false;
        }

        return true;
    }

    public override void OnNeighbourBlockChange(IWorldAccessor world, BlockPos pos, BlockPos neibpos)
    {
        string orientations = Orientation;
        if (orientations.Length == 2 && orientations[0] == orientations[1])
        {
            orientations = orientations[0].ToString();
        }

        BlockFacing[] facings = orientations.Length == 1
            ? [BlockFacing.FromFirstLetter(orientations[0])]
            : [
                BlockFacing.FromFirstLetter(orientations[0]),
                BlockFacing.FromFirstLetter(orientations[1])
            ];

        List<BlockFacing> lostFacings = [];

        foreach (BlockFacing facing in facings)
        {
            BlockPos neighbourPos = pos.AddCopy(facing);
            bool disconnected = world.BlockAccessor
                .GetBlockEntity(pos)?
                .GetBehavior<BEBehaviorMPBase>()?
                .disconnected == true;

            if (world.BlockAccessor.GetBlock(neighbourPos) is not IMechanicalPowerBlock neighbour ||
                !neighbour.HasMechPowerConnectorAt(world, neighbourPos, facing.Opposite, this) ||
                disconnected)
            {
                lostFacings.Add(facing);
            }
        }

        if (lostFacings.Count == orientations.Length)
        {
            world.BlockAccessor.BreakBlock(pos, null);
            return;
        }

        if (lostFacings.Count == 0)
        {
            return;
        }

        orientations = orientations.Replace(lostFacings[0].Code[0].ToString(), string.Empty);
        Block replacement = world.GetBlock(CodeWithVariant("orientation", orientations));
        if (replacement == null)
        {
            world.Logger.Error(
                "Axle in blocks could not find orientation variant {0} for {1} at {2}.",
                orientations,
                Code,
                pos);
            return;
        }

        ExchangeBlockAt(world, pos, replacement);
        BlockEntity ownEntity = world.BlockAccessor.GetBlockEntity(pos);
        ownEntity?.GetBehavior<BEBehaviorMPBase>()?.LeaveNetwork();

        BlockFacing remainingFace = BlockFacing.FromFirstLetter(orientations[0]);
        BlockPos remainingPos = pos.AddCopy(remainingFace);
        BlockEntity remainingEntity = world.BlockAccessor.GetBlockEntity(remainingPos);
        IMechanicalPowerBlock remainingNeighbour = world.BlockAccessor.GetBlock(remainingPos) as IMechanicalPowerBlock;

        if (IsUnsupportedAxle(world, remainingEntity, remainingNeighbour as Block, remainingPos))
        {
            return;
        }

        remainingNeighbour?.DidConnectAt(world, remainingPos, remainingFace.Opposite);
        WasPlaced(world, pos, remainingFace);
    }

    public override AssetLocation GetRotatedBlockCode(int angle)
    {
        string orientation = Variant["orientation"];
        string first = RotateDirection(orientation[..1], angle);
        string second = orientation.Length > 1
            ? RotateDirection(orientation[1..], angle)
            : string.Empty;

        AssetLocation rotatedCode = CodeWithVariant("orientation", first + second);
        if (second.Length > 0 && api.World.GetBlock(rotatedCode) == null)
        {
            rotatedCode = CodeWithVariant("orientation", second + first);
        }

        return rotatedCode;
    }

    public bool ConvertToPegGear(IWorldAccessor world, BlockPos pos)
    {
        string orientation = Variant["orientation"];
        if (orientation.Length != 2 || orientation[0] != orientation[1])
        {
            return false;
        }

        Block replacement = world.GetBlock(CodeWithVariant(
            "orientation",
            orientation[0].ToString()));

        if (replacement == null)
        {
            return false;
        }

        ExchangeBlockAt(world, pos, replacement);
        world.BlockAccessor
            .GetBlockEntity(pos)?
            .GetBehavior<BEBehaviorMPAngledGears>()?
            .ClearLargeGear();

        return true;
    }

    private static bool IsUnsupportedAxle(
        IWorldAccessor world,
        BlockEntity blockEntity,
        Block block,
        BlockPos pos)
    {
        BEBehaviorMPAxle axle = blockEntity?.GetBehavior<BEBehaviorMPAxle>();
        return axle != null &&
               block != null &&
               axle is not BEBehaviorAxleInBlocks &&
               !BEBehaviorMPAxle.IsAttachedToBlock(world.BlockAccessor, block, pos);
    }

    private static string RotateDirection(string direction, int angle)
    {
        if (direction.Length == 0 || direction is "u" or "d")
        {
            return direction;
        }

        BlockFacing before = BlockFacing.FromFirstLetter(direction);
        int index = GameMath.Mod(before.HorizontalAngleIndex - angle / 90, 4);
        return BlockFacing.HORIZONTALS_ANGLEORDER[index].Code[..1];
    }

    private static void ExchangeBlockAt(IWorldAccessor world, BlockPos pos, Block replacement)
    {
        world.BlockAccessor.ExchangeBlock(replacement.BlockId, pos);

        BlockEntity blockEntity = world.BlockAccessor.GetBlockEntity(pos);
        BEBehaviorMPBase behavior = blockEntity?.GetBehavior<BEBehaviorMPBase>();
        if (behavior == null)
        {
            return;
        }

        behavior.SetOrientations();
        behavior.Shape = replacement.Shape;
        blockEntity.MarkDirty();
    }
}
