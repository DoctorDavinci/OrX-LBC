using System;
using KSP.UI.Screens;
using UnityEngine;

namespace OrX.sportingGoods
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class OrX_SportingGoods : MonoBehaviour
    {
        private const float WindowWidth = 220;
        private const float DraggableHeight = 40;
        private const float LeftIndent = 12;
        private const float ContentTop = 20;
        public static OrX_SportingGoods instance;
        public static bool GuiEnabledOrXSG;
        public static bool HasAddedButton;
        private readonly float _incrButtonWidth = 26;
        private readonly float contentWidth = WindowWidth - 2 * LeftIndent;
        private readonly float entryHeight = 20;
        private float _contentWidth;
        private bool _gameUiToggle;
        private float _windowHeight = 250;
        private Rect _windowRect;

        public bool guiActive;
        public bool ironKerbal;

        private void Awake()
        {        
            if (instance)
                Destroy(instance);

            instance = this;
        }
        
        private void Start()
        {
            _windowRect = new Rect(Screen.width - 320 - WindowWidth, 140, WindowWidth, _windowHeight);
            AddToolbarButton();
            GameEvents.onHideUI.Add(GameUiDisableOrXSG);
            GameEvents.onShowUI.Add(GameUiEnableOrXSG);
            _gameUiToggle = true;
        }

        private void AddToolbarButton()
        {
            string textureDir = "OrX/Plugin/";

            if (!HasAddedButton && OrX_Log.instance.devKitInstalled)
            {
                Texture buttonTexture = GameDatabase.Instance.GetTexture(textureDir + "OrX_dev", false); //texture to use for the button
                ApplicationLauncher.Instance.AddModApplication(EnableGuiOrXSG, DisableGuiOrXSG, Dummy, Dummy, Dummy, Dummy,
                    ApplicationLauncher.AppScenes.FLIGHT, buttonTexture);
                HasAddedButton = true;
            }
        }


        private void OnGUI()
        {
            if (GuiEnabledOrXSG && _gameUiToggle)
            {
                _windowRect = GUI.Window(618832412, _windowRect, GuiWindowOrXSG, "");
            }
        }

        public void ToggleGUI()
        {
            if (GuiEnabledOrXSG)
            {
                GuiEnabledOrXSG = false;
                guiActive = false;

            }
            else
            {
                GuiEnabledOrXSG = true;
                guiActive = true;
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

        private void GuiWindowOrXSG(int OrX_SG)
        {
            GUI.DragWindow(new Rect(0, 0, WindowWidth, DraggableHeight));
            float line = 0;
            _contentWidth = WindowWidth - 2 * LeftIndent;

            DrawOrXSGTitle(line);
            DrawXP(line);
            line++;
            DrawKerbalUpgrades(line);
            line++;
            DrawTacticalToys(line);
            line++;
            DrawBDAc(line);
            line++;
            DrawSMAFVs(line);
            line++;
            DrawDCKFT(line);

            _windowHeight = ContentTop + line * entryHeight + entryHeight + (entryHeight / 2);
            _windowRect.height = _windowHeight;
        }

        private void DrawXP(float line)
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
                "Categories",
                titleStyle);
        }

        private void DrawKerbalUpgrades(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);

            if (GUI.Button(saveRect, "Kerbal Upgrades"))
            {
                DisableGuiOrXSG();
                OrX_KerbalUpgrades.instance.ToggleGUI();
            }
        }


        private void DrawTacticalToys(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);

            if (GUI.Button(saveRect, "Tactical Toys"))
            {
                DisableGuiOrXSG();
                OrX_TacticalToys.instance.ToggleGUI();
                ScreenMsg("<b>Sorry, no Tactical Toys in Stock ... Please come back later</b>");

            }
        }


        private void DrawBDAc(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);

            if (GUI.Button(saveRect, "BD Armory"))
            {
                DisableGuiOrXSG();
                OrX_BDAc.instance.ToggleGUI();
            }
        }

        private void DrawSMAFVs(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);

            if (GUI.Button(saveRect, "SM AFVs"))
            {
                SMAFVs();
                DisableGuiOrXSG();
            }
        }

        private void SMAFVs()
        {
            if (OrX_Log.instance.smAFVs && OrX_Log.instance.kf)
            {
                OrX_SMI.instance.ToggleGUI();
            }
            else
            {
                ScreenMsg("<b>Sorry, no SM AFVs in Stock ... Please come back later after installing SM_AFVs and Kerbal Foundries</b>");
            }
        }

        private void DrawDCKFT(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);

            if (GUI.Button(saveRect, "FutureTech"))
            {
                DCKFT();
                DisableGuiOrXSG();
            }
        }

        private void DCKFT()
        {
            if (OrX_Log.instance.smAFVs && OrX_Log.instance.kf)
            {
                OrX_DCKFT.instance.ToggleGUI();
            }
            else
            {
                ScreenMsg("<b>Sorry, no DCK FutureTech Vehicles in Stock ... Please come back later after installing DCK FutureTech</b>");
            }
        }


        private void EnableGuiOrXSG()
        {
            GuiEnabledOrXSG = true;
            guiActive = true;

            Debug.Log("[OrX Controls]: Showing OrXSG GUI");
        }

        private void DisableGuiOrXSG()
        {
            GuiEnabledOrXSG = false;
            guiActive = false;

            Debug.Log("[OrX Controls]: Hiding OrXSG GUI");
        }

        private void GameUiEnableOrXSG()
        {
            _gameUiToggle = true;
        }

        private void GameUiDisableOrXSG()
        {
            _gameUiToggle = false;
        }

        private void DrawOrXSGTitle(float line)
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
            GUI.Label(new Rect(0, 0, WindowWidth, 20), "Sporting Goods Dept", titleStyle);
        }

        #endregion

        private void Dummy()
        {
        }
    }
}