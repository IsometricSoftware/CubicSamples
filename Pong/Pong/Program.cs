using Cubic;
using System.Drawing;
using Cubic.Scenes;
using Pong.Scenes;
using Cubic.Windowing;

// See https://aka.ms/new-console-template for more information

GameSettings settings = new GameSettings()
{
    Title = "Pong demo",
    Size = new Size(600, 400),
    AudioChannels = 4
};

using CubicGame game = new CubicGame(settings);
SceneManager.RegisterScene<MainScene>("main");
game.Run();