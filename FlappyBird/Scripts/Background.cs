using System.Drawing;
using Cubic;
using Cubic.Entities.Components;
using FlappyBird.Scenes;

namespace FlappyBird.Scripts;

public class Background : Component
{
    private Player _player;
    private Size _textureSize;

    public Background(Player player)
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

        // Move the background with the movement speed, however we multiply by 0.25 to get the parallax effect.
        Transform.Position.X -= MainScene.MovementSpeed * Time.DeltaTime * 0.25f;

        // Move the background back to its official starting position.
        // We also account for the amount it has travelled off screen to get true seamless infinite backgrounds.
        if (Transform.Position.X <= -_textureSize.Width * Transform.Scale.X)
            Transform.Position.X = _textureSize.Width * (MainScene.NumBackgrounds - 1) * Transform.Scale.X -
                                   (-_textureSize.Width * Transform.Scale.X - Transform.Position.X);
    }
}