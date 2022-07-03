using System.Drawing;
using Cubic;
using Cubic.Scenes;
using Cubic.Entities;
using Cubic.Render;
using System.Numerics;
using Cubic.Audio;
using Cubic.Entities.Components;
using Cubic.GUI;
using Cubic.Render.Text;
using Pong.Components;

namespace Pong.Scenes;

public class MainScene : Scene
{
    public const float PaddleSize = 7f;
    public const float BallSize = 5f;
    public const float BallSpeed = 300f;

    private Entity ball;
    private Entity leftPlayer;
    private Entity rightPlayer;

    // We store references to the components themselves, so we don't have to call getcomponent per frame to grab the
    // scores when displaying them in UI
    private Paddle leftPaddle;
    private Paddle rightPaddle;
    
    protected override void Initialize()
    {
        base.Initialize();

        // We create our paddle and ball entities, add appropriate sprite and associated components,
        // then add them to the scene.
        ball = new Entity();
        leftPlayer = new Entity();
        rightPlayer = new Entity();

        Texture2D ballTexture = new Texture2D("Content/ball.png");
        Texture2D paddleTexture = new Texture2D("Content/paddle.png");

        ball.AddComponent(new Sprite(ballTexture));
        ball.AddComponent(new Ball(12.5f, BallSpeed));
        
        //ball.AddComponent(new AudioSource(("score", "combobreak.wav")));
        
        leftPlayer.AddComponent(new Sprite(paddleTexture));
        leftPlayer.AddComponent(new Paddle(Side.Left, 500f));
        
        rightPlayer.AddComponent(new Sprite(paddleTexture));
        rightPlayer.AddComponent(new Paddle(Side.Right, 500f));

        // Some size refactoring to better fit the screen + gameplay
        ball.Transform.Scale = new Vector3(BallSize, BallSize, 1f);
        ball.Transform.Origin = new Vector2(ballTexture.Size.Width / 2, ballTexture.Size.Height / 2);
        
        leftPlayer.Transform.Scale = new Vector3(PaddleSize, PaddleSize, 1f);
        leftPlayer.Transform.Origin = new Vector2(paddleTexture.Size.Width / 2, paddleTexture.Size.Height / 2);
        rightPlayer.Transform.Scale = new Vector3(PaddleSize, PaddleSize, 1f);
        rightPlayer.Transform.Origin = new Vector2(paddleTexture.Size.Width / 2, paddleTexture.Size.Height / 2);

        ball.Transform.Position = new Vector3(300f, 200f, 1f);
        leftPlayer.Transform.Position = new Vector3(paddleTexture.Size.Width * PaddleSize / 2, Graphics.Viewport.Height / 2, 1f);
        rightPlayer.Transform.Position = new Vector3(
            Graphics.Viewport.Width - paddleTexture.Size.Width * PaddleSize / 2, 
            Graphics.Viewport.Height / 2, 
            1f
        );
        
        AddEntity(leftPlayer);
        AddEntity(rightPlayer);
        AddEntity(ball);
        leftPaddle = leftPlayer.GetComponent<Paddle>(); 
        rightPaddle = rightPlayer.GetComponent<Paddle>();

        UI.Theme.Font = new Font("Content/square.ttf", autoDispose:false);
        UI.Theme.TextColor = Color.White;
        
    }

    protected override void Update()
    {
        base.Update();
        
        UI.Label(Anchor.TopLeft, new Point(100, 20), leftPaddle.Score.ToString(), 72);
        UI.Label(Anchor.TopRight, new Point(-100, 20), rightPaddle.Score.ToString(), 72);
    }
}