using System;
using KSP.UI.Screens;
using UnityEngine;
using OrXBDAc.missions;
using OrXBDAc.sportingGoods;

namespace OrXBDAc
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class OrX_Controls : MonoBehaviour
    {
        private const float WindowWidth = 220;
        private const float DraggableHeight = 40;
        private const float LeftIndent = 12;
        private const float ContentTop = 20;
        public static OrX_Controls instance;
        public static bool GuiEnabledOrXControl;
        public static bool HasAddedButton;
        public bool waldo = false;
        private readonly float _incrButtonWidth = 26;
        private readonly float contentWidth = WindowWidth - 2 * LeftIndent;
        private readonly float entryHeight = 20;
        private float _contentWidth;
        private bool _gameUiToggle;
        private float _windowHeight = 250;
        private Rect _windowRect;

        private static bool sgOpen;

        private void Awake()
        {        
            if (instance)
                Destroy(instance);

            instance = this;
        }
        
        private void Start()
        {
            waldo = false;
            _windowRect = new Rect((Screen.width / 4) * 3 - WindowWidth - 20, 10, WindowWidth, _windowHeight);
            GameEvents.onHideUI.Add(GameUiDisableOrXControl);
            GameEvents.onShowUI.Add(GameUiEnableOrXControl);
            _gameUiToggle = true;
        }

        private void OnGUI()
        {
            if (waldo) return;

            if (GuiEnabledOrXControl && _gameUiToggle)
            {
                _windowRect = GUI.Window(613297112, _windowRect, GuiWindowOrXControl, "");
            }
        }

        public bool guiOpen = false;

        public void DisableGUI()
        {
            if (GuiEnabledOrXControl)
            {
                waldo = false;
                GuiEnabledOrXControl = false;
                _gameUiToggle = true;
                guiOpen = false;
            }
        }

        public void WaldoToggle()
        {
            if (waldo)
            {
                guiOpen = false;
                GuiEnabledOrXControl = false;
            }
            else
            {
                OrXVesselSwitcher.instance.missions = true;
                GuiEnabledOrXControl = true;
                guiOpen = true;
            }
        }

        public void OpenGUI()
        {
            if (!GuiEnabledOrXControl)
            {
                waldo = false;
                OrX_Log.instance.playerVessel = FlightGlobals.ActiveVessel.vesselName;
                OrX_Log.instance.playerVessel01 = FlightGlobals.ActiveVessel.vesselName;
                OrXVesselSwitcher.instance.missions = true;
                GuiEnabledOrXControl = true;
                _gameUiToggle = true;
                guiOpen = true;
            }
        }

        public void OpenGUIHC()
        {
            if (!GuiEnabledOrXControl)
            {
                waldo = false;
                OrX_Log.instance.playerVessel = FlightGlobals.ActiveVessel.vesselName;
                OrX_Log.instance.playerVessel01 = FlightGlobals.ActiveVessel.vesselName;
                OrXVesselSwitcher.instance.missions = true;
                KerbinMissions.instance.HoloCacheMode();
                GuiEnabledOrXControl = true;
                _gameUiToggle = true;
                guiOpen = true;
            }
        }

        #region GUI
        /// <summary>
        /// GUI
        /// </summary>

        private void ScreenMsg(string msg)
        {
            ScreenMessages.PostScreenMessage(new ScreenMessage(msg, 6, ScreenMessageStyle.UPPER_CENTER));
        }

        private void GuiWindowOrXControl(int OrX_Controls)
        {
            GUI.DragWindow(new Rect(0, 0, WindowWidth, DraggableHeight));
            float line = 0;
            _contentWidth = WindowWidth - 2 * LeftIndent;

            DrawOrXControlTitle(line);
            Level(line);
            line++;
            Salt(line);
            line++;
            SportingGoods(line);

            _windowHeight = ContentTop + line * entryHeight + entryHeight + (entryHeight / 2);
            _windowRect.height = _windowHeight;
        }

        private void Level(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "Level: " + KerbinMissions.instance.level,
                titleStyle);
        }

        private void Salt(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "Salt: " + KerbinMissions.instance.saltTotal,
                titleStyle);
        }

        private void SportingGoods(float line)
        {
            GUIStyle guardStyle = sgOpen ? HighLogic.Skin.box : HighLogic.Skin.button;
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);

            if (sgOpen)
            {
                if (GUI.Button(saveRect, "Sporting Goods Dept", guardStyle))
                {
                    sgOpen = false;
                    if (OrX_SportingGoods.instance.guiActive)
                    {
                        ScreenMsg("<color=#cc4500ff><b>Sporting Goods Department is closed</b></color>");
                        OrX_SportingGoods.instance.ToggleGUI();
                    }
                }
            }
            else
            {
                if (GUI.Button(saveRect, "Sporting Goods Dept", guardStyle))
                {
                    sgOpen = true;
                    if (!OrX_SportingGoods.instance.guiActive)
                    {
                        ScreenMsg("<color=#cc4500ff><b>Sporting Goods Department is open for business</b></color>");
                        OrX_SportingGoods.instance.ToggleGUI();
                    }
                }
            }
        }

        public void EnableGuiOrXControl()
        {
            GuiEnabledOrXControl = true;
            Debug.Log("[OrX Controls]: Showing OrXControl GUI");
            OrX_Log.instance.playerVessel = FlightGlobals.ActiveVessel.vesselName;
            OrX_Log.instance.playerVessel01 = FlightGlobals.ActiveVessel.vesselName;
        }

        public void DisableGuiOrXControl()
        {
            //ScreenMsg("<color=#cc4500ff><b>What, are you scared?</b></color>");
            guiOpen = false;
            GuiEnabledOrXControl = false;
            Debug.Log("[OrX Controls]: Hiding OrXControl GUI");
        }

        private void GameUiEnableOrXControl()
        {
            _gameUiToggle = true;
            guiOpen = true;
        }

        private void GameUiDisableOrXControl()
        {
            _gameUiToggle = false;
            guiOpen = false;
        }

        private void DrawOrXControlTitle(float line)
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
            GUI.Label(new Rect(0, 0, WindowWidth, 20), "OrX Missions", titleStyle);
        }

        #endregion

        private void Dummy()
        {
        }
    }
}