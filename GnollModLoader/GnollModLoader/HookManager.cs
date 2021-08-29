﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GnollModLoader.GUI;

namespace GnollModLoader
{
    public class HookManager
    {
        public delegate Game.GUI.Controls.Button AddButton(string label);

        public delegate void ExportMenuListInitHandler(Game.GUI.ImportExportMenu importExportMenu, Game.GUI.Controls.Manager manager, AddButton context);
        public delegate void InGameHUDInitHandler(Game.GUI.InGameHUD inGameHUD, Game.GUI.Controls.Manager manager);
        public delegate void UpdateInGameHandler(float realTimeDelta, float gameTimeDelta);

        private List<IMod> _listOfMods;

        public HookManager()
        {
            instance = this;
        }

        public void RegisterMods(List<IMod> listOfMods)
        {
            this._listOfMods = listOfMods;
        }

        public static int HookImportExportListInit(int Y, Game.GUI.ImportExportMenu importExportMenu, Game.GUI.Controls.Manager manager)
        {
            AddButton addButton = (string label) =>
            {
                var button = new Game.GUI.Controls.Button(importExportMenu.Manager);
                button.Init();
                Y += button.Margins.Top;
                button.Width = 200;
                button.Top = Y;
                button.Text = label;
                importExportMenu.panel_0.Add(button);
                Y += button.Height + button.Margins.Bottom;
                return button;
            };

            if (instance.ExportMenuListInit != null)
            {
                Console.WriteLine("-- Hook Import/Export list");
                instance.ExportMenuListInit(importExportMenu, manager, addButton);
            }

            return Y;
        }

        public static void HookInGameHUDInit(Game.GUI.InGameHUD inGameHUD, Game.GUI.Controls.Manager manager)
        {
            if (instance.InGameHUDInit != null)
            {
                Console.WriteLine("-- Hook In Game HUD Init");
                instance.InGameHUDInit(inGameHUD, manager);
            }
        }

        public static void HookUpdateInGame(float realTimeDelta, float gameTimeDelta)
        {
            if (instance.UpdateInGame != null)
            {
                float timeElapsedInGame = Game.GnomanEmpire.Instance.world_0.Paused ? 0.0f : gameTimeDelta;
                instance.UpdateInGame(realTimeDelta, timeElapsedInGame);
            }
        }

        public static void HookMainMenuGuiInit(Game.GUI.MainMenuWindow window, Game.GUI.Controls.Manager manager)
        {
            Console.WriteLine("-- Hook Main Menu Init");
            Game.GUI.Controls.Button modButton = window.method_39(manager, GnollMain.NAME);
            modButton.Click += (object sender, Game.GUI.Controls.EventArgs e) =>
            {
                Game.GnomanEmpire.Instance.GuiManager.MenuStack.PushWindow(new ModLoaderMenu(Game.GnomanEmpire.Instance.GuiManager.Manager, instance._listOfMods));
            };
            window.panel_0.Add(modButton);
        }

        public event ExportMenuListInitHandler ExportMenuListInit;
        public event InGameHUDInitHandler InGameHUDInit;
        public event UpdateInGameHandler UpdateInGame;

        private static HookManager instance;
    }
}