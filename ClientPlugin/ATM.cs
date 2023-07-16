using System;
using System.Reflection;
using System.Text;
using avaness.PluginLoader.GUI;
using HarmonyLib;
using Sandbox.Engine.Multiplayer;
using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Blocks;
using Sandbox.Game.GameSystems.BankingAndCurrency;
using Sandbox.Game.Localization;
using Sandbox.Game.World;
using Sandbox.Graphics.GUI;
using VRage;
using VRage.Game;
using VRage.Network;
using VRageMath;

namespace ClientPlugin
{
    public class ATM : PluginScreen
    {
        private MyInventory inventory;
        private MyPlayer player;
        private MyAccountInfo accountInfo;
        private int balanceChangeValue = 0;
        private MyStoreBlock storeBlock;
        
        MyGuiControlLabel balanceValue;
        MyGuiControlLabel inventoryValue;

        public ATM(MyStoreBlock storeBlock) : base(size: new Vector2(0.45f, 0.3f))
        {
            player = MySession.Static.LocalHumanPlayer;
            inventory = player.Character.GetInventory();
            MyBankingSystem.Static.TryGetAccountInfo(player.Identity.IdentityId, out accountInfo);
            this.storeBlock = storeBlock;
        }

        public override string GetFriendlyName()
        {
            return typeof(ATM).FullName;
        }

        public override void RecreateControls(bool constructor)
        {
            base.RecreateControls(constructor);

            // Top
            MyGuiControlLabel caption = AddCaption(MySpaceTexts.ScreenCaptionATM, captionScale: 1);
            AddBarBelow(caption);

            // Bottom
            Vector2 bottomMid = new Vector2(0, m_size.Value.Y / 2);
            MyGuiControlButton btnDeposit = new MyGuiControlButton(
                position: new Vector2(bottomMid.X - GuiSpacing, bottomMid.Y - GuiSpacing),
                text: MyTexts.Get(MySpaceTexts.FactionTerminal_Deposit_Currency),
                originAlign: VRage.Utils.MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_BOTTOM,
                onButtonClick: OnDepositClick);
            MyGuiControlButton btnWithdraw = new MyGuiControlButton(
                position: new Vector2(bottomMid.X + GuiSpacing, bottomMid.Y - GuiSpacing),
                text: MyTexts.Get(MySpaceTexts.FactionTerminal_Withdraw_Currency),
                originAlign: VRage.Utils.MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_BOTTOM,
                onButtonClick: OnWithdrawClick);
            Controls.Add(btnDeposit);
            Controls.Add(btnWithdraw);
            AddBarAbove(btnDeposit);

            // Center
            MyLayoutTable layout = GetLayoutTableBetween(caption, btnDeposit, verticalSpacing: GuiSpacing * 2);
            layout.SetColumnWidthsNormalized(0.2f, 0.15f, 0.02f);
            layout.SetRowHeightsNormalized(0.10f, 0.10f, 0.10f);

            // Row 0
            MyGuiControlLabel inventoryLabel = new MyGuiControlLabel(text: "Cash:");
            layout.Add(inventoryLabel, MyAlignH.Left, MyAlignV.Center, 0, 0);
            inventoryValue = new MyGuiControlLabel(
                text: MyBankingSystem.GetFormatedValue(inventory.GetItemAmount(MyBankingSystem.BankingSystemDefinition.PhysicalItemId).ToIntSafe()));
            layout.Add(inventoryValue, MyAlignH.Right, MyAlignV.Center, 0, 1);
            //Add currency icon
            MyGuiControlImage row0currencyIcon = new MyGuiControlImage(size: new Vector2(0.02f),
                textures: new string[] { MyBankingSystem.BankingSystemDefinition.Icons[0] });
            layout.Add(row0currencyIcon, MyAlignH.Right, MyAlignV.Center, 0, 2);


            // Row 1
            MyGuiControlLabel balanceLabel = new MyGuiControlLabel(text: MyTexts.Get(MySpaceTexts.Currency_Default_Account_Label).ToString());
            layout.Add(balanceLabel, MyAlignH.Left, MyAlignV.Center, 1, 0);
            balanceValue = new MyGuiControlLabel(text: MyBankingSystem.GetFormatedValue(accountInfo.Balance));
            layout.Add(balanceValue, MyAlignH.Right, MyAlignV.Center, 1, 1);
            //Add currency icon
            MyGuiControlImage row1currencyIcon = new MyGuiControlImage(size: new Vector2(0.02f),
                textures: new string[] { MyBankingSystem.BankingSystemDefinition.Icons[0] });
            layout.Add(row1currencyIcon, MyAlignH.Right, MyAlignV.Center, 1, 2);

            // Row 2
            MyGuiControlTextbox cashbackValue = new MyGuiControlTextbox(type: MyGuiControlTextboxType.DigitsOnly,
                defaultText: balanceChangeValue.ToString());
            cashbackValue.TextChanged += sender =>
            {
                if (int.TryParse(sender.Text, out int value))
                {
                    balanceChangeValue = value;
                }
            };
            layout.AddWithSize(cashbackValue, MyAlignH.Right, MyAlignV.Center, 2, 0, colSpan: 2);
            //Add currency icon
            MyGuiControlImage row2currencyIcon = new MyGuiControlImage(size: new Vector2(0.02f),
                textures: new string[] { MyBankingSystem.BankingSystemDefinition.Icons[0] });
            layout.Add(row2currencyIcon, MyAlignH.Right, MyAlignV.Center, 2, 2);
        }

