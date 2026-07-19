using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;
using Vintagestory.GameContent.Mechanics;

#nullable disable

namespace axleinblocks;

/// <summary>
/// Uses the current vanilla transmission mechanics and adds the cover mesh
/// shipped by Axle in Blocks.
/// </summary>
public sealed class BEBehaviorMPTransmissionCovered : BEBehaviorMPTransmission
{
    private ICoreClientAPI clientApi;

    public BEBehaviorMPTransmissionCovered(BlockEntity blockEntity) : base(blockEntity)
    {
    }

    public override void Initialize(ICoreAPI api, JsonObject properties)
    {
        base.Initialize(api, properties);
        clientApi = api as ICoreClientAPI;
    }

    public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tesselator)
    {
        MeshData wrapperMesh = GetWrapperMesh(tesselator);
        if (wrapperMesh != null)
        {
            mesher.AddMeshData(wrapperMesh);
        }

        return base.OnTesselation(mesher, tesselator);
    }

    private MeshData GetWrapperMesh(ITesselatorAPI tesselator)
    {
        if (clientApi == null)
        {
            return null;
        }

        string orientation = Block.Variant["orientation"];
        string cacheKey = $"axleinblocks:transmission-cover-{Block.Code}";

        return ObjectCacheUtil.GetOrCreate(Api, cacheKey, () =>
        {
            AssetLocation shapeLocation = new(
                "axleinblocks",
                "shapes/transmission-covered.json");
            Vintagestory.API.Common.Shape shape =
                Vintagestory.API.Common.Shape.TryGet(clientApi, shapeLocation);

            if (shape == null)
            {
                Api.Logger.Error("Missing transmission cover shape {0}.", shapeLocation);
                return null;
            }

            ITexPositionSource textureSource = tesselator.GetTextureSource(Block);
            tesselator.TesselateShape(
                "Axle in blocks transmission cover",
                shape,
                out MeshData mesh,
                textureSource);

            if (orientation == "ns")
            {
                mesh.Rotate(
                    new Vec3f(0.5f, 0.5f, 0.5f),
                    0,
                    GameMath.PIHALF,
                    0);
            }

            return mesh;
        });
    }
}
