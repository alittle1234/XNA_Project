using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using JigLibX.Math;
using JigLibX.Physics;
using JigLibX.Geometry;
using JigLibX.Collision;

public class BoxActor : DrawableGameComponent
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

    public BoxActor(Game game, Vector3 position, Vector3 scale)
        : base(game)
    {
        this.position = position;
        this.scale = scale;

        _body = new Body();
        _skin = new CollisionSkin(_body);
        _body.CollisionSkin = _skin;

        
        Box box = new Box(Vector3.Zero, Matrix.Identity, scale);
        _skin.AddPrimitive(box, new MaterialProperties(0.8f, 0.8f, 0.7f));

        Vector3 com = SetMass(2.0f);

        _body.MoveTo(position, Matrix.Identity);
        _skin.ApplyLocalTransform(new Transform(-com, Matrix.Identity));
        _body.EnableBody();

        
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
            _body.CollisionSkin.GetPrimitiveLocal(0).Transform.Orientation *
            _body.Orientation *
            Matrix.CreateTranslation(_body.Position);
    }

    public Matrix GetWorldMatrixScale(float f_scale)
    {
        return
            Matrix.CreateScale(f_scale) *
            _body.CollisionSkin.GetPrimitiveLocal(0).Transform.Orientation *
            _body.Orientation *
            Matrix.CreateTranslation(_body.Position);
    }

    public Matrix GetOrientation()
    {
        return
            _skin.GetPrimitiveLocal(0).Transform.Orientation *
            _body.Orientation;
    }

    public Matrix GetTranslation()
    {
        return
            Matrix.CreateTranslation(_body.Position);
    }


}
