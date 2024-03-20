using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Math;

namespace Galaga;
public class PlayerShot : Entity {
    private static Vec2F extent = new(0.008f, 0.021f);
    private static Vec2F direction = new(0.0f, 0.03f);
    public Vec2F Direction {
        get {return direction;}
    }

    public PlayerShot(Vec2F position, IBaseImage image) : base(new DynamicShape(position, extent), image) {
        Shape.AsDynamicShape().Direction = direction;
    }
}