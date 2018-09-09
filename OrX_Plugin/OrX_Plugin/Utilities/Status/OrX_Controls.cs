using System;
using KSP.UI.Screens;
using UnityEngine;
using OrX.missions;
using OrX.sportingGoods;

namespace OrX
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
        private readonly float _incrButtonWidth = 26;
        private readonly float contentWidth = WindowWidth - 2 * LeftIndent;
        private readonly float entryHeight = 20;
        private float _contentWidth;
        private bool _gameUiToggle;
        private float _windowHeight = 250;
        private Rect _windowRect;

        private void Awake()
        {        
            if (instance)
                Destroy(instance);

            instance = this;
        }
        
        private void Start()
        {
            _windowRect = new Rect(Screen.width - 320 - WindowWidth, 10, WindowWidth, _windowHeight);
            //AddToolbarButton();
            GameEvents.onHideUI.Add(GameUiDisableOrXControl);
            GameEvents.onShowUI.Add(GameUiEnableOrXControl);
            _gameUiToggle = true;
        }

        private void AddToolbarButton()
        {
            string textureDir = "OrX/Plugin/";

            if (!HasAddedButton)
            {
                Texture buttonTexture = GameDatabase.Instance.GetTexture(textureDir + "OrX_icon", false); //texture to use for the button
                ApplicationLauncher.Instance.AddModApplication(EnableGuiOrXControl, DisableGuiOrXControl, Dummy, Dummy, Dummy, Dummy,
                    ApplicationLauncher.AppScenes.FLIGHT, buttonTexture);
                HasAddedButton = true;
            }
        }


        private void OnGUI()
        {
            if (GuiEnabledOrXControl && _gameUiToggle)
            {
                _windowRect = GUI.Window(613297112, _windowRect, GuiWindowOrXControl, "");
                guiOpen = true;
            }
        }

        public bool guiOpen = false;

        public void DisableGUI()
        {
            if (GuiEnabledOrXControl)
            {
                GuiEnabledOrXControl = false;
                _gameUiToggle = true;
                guiOpen = false;
            }
        }

        public void OpenGUI()
        {
            if (!GuiEnabledOrXControl)
            {
                OrX_Log.instance.playerVessel = FlightGlobals.ActiveVessel.vesselName;
                OrX_Log.instance.playerVessel01 = FlightGlobals.ActiveVessel.vesselName;

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
                "Level: " + Missions.instance.level,
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
                "Salt: " + Missions.instance.saltTotal,
                titleStyle);
        }

        private void SportingGoods(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);

            if (GUI.Button(saveRect, "Sporting Goods Dept"))
            {
                if (Missions.instance.saltTotal >= 1000)
                {
                    if (!FlightGlobals.ActiveVessel.isEVA)
                    {
                        ScreenMsg("<b>Sporting Goods Department has no drive through window ... Please exit your vehicle in order to make a purchase</b>");
                    }
                    else
                    {
                        OrX_SportingGoods.instance.ToggleGUI();
                        //OrX_Log.instance.sound_OrXBoomstick.Play();
                    }
                }
                else
                {
                    ScreenMsg("<b>Sporting Goods Department is Closed ... Please come back later with more Salt</b>");
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
            //ScreenMsg("<b>What, are you scared?</b>");
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