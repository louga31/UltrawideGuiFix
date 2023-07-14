using avaness.PluginLoader.GUI;
using NLog.Fluent;
using Sandbox;
using Sandbox.Game;
using Sandbox.Game.Gui;
using Sandbox.Game.World;
using Sandbox.Graphics.GUI;
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
            // Inspired by https://github.com/austinvaness/BlockPicker/blob/486e1380b375920ec0ebdb2f01790f3ad9941742/BlockPicker/BlockPickerSession.cs#L31-L32
            if (!MySandboxGame.IsPaused && MySession.Static.LocalHumanPlayer != null &&
                !MyGuiScreenGamePlay.DisableInput && IsKeybindPressed())
            {
                Log.Debug("Key Pressed");
                ATM.Open();
            }
        }
        
        // Inspired by https://github.com/austinvaness/BlockPicker/blob/486e1380b375920ec0ebdb2f01790f3ad9941742/BlockPicker/BlockPickerSession.cs#L81-L84
        private bool IsKeybindPressed()
        {
            return MyInput.Static.IsNewGameControlPressed(MyControlsSpace.BUILD_SCREEN) && MyInput.Static.IsAnyCtrlKeyPressed();
        }
    }
}