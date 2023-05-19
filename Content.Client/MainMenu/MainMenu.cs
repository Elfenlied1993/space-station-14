using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Content.Client.MainMenu.UI;
using Content.Client.Maps;
using Content.Client.UserInterface.Systems.EscapeMenu;
using Content.Shared.CCVar;
using Robust.Client;
using Robust.Client.Configuration;
using Robust.Client.Console;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.ContentPack;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using UsernameHelpers = Robust.Shared.AuthLib.UsernameHelpers;

namespace Content.Client.MainMenu
{
    /// <summary>
    ///     Main menu screen that is the first screen to be displayed when the game starts.
    /// </summary>
    // Instantiated dynamically through the StateManager, Dependencies will be resolved.
    public sealed class MainScreen : Robust.Client.State.State
    {

        [Dependency] private readonly IBaseClient _client = default!;
        [Dependency] private readonly IClientNetManager _netManager = default!;
        [Dependency] private readonly IConfigurationManager _configurationManager = default!;
        [Dependency] private readonly IGameController _controllerProxy = default!;
        [Dependency] private readonly IResourceCache _resourceCache = default!;
        [Dependency] private readonly IUserInterfaceManager _userInterfaceManager = default!;
        [Dependency] private readonly IClientNetConfigurationManager _configManager = default!;
        [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
        [Dependency] private readonly IModLoaderInternal _modLoader = default!;
        private MainMenuControl _mainMenuControl = default!;
        private bool _isConnecting;
        private static readonly string UserName = "Elfen";
        private const string Address = "localhost";

        // ReSharper disable once InconsistentNaming
        private static readonly Regex IPv6Regex = new(@"\[(.*:.*:.*)](?::(\d+))?");

        /// <inheritdoc />
        protected override void Startup()
        {
            _mainMenuControl = new MainMenuControl(_resourceCache, _configurationManager);
            _userInterfaceManager.StateRoot.AddChild(_mainMenuControl);

            _mainMenuControl.QuitButton.OnPressed += QuitButtonPressed;
            _mainMenuControl.OptionsButton.OnPressed += OptionsButtonPressed;
            _mainMenuControl.DirectConnectButton.OnPressed += DirectConnectButtonPressed;
            _mainMenuControl.EditButton.OnPressed += EditButtonPressed;
            _mainMenuControl.DeleteButton.OnPressed += DeleteButtonPressed;
            //_mainMenuControl.AddressBox.OnTextEntered += AddressBoxEntered;
            //_mainMenuControl.ChangelogButton.OnPressed += ChangelogButtonPressed;
            Test();

          
            _client.RunLevelChanged += RunLevelChanged;
        }

        private async void Test()
        {
            var result = _prototypeManager.EnumeratePrototypes<GameMapPrototype>()
                .ToArray();
            foreach (var gameMapPrototype in result)
            {
                _mainMenuControl.MapList.AddItem($"{gameMapPrototype.ID} - {gameMapPrototype.MapName}|{gameMapPrototype.MapPath}");
            }
        }
        /// <inheritdoc />
        protected override void Shutdown()
        {
            _client.RunLevelChanged -= RunLevelChanged;
            _netManager.ConnectFailed -= _onConnectFailed;

            _mainMenuControl.Dispose();
        }

        private void EditButtonPressed(BaseButton.ButtonEventArgs args)
        {
            var selected = _mainMenuControl.MapList.GetSelected().ToList().ElementAt(0);
            if (selected != null)
            {
                var split = selected.Text.Split("|") ?? new[] { "new" };
                _configManager.SetCVar(CVars.ActiveWorkingMap, split[1]);
                //_configurationManager.SetCVar(CCVars.GameMap, split[0]);
                TryConnect(Address);

            }
        }

        private void DeleteButtonPressed(BaseButton.ButtonEventArgs args)
        {
            var selected = _mainMenuControl.MapList.GetSelected();
        }
 

        private void OptionsButtonPressed(BaseButton.ButtonEventArgs args)
        {
            _userInterfaceManager.GetUIController<OptionsUIController>().ToggleWindow();
        }

        private void QuitButtonPressed(BaseButton.ButtonEventArgs args)
        {
            _controllerProxy.Shutdown();
        }

        private void DirectConnectButtonPressed(BaseButton.ButtonEventArgs args)
        {
            TryConnect(Address);
        }

        private void AddressBoxEntered(LineEdit.LineEditEventArgs args)
        {
            if (_isConnecting)
            {
                return;
            }

            TryConnect(args.Text);
        }

        private void TryConnect(string address)
        {

            if (!UsernameHelpers.IsNameValid(UserName, out var reason))
            {
                var invalidReason = Loc.GetString(reason.ToText());
                _userInterfaceManager.Popup(
                    Loc.GetString("main-menu-invalid-username-with-reason", ("invalidReason", invalidReason)),
                    Loc.GetString("main-menu-invalid-username"));
                return;
            }

            var configName = _configurationManager.GetCVar(CVars.PlayerName);
            if (UserName != configName)
            {
                _configurationManager.SetCVar(CVars.PlayerName, UserName);
                _configurationManager.SaveToFile();
            }

            _setConnectingState(true);
            _netManager.ConnectFailed += _onConnectFailed;
            try
            {
                ParseAddress(address, out var ip, out var port);
                _client.ConnectToServer(ip, port);
            }
            catch (ArgumentException e)
            {
                _userInterfaceManager.Popup($"Unable to connect: {e.Message}", "Connection error.");
                Logger.Warning(e.ToString());
                _netManager.ConnectFailed -= _onConnectFailed;
                _setConnectingState(false);
            }
        }

        private void RunLevelChanged(object? obj, RunLevelChangedEventArgs args)
        {
            switch (args.NewLevel)
            {
                case ClientRunLevel.Connecting:
                    _setConnectingState(true);
                    break;
                case ClientRunLevel.Initialize:
                    _setConnectingState(false);
                    _netManager.ConnectFailed -= _onConnectFailed;
                    break;
            }
        }

        private void ParseAddress(string address, out string ip, out ushort port)
        {
            var match6 = IPv6Regex.Match(address);
            if (match6 != Match.Empty)
            {
                ip = match6.Groups[1].Value;
                if (!match6.Groups[2].Success)
                {
                    port = _client.DefaultPort;
                }
                else if (!ushort.TryParse(match6.Groups[2].Value, out port))
                {
                    throw new ArgumentException("Not a valid port.");
                }

                return;
            }

            // See if the IP includes a port.
            var split = address.Split(':');
            ip = address;
            port = _client.DefaultPort;
            if (split.Length > 2)
            {
                throw new ArgumentException("Not a valid Address.");
            }

            // IP:port format.
            if (split.Length == 2)
            {
                ip = split[0];
                if (!ushort.TryParse(split[1], out port))
                {
                    throw new ArgumentException("Not a valid port.");
                }
            }
        }

        private void _onConnectFailed(object? _, NetConnectFailArgs args)
        {
            _userInterfaceManager.Popup(Loc.GetString("main-menu-failed-to-connect", ("reason", args.Reason)));
            _netManager.ConnectFailed -= _onConnectFailed;
            _setConnectingState(false);
        }

        private void _setConnectingState(bool state)
        {
            _isConnecting = state;
            _mainMenuControl.DirectConnectButton.Disabled = state;
        }
    }
}
