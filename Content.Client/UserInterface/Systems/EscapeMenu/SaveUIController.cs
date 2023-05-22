using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Content.Client.Options.UI;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.Console;

namespace Content.Client.UserInterface.Systems.EscapeMenu
{
    public sealed class SaveUIController : UIController
    {
        [Dependency] private readonly IConsoleHost _con = default!;

        public override void Initialize()
        {
        }

       

        private SaveMenu _saveWindow = default!;

        private void EnsureWindow()
        {
            if (_saveWindow is { Disposed: false })
                return;

            _saveWindow = UIManager.CreateWindow<SaveMenu>();
        }

        public void OpenWindow()
        {
            EnsureWindow();

            _saveWindow.OpenCentered();
            _saveWindow.MoveToFront();
        }

        public void ToggleWindow()
        {
            EnsureWindow();

            if (_saveWindow.IsOpen)
            {
                _saveWindow.Close();
            }
            else
            {
                OpenWindow();
            }
        }
    }
}
