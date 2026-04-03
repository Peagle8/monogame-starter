using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyGame.Core;
using MyGame.Core.Assets;
using MyGame.Core.Input;
using MyGame.Core.Rendering;
using MyGame.Core.Scenes;
using MyGame.Gameplay.Props;
using MyGame.Gameplay.Player;
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
        PlayerActor player,
        IRenderer<GameplayScene> renderer,
        IRenderContext renderContext,
        Action onReturnToMainMenu)
    {
        _inputService = inputService;
        Player = player;
        _renderer = renderer;
        _renderContext = renderContext;
        GameplayPauseMenu? pauseMenu = null;
        pauseMenu = new GameplayPauseMenu(
            onResume: () => pauseMenu!.Close(),
            onReturnToMainMenu: onReturnToMainMenu);
        _pauseMenu = pauseMenu;
    }

    public string Name => "Gameplay";

    public PlayerActor Player { get; }

    public GameplayPauseMenu PauseMenu => _pauseMenu;

    public IReadOnlyList<TreeProp> TreeProps { get; } =
    [
        new(new Vector2(120f, 120f), new Point(72, 104)),
        new(new Vector2(560f, 160f), new Point(64, 96)),
        new(new Vector2(620f, 320f), new Point(80, 112))
    ];

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

        Player.Update(frameTime);
    }

    public void Draw(FrameTime frameTime, SpriteBatch spriteBatch, IAssetCatalog assetCatalog)
    {
        var viewport = spriteBatch.GraphicsDevice.Viewport;
        var camera = GameplayCamera.Create(
            Player.Position,
            new Point(viewport.Width, viewport.Height),
            new Point(Player.Bounds.Width, Player.Bounds.Height));
        _renderContext.Bind(spriteBatch, assetCatalog, camera);
        spriteBatch.Begin();
        _renderer.Draw(this, frameTime);
        spriteBatch.End();
    }

    public IReadOnlyDictionary<string, string> GetDebugState()
    {
        return new Dictionary<string, string>
        {
            ["PlayerPosition"] = $"{Player.Position.X:0.00}, {Player.Position.Y:0.00}",
            ["PlayerFacing"] = Player.Facing.ToString(),
            ["TreePropCount"] = TreeProps.Count.ToString(),
            ["PauseMenuOpen"] = _pauseMenu.IsOpen.ToString(),
            ["PauseMenuSelection"] = _pauseMenu.SelectedText
        };
    }
}
