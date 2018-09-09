using System;
using KSP.UI.Screens;
using UnityEngine;

namespace OrX.parts
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class OrX_Scuba : MonoBehaviour
    {
        private const float WindowWidth = 220;
        private const float DraggableHeight = 40;
        private const float LeftIndent = 12;
        private const float ContentTop = 20;
        public static OrX_Scuba instance;
        public static bool GuiEnabled;
        public static bool HasAddedButton;
        private readonly float _incrButtonWidth = 26;
        private readonly float contentWidth = WindowWidth - 2 * LeftIndent;
        private readonly float entryHeight = 20;
        private float _contentWidth;
        private bool _gameUiToggle;
        private float _windowHeight = 250;
        private Rect _windowRect;

        private bool _vesselName = true;
        public string vesselName = string.Empty;
        public float _trim = 1;
        public float _oxygen = 0;
        public bool getOrXScubaTeam = false;

        public bool guiopen = false;

        private bool controlGUIswitched = false;
        private bool pauseCheck = true;

        private void Awake()
        {        
            if (instance)
                Destroy(instance);

            instance = this;
        }

        private void Start()
        {
            _windowRect = new Rect(Screen.width - 320 - WindowWidth, 140, WindowWidth, _windowHeight);
            GameEvents.onHideUI.Add(GameUiDisableOrXScuba);
            GameEvents.onShowUI.Add(GameUiEnableOrXScuba);
            _gameUiToggle = true;
        }

        private void OnGUI()
        {
            if (PauseMenu.isOpen) return;

            if (GuiEnabled && _gameUiToggle)
            {
                _windowRect = GUI.Window(628212315, _windowRect, GuiWindowOrXScuba, "");
            }
        }

        public void Update()
        {
            if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ready)
            {
                if (FlightGlobals.ActiveVessel.isEVA)
                {
                    if (FlightGlobals.ActiveVessel.Splashed)
                    {
                        if (!GuiEnabled)
                        {
                            guiopen = true;
                            GuiEnabled = true;

                            if (!controlGUIswitched)
                            {
                                controlGUIswitched = true;
                            }
                        }
                    }
                    else
                    {
                        if (GuiEnabled)
                        {
                            guiopen = false;
                            GuiEnabled = false;
                        }

                        if (controlGUIswitched)
                        {
                            controlGUIswitched = false;
                        }
                    }
                }

                if (GuiEnabled)
                {
                    ScubaCheck();
                }

                if (pauseCheck)
                {
                    if (PauseMenu.isOpen || Time.timeScale == 0)
                    {
                        pauseCheck = false;
                        if (GuiEnabled)
                        {
                            DisableGuiOrXScuba();
                        }
                    }
                }
                else
                {
                    if (!PauseMenu.isOpen || Time.timeScale >= 0)
                    {
                        pauseCheck = true;
                    }
                    else
                    {
                        if (GuiEnabled)
                        {
                            DisableGuiOrXScuba();
                        }
                    }
                }

            }
        }

        private void ScubaCheck()
        {
            foreach (Part p in FlightGlobals.ActiveVessel.Parts)
            {
                if (p.vessel.isEVA)
                {
                    var scuba = p.vessel.FindPartModuleImplementing<ModuleScubaTank>();
                    var kerbal = p.vessel.FindPartModuleImplementing<ModuleOrX>();

                    if (scuba != null)
                    {
                        var oxy = ((scuba.oxygen + kerbal.oxygen) / (scuba.oxygenMax + kerbal.oxygenMax)) * 100;
                        _oxygen = oxy;
                    }
                    else
                    {
                        _oxygen = (kerbal.oxygen / kerbal.oxygenMax) * 100;
                    }
                }
                else
                {
                    GuiEnabled = false;
                }
            }
        }

        private void TrimUp()
        {
            foreach (Part p in FlightGlobals.ActiveVessel.Parts)
            {
                var scuba = p.vessel.FindPartModuleImplementing<ModuleOrX>();
                scuba.trimModifier = _trim;
                scuba.trimUp = true;
            }
        }

        private void TrimDown()
        {
            foreach (Part p in FlightGlobals.ActiveVessel.Parts)
            {
                var scuba = p.vessel.FindPartModuleImplementing<ModuleOrX>();
                scuba.trimModifier = _trim;
                scuba.trimDown = true;
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

        private void GuiWindowOrXScuba(int OrXScuba)
        {
            GUI.DragWindow(new Rect(0, 0, WindowWidth, DraggableHeight));
            float line = 0;
            _contentWidth = WindowWidth - 2 * LeftIndent;

            DrawTitle();
            DrawOxygenText(line);
            line++;
            DrawOxygen(line);
            line++;
            DrawScubaText(line);
            line++;
            DrawTrimModifier(line);
            line++;
            DrawTrimUp(line);
            line++;
            DrawTrimDown(line);


            _windowHeight = ContentTop + line * entryHeight + entryHeight + (entryHeight / 2);
            _windowRect.height = _windowHeight;
        }

        private void DrawScubaText(float line)
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
                "Trim Modifier",
                titleStyle);
        }

        private void DrawTrimModifier(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);
            GUI.Label(new Rect(8, ContentTop + line * entryHeight, contentWidth * 0.9f, 20), "0");
            GUI.Label(new Rect(100, ContentTop + line * entryHeight, contentWidth * 0.9f, 20), "|");
            GUI.Label(new Rect(178, ContentTop + line * entryHeight, contentWidth * 0.9f, 20), "5");
            _trim = GUI.HorizontalSlider(saveRect, _trim, 0, 5);
        }

        private void DrawOxygenText(float line)
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
                "OXYGEN %",
                titleStyle);
        }

        private void DrawOxygen(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);
            GUI.Label(new Rect(8, ContentTop + line * entryHeight, contentWidth * 0.9f, 20), "0");
            GUI.Label(new Rect(90, ContentTop + line * entryHeight, contentWidth * 0.9f, 20), "|");
            GUI.Label(new Rect(175, ContentTop + line * entryHeight, contentWidth * 0.9f, 20), "100");
            _oxygen = GUI.HorizontalSlider(saveRect, _oxygen, 0, 100);
        }

        private void DrawTrimUp(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);
            if (GUI.Button(saveRect, "Trim Up"))
            {
                TrimUp();
            }
        }

        private void DrawTrimDown(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);
            if (GUI.Button(saveRect, "Trim Down"))
            {
                TrimDown();
            }
        }

        private void EnableGuiOrXScuba()
        {
            guiopen = true;
            GuiEnabled = true;
            Debug.Log("[OrXScuba]: Showing OrXScuba GUI");
        }

        private void DisableGuiOrXScuba()
        {
            guiopen = false;
            GuiEnabled = false;
            Debug.Log("[OrXScuba]: Hiding OrXScuba GUI");
        }


        private void GameUiEnableOrXScuba()
        {
            _gameUiToggle = true;
        }

        private void GameUiDisableOrXScuba()
        {
            _gameUiToggle = false;
        }

        private void DrawTitle()
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
            GUI.Label(new Rect(0, 0, WindowWidth, 20), "OrX Scuba Kerb Menu", titleStyle);
        }

        #endregion

        private void Dummy()
        {
        }
    }
}