using System.Drawing;
using System.Reflection;
using Cubic.Scenes;
using Cubic.Windowing;
using FlappyBird.Scenes;

GameSettings settings = new GameSettings()
{
    Title = "Flappy bird demo",
    Size = new Size(405, 720),
    AudioChannels = 4
};

using CubicGame game = new CubicGame(settings);
SceneManager.RegisterScene<MainScene>("main");
game.Run();
