using System;
using System.Drawing;
using System.Numerics;
using Cubic;
using Cubic.Audio;
using Cubic.Entities;
using Cubic.Entities.Components;
using Cubic.GUI;
using Cubic.Render;
using Cubic.Render.Text;
using Cubic.Scenes;
using Cubic.Utilities;
using FlappyBird.Scripts;

namespace FlappyBird.Scenes;

public class MainScene : Scene
{
    public const int MovementSpeed = 100;
    public const int NumGrounds = 3;
    public const int NumBackgrounds = 2;
    public const int GroundHeight = 620;
    private const int PipeGap = 580;
    private const float SpawnTime = 2.25f;

    private int _score;

    private Random _random;
    private Texture2D _pipeTexture;
    private float _timer;

    private Sound _scoreSound;

    public static bool Playing;
    private Sprite.DropShadow _dropShadow;
    
    protected override void Initialize()
    {
        base.Initialize();

        // The background image doesn't reach the full height of the screen, solution is to just set our clear colour
        // to the same colour as the background image.
        World.ClearColor = Utils.ColorFromHex(0x0099CC);
        
        _dropShadow = new Sprite.DropShadow(new Vector2(4, 3), 96);

        // Player entity - add a sprite and player component, then add it to the scene.
        Entity player = new Entity();
        player.AddComponent(new Sprite(new Texture2D("Content/fbird.png")) { Shadow = _dropShadow });
        player.AddComponent(new Player());
        AddEntity("Player", player);
        
        Texture2D groundTexture = new Texture2D("Content/ground.png");
        Player playerScript = GetEntity("Player").GetComponent<Player>();
        // To create the illusion of infinite ground, we create a few instances of the object and cycle them round once
        // they go off screen.
        for (int i = 0; i < NumGrounds; i++)
        {
            Entity ground = new Entity(new Transform()
            {
                Position = new Vector3(groundTexture.Size.Width * i, GroundHeight, 1)
            });
            ground.AddComponent(new Sprite(groundTexture));
            ground.AddComponent(new Ground(playerScript));
            AddEntity(ground);
        }

        // Similar idea for the backgrounds, however since the backgrounds are larger we don't need as many of them.
        Texture2D backgroundTexture = new Texture2D("Content/background.png");
        for (int i = 0; i < NumBackgrounds; i++)
        {
            // The background by itself feels too large, this scale feels much nicer.
            const float scale = 0.65f;
            Entity background = new Entity(new Transform()
            {
                Position = new Vector3(i * backgroundTexture.Size.Width * scale, 300, 5),
                Scale = new Vector3(scale, scale, 1)
            });
            background.AddComponent(new Sprite(backgroundTexture));
            background.AddComponent(new Background(playerScript));
            AddEntity(background);
        }
        
        _pipeTexture = new Texture2D("Content/pipe.png");
        _random = new Random();

        UI.Theme.TextColor = Color.White;
        _score = 0;

        _scoreSound = new Sound(Game.AudioDevice, "Content/score.wav");
        _timer = 0;

        Playing = false;
    }

    protected override void Update()
    {
        base.Update();
        
        // Display our score in the top right, with a padding of 10 pixels
        UI.Label(Anchor.TopRight, new Point(-10, 10), _score.ToString(), 24);

        // This creates the effect of the "tap to play" text sliding down when you tap, and the game starting.
        if (!Playing)
        {
            if (_timer > 0)
                _timer += Time.DeltaTime;

            UI.Label(Anchor.Center, new Point(0, (int) CubicMath.Lerp(-200, 360, _timer * _timer * 2)), "Tap to start",
                24);
            if (Input.KeyPressed(Keys.Space) || Input.MouseButtonPressed(MouseButtons.Left) ||
                Input.ControllerButtonPressed(ControllerButton.A))
                _timer += float.Epsilon;
            if (_timer >= 0.75)
            {
                _timer = 999;
                Playing = true;
            }
        }

        // Restart the scene if the game is over.
        if (GetEntity("Player").GetComponent<Player>().GameOver)
        {
            if (Input.KeyPressed(Keys.Space) || Input.MouseButtonPressed(MouseButtons.Left) ||
                Input.ControllerButtonPressed(ControllerButton.A))
                SceneManager.SetScene("main");
            
            return;
        }

        if (!Playing)
            return;
        
        _timer += Time.DeltaTime;

        // Spawn a new set of pipes every few seconds.
        if (_timer >= SpawnTime)
        {
            _timer = 0;
            Player player = GetEntity("Player").GetComponent<Player>();

            int randomY = _random.Next(230, 570);
            int x = Graphics.Viewport.Width + 50;
            const int depth = 2;
            Entity bottomPipe = new Entity(new Transform()
            {
                Position = new Vector3(x, randomY, depth)
            });
            bottomPipe.AddComponent(new Sprite(_pipeTexture) { Shadow = _dropShadow });
            bottomPipe.AddComponent(new Pipe(true, player));

            Entity topPipe = new Entity(new Transform()
            {
                Position = new Vector3(x, randomY - PipeGap, depth)
            });
            topPipe.AddComponent(new Sprite(_pipeTexture) { Flip = SpriteFlipMode.FlipY, Shadow = _dropShadow });
            topPipe.AddComponent(new Pipe(false, player));

            // We can add entities without names - this is very useful for stuff like this!
            AddEntity(bottomPipe);
            AddEntity(topPipe);
        }
    }

    public void IncrementScore()
    {
        _score++;
        _scoreSound.Play();
    }
}