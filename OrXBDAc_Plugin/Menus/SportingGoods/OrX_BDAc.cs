using UnityEngine;
using OrXBDAc.missions;
using OrXBDAc.spawn;

namespace OrXBDAc.sportingGoods
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class OrX_BDAc : MonoBehaviour
    {

        /// <summary>
        /// /////////////////////////
        /// </summary>
        public static OrX_BDAc instance;

        private const float WindowWidth = 220;
        private const float DraggableHeight = 40;
        private const float LeftIndent = 12;
        private const float ContentTop = 20;
        public static bool GuiEnabledOrXBDArmory;
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
            if (instance) Destroy(instance);
            instance = this;
        }

        private void Start()
        {
            _windowRect = new Rect((Screen.width / 4) * 3 - WindowWidth - 20, 140, WindowWidth, _windowHeight);
            GameEvents.onHideUI.Add(GameUiDisableOrXBDArmory);
            GameEvents.onShowUI.Add(GameUiEnableOrXBDArmory);
            _gameUiToggle = true;
        }

        private void OnGUI()
        {
            if (GuiEnabledOrXBDArmory && _gameUiToggle)
            {
                _windowRect = GUI.Window(74393291, _windowRect, GuiWindowOrXBDArmory, "");
            }
        }

        public void ToggleGUI()
        {
            if (GuiEnabledOrXBDArmory)
            {
                _gameUiToggle = true;
                GuiEnabledOrXBDArmory = false;
            }
            else
            {
                _gameUiToggle = true;
                GuiEnabledOrXBDArmory = true;
            }
        }

        private void GuiWindowOrXBDArmory(int OrX_BDArmory)
        {
            GUI.DragWindow(new Rect(0, 0, WindowWidth, DraggableHeight));
            float line = 0;
            _contentWidth = WindowWidth - 2 * LeftIndent;

            DrawOrXBDArmoryTitle(line);
            DrawSelectVessel(line);
            line++;
            Vessel01(line);
            //line++;
            //Vessel02(line);
            //line++;
            //Vessel03(line);
            //line++;
            //Vessel04(line);
            //line++;
            //Vessel05(line);
            //line++;
            //Vessel06(line);
            //line++;
            //Vessel07(line);
            //line++;
            //Vessel08(line);
            //line++;
            //Vessel09(line);
            //line++;
            //Vessel10(line);

            _windowHeight = ContentTop + line * entryHeight + entryHeight + (entryHeight / 2);
            _windowRect.height = _windowHeight;
        }

        public void EnableGuiOrXBDArmory()
        {
            GuiEnabledOrXBDArmory = true;
        }

        public void DisableGuiOrXBDArmory()
        {
            if (GuiEnabledOrXBDArmory)
            {
                GuiEnabledOrXBDArmory = false;
                ScreenMsg("<color=#cc4500ff><b>Shop Smart ... Shop S-Mart</b></color>");
            }
        }

        private void GameUiEnableOrXBDArmory()
        {
            _gameUiToggle = true;
        }

        private void GameUiDisableOrXBDArmory()
        {
            _gameUiToggle = false;
        }

        private void DrawOrXBDArmoryTitle(float line)
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
            GUI.Label(new Rect(0, 0, WindowWidth, 20), "BD Armory", titleStyle);
        }

        private void DrawSelectVessel(float line)
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
                "Select Vessel",
                titleStyle);
        }

        private void Vessel01(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);

            if (GUI.Button(saveRect, "Lynx: 1000 Salt"))
            {
                if (KerbinMissions.instance.saltTotal >= 1000)
                {
                    KerbinMissions.instance.saltTotal -= 1000;
                    SpawnOrX_BDarmory.instance.sg01 = true;
                    SpawnOrX_BDarmory.instance.CheckSpawnTimer();
                    DisableGuiOrXBDArmory();
                }
                else
                {
                    ScreenMsg("<color=#cc4500ff><b>Insufficient XP to get that ride ... Mine more salt or hunt down more OrX</b></color>");
                    DisableGuiOrXBDArmory();
                }
            }
        }

        private void Vessel02(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);

            if (GUI.Button(saveRect, "TBD 2: 1250 Salt"))
            {
                ScreenMsg("<color=#cc4500ff><b>Sorry, none in stock ... Please come back later</b></color>");
                OrX_SportingGoods.instance.EnableGuiOrXSG();

                /*
                if (KerbinMissions.instance.saltTotal >= 1250)
                {
                    KerbinMissions.instance.saltTotal -= 1250;
                    SpawnOrX_BDarmory.instance.sg02 = true;
                    SpawnOrX_BDarmory.instance.CheckSpawnTimer();
                    OrX_SportingGoods.instance.EnableGuiOrXSG();
                }
                else
                {
                    ScreenMsg("<color=#cc4500ff><b>Insufficient XP to get that ride ... Mine more salt or hunt down more OrX</b></color>");
                    OrX_SportingGoods.instance.EnableGuiOrXSG();
                    OrX_SportingGoods.instance.ToggleGUI();

                }
                */
            }
        }

        private void Vessel03(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);

            if (GUI.Button(saveRect, "TBD 3: 1500 Salt"))
            {
                ScreenMsg("<color=#cc4500ff><b>Sorry, none in stock ... Please come back later</b></color>");
                OrX_SportingGoods.instance.EnableGuiOrXSG();

                /*

                if (KerbinMissions.instance.saltTotal >= 1500)
                {
                    KerbinMissions.instance.saltTotal -= 1500;
                    SpawnOrX_BDarmory.instance.sg03 = true;
                    SpawnOrX_BDarmory.instance.CheckSpawnTimer();
                    OrX_SportingGoods.instance.EnableGuiOrXSG();
                }
                else
                {
                    ScreenMsg("<color=#cc4500ff><b>Insufficient XP to get that ride ... Mine more salt or hunt down more OrX</b></color>");
                    OrX_SportingGoods.instance.EnableGuiOrXSG();
                    OrX_SportingGoods.instance.ToggleGUI();

                }
                */
            }
        }

        private void Vessel04(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);

            if (GUI.Button(saveRect, "TBD 4: 1750 Salt"))
            {
                ScreenMsg("<color=#cc4500ff><b>Sorry, none in stock ... Please come back later</b></color>");
                OrX_SportingGoods.instance.EnableGuiOrXSG();

                /*

                if (KerbinMissions.instance.saltTotal >= 1750)
                {
                    KerbinMissions.instance.saltTotal -= 1750;
                    SpawnOrX_BDarmory.instance.sg04 = true;
                    SpawnOrX_BDarmory.instance.CheckSpawnTimer();
                    OrX_SportingGoods.instance.EnableGuiOrXSG();
                }
                else
                {
                    ScreenMsg("<color=#cc4500ff><b>Insufficient XP to get that ride ... Mine more salt or hunt down more OrX</b></color>");
                    OrX_SportingGoods.instance.EnableGuiOrXSG();
                    OrX_SportingGoods.instance.ToggleGUI();

                }
                */
            }
        }

        private void Vessel05(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);

            if (GUI.Button(saveRect, "TBD 5: 2000 Salt"))
            {
                ScreenMsg("<color=#cc4500ff><b>Sorry, none in stock ... Please come back later</b></color>");
                OrX_SportingGoods.instance.EnableGuiOrXSG();

                /*

                if (KerbinMissions.instance.saltTotal >= 2000)
                {
                    KerbinMissions.instance.saltTotal -= 2000;
                    SpawnOrX_BDarmory.instance.sg05 = true;
                    SpawnOrX_BDarmory.instance.CheckSpawnTimer();
                    OrX_SportingGoods.instance.EnableGuiOrXSG();
                }
                else
                {
                    ScreenMsg("<color=#cc4500ff><b>Insufficient XP to get that ride ... Mine more salt or hunt down more OrX</b></color>");
                    OrX_SportingGoods.instance.EnableGuiOrXSG();
                    OrX_SportingGoods.instance.ToggleGUI();

                }
                */
            }
        }

        private void Vessel06(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);

            if (GUI.Button(saveRect, "TBD 6: 2500 Salt"))
            {
                ScreenMsg("<color=#cc4500ff><b>Sorry, none in stock ... Please come back later</b></color>");
                OrX_SportingGoods.instance.EnableGuiOrXSG();

                /*

                if (KerbinMissions.instance.saltTotal >= 2500)
                {
                    KerbinMissions.instance.saltTotal -= 2500;
                    SpawnOrX_BDarmory.instance.sg06 = true;
                    SpawnOrX_BDarmory.instance.CheckSpawnTimer();
                    OrX_SportingGoods.instance.EnableGuiOrXSG();
                }
                else
                {
                    ScreenMsg("<color=#cc4500ff><b>Insufficient XP to get that ride ... Mine more salt or hunt down more OrX</b></color>");
                    OrX_SportingGoods.instance.EnableGuiOrXSG();
                    OrX_SportingGoods.instance.ToggleGUI();

                }
                */
            }
        }

        private void Vessel07(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);

            if (GUI.Button(saveRect, "TBD 7: 3000 Salt"))
            {
                ScreenMsg("<color=#cc4500ff><b>Sorry, none in stock ... Please come back later</b></color>");
                OrX_SportingGoods.instance.EnableGuiOrXSG();

                /*

                if (KerbinMissions.instance.saltTotal >= 3000)
                {
                    KerbinMissions.instance.saltTotal -= 3000;
                    SpawnOrX_BDarmory.instance.sg07 = true;
                    SpawnOrX_BDarmory.instance.CheckSpawnTimer();
                    OrX_SportingGoods.instance.EnableGuiOrXSG();
                }
                else
                {
                    ScreenMsg("<color=#cc4500ff><b>Insufficient XP to get that ride ... Mine more salt or hunt down more OrX</b></color>");
                    OrX_SportingGoods.instance.EnableGuiOrXSG();
                    OrX_SportingGoods.instance.ToggleGUI();

                }
                */
            }
        }

        private void Vessel08(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);

            if (GUI.Button(saveRect, "TBD 8: 4000 Salt"))
            {
                ScreenMsg("<color=#cc4500ff><b>Sorry, none in stock ... Please come back later</b></color>");
                OrX_SportingGoods.instance.EnableGuiOrXSG();

                /*

                if (KerbinMissions.instance.saltTotal >= 4000)
                {
                    KerbinMissions.instance.saltTotal -= 4000;
                    SpawnOrX_BDarmory.instance.sg08 = true;
                    SpawnOrX_BDarmory.instance.CheckSpawnTimer();
                    OrX_SportingGoods.instance.EnableGuiOrXSG();
                }
                else
                {
                    ScreenMsg("<color=#cc4500ff><b>Insufficient XP to get that ride ... Mine more salt or hunt down more OrX</b></color>");
                    OrX_SportingGoods.instance.EnableGuiOrXSG();
                    OrX_SportingGoods.instance.ToggleGUI();

                }
                */
            }
        }

        private void Vessel09(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);

            if (GUI.Button(saveRect, "TBD 9: 6000 Salt"))
            {
                ScreenMsg("<color=#cc4500ff><b>Sorry, none in stock ... Please come back later</b></color>");
                OrX_SportingGoods.instance.EnableGuiOrXSG();

                /*

                if (KerbinMissions.instance.saltTotal >= 6000)
                {
                    KerbinMissions.instance.saltTotal -= 6000;
                    SpawnOrX_BDarmory.instance.sg09 = true;
                    SpawnOrX_BDarmory.instance.CheckSpawnTimer();
                    OrX_SportingGoods.instance.EnableGuiOrXSG();
                }
                else
                {
                    ScreenMsg("<color=#cc4500ff><b>Insufficient XP to get that ride ... Mine more salt or hunt down more OrX</b></color>");
                    OrX_SportingGoods.instance.EnableGuiOrXSG();
                    OrX_SportingGoods.instance.ToggleGUI();
                }
                */
            }
        }

        private void Vessel10(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);

            if (GUI.Button(saveRect, "TBD 10: 8000 Salt"))
            {
                ScreenMsg("<color=#cc4500ff><b>Sorry, none in stock ... Please come back later</b></color>");
                OrX_SportingGoods.instance.EnableGuiOrXSG();

                /*

                if (KerbinMissions.instance.saltTotal >= 8000)
                {
                    KerbinMissions.instance.saltTotal -= 8000;
                    SpawnOrX_BDarmory.instance.sg10 = true;
                    SpawnOrX_BDarmory.instance.CheckSpawnTimer();
                    OrX_SportingGoods.instance.EnableGuiOrXSG();
                }
                else
                {
                    ScreenMsg("<color=#cc4500ff><b>Insufficient XP to get that ride ... Mine more salt or hunt down more OrX</b></color>");
                    OrX_SportingGoods.instance.EnableGuiOrXSG();
                    OrX_SportingGoods.instance.ToggleGUI();

                }
                */
            }
        }

        private void ScreenMsg(string msg)
        {
            ScreenMessages.PostScreenMessage(new ScreenMessage(msg, 6, ScreenMessageStyle.UPPER_CENTER));
        }
    }
}

