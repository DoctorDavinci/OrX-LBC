using UnityEngine;
using OrX.parts;
using OrX.missions;

namespace OrX.sportingGoods
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class OrX_KerbalUpgrades : MonoBehaviour
    {

        /// <summary>
        /// /////////////////////////
        /// </summary>
        public static OrX_KerbalUpgrades instance;

        private void Awake()
        {

            // Add vessels





            if (instance) Destroy(instance);
            instance = this;
        }


        #region Core
        /// <summary>
        /// ////////////////////
        /// </summary>
        /// <param name="msg"></param>
        /// 

        private void ScreenMsg(string msg)
        {
            ScreenMessages.PostScreenMessage(new ScreenMessage(msg, 6, ScreenMessageStyle.UPPER_CENTER));
        }

        #endregion

        #region GUI

        private const float WindowWidth = 220;
        private const float DraggableHeight = 40;
        private const float LeftIndent = 12;
        private const float ContentTop = 20;
        public static bool GuiEnabledOrXUpgrades;
        public static bool HasAddedButton;
        private readonly float _incrButtonWidth = 26;
        private readonly float contentWidth = WindowWidth - 2 * LeftIndent;
        private readonly float entryHeight = 20;
        private float _contentWidth;
        private bool _gameUiToggle;
        private float _windowHeight = 250;
        private Rect _windowRect;

        private void Start()
        {
            _windowRect = new Rect(Screen.width - 320 - WindowWidth, 140, WindowWidth, _windowHeight);
            //AddToolbarButton();
            GameEvents.onHideUI.Add(GameUiDisableOrXUpgrades);
            GameEvents.onShowUI.Add(GameUiEnableOrXUpgrades);
            _gameUiToggle = true;
        }

        public void Update()
        {
            if (OrX_SportingGoods.instance.guiActive && GuiEnabledOrXUpgrades)
            {
                GuiEnabledOrXUpgrades = false;
            }
        }

        /*
        private void AddToolbarButton()
        {
            string textureDir = "OrX/Plugin/";

            if (!HasAddedButton)
            {
                Texture buttonTexture = GameDatabase.Instance.GetTexture(textureDir + "OrX_icon", false); //texture to use for the button
                ApplicationLauncher.Instance.AddModApplication(EnableGuiOrXUpgrades, DisableGuiOrXUpgrades, Dummy, Dummy, Dummy, Dummy,
                    ApplicationLauncher.AppScenes.FLIGHT, buttonTexture);
                HasAddedButton = true;
            }
        }
        */

        private void UpgradeKerbal()
        {
            var kerbal = FlightGlobals.ActiveVessel.FindPartModuleImplementing<ModuleOrX>();

            if (walk)
            {
                kerbal._walkSpeed += 0.3f;
                kerbal.ResetKerbalRoutine();
            }

            if (run)
            {
                kerbal._runSpeed += 0.5f;
                kerbal.ResetKerbalRoutine();
            }

            if (strafe)
            {
                kerbal._strafeSpeed += 0.5f;
                kerbal.ResetKerbalRoutine();
            }

            if (swim)
            {
                kerbal._swimSpeed += 0.3f;
                kerbal.ResetKerbalRoutine();
            }

            if (jump)
            {
                kerbal._maxJumpForce += 0.5f;
                kerbal.ResetKerbalRoutine();
            }
        }

        private bool walk = false;
        private bool run = false;
        private bool strafe = false;
        private bool swim = false;
        private bool jump = false;

        private void OnGUI()
        {
            if (GuiEnabledOrXUpgrades && _gameUiToggle)
            {
                _windowRect = GUI.Window(661211691, _windowRect, GuiWindowOrXUpgrades, "");
            }
        }

        public void ToggleGUI()
        {
            if (GuiEnabledOrXUpgrades)
            {
                _gameUiToggle = true;

                GuiEnabledOrXUpgrades = false;
            }
            else
            {
                _gameUiToggle = true;

                GuiEnabledOrXUpgrades = true;
            }
        }

        #region GUI
        /// <summary>
        /// GUI
        /// </summary>

        private void GuiWindowOrXUpgrades(int OrX_KerbalUpgrades)
        {
            GUI.DragWindow(new Rect(0, 0, WindowWidth, DraggableHeight));
            float line = 0;
            _contentWidth = WindowWidth - 2 * LeftIndent;

            DrawOrXUpgradesTitle(line);
            DrawText(line);
            line++;
            WalkSpeed(line);
            line++;
            RunSpeed(line);
            line++;
            StrafeSpeed(line);
            line++;
            SwimSpeed(line);
            line++;
            JumpForce(line);

            _windowHeight = ContentTop + line * entryHeight + entryHeight + (entryHeight / 2);
            _windowRect.height = _windowHeight;
        }

        private void DrawText(float line)
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
                "Select upgrade from below",
                titleStyle);
        }

        private void WalkSpeed(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);

            if (GUI.Button(saveRect, "Walk Speed: 1000 Salt"))
            {
                if (Missions.instance.saltTotal >= 1000)
                {
                    Missions.instance.saltTotal -= 1000;
                    ScreenMsg("<b>Walk Speed Increased</b>");
                    UpgradeKerbal();

                    if (Missions.instance.saltTotal <= 1000)
                    {
                        DisableGuiOrXUpgrades();
                    }
                }
                else
                {
                    ScreenMsg("<b>Insufficient Salt for that upgrade ... Mine more salt or hunt down more OrX</b>");
                    DisableGuiOrXUpgrades();
                    OrX_SportingGoods.instance.ToggleGUI();

                }
            }
        }

        private void RunSpeed(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);

            if (GUI.Button(saveRect, "Run Speed: 1000 Salt"))
            {
                if (Missions.instance.saltTotal >= 1000)
                {
                    Missions.instance.saltTotal -= 1000;
                    ScreenMsg("<b>Run Speed Increased</b>");

                    if (Missions.instance.saltTotal <= 1000)
                    {
                        DisableGuiOrXUpgrades();
                    }
                    UpgradeKerbal();
                }
                else
                {
                    ScreenMsg("<b>Insufficient Salt for that upgrade ... Mine more salt or hunt down more OrX</b>");
                    DisableGuiOrXUpgrades();
                    OrX_SportingGoods.instance.ToggleGUI();

                }
            }
        }

        private void StrafeSpeed(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);

            if (GUI.Button(saveRect, "Strafe Speed: 1000 Salt"))
            {
                if (Missions.instance.saltTotal >= 1000)
                {
                    Missions.instance.saltTotal -= 1000;
                    ScreenMsg("<b>Strafe Speed Increased</b>");

                    if (Missions.instance.saltTotal <= 1000)
                    {
                        DisableGuiOrXUpgrades();
                    }
                    UpgradeKerbal();
                }
                else
                {
                    ScreenMsg("<b>Insufficient Salt for that upgrade ... Mine more salt or hunt down more OrX</b>");
                    DisableGuiOrXUpgrades();
                    OrX_SportingGoods.instance.ToggleGUI();

                }
            }
        }

        private void SwimSpeed(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);

            if (GUI.Button(saveRect, "Swim Speed: 1000 Salt"))
            {
                if (Missions.instance.saltTotal >= 1000)
                {
                    Missions.instance.saltTotal -= 1000;
                    ScreenMsg("<b>Swim Speed Increased</b>");

                    if (Missions.instance.saltTotal <= 1000)
                    {
                        DisableGuiOrXUpgrades();
                    }
                    UpgradeKerbal();
                }
                else
                {
                    ScreenMsg("<b>Insufficient Salt for that upgrade ... Mine more salt or hunt down more OrX</b>");
                    DisableGuiOrXUpgrades();
                    OrX_SportingGoods.instance.ToggleGUI();

                }
            }
        }

        private void JumpForce(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);

            if (GUI.Button(saveRect, "Jump Force: 1000 Salt"))
            {
                if (Missions.instance.saltTotal >= 1000)
                {
                    Missions.instance.saltTotal -= 1000;
                    ScreenMsg("<b>Jump Force Increased</b>");

                    if (Missions.instance.saltTotal <= 1000)
                    {
                        DisableGuiOrXUpgrades();
                    }
                    UpgradeKerbal();
                }
                else
                {
                    ScreenMsg("<b>Insufficient Salt for that upgrade ... Mine more salt or hunt down more OrX</b>");
                    DisableGuiOrXUpgrades();
                    OrX_SportingGoods.instance.ToggleGUI();

                }
            }
        }



        private void EnableGuiOrXUpgrades()
        {
            GuiEnabledOrXUpgrades = true;
        }

        private void DisableGuiOrXUpgrades()
        {
            GuiEnabledOrXUpgrades = false;
            ScreenMsg("<b>Shop Smart ... Shop S-Mart</b>");

        }

        private void GameUiEnableOrXUpgrades()
        {
            _gameUiToggle = true;
        }

        private void GameUiDisableOrXUpgrades()
        {
            _gameUiToggle = false;
        }

        private void DrawOrXUpgradesTitle(float line)
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
            GUI.Label(new Rect(0, 0, WindowWidth, 20), "Kerbal Upgrades", titleStyle);
        }

        #endregion

        private void Dummy()
        {
        }



        #endregion
    }
}