        private void OnDepositClick(MyGuiControlButton btn)
        {
            AccessTools.Method(typeof(MyStoreBlock), "CreateChangeBalanceRequest")
                .Invoke(storeBlock,
                    new object[]
                    {
                        balanceChangeValue, inventory.Entity.EntityId,
                        new Action<MyStoreBuyItemResults>(OnChangeBalanceCompleted)
                    });
        }

        private void OnWithdrawClick(MyGuiControlButton btn)
        {
            AccessTools.Method(typeof(MyStoreBlock), "CreateChangeBalanceRequest")
                .Invoke(storeBlock,
                    new object[]
                    {
                        balanceChangeValue, inventory.Entity.EntityId,
                        new Action<MyStoreBuyItemResults>(OnChangeBalanceCompleted)
                    });
        }

        private void OnChangeBalanceCompleted(MyStoreBuyItemResults result)
        {
            ProcessResult(result);
        }

        private void ProcessResult(MyStoreBuyItemResults result)
        {
            switch (result)
            {
                case MyStoreBuyItemResults.WrongAmount:
                    MyGuiSandbox.AddScreen(MyGuiSandbox.CreateMessageBox(buttonType: MyMessageBoxButtonsType.OK,
                        messageCaption: MyTexts.Get(MySpaceTexts.StoreBuy_Error_Caption_WrongAmount),
                        messageText: MyTexts.Get(MySpaceTexts.StoreBuy_Error_Text_WrongAmount)));
                    break;
                case MyStoreBuyItemResults.NotEnoughMoney:
                    MyGuiSandbox.AddScreen(MyGuiSandbox.CreateMessageBox(buttonType: MyMessageBoxButtonsType.OK,
                        messageCaption: MyTexts.Get(MySpaceTexts.StoreBuy_Error_Caption_NotEnoughMoney),
                        messageText: MyTexts.Get(MySpaceTexts.StoreBuy_Error_Text_NotEnoughMoney)));
                    break;
                case MyStoreBuyItemResults.NotEnoughInventorySpace:
                    MyGuiSandbox.AddScreen(MyGuiSandbox.CreateMessageBox(buttonType: MyMessageBoxButtonsType.OK,
                        messageCaption: MyTexts.Get(MySpaceTexts.StoreBuy_Error_Caption_NotEnoughMoney),
                        messageText: MyTexts.Get(MySpaceTexts.StoreBuy_Error_Text_NotEnoughInventorySpace)));
                    break;
            }
            UpdateLocalPlayerCurrency();
        }

        private void UpdateLocalPlayerCurrency()
        {
            MyBankingSystem.Static.TryGetAccountInfo(MySession.Static.LocalPlayerId, out accountInfo);
            balanceValue.Text = MyBankingSystem.GetFormatedValue(accountInfo.Balance);
            inventoryValue.Text = MyBankingSystem.GetFormatedValue(inventory.GetItemAmount(MyBankingSystem.BankingSystemDefinition.PhysicalItemId).ToIntSafe());
        }
    }
}