using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyGame.Configuration;
using MyGame.Core;
using MyGame.Core.Assets;
using MyGame.Core.Diagnostics;
using MyGame.Core.Input;
using MyGame.Core.Rendering;
using MyGame.Core.Scenes;
using MyGame.Gameplay.Narrative;
using MyGame.Gameplay.Props;
using MyGame.Gameplay.Shops;
using MyGame.Gameplay.World;
using MyGame.Infrastructure.Save;
using MyGame.Rendering.Gameplay;

namespace MyGame.Scenes.Gameplay;

public sealed class GameplayScene : IScene
{
    private readonly string _name;
    private readonly Action _onRestart;
    private readonly Action _onReturnToMainMenu;
    private readonly Action<WorldSceneTransition> _onSceneTransition;
    private readonly IInputService _inputService;
    private readonly IRenderer<GameplayScene> _renderer;
    private readonly IRenderContext _renderContext;
    private readonly ISaveGameService _saveGameService;
    private readonly GameRecorder _gameRecorder;
    private readonly GameplayPauseMenu _pauseMenu;
    private readonly NpcDialogueService _npcDialogueService;
    private readonly HintService? _hintService;
    private readonly JournalService? _journalService;
    private readonly NpcDialogueController _npcDialogueController = new();
    private readonly ShopDialogueController _shopDialogueController = new();
    private NpcDialogueState _npcDialogueState = NpcDialogueState.Default;
    private ShopDialogueState _shopDialogueState = ShopDialogueState.Default;
    private bool _openShopAfterGreeting;

    public GameplayScene(
        IInputService inputService,
        World world,
        IRenderer<GameplayScene> renderer,
        IRenderContext renderContext,
        ISaveGameService saveGameService,
        GameRecorder gameRecorder,
        DiagnosticsSettings diagnosticsSettings,
        Action onRestart,
        Action onReturnToMainMenu)
        : this(
            GameplaySceneNames.Overworld,
            inputService,
            world,
            renderer,
            renderContext,
            saveGameService,
            gameRecorder,
            diagnosticsSettings,
            onRestart,
            onReturnToMainMenu,
            _ => { })
    {
    }

    public GameplayScene(
        string name,
        IInputService inputService,
        World world,
        IRenderer<GameplayScene> renderer,
        IRenderContext renderContext,
        ISaveGameService saveGameService,
        GameRecorder gameRecorder,
        DiagnosticsSettings diagnosticsSettings,
        Action onRestart,
        Action onReturnToMainMenu,
        Action<WorldSceneTransition> onSceneTransition,
        NpcDialogueService? npcDialogueService = null,
        HintService? hintService = null,
        JournalService? journalService = null)
    {
        _name = name;
        _onRestart = onRestart;
        _onReturnToMainMenu = onReturnToMainMenu;
        _onSceneTransition = onSceneTransition;
        _inputService = inputService;
        World = world;
        _renderer = renderer;
        _renderContext = renderContext;
        _saveGameService = saveGameService;
        _gameRecorder = gameRecorder;
        _npcDialogueService = npcDialogueService ?? CreateFallbackDialogueService();
        _hintService = hintService;
        _journalService = journalService;
        _pauseMenu = GameplayPauseMenu.CreateGameplayMenu(
            World.Player,
            _saveGameService,
            _gameRecorder,
            diagnosticsSettings,
            Name,
            () => World.CreateSaveData(Name),
            World.ApplySaveData,
            () => OverworldMapProjector.Create(Name, World.Player.Position),
            _onRestart,
            ReturnToMainMenu);
    }

    public string Name => _name;

    public World World { get; }

    public GameplayPauseMenu PauseMenu => _pauseMenu;

    public bool IsPlayerDead => World.Player.IsDead;

    public bool IsRecording => _gameRecorder.IsRecording;

    public bool IsReplaying => _gameRecorder.IsReplaying;

    public bool IsReplayPaused => _gameRecorder.IsReplayPaused;

    public ShopDialogueState ShopDialogue => _shopDialogueState;

    public NpcDialogueState NpcDialogue => _npcDialogueState;

    public void Enter()
    {
        if (Name == GameplaySceneNames.Arena && World.IsObjectiveComplete)
        {
            World.ResetEventProgress();
        }
    }

    public void Exit()
    {
    }

