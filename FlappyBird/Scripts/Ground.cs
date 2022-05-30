using System;
using System.Drawing;
using Cubic;
using Cubic.Entities.Components;
using FlappyBird.Scenes;

namespace FlappyBird.Scripts;

public class Ground : Component
{
    private Size _textureSize;
    private Player _player;

    public Ground(Player player)
    {
        _player = player;
    }

    protected override void Initialize()
    {
        base.Initialize();

        _textureSize = GetComponent<Sprite>().SpriteTexture.Size;
    }

    protected override void Update()
    {
        base.Update();

        if (_player.GameOver)
            return;

        // This code is almost the same as what you see in Background.cs
        
        Transform.Position.X -= MainScene.MovementSpeed * Time.DeltaTime;

        if (Transform.Position.X <= -_textureSize.Width)
            Transform.Position.X = _textureSize.Width * (MainScene.NumGrounds - 1) -
                                   (-_textureSize.Width - Transform.Position.X);
    }
}