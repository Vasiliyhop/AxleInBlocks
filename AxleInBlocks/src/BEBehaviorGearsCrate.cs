using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Util;
using Vintagestory.GameContent.Mechanics;

#nullable disable

namespace axleinblocks;

public sealed class BEBehaviorMPGearsCrate : BEBehaviorMPAngledGears
{
    private ICoreClientAPI clientApi;

    public BEBehaviorMPGearsCrate(BlockEntity blockEntity) : base(blockEntity)
    {
    }

    public override void Initialize(ICoreAPI api, JsonObject properties)
    {
        base.Initialize(api, properties);
        clientApi = api as ICoreClientAPI;
    }

    public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tesselator)
    {
        MeshData crateMesh = GetCrateMesh(tesselator);
        if (crateMesh != null)
        {
            mesher.AddMeshData(crateMesh);
        }

        return base.OnTesselation(mesher, tesselator);
    }

    private MeshData GetCrateMesh(ITesselatorAPI tesselator)
    {
        if (clientApi == null)
        {
            return null;
        }

        string shapeVariant = Block.Variant["shape"];
        string materialVariant = Block.Variant["material"];
        AssetLocation shapeLocation = new(
            "axleinblocks",
            $"shapes/gear-crate-{shapeVariant}.json");

        Vintagestory.API.Common.Shape shape =
            Vintagestory.API.Common.Shape.TryGet(clientApi, shapeLocation);
        if (shape == null)
        {
            Api.Logger.Error("Missing gear crate shape {0}.", shapeLocation);
            return null;
        }

        string cacheKey = $"axleinblocks:gear-crate-{Block.FirstCodePart()}-{shapeVariant}-{materialVariant}";
        return ObjectCacheUtil.GetOrCreate(Api, cacheKey, () =>
        {
            ITexPositionSource textureSource = tesselator.GetTextureSource(Block);
            tesselator.TesselateShape(
                "Axle in blocks gear crate",
                shape,
                out MeshData mesh,
                textureSource);
            return mesh;
        });
    }
}
