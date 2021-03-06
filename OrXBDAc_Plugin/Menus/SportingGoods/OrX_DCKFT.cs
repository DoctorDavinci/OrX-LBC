﻿using UnityEngine;
using OrXBDAc.missions;

namespace OrXBDAc.sportingGoods
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class OrX_DCKFT : MonoBehaviour
    {

        /// <summary>
        /// /////////////////////////
        /// </summary>
        public static OrX_DCKFT instance;

        private const float WindowWidth = 200;
        private const float DraggableHeight = 40;
        private const float LeftIndent = 12;
        private const float ContentTop = 20;
        public static bool GuiEnabledOrXDCKFutureTech;
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
            // Add vessels

            if (instance) Destroy(instance);
            instance = this;
        }

        private void Start()
        {
            _windowRect = new Rect((Screen.width / 4) * 3 - WindowWidth - 20, 140, WindowWidth, _windowHeight);
            GameEvents.onHideUI.Add(GameUiDisableOrXDCKFutureTech);
            GameEvents.onShowUI.Add(GameUiEnableOrXDCKFutureTech);
            _gameUiToggle = true;
        }

        private void OnGUI()
        {
            if (GuiEnabledOrXDCKFutureTech && _gameUiToggle)
            {
                _windowRect = GUI.Window(328799391, _windowRect, GuiWindowOrXControl, "");
            }
        }

        public void ToggleGUI()
        {
            if (GuiEnabledOrXDCKFutureTech)
            {
                GuiEnabledOrXDCKFutureTech = false;
            }
            else
            {
                GuiEnabledOrXDCKFutureTech = true;
            }
        }

        public void EnableGuiOrXDCKFutureTech()
        {
            GuiEnabledOrXDCKFutureTech = true;
        }

        public void DisableGuiOrXDCKFutureTech()
        {
            if (GuiEnabledOrXDCKFutureTech)
            {
                GuiEnabledOrXDCKFutureTech = false;
                ScreenMsg("<color=#cc4500ff><b>Shop Smart ... Shop S-Mart</b></color>");
            }
        }

        private void GameUiEnableOrXDCKFutureTech()
        {
            _gameUiToggle = true;
        }

        private void GameUiDisableOrXDCKFutureTech()
        {
            _gameUiToggle = false;
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
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };
            GUI.Label(new Rect(0, 0, WindowWidth, 20), "DCK FutureTech", titleStyle);
        }

        private void GuiWindowOrXControl(int OrX_DCKFT)
        {
            GUI.DragWindow(new Rect(0, 0, WindowWidth, DraggableHeight));
            float line = 0;
            _contentWidth = WindowWidth - 2 * LeftIndent;

            DrawOrXControlTitle(line);
            DrawSelectVessel(line);
            line++;
            Vessel01(line);
            line++;
            Vessel02(line);
            line++;
            Vessel03(line);
            line++;
            Vessel04(line);
            line++;
            Vessel05(line);
            line++;
            Vessel06(line);
            line++;
            Vessel07(line);
            line++;
            Vessel08(line);
            line++;
            Vessel09(line);
            line++;
            Vessel10(line);

            _windowHeight = ContentTop + line * entryHeight + entryHeight + (entryHeight / 2);
            _windowRect.height = _windowHeight;
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

            if (GUI.Button(saveRect, "FT 1: 1000 Salt"))
            {
                ScreenMsg("<color=#cc4500ff><b>Sorry, none in stock ... Please come back later</b></color>");
                OrX_SportingGoods.instance.EnableGuiOrXSG();
                /*
                if (KerbinMissions.instance.saltTotal >= 1000)
                {
                    //KerbinMissions.instance.saltTotal -= 1000;
                    SpawnOrX_DCKFT.instance.sg01 = true;
                    SpawnOrX_DCKFT.instance.CheckSpawnTimer();
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

        private void Vessel02(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);

            if (GUI.Button(saveRect, "FT 2: 1250 Salt"))
            {
                ScreenMsg("<color=#cc4500ff><b>Sorry, none in stock ... Please come back later</b></color>");
                OrX_SportingGoods.instance.EnableGuiOrXSG();
                /*

                if (KerbinMissions.instance.saltTotal >= 1250)
                {
                    ////KerbinMissions.instance.saltTotal -= 1250;
                    SpawnOrX_DCKFT.instance.sg02 = true;
                    SpawnOrX_DCKFT.instance.CheckSpawnTimer();
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

            if (GUI.Button(saveRect, "FT 3: 1500 Salt"))
            {
                ScreenMsg("<color=#cc4500ff><b>Sorry, none in stock ... Please come back later</b></color>");
                OrX_SportingGoods.instance.EnableGuiOrXSG();
                /*

                if (KerbinMissions.instance.saltTotal >= 1500)
                {
                    ////KerbinMissions.instance.saltTotal -= 1500;
                    SpawnOrX_DCKFT.instance.sg03 = true;
                    SpawnOrX_DCKFT.instance.CheckSpawnTimer();
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

            if (GUI.Button(saveRect, "FT 4: 1750 Salt"))
            {
                ScreenMsg("<color=#cc4500ff><b>Sorry, none in stock ... Please come back later</b></color>");
                OrX_SportingGoods.instance.EnableGuiOrXSG();
                /*

                if (KerbinMissions.instance.saltTotal >= 1750)
                {
                    ////KerbinMissions.instance.saltTotal -= 1750;
                    SpawnOrX_DCKFT.instance.sg04 = true;
                    SpawnOrX_DCKFT.instance.CheckSpawnTimer();
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

            if (GUI.Button(saveRect, "FT 5: 2000 Salt"))
            {
                ScreenMsg("<color=#cc4500ff><b>Sorry, none in stock ... Please come back later</b></color>");
                OrX_SportingGoods.instance.EnableGuiOrXSG();
                /*

                if (KerbinMissions.instance.saltTotal >= 2000)
                {
                    ////KerbinMissions.instance.saltTotal -= 2000;
                    SpawnOrX_DCKFT.instance.sg05 = true;
                    SpawnOrX_DCKFT.instance.CheckSpawnTimer();
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

            if (GUI.Button(saveRect, "FT 6: 2500 Salt"))
            {
                ScreenMsg("<color=#cc4500ff><b>Sorry, none in stock ... Please come back later</b></color>");
                OrX_SportingGoods.instance.EnableGuiOrXSG();
                /*

                if (KerbinMissions.instance.saltTotal >= 2500)
                {
                    ////KerbinMissions.instance.saltTotal -= 2500;
                    SpawnOrX_DCKFT.instance.sg06 = true;
                    SpawnOrX_DCKFT.instance.CheckSpawnTimer();
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

            if (GUI.Button(saveRect, "FT 7: 3000 Salt"))
            {
                ScreenMsg("<color=#cc4500ff><b>Sorry, none in stock ... Please come back later</b></color>");
                OrX_SportingGoods.instance.EnableGuiOrXSG();
                /*

                if (KerbinMissions.instance.saltTotal >= 3000)
                {
                    ////KerbinMissions.instance.saltTotal -= 3000;
                    SpawnOrX_DCKFT.instance.sg07 = true;
                    SpawnOrX_DCKFT.instance.CheckSpawnTimer();
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

            if (GUI.Button(saveRect, "FT 8: 4000 Salt"))
            {
                ScreenMsg("<color=#cc4500ff><b>Sorry, none in stock ... Please come back later</b></color>");
                OrX_SportingGoods.instance.EnableGuiOrXSG();
                /*

                if (KerbinMissions.instance.saltTotal >= 4000)
                {
                    ////KerbinMissions.instance.saltTotal -= 4000;
                    SpawnOrX_DCKFT.instance.sg08 = true;
                    SpawnOrX_DCKFT.instance.CheckSpawnTimer();
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

            if (GUI.Button(saveRect, "FT 9: 6000 Salt"))
            {
                ScreenMsg("<color=#cc4500ff><b>Sorry, none in stock ... Please come back later</b></color>");
                OrX_SportingGoods.instance.EnableGuiOrXSG();
                /*

                if (KerbinMissions.instance.saltTotal >= 6000)
                {
                    ////KerbinMissions.instance.saltTotal -= 6000;
                    SpawnOrX_DCKFT.instance.sg09 = true;
                    SpawnOrX_DCKFT.instance.CheckSpawnTimer();
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

            if (GUI.Button(saveRect, "FT 10: 8000 Salt"))
            {
                ScreenMsg("<color=#cc4500ff><b>Sorry, none in stock ... Please come back later</b></color>");
                OrX_SportingGoods.instance.EnableGuiOrXSG();
                /*

                if (KerbinMissions.instance.saltTotal >= 8000)
                {
                    ////KerbinMissions.instance.saltTotal -= 8000;
                    SpawnOrX_DCKFT.instance.sg10 = true;
                    SpawnOrX_DCKFT.instance.CheckSpawnTimer();
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

