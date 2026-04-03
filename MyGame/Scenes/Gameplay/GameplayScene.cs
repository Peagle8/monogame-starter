using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyGame.Core;
using MyGame.Core.Assets;
using MyGame.Core.Input;
using MyGame.Core.Rendering;
using MyGame.Core.Scenes;
using MyGame.Gameplay.World;
using MyGame.Rendering.Gameplay;

namespace MyGame.Scenes.Gameplay;

public sealed class GameplayScene : IScene
{
    private readonly IInputService _inputService;
    private readonly IRenderer<GameplayScene> _renderer;
    private readonly IRenderContext _renderContext;
    private readonly GameplayPauseMenu _pauseMenu;

    public GameplayScene(
        IInputService inputService,
        World world,
        IRenderer<GameplayScene> renderer,
        IRenderContext renderContext,
        Action onReturnToMainMenu)
    {
        _inputService = inputService;
        World = world;
        _renderer = renderer;
        _renderContext = renderContext;
        GameplayPauseMenu? pauseMenu = null;
        pauseMenu = new GameplayPauseMenu(
            onResume: () => pauseMenu!.Close(),
            onReturnToMainMenu: onReturnToMainMenu);
        _pauseMenu = pauseMenu;
    }

    public string Name => "Gameplay";

    public World World { get; }

    public GameplayPauseMenu PauseMenu => _pauseMenu;

    public void Enter()
    {
    }

    public void Exit()
    {
    }

    public void Update(FrameTime frameTime)
    {
        if (_inputService.IsJustPressed(GameAction.Pause))
        {
            _pauseMenu.Toggle();
        }

        if (_pauseMenu.IsOpen)
        {
            _pauseMenu.Update(_inputService);
            return;
        }

        World.Update(frameTime);
    }

    public void Draw(FrameTime frameTime, SpriteBatch spriteBatch, IAssetCatalog assetCatalog)
    {
        var viewport = spriteBatch.GraphicsDevice.Viewport;
        var camera = GameplayCamera.Create(
            World.Player.Position,
            new Point(viewport.Width, viewport.Height),
            new Point(World.Player.Bounds.Width, World.Player.Bounds.Height));
        _renderContext.Bind(spriteBatch, assetCatalog, camera);
        spriteBatch.Begin();
        _renderer.Draw(this, frameTime);
        spriteBatch.End();
    }

    public IReadOnlyDictionary<string, string> GetDebugState()
    {
        var debugState = new Dictionary<string, string>(World.GetDebugState())
        {
            ["PauseMenuOpen"] = _pauseMenu.IsOpen.ToString(),
            ["PauseMenuSelection"] = _pauseMenu.SelectedText
        };

        return debugState;
    }
}
