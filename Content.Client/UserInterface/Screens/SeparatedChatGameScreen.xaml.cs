using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.Chat.Widgets;
using Content.Client.UserInterface.Systems.EscapeMenu;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.Controllers.Implementations;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.UserInterface.XAML;

namespace Content.Client.UserInterface.Screens;

[GenerateTypedNameReferences]
public sealed partial class SeparatedChatGameScreen : InGameScreen
{
    private readonly EntitySpawningUIController _entitySpawningController;
    private readonly TileSpawningUIController _tileSpawningController;

    public SeparatedChatGameScreen()
    {
        RobustXamlLoader.Load(this);

        AutoscaleMaxResolution = new Vector2i(1080, 770);

        _entitySpawningController = UserInterfaceManager.GetUIController<EntitySpawningUIController>();
        _tileSpawningController = UserInterfaceManager.GetUIController<TileSpawningUIController>();
    
        SetAnchorPreset(ScreenContainer, LayoutPreset.Wide);
        SetAnchorPreset(ViewportContainer, LayoutPreset.Wide);
        SetAnchorPreset(MainViewport, LayoutPreset.Wide);
        //SetAnchorAndMarginPreset(VoteMenu, LayoutPreset.TopLeft, margin: 10);
        SetAnchorAndMarginPreset(Actions, LayoutPreset.BottomLeft, margin: 10);
        SetAnchorAndMarginPreset(Ghost, LayoutPreset.BottomWide, margin: 80);
        //SetAnchorAndMarginPreset(Hotbar, LayoutPreset.BottomWide, margin: 5);
        //SetAnchorAndMarginPreset(Alerts, LayoutPreset.CenterRight, margin: 10);
        ScreenContainer.OnSplitResizeFinish += (first, second) =>
            OnChatResized?.Invoke(new Vector2(ScreenContainer.SplitFraction, 0));
    }

    public override ChatBox ChatBox => new ChatBox();

    public override void SetChatSize(Vector2 size)
    {
        //ScreenContainer.DesiredSplitCenter = size.X;
        _entitySpawningController.ToggleWindow();
        _tileSpawningController.ToggleWindow();
        ScreenContainer.DesiredSplitCenter = 192000;
        ScreenContainer.ResizeMode = SplitContainer.SplitResizeMode.NotResizable;
    }
}