    public void Update(FrameTime frameTime)
    {
        if (IsPlayerDead)
        {
            HandleDeathInput();
            return;
        }

        if (_inputService.IsJustPressed(GameAction.Pause))
        {
            _gameRecorder.PauseReplay();
            _pauseMenu.Toggle();
            return;
        }

        if (_inputService.IsJustPressed(GameAction.Map) && _pauseMenu.CanShowMap)
        {
            _gameRecorder.PauseReplay();
            _pauseMenu.ToggleMap();
            return;
        }

        if (_pauseMenu.IsOpen)
        {
            _pauseMenu.Update(_inputService);
            return;
        }

        if (_shopDialogueState.IsOpen)
        {
            UpdateShopDialogue();
            return;
        }

        if (_npcDialogueState.IsOpen)
        {
            var shouldOpenShop = _openShopAfterGreeting
                && (_inputService.IsJustPressed(GameAction.Confirm) || _inputService.IsJustPressed(GameAction.Interact));
            UpdateNpcDialogue();
            if (!_npcDialogueState.IsOpen && shouldOpenShop)
            {
                OpenShopDialogue();
            }
            else if (!_npcDialogueState.IsOpen)
            {
                _openShopAfterGreeting = false;
            }

            return;
        }

        World.Update(frameTime);
        UpdateNpcDialogue();
        if (!_npcDialogueState.IsOpen)
        {
            UpdateShopDialogue();
        }

        var pendingTransition = World.ConsumePendingSceneTransition();
        if (pendingTransition is not null)
        {
            _onSceneTransition(pendingTransition);
        }
    }

    public void Draw(FrameTime frameTime, SpriteBatch spriteBatch, IAssetCatalog assetCatalog)
    {
        var viewport = spriteBatch.GraphicsDevice.Viewport;
        var camera = GameplayCamera.Create(
            World.Player.Position,
            new Point(viewport.Width, viewport.Height),
            new Point(World.Player.Bounds.Width, World.Player.Bounds.Height),
            World.WorldBounds);
        _renderContext.Bind(spriteBatch, assetCatalog, camera);
        spriteBatch.Begin();
        _renderer.Draw(this, frameTime);
        spriteBatch.End();
    }

    public IReadOnlyDictionary<string, string> GetDebugState()
    {
        var debugState = new Dictionary<string, string>(World.GetDebugState())
        {
            ["PlayerDead"] = IsPlayerDead.ToString(),
            ["PauseMenuOpen"] = _pauseMenu.IsOpen.ToString(),
            ["PauseMenuSelection"] = _pauseMenu.SelectedText,
            ["PauseMenuInventoryOpen"] = _pauseMenu.IsShowingInventoryMenu.ToString(),
            ["PauseMenuInventoryTab"] = _pauseMenu.InventoryTab.ToString(),
            ["ReplayMenuOpen"] = _pauseMenu.IsShowingReplayMenu.ToString(),
            ["MapMenuOpen"] = _pauseMenu.IsShowingMap.ToString(),
            ["PauseMenuFooterText"] = _pauseMenu.FooterText,
            ["RecorderRecording"] = _gameRecorder.IsRecording.ToString(),
            ["RecorderReplaying"] = _gameRecorder.IsReplaying.ToString(),
            ["RecorderReplayPaused"] = _gameRecorder.IsReplayPaused.ToString(),
            ["ShopDialogueOpen"] = _shopDialogueState.IsOpen.ToString(),
            ["ShopDialoguePromptVisible"] = _shopDialogueState.IsPromptVisible.ToString(),
            ["ShopDialogueTab"] = _shopDialogueState.ActiveTab.ToString(),
            ["NpcDialogueOpen"] = _npcDialogueState.IsOpen.ToString(),
            ["NpcDialoguePromptVisible"] = _npcDialogueState.IsPromptVisible.ToString(),
            ["NpcDialogueSpeaker"] = _npcDialogueState.SpeakerName,
            ["NpcDialogueDebugSpeakerId"] = _npcDialogueService.LastDebugInfo.SpeakerId,
            ["NpcDialogueDebugLineStyle"] = _npcDialogueService.LastDebugInfo.LineStyle,
            ["NpcDialogueMatchedIds"] = FormatDebugIds(_npcDialogueService.LastDebugInfo.MatchedEntryIds),
            ["NpcDialogueSuppressedIds"] = FormatDebugIds(_npcDialogueService.LastDebugInfo.SuppressedEntryIds),
            ["NpcDialogueSelectedId"] = _npcDialogueService.LastDebugInfo.SelectedEntryId,
            ["NpcDialogueFallbackReason"] = _npcDialogueService.LastDebugInfo.FallbackReason,
            ["JournalDiscoveredCount"] = World.JournalState.DiscoveredEntryIds.Count.ToString(),
            ["JournalReadCount"] = World.JournalState.ReadEntryIds.Count.ToString(),
            ["TownAlertLevel"] = World.NarrativeState.TownAlertLevel.ToString(),
            ["PlayerReputation"] = World.NarrativeState.PlayerReputation.ToString()
        };

        return debugState;
    }

    private void HandleDeathInput()
    {
        if (_inputService.IsJustPressed(GameAction.Confirm))
        {
            _gameRecorder.StopReplay();
            _onRestart();
            return;
        }

        if (_inputService.IsJustPressed(GameAction.Cancel) || _inputService.IsJustPressed(GameAction.Pause))
        {
            ReturnToMainMenu();
        }
    }

    private void ReturnToMainMenu()
    {
        _onReturnToMainMenu();
    }

    private void UpdateShopDialogue()
    {
        var counterBounds = World.GetProps<CounterProp>().FirstOrDefault()?.Bounds;
        _shopDialogueState = _shopDialogueController.Update(
            _shopDialogueState,
            World.Player.Bounds,
            counterBounds,
            _inputService.IsJustPressed(GameAction.Interact),
            _inputService.IsJustPressed(GameAction.Cancel),
            _inputService.IsJustPressed(GameAction.PreviousTab),
            _inputService.IsJustPressed(GameAction.NextTab));
    }

    private void UpdateNpcDialogue()
    {
        var wasOpen = _npcDialogueState.IsOpen;
        _npcDialogueState = _npcDialogueController.Update(
            _npcDialogueState,
            World.Player.Bounds,
            World.GetProps<IConversationProp>(),
            _inputService.IsJustPressed(GameAction.Interact),
            _inputService.IsJustPressed(GameAction.Confirm),
            _inputService.IsJustPressed(GameAction.Cancel),
            _npcDialogueService,
            _hintService,
            Name,
            World.NarrativeState,
            World.NarrativeHistory);

        if (!wasOpen && _npcDialogueState.IsOpen)
        {
            _openShopAfterGreeting = IsShopkeeperGreetingAtCounter();
            World.ShowHintToast(_npcDialogueState.HintText);
            _journalService?.DiscoverAvailableEntries(World.NarrativeState, World.JournalState);
        }
    }

    private void OpenShopDialogue()
    {
        _openShopAfterGreeting = false;
        _shopDialogueState = _shopDialogueState with
        {
            IsPromptVisible = true,
            IsOpen = true,
            ActiveTab = ShopDialogueTab.Buy
        };
    }

    private bool IsShopkeeperGreetingAtCounter()
    {
        if (_npcDialogueState.SpeakerId != NarrativeIds.SpeakerShopkeeper)
        {
            return false;
        }

        var counterBounds = World.GetProps<CounterProp>().FirstOrDefault()?.Bounds;
        return counterBounds is Rectangle bounds && GetShopInteractionBounds(bounds).Intersects(World.Player.Bounds);
    }

    private static Rectangle GetShopInteractionBounds(Rectangle counterBounds)
    {
        const int horizontalPromptRange = 24;
        const int verticalPromptRange = 36;
        return new Rectangle(
            counterBounds.X - horizontalPromptRange,
            counterBounds.Y - verticalPromptRange,
            counterBounds.Width + (horizontalPromptRange * 2),
            counterBounds.Height + (verticalPromptRange * 2));
    }

    private static string FormatDebugIds(IReadOnlyList<string> ids)
    {
        return ids.Count == 0 ? "<none>" : string.Join(", ", ids);
    }

    private static NpcDialogueService CreateFallbackDialogueService()
    {
        return new NpcDialogueService(
            new NpcDialogueDataFile(),
            new WeightedRandomSelector(new Random(1)),
            new RecentSelectionHistory());
    }
}
