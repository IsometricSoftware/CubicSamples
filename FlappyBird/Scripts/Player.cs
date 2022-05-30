using System;
using System.Drawing;
using System.Numerics;
using Cubic;
using Cubic.Audio;
using Cubic.Entities.Components;
using Cubic.Render;
using Cubic.Utilities;
using FlappyBird.Scenes;

namespace FlappyBird.Scripts;

public class Player : Component
{
    private const int Gravity = 1000;
    private const int JumpVelocity = 500;

    private float _velocity;
    public Rectangle CollisionRect;
    private Size _textureSize;
    public bool GameOver;

    private Sound _death;
    private bool _playDeath;

    private Sound _jump;

    protected override void Initialize()
    {
        base.Initialize();

        _textureSize = GetComponent<Sprite>().SpriteTexture.Size;
        
        Transform.Position = new Vector3(50, Graphics.Viewport.Height / 2f, 0);
        Transform.Scale = new Vector3(0.135f, 0.135f, 1);
        Transform.Origin = _textureSize.ToVector2() / 2;
        
        _textureSize = new Size((int) (_textureSize.Width * Transform.Scale.X),
            (int) (_textureSize.Height * Transform.Scale.Y)) - new Size(5, 5);

        CollisionRect = new Rectangle((Transform.Position.ToVector2() - _textureSize.ToVector2() / 2).ToPoint(),
            _textureSize);

        _death = new Sound(Game.AudioDevice, "Content/death.wav");
        _playDeath = true;
        _jump = new Sound(Game.AudioDevice, "Content/jump.wav");
    }

    protected override void Update()
    {
        base.Update();

        // Only play the death sound once.
        if (GameOver && _playDeath)
        {
            _death.Play(pitch: 0.5f);
            _playDeath = false;
        }

        // Increase our downward velocity by gravity.
        _velocity += Gravity * Time.DeltaTime;

        // This creates the little bounce effect on the bird when the game is idle.
        if (!MainScene.Playing)
        {
            if (Transform.Position.Y >= 400)
                _velocity = -JumpVelocity;
        }

        if (MainScene.Playing)
        {
            // Make the player jump when the key is pressed.
            if (Input.KeyPressed(Keys.Space) || Input.MouseButtonPressed(MouseButtons.Left) && !GameOver)
            {
                _velocity = -JumpVelocity;
                _jump.Play();
            }

            // If the player goes below the ground, we say game over, and prevent the player from going through the ground.
            if (Transform.Position.Y >= MainScene.GroundHeight)
            {
                GameOver = true;
                _velocity = 0;
                Transform.Position.Y = MainScene.GroundHeight;
            }

            // Likewise, the player cannot go above the ceiling.
            if (Transform.Position.Y <= 0)
            {
                _velocity = 0;
                Transform.Position.Y = 1;
            }

        }

        // Rotate the sprite with the velocity.
        Transform.SpriteRotation = _velocity / 1000;

        Transform.Position += new Vector3(0, _velocity, 0) * Time.DeltaTime;
        
        CollisionRect.Location = (Transform.Position.ToVector2() - _textureSize.ToVector2() / 2).ToPoint();
    }
}