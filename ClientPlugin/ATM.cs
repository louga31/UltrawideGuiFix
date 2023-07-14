using System.Text;
using avaness.PluginLoader.GUI;
using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.Game.World;
using Sandbox.Graphics.GUI;
using VRageMath;

namespace ClientPlugin
{
    public class ATM : PluginScreen
    {
        public ATM(MyInventory inventory)
        {
            this.inventory = inventory;
        }

        private MyInventory inventory;
        
        public static void Open()
        {
            MyInventory inventory = MySession.Static.LocalCharacter.GetInventory();
            ATM atm = new ATM(inventory);
            MyGuiSandbox.AddScreen(atm);
        }
        
        public override string GetFriendlyName()
        {
            return typeof(ATM).FullName;
        }

        public override void RecreateControls(bool constructor)
        {
            base.RecreateControls(constructor);
            
            // Top
            MyGuiControlLabel caption = AddCaption("ATM", captionScale: 1);
            AddBarBelow(caption);
            
            // Bottom
            Vector2 bottomMid = new Vector2(0, m_size.Value.Y / 2);
            MyGuiControlButton btnDeposit = new MyGuiControlButton(position: new Vector2(bottomMid.X - GuiSpacing, bottomMid.Y - GuiSpacing), text: new StringBuilder("Deposit"), originAlign: VRage.Utils.MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_BOTTOM, onButtonClick: OnDepositClick);
            MyGuiControlButton btnWithdraw = new MyGuiControlButton(position: new Vector2(bottomMid.X + GuiSpacing, bottomMid.Y - GuiSpacing), text: new StringBuilder("Withdraw"), originAlign: VRage.Utils.MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_BOTTOM, onButtonClick: OnWithdrawClick);
            Controls.Add(btnDeposit);
            Controls.Add(btnWithdraw);
            AddBarAbove(btnDeposit);
            
            // Center
            MyLayoutTable grid = GetLayoutTableBetween(caption, btnDeposit, verticalSpacing: GuiSpacing * 2);
            grid.SetColumnWidthsNormalized(0.5f, 0.3f, 0.2f);
            grid.SetRowHeightsNormalized(0.05f, 0.95f);
            
            // Column 1
            MyGuiControlLabel volume = new MyGuiControlLabel(text: "Volume: " + inventory.CurrentVolume * VRage.MyFixedPoint. + " L / " + (inventory.MaxVolume > 1000000 ? "Unlimited" : inventory.MaxVolume.ToString()));
            grid.Add(volume, MyAlignH.Center, MyAlignV.Bottom, 0, 0);

        }
        
        private void OnDepositClick(MyGuiControlButton btn)
        {
            CloseScreen();
        }

        private void OnWithdrawClick(MyGuiControlButton btn)
        {
            CloseScreen();
        }
    }
}