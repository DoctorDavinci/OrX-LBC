using UnityEngine;
using OrXBDAc.parts;
using OrXBDAc.missions;

namespace OrXBDAc.sportingGoods
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class OrX_KerbalUpgrades : MonoBehaviour
    {

        /// <summary>
        /// /////////////////////////
        /// </summary>
        public static OrX_KerbalUpgrades instance;

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

        private bool walk = false;
        private bool run = false;
        private bool strafe = false;
        private bool swim = false;
        private bool jump = false;

        private void Awake()
        {
            if (instance) Destroy(instance);
            instance = this;
        }

        private void Start()
        {
            _windowRect = new Rect((Screen.width / 4) * 3 - WindowWidth - 20, 140, WindowWidth, _windowHeight);
            GameEvents.onHideUI.Add(GameUiDisableOrXUpgrades);
            GameEvents.onShowUI.Add(GameUiEnableOrXUpgrades);
            _gameUiToggle = true;
        }

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

        public void EnableGuiOrXUpgrades()
        {
            GuiEnabledOrXUpgrades = true;
        }

        public void DisableGuiOrXUpgrades()
        {
            if (GuiEnabledOrXUpgrades)
            {
                GuiEnabledOrXUpgrades = false;
                ScreenMsg("<color=#cc4500ff><b>Shop Smart ... Shop S-Mart</b></color>");
            }
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

            if (GUI.Button(saveRect, "+0.3 Walk Speed: 1000 Salt"))
            {
                if (KerbinMissions.instance.saltTotal >= 1000)
                {
                    KerbinMissions.instance.saltTotal -= 1000;
                    walk = true;
                    UpgradeKerbal();
                }
                else
                {
                    ScreenMsg("<color=#cc4500ff><b>Insufficient Salt for that upgrade ... Mine more salt or hunt down more OrX</b></color>");
                    DisableGuiOrXUpgrades();
                }
            }
        }

        private void RunSpeed(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);

            if (GUI.Button(saveRect, "+0.3 Run Speed: 1000 Salt"))
            {
                if (KerbinMissions.instance.saltTotal >= 1000)
                {
                    KerbinMissions.instance.saltTotal -= 1000;
                    run = true;
                    UpgradeKerbal();
                }
                else
                {
                    ScreenMsg("<color=#cc4500ff><b>Insufficient Salt for that upgrade ... Mine more salt or hunt down more OrX</b></color>");
                    DisableGuiOrXUpgrades();
                }
            }
        }

        private void StrafeSpeed(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);

            if (GUI.Button(saveRect, "+0.3 Strafe Speed: 1000 Salt"))
            {
                if (KerbinMissions.instance.saltTotal >= 1000)
                {
                    KerbinMissions.instance.saltTotal -= 1000;
                    strafe = true;
                    UpgradeKerbal();
                }
                else
                {
                    ScreenMsg("<color=#cc4500ff><b>Insufficient Salt for that upgrade ... Mine more salt or hunt down more OrX</b></color>");
                    DisableGuiOrXUpgrades();
                }
            }
        }

        private void SwimSpeed(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);

            if (GUI.Button(saveRect, "+0.3 Swim Speed: 1000 Salt"))
            {
                if (KerbinMissions.instance.saltTotal >= 1000)
                {
                    KerbinMissions.instance.saltTotal -= 1000;
                    swim = true;
                    UpgradeKerbal();
                }
                else
                {
                    ScreenMsg("<color=#cc4500ff><b>Insufficient Salt for that upgrade ... Mine more salt or hunt down more OrX</b></color>");
                    DisableGuiOrXUpgrades();
                }
            }
        }

        private void JumpForce(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);

            if (GUI.Button(saveRect, "+0.3 Jump Force: 1000 Salt"))
            {
                if (KerbinMissions.instance.saltTotal >= 1000)
                {
                    KerbinMissions.instance.saltTotal -= 1000;
                    jump = true;
                    UpgradeKerbal();
                }
                else
                {
                    ScreenMsg("<color=#cc4500ff><b>Insufficient Salt for that upgrade ... Mine more salt or hunt down more OrX</b></color>");
                    DisableGuiOrXUpgrades();
                }
            }
        }

        private void UpgradeKerbal()
        {
            DisableGuiOrXUpgrades();

            var kerbal = FlightGlobals.ActiveVessel.FindPartModuleImplementing<ModuleOrXBDAc>();

            if (kerbal != null)
            {
                if (walk)
                {
                    ScreenMsg("<color=#cc4500ff><b>Walk Speed Increased</b></color>");

                    kerbal._walkSpeed += 0.3f;
                    kerbal.ResetKerbalRoutine();
                }

                if (run)
                {
                    ScreenMsg("<color=#cc4500ff><b>Run Speed Increased</b></color>");

                    kerbal._runSpeed += 0.3f;
                    kerbal.ResetKerbalRoutine();
                }

                if (strafe)
                {
                    ScreenMsg("<color=#cc4500ff><b>Strafe Speed Increased</b></color>");

                    kerbal._strafeSpeed += 0.3f;
                    kerbal.ResetKerbalRoutine();
                }

                if (swim)
                {
                    ScreenMsg("<color=#cc4500ff><b>Swim Speed Increased</b></color>");

                    kerbal._swimSpeed += 0.3f;
                    kerbal.ResetKerbalRoutine();
                }

                if (jump)
                {
                    ScreenMsg("<color=#cc4500ff><b>Jump Force Increased</b></color>");

                    kerbal._maxJumpForce += 0.3f;
                    kerbal.ResetKerbalRoutine();
                }
                ResetSelection();
            }
            else
            {
                ResetSelection();
                ScreenMsg("<color=#cc4500ff><b>Sorry, Active Vessel is not a Kerbal ... Unable to Comply</b></color>");
            }
        }

        private void ResetSelection()
        {
            walk = false;
            run = false;
            strafe = false;
            swim = false;
            jump = false;

            OrX_SportingGoods.instance.EnableGuiOrXSG();
        }

        private void ScreenMsg(string msg)
        {
            ScreenMessages.PostScreenMessage(new ScreenMessage(msg, 6, ScreenMessageStyle.UPPER_CENTER));
        }
    }
}

