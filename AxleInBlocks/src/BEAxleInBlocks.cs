using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace axleinblocks;

/// <summary>
/// Retains the block-entity identity used by existing Axle in Blocks worlds.
/// Returning false from tesselation keeps the static cover mesh in addition to
/// the animated axle mesh supplied by <see cref="BEBehaviorAxleInBlocks"/>.
/// </summary>
public sealed class BEAxleInBlocks : BlockEntity
{
    public bool Engaged { get; private set; }

    public override void Initialize(ICoreAPI api)
    {
        base.Initialize(api);
        Engaged = true;
    }

    public override bool OnTesselation(
        ITerrainMeshPool mesher,
        ITesselatorAPI tessThreadTesselator)
    {
        base.OnTesselation(mesher, tessThreadTesselator);
        return false;
    }

    public override void FromTreeAttributes(
        ITreeAttribute tree,
        IWorldAccessor worldAccessForResolve)
    {
        base.FromTreeAttributes(tree, worldAccessForResolve);
        Engaged = tree.GetBool("engaged");
    }

    public override void ToTreeAttributes(ITreeAttribute tree)
    {
        base.ToTreeAttributes(tree);
        tree.SetBool("engaged", Engaged);
    }
}
