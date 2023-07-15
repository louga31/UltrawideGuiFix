using System.Text;
using avaness.PluginLoader.GUI;
using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.Game.World;
using Sandbox.Graphics.GUI;
using VRage.Game;
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
            MyLayoutTable layout = GetLayoutTableBetween(caption, btnDeposit, verticalSpacing: GuiSpacing * 2);
            layout.SetColumnWidthsNormalized(0.5f, 0.35f, 0.05f, 0.05f, 0.05f);
            layout.SetRowHeightsNormalized(0.05f, 0.05f, 0.05f, 0.85f);
            
            // Row 0
            
            
            // Row 1
            MyGuiControlLabel volumeLabel = new MyGuiControlLabel(text: "Volume:");
            layout.Add(volumeLabel, MyAlignH.Left, MyAlignV.Center, 1, 0);
            MyGuiControlLabel volumeValue = new MyGuiControlLabel(text: (inventory.CurrentVolume * 1000.00f) + " L / " + (inventory.MaxVolume > 1000000 ? "Unlimited" : inventory.MaxVolume.ToString()));
            layout.AddWithSize(volumeValue, MyAlignH.Right, MyAlignV.Center, 1, 1, colSpan: 4);
            
            // Row 2
            MyGuiControlLabel balanceLabel = new MyGuiControlLabel(text: "Account Balance:");
            layout.Add(balanceLabel, MyAlignH.Left, MyAlignV.Center, 2, 0);
            MyGuiControlLabel balanceValue = new MyGuiControlLabel(text: 0.0.ToString());
            layout.AddWithSize(balanceValue, MyAlignH.Right, MyAlignV.Center, 2, 1, colSpan: 3);
            //Add currency icon
            MyGuiControlImage currencyIcon = new MyGuiControlImage(size: new Vector2(0.02f), textures: new string[1]
            {
                @"Textures\GUI\Icons\SpaceCredits.dds"
            });
            layout.Add(currencyIcon, MyAlignH.Right, MyAlignV.Center, 2, 4);
            
            // Row 3
            MyGuiControlLabel cashbackLabel = new MyGuiControlLabel(text: "Cashback:");
            layout.Add(cashbackLabel, MyAlignH.Left, MyAlignV.Center, 3, 0);
            MyGuiControlTextbox cashbackValue = new MyGuiControlTextbox(type: MyGuiControlTextboxType.DigitsOnly ,defaultText: 0.0.ToString());
            layout.Add(cashbackValue, MyAlignH.Right, MyAlignV.Center, 3, 1);
            MyGuiControlButton btnMinus = new MyGuiControlButton(visualStyle: MyGuiControlButtonStyleEnum.SquareSmall);
            AddImageToButton(btnMinus, @"Textures\GUI\Icons\HUD 2017\Minus.dds", 0.8f);
            layout.Add(btnMinus, MyAlignH.Right, MyAlignV.Center, 3, 2);
            MyGuiControlButton btnPlus = new MyGuiControlButton(visualStyle: MyGuiControlButtonStyleEnum.SquareSmall);
            AddImageToButton(btnPlus, @"Textures\GUI\Icons\HUD 2017\Plus.dds", 0.8f);
            layout.Add(btnPlus, MyAlignH.Right, MyAlignV.Center, 3, 3);
            //Add currency icon
            MyGuiControlImage currencyIcon2 = new MyGuiControlImage(size: new Vector2(0.02f), textures: new string[1]
            {
                @"Textures\GUI\Icons\SpaceCredits.dds"
            });
            layout.Add(currencyIcon2, MyAlignH.Right, MyAlignV.Center, 3, 4);

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