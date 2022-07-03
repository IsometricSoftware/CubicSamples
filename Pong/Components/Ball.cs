using System.Numerics;
using Cubic;
using Cubic.Entities;
using Cubic.Entities.Components;
using Cubic.Audio;

using Cubic.Scenes;

namespace Pong.Components;

public class Ball : Component
{
    private Vector3 velocity;
    private float speed;
    private float radius;
    private Random random;
    private Sound scoreSound;
    private Entity[] paddles;

    public Ball(float radius, float speed)
    {
        this.radius = radius;
        this.speed = speed;
        random = new Random();
    }

    protected override void Initialize()
    {
        base.Initialize();
        paddles = GetEntitiesWithComponent<Paddle>();
        Serve(Side.Right);
        //scoreSound = new Sound(Game.AudioDevice, "Content/combobreak.wav");
    }

    protected override void Update()
    {
        base.Update();
        Transform.Position += velocity * Time.DeltaTime;

        
        // Whenever the ball hits the ceiling or floor, make it bounce. 
        // Thtanks to a weird bug where the ball would get stuck inside of the ceiling/floor and continually bounce, 
        // he if statements are split up so we can just set it to a position where it can't get stuck
        if (Transform.Position.Y <= radius)
        {
            Transform.Position.Y = radius;
            velocity.Y = -velocity.Y;
        }

        if (Transform.Position.Y >= Graphics.Viewport.Height - radius)
        {
            Transform.Position.Y = Graphics.Viewport.Height - radius;
            velocity.Y = -velocity.Y;

        }

        // If the ball hits the left or right ends of the screen, score for the appropriate player.
        if (Transform.Position.X <= radius)
        {
            Serve(Side.Left);
            paddles[1].GetComponent<Paddle>().Score++;
            //scoreSound.Play();

        }

        else if (Transform.Position.X >= Graphics.Viewport.Width - radius)
        {
            Serve(Side.Right);
            paddles[0].GetComponent<Paddle>().Score++;
            //scoreSound.Play();
        }
        
        // Check if the ball has collided with either paddle.
        foreach (var entity in paddles)
        {
            // As the paddle components are reference type, could also store them as well to avoid constant
            // getcomponent calls?
            var paddle = entity.GetComponent<Paddle>();
            if (CheckCollision(
                    entity.Transform.Position.X - paddle.PaddleSize.X / 2 - radius,
                    entity.Transform.Position.X + paddle.PaddleSize.X / 2 + radius, 
                    entity.Transform.Position.Y - paddle.PaddleSize.Y / 2 - radius,
                    entity.Transform.Position.Y + paddle.PaddleSize.Y / 2 + radius))
            {
                // If the ball has collided with either paddle, make it bounce. Made the velocity upon bouncing
                // random for extra flair (can just reverse it if not wanted).
                if (paddle.PaddleSide == Side.Left && velocity.X < 0)
                {
                    velocity.X = random.Next((int)(speed / 1.5), (int)speed * 2);

                }

                if (paddle.PaddleSide == Side.Right && velocity.X > 0)
                {                    
                    velocity.X = -random.Next((int)(speed / 1.5), (int)speed * 2);
                }
            }
        }

    }

    // Resets the position of the ball and adds velocity towards the last non scored player.
    private void Serve(Side side)
    {
        Transform.Position = new Vector3(300f, 200f, 1f);
        if (side == Side.Left)
            velocity = new Vector3(-speed, speed, 0f);
        else if (side == Side.Right)
            velocity = new Vector3(speed, speed, 0f);
    }

    
    private bool CheckCollision(float left, float right, float bottom, float top)
    {
        // Shorthands so that our conditional isn't too wordy
        var x = Transform.Position.X;
        var y = Transform.Position.Y;
        return x >= left && x <= right && y >= bottom && y <= top;
    }
    
}