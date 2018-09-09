using System;
using KSP.UI.Screens;
using UnityEngine;
using OrX.parts;
using System.Collections.Generic;
using BDArmory.Core.Module;

namespace OrX
{
    public class OrX_WaldoHP : PartModule
    {
        private const float WindowWidth = 200;
        private const float DraggableHeight = 40;
        private const float LeftIndent = 12;
        private const float ContentTop = 20;
        public static OrX_WaldoHP instance;
        public bool GuiEnabledOrX_WaldoHP = false;
        public static bool HasAddedButton;
        private readonly float _incrButtonWidth = 26;
        private readonly float contentWidth = WindowWidth - 2 * LeftIndent;
        private readonly float entryHeight = 20;
        private float _contentWidth;
        private bool _gameUiToggle;
        private float _windowHeight = 250;
        private Rect _windowRect;

        public bool ironKerbal;

        private string hpMax = string.Empty;
        public bool waldo = false;

        public float _hp = 0;
        private string hp = string.Empty;
        private float _oxygen = 0.0f;

        public HitpointTracker hpTracker;
        private HitpointTracker GetHP()
        {
            HitpointTracker hp = null;

            hp = part.FindModuleImplementing<HitpointTracker>();

            return hp;
        }

        public ModuleOrX orx;
        private ModuleOrX Getorx()
        {
            ModuleOrX hp = null;

            hp = part.FindModuleImplementing<ModuleOrX>();

            return hp;
        }


        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            hpTracker = GetHP();
            orx = Getorx();

        }

        private void Start()
        {
            hpTracker = GetHP();
            orx = Getorx();
            _windowRect = new Rect(300, 10, WindowWidth, _windowHeight);
            GameEvents.onHideUI.Add(GameUiDisableOrXWaldoHP);
            GameEvents.onShowUI.Add(GameUiEnableOrXWaldoHP);
            _gameUiToggle = true;
            _hp = 0;
        }

        private void OnGUI()
        {
            if (GuiEnabledOrX_WaldoHP && _gameUiToggle)
            {
                _windowRect = GUI.Window(933206702, _windowRect, GuiWindowOrX_WaldoHP, "");
            }
        }

        public void Update()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                orx = Getorx();

                if (!GuiEnabledOrX_WaldoHP)
                {
                    GuiEnabledOrX_WaldoHP = true;
                    EnableGui();
                }

                var hpMaxString = Convert.ToString(orx.hpMax);
                hpMax = string.Format("{0:0.0}", hpMaxString);

                _hp = (orx.hp / orx.hpMax) * 100;
                var hpString = Convert.ToString(orx.hp);
                hp = string.Format("{0:0.0}", hpString);

            }
        }

        #region GUI
        /// <summary>
        /// GUI
        /// </summary>

        private void ScreenMsg(string msg)
        {
            ScreenMessages.PostScreenMessage(new ScreenMessage(msg, 4, ScreenMessageStyle.UPPER_CENTER));
        }

        private void GuiWindowOrX_WaldoHP(int OrX_WaldoHP)
        {
            GUI.DragWindow(new Rect(0, 0, WindowWidth, DraggableHeight));
            float line = 0;
            _contentWidth = WindowWidth - 2 * LeftIndent;

            DrawHPNum(line);
            DrawTitleHP(line);
            line++;
            DrawHP(line);

            _windowHeight = ContentTop + line * entryHeight + entryHeight + (entryHeight / 2);
            _windowRect.height = _windowHeight;
        }

        private void EnableGui()
        {
            GuiEnabledOrX_WaldoHP = true;
            Debug.Log("[OrX]: Showing Waldo HP GUI");
        }

        private void DisableGui()
        {
            GuiEnabledOrX_WaldoHP = false;
            Debug.Log("[OrX]: Hiding Waldo HP GUI");
        }

        private void GameUiEnableOrXWaldoHP()
        {
            _gameUiToggle = true;
        }

        private void GameUiDisableOrXWaldoHP()
        {
            _gameUiToggle = false;
        }

        private void DrawHPNum(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter
            };
            GUI.Label(new Rect(0, 0, WindowWidth, 20), "HP: " + hp + " / " + hpMax, titleStyle);
        }

        private void DrawTitleHP(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 10,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "WALDO'S HIT POINT %",
                titleStyle);
        }


        private void DrawHP(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);
            GUI.Label(new Rect(8, ContentTop + line * entryHeight, contentWidth * 0.9f, 20), "0");
            GUI.Label(new Rect(30, ContentTop + line * entryHeight, contentWidth * 0.9f, 20), "|");
            GUI.Label(new Rect(90, ContentTop + line * entryHeight, contentWidth * 0.9f, 20), "|");
            GUI.Label(new Rect(150, ContentTop + line * entryHeight, contentWidth * 0.9f, 20), "|");
            GUI.Label(new Rect(175, ContentTop + line * entryHeight, contentWidth * 0.9f, 20), "100");
            _hp = GUI.HorizontalSlider(saveRect, _hp, 0, 100);
        }

        private void DrawOXY(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);
            GUI.Label(new Rect(8, ContentTop + line * entryHeight, contentWidth * 0.9f, 20), "0");
            GUI.Label(new Rect(30, ContentTop + line * entryHeight, contentWidth * 0.9f, 20), "|");
            GUI.Label(new Rect(90, ContentTop + line * entryHeight, contentWidth * 0.9f, 20), "|");
            GUI.Label(new Rect(150, ContentTop + line * entryHeight, contentWidth * 0.9f, 20), "|");
            GUI.Label(new Rect(175, ContentTop + line * entryHeight, contentWidth * 0.9f, 20), "100");
            _hp = GUI.HorizontalSlider(saveRect, _hp, 0, 100);
        }


        #endregion

        private void Dummy()
        {
        }
    }
}