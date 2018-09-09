using System;
using KSP.UI.Screens;
using UnityEngine;
using OrXBDAc.parts;

namespace OrX
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class OrX_HP : MonoBehaviour
    {
        private const float WindowWidth = 200;
        private const float DraggableHeight = 40;
        private const float LeftIndent = 12;
        private const float ContentTop = 20;
        public static OrX_HP Fetch;
        public bool GuiEnabledOrX_HP = false;
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

        public float _hp = 0;
        private string hp = string.Empty;
        private float _oxygen = 0.0f;
        private void Awake()
        {        
            if (Fetch)
                Destroy(Fetch);

            Fetch = this;
        }
        
        private void Start()
        {
            _windowRect = new Rect((Screen.width / 16) * 3, 10, WindowWidth, _windowHeight);
            GameEvents.onHideUI.Add(GameUiDisableOrXHP);
            GameEvents.onShowUI.Add(GameUiEnableOrXHP);
            _gameUiToggle = true;
            _hp = 0;
        }

        private void OnGUI()
        {
            if (GuiEnabledOrX_HP && _gameUiToggle)
            {
                _windowRect = GUI.Window(613937212, _windowRect, GuiWindowOrX_HP, "");
            }
        }

        public void Update()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (FlightGlobals.ActiveVessel.isEVA)
                {
                    if (!GuiEnabledOrX_HP)
                    {
                        GuiEnabledOrX_HP = true;
                    }

                    var orx = FlightGlobals.ActiveVessel.FindPartModuleImplementing<ModuleOrXBDAc>();

                    if (orx != null)
                    {
                        var hpMaxString = Convert.ToString(orx.hpMax);
                        hpMax = string.Format("{0:0.0}", hpMaxString);

                        _hp = (orx.hp / orx.hpMax) * 100;
                        var hpString = Convert.ToString(orx.hp);
                        hp = string.Format("{0:0.0}", hpString);
                    }
                }
                else
                {
                    if (GuiEnabledOrX_HP)
                    {
                        GuiEnabledOrX_HP = false;
                    }
                }
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

        private void GuiWindowOrX_HP(int OrX_HP)
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
            GuiEnabledOrX_HP = true;
            Debug.Log("[OrX]: Showing HP GUI");
        }

        private void DisableGui()
        {
            GuiEnabledOrX_HP = false;
            Debug.Log("[OrX]: Hiding HP GUI");
        }

        private void GameUiEnableOrXHP()
        {
            _gameUiToggle = true;
        }

        private void GameUiDisableOrXHP()
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
                "HIT POINT %",
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