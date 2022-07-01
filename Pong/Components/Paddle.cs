using System.Text.RegularExpressions;
using Cubic;
using Cubic.Entities.Components;
using System.Numerics;
using System.Drawing;

namespace Pong.Components;

public enum Side
{
    Left,
    Right
}

public class Paddle : Component
{
    public Side PaddleSide;
    private float speed;
    private float velocity;
    public Vector2 PaddleSize;
    public int Score { get; set; }
    
    // The set keybinds to use to control either side.
    private Keys[] leftPlayerInputs = { Keys.W, Keys.S };
    private Keys[] rightPlayerInputs = { Keys.Up, Keys.Down };

    public Paddle(Side side, float speed)
    {
        PaddleSide = side;
        this.speed = speed;
        Score = 0;
    }

    protected override void Initialize()
    {
        base.Initialize();
        // We store paddleSize here so that we can use it when checking collisions in the ball component
        // (otherwise, we'd have to call getcomponent<sprite>() per frame)
        var paddleSize = Entity.GetComponent<Sprite>().SpriteTexture.Size;
        PaddleSize = new Vector2(paddleSize.Width * Transform.Scale.X, 
            paddleSize.Height * Transform.Scale.Y);
    }

    protected override void Update()
    {
        base.Update();
        
        // Basic switch statements that moves each paddle to its accordingly bound keys, clamped to screen bounds.
        velocity = 0f;

        switch (PaddleSide)
        {
            case Side.Left:
                if (Input.KeyDown(leftPlayerInputs[0]))
                    velocity = -speed;
                if (Input.KeyDown(leftPlayerInputs[1]))
                    velocity = speed;
                break;
            
            case Side.Right:
                if (Input.KeyDown(rightPlayerInputs[0]))
                    velocity = -speed;
                if (Input.KeyDown(rightPlayerInputs[1]))
                    velocity = speed;
                break;
            
            default:
                break;
        }

        var yOffset = PaddleSize.Y / 2;
        Transform.Position.Y += velocity * Time.DeltaTime;
        Transform.Position.Y = Math.Clamp(Transform.Position.Y, yOffset, Graphics.Viewport.Height- yOffset);

    }

}