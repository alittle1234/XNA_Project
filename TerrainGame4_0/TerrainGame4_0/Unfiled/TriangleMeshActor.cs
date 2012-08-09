using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using JigLibX.Math;
using JigLibX.Physics;
using JigLibX.Geometry;
using JigLibX.Collision;
using JigLibX.Utils;

/// <summary>
/// Physics engine actor made for terrain to become immovable body.
/// </summary>
public class TriangleMeshActor : DrawableGameComponent
{
    private Vector3 position;
    private Vector3 scale;

    private Body _body;
    public Body Body
    {
        get
        {
            return _body;
        }
    }

    private CollisionSkin _skin;
    public CollisionSkin Skin
    {
        get
        {
            return _skin;
        }
    }

    public TriangleMeshActor(Game game, Vector3 position, float scale,
        Texture2D heightMap,
        float[,] heightData)
        : base(game)
    {
        this.position = position;
        this.scale = new Vector3(1,1,1);

        _body = new Body();

        _body.MoveTo(position, Matrix.Identity);

        Array2D field = new Array2D(heightData.GetUpperBound(0), heightData.GetUpperBound(1));

        int upperZ = heightData.GetUpperBound(1);
        for (int x = 0; x < heightData.GetUpperBound(0); x++)
        {
            for (int z = 0; z < upperZ; z++)
            {
                field.SetAt(x, z, heightData[x, upperZ - 1 - z]);
            }
        }


        _skin = new CollisionSkin(null);

        float X = heightMap.Width / 2 * scale;
        float Z = heightMap.Height / 2 * scale;
        _skin.AddPrimitive(new Heightmap(field, X, -Z, scale, scale), new MaterialProperties(0.7f, 0.7f, 0.6f));

        _skin.ExternalData = this;
               

        PhysicsSystem.CurrentPhysicsSystem.CollisionSystem.AddCollisionSkin(_skin);

    }

    private Vector3 SetMass(float mass)
    {
        PrimitiveProperties primitiveProperties = new PrimitiveProperties(
            PrimitiveProperties.MassDistributionEnum.Solid,
            PrimitiveProperties.MassTypeEnum.Mass, mass);

        float junk;
        Vector3 com;
        Matrix it;
        Matrix itCoM;

        Skin.GetMassProperties(primitiveProperties, out junk, out com, out it, out itCoM);

        Body.BodyInertia = itCoM;
        Body.Mass = junk;

        return com;
    }

    public Matrix GetWorldMatrix()
    {
        return
            Matrix.CreateScale(scale) *
            _skin.GetPrimitiveLocal(0).Transform.Orientation *
            _body.Orientation *
            Matrix.CreateTranslation(_body.Position);
    }


}
