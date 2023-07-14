using NLog.Fluent;
using Sandbox;
using Sandbox.Game;
using Sandbox.Game.Gui;
using Sandbox.Game.World;
using VRage.Game.Components;
using VRage.Input;

namespace ClientPlugin
{
    [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
    public class UltrawideGuiFix : MySessionComponentBase
    {
        public UltrawideGuiFix()
        {
            
        }

        public override void HandleInput()
        {
            if (!MySandboxGame.IsPaused && MySession.Static.LocalHumanPlayer != null &&
                !MyGuiScreenGamePlay.DisableInput && IsKeybindPressed())
            {
                Log.Debug("Key Pressed");
                // Open Gui ???
            }
        }

        private bool IsKeybindPressed()
        {
            return MyInput.Static.IsNewGameControlPressed(MyControlsSpace.BUILD_SCREEN) && MyInput.Static.IsAnyCtrlKeyPressed();
        }
    }
}