using System;
using System.Drawing;
using System.Numerics;
using Cubic;
using Cubic.Entities.Components;
using Cubic.Utilities;
using FlappyBird.Scenes;

namespace FlappyBird.Scripts;

public class Pipe : Component
{
    private bool _pointsScorer;
    private Rectangle _collisionRect;
    private Size _textureSize;
    private Player _player;
    
    public Pipe(bool pointsScorer, Player player)
    {
        _pointsScorer = pointsScorer;
        _player = player;
    }

    protected override void Initialize()
    {
        base.Initialize();

        _textureSize = GetComponent<Sprite>().SpriteTexture.Size;
        Transform.Origin = new Vector2(_textureSize.Width / 2f, 0);

        _collisionRect =
            new Rectangle(new Point((int) Transform.Position.X - _textureSize.Width / 2, (int) Transform.Position.Y),
                _textureSize);
    }

    protected override void Update()
    {
        base.Update();

        if (_player.GameOver)
            return;

        // Move the pipe at the movement speed to give the illusion of camera movement.
        // We do this instead of moving the camera to reduce floating point errors.
        // Over time, if the camera position moves too far right, it will start to jitter as we reach the limits of
        // floating point. For the game to be a true infinite scroller, we can't have this. Not that anyone would reach
        // that point without a helping hand of course.
        Transform.Position.X -= MainScene.MovementSpeed * Time.DeltaTime;

        _collisionRect.Location =
            new Point((int) Transform.Position.X - _textureSize.Width / 2, (int) Transform.Position.Y);

        // Score a point if the player goes past the pipe.
        // The pointsScorer means two things:
        // - Only one pipe in the set will score a point
        // - The point will only be scored once.
        if (_pointsScorer && GetEntity("Player").Transform.Position.X >= Transform.Position.X)
        {
            ((MainScene) CurrentScene).IncrementScore();
            _pointsScorer = false;
        }

        // Delete it once it goes off screen.
        if (Transform.Position.X <= -50)
            RemoveEntity(Entity.Name);

        // Check for collision
        if (_player.CollisionRect.IntersectsWith(_collisionRect))
            _player.GameOver = true;
    }
}