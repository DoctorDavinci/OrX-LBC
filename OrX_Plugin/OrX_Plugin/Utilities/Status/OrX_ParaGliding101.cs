using System;
using KSP.UI.Screens;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace OrX.parts
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class OrX_ParaGliding101 : MonoBehaviour
    {
        private const float WindowWidth = 200;
        private const float DraggableHeight = 40;
        private const float LeftIndent = 12;
        private const float ContentTop = 20;
        public static OrX_ParaGliding101 Fetch;
        public static bool GuiEnabled;
        public static bool HasAddedButton;
        private readonly float _incrButtonWidth = 26;
        private readonly float contentWidth = WindowWidth - 2 * LeftIndent;
        private readonly float entryHeight = 20;
        private float _contentWidth;
        private bool _gameUiToggle;
        private float _windowHeight = 250;
        private Rect _windowRect;

        private float _throttle = 50.0f;
        public float jumpForce = 0.3f;
        private bool getJumpForce = true;

        private void Awake()
        {        
            if (Fetch)
                Destroy(Fetch);

            Fetch = this;
        }
        
        private void Start()
        {
            _windowRect = new Rect((Screen.width / 2) - (WindowWidth / 2), 80, WindowWidth, _windowHeight);
            GameEvents.onHideUI.Add(GameUiDisablePara);
            GameEvents.onShowUI.Add(GameUiEnablePara);
            _gameUiToggle = true;
        }

        private void OnGUI()
        {
            if (GuiEnabled && _gameUiToggle)
            {
                _windowRect = GUI.Window(69921843, _windowRect, GuiWindowPara, "");
            }
        }

        public void Update()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (FlightGlobals.ActiveVessel.isEVA)
                {
                    CheckKerbal();
                }
                else
                {
                    if (GuiEnabled)
                    {
                        DisableGui();
                    }
                }
            }
        }

        private void CheckKerbal()
        {
            foreach (Part p in FlightGlobals.ActiveVessel.Parts)
            {
                var kerbal = FlightGlobals.ActiveVessel.FindPartModuleImplementing<KerbalEVA>();
                var paraMotor = p.vessel.FindPartModuleImplementing<ModuleOrXParaMotor>();
                if (paraMotor == null)
                {
                    if (GuiEnabled)
                    {
                        DisableGui();
                    }

                    if (!getJumpForce)
                    {
                        kerbal.maxJumpForce = jumpForce;
                        getJumpForce = true;
                    }
                }
                else
                {
                    if (!GuiEnabled)
                    {
                        EnableGui();
                    }

                    if (getJumpForce)
                    {
                        {
                            jumpForce = kerbal.maxJumpForce;
                            getJumpForce = false;
                        }
                    }

                    paraMotor._throttle = _throttle;
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

        private void GuiWindowPara(int OrX_Para)
        {
            GUI.DragWindow(new Rect(0, 0, WindowWidth, DraggableHeight));
            float line = 0;
            _contentWidth = WindowWidth - 2 * LeftIndent;

            DrawTitle();
//            DrawGround(line);
//            line++;
//            DrawGroundLaunch(line);
//            line++;
            DrawThrottle(line);
            line++;
            DrawThrottleSlider(line);


            _windowHeight = ContentTop + line * entryHeight + entryHeight + (entryHeight / 2);
            _windowRect.height = _windowHeight;
        }

        private void AddToolbarButton()
        {
            string textureDir = "OrX/Plugin/";

            if (!HasAddedButton)
            {
                Texture buttonTexture = GameDatabase.Instance.GetTexture(textureDir + "OrX_ParaMotor_icon", false); //texture to use for the button
                ApplicationLauncher.Instance.AddModApplication(EnableGui, DisableGui, Dummy, Dummy, Dummy, Dummy,
                    ApplicationLauncher.AppScenes.FLIGHT, buttonTexture);
                HasAddedButton = true;
            }
        }

        private void EnableGui()
        {
            GuiEnabled = true;
            Debug.Log("[SM_ParaMotor]: Showing OrX ParaMotor GUI");
        }

        private void DisableGui()
        {
            GuiEnabled = false;
            Debug.Log("[SM_ParaMotor]: Hiding OrX ParaMotor GUI");
        }

        private void GameUiEnablePara()
        {
            _gameUiToggle = true;
        }

        private void GameUiDisablePara()
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
            GUI.Label(new Rect(0, 0, WindowWidth, 20), "OrX Para Motor", titleStyle);
        }

        private void DrawGround(float line)
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
                "Prepare for Launch",
                titleStyle);
        }


        private void DrawThrottle(float line)
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
                "Throttle %",
                titleStyle);
        }

        private void DrawGroundLaunch(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, 15);
            if (GUI.Button(saveRect, "READY FOR LAUNCH"))
            {
//                GroundLaunch();
            }
        }

        private void DrawThrottleSlider(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);
            GUI.Label(new Rect(8, ContentTop + line * entryHeight, contentWidth * 0.9f, 20), "1");
            GUI.Label(new Rect(90, ContentTop + line * entryHeight, contentWidth * 0.9f, 20), "50");
            GUI.Label(new Rect(178, ContentTop + line * entryHeight, contentWidth * 0.9f, 20), "100");
            _throttle = GUI.HorizontalSlider(saveRect, _throttle, 0, 100);

        }
        #endregion

        private void Dummy()
        {
        }
    }
}
