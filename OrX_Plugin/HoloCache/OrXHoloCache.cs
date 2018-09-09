using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UnityEngine;
using KSP.UI.Screens;
using System.Collections;
using OrX.spawn;

namespace OrX
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class OrXHoloCache : MonoBehaviour
    {

        public enum OrXCoords
        {
            Kerbin,
            Mun,
            None
        }

        #region GUI Styles

        //gui styles
        GUIStyle centerLabel;
        GUIStyle centerLabelRed;
        GUIStyle centerLabelOrange;
        GUIStyle centerLabelBlue;
        GUIStyle leftLabel;
        GUIStyle leftLabelRed;
        GUIStyle rightLabelRed;
        GUIStyle leftLabelGray;
        GUIStyle rippleSliderStyle;
        GUIStyle rippleThumbStyle;
        GUIStyle kspTitleLabel;
        GUIStyle middleLeftLabel;
        GUIStyle middleLeftLabelOrange;
        GUIStyle targetModeStyle;
        GUIStyle targetModeStyleSelected;
        GUIStyle redErrorStyle;
        GUIStyle redErrorShadowStyle;

        #endregion

        public static bool showTargets = true;
        public static Rect WindowRectToolbar;
        public static Rect WindowRectGps;
        public static OrXHoloCache instance;
        public static bool GAME_UI_ENABLED;
        public static GUISkin OrXGUISkin = HighLogic.Skin;
        public static bool hasAddedButton = false;
        public static bool OrXGPSEnabled;

        private bool scanning = false;
        public bool spawnHoloCache = false;

        float toolWindowWidth = 250;
        float toolWindowHeight = 100;
        bool showWindowGPS = true;
        bool maySavethisInstance = false;

        private int count = 0;

        float gpsHeight;
        public bool reload;

        private double lat = 0;
        private double lon = 0;
        private double alt = 0;

        public float minLoadRange = 12500;

        float gpsEntryCount;
        float gpsEntryHeight = 24;
        float gpsBorder = 5;
        bool TargetGPS;
        int TargetGPSIndex;
        bool resetTargetGPS;
        string newGPSName = string.Empty;
        bool validGPSName = true;
        public OrXHoloCacheinfo designatedGPSInfo;
        Vessel OrXGPSCoords;

        private static Vector2 _displayViewerPosition = Vector2.zero;
        public Vector3d designatedGPSCoords => designatedGPSInfo.gpsCoordinates;

        Vector3 worldPos;

        private Texture2D redDot;
        public Texture2D HoloTargetTexture
        {
            get { return redDot ? redDot : redDot = GameDatabase.Instance.GetTexture("OrX/Plugin/HoloTarget", false); }
        }

        public bool sth = true;
        public bool hide = false;

        public void Awake()
        {
            if (instance)
            {
                Destroy(instance);
            }
            instance = this;
        }

        void Start()
        {
            OrXGPSEnabled = false;
            GAME_UI_ENABLED = true;
            AddToolbarButton();
            TargetGPS = false;
            spawnHoloCache = false;
            scanning = false;

            //wmgr toolbar
            if (HighLogic.LoadedSceneIsFlight)
                maySavethisInstance = true;     //otherwise later we should NOT save the current window positions!

            // window position settings
            WindowRectToolbar = new Rect((Screen.width / 16) * 2.5f, 140, toolWindowWidth, toolWindowHeight);
            // Default, if not in file.
            WindowRectGps = new Rect(0, 0, WindowRectToolbar.width - 10, 0);

            WindowRectGps.width = WindowRectToolbar.width - 10;

            //setup gui styles
            centerLabel = new GUIStyle();
            centerLabel.alignment = TextAnchor.UpperCenter;
            centerLabel.normal.textColor = Color.white;

            centerLabelRed = new GUIStyle();
            centerLabelRed.alignment = TextAnchor.UpperCenter;
            centerLabelRed.normal.textColor = Color.red;

            centerLabelOrange = new GUIStyle();
            centerLabelOrange.alignment = TextAnchor.UpperCenter;
            centerLabelOrange.normal.textColor = XKCDColors.BloodOrange;

            centerLabelBlue = new GUIStyle();
            centerLabelBlue.alignment = TextAnchor.UpperCenter;
            centerLabelBlue.normal.textColor = XKCDColors.AquaBlue;

            leftLabel = new GUIStyle();
            leftLabel.alignment = TextAnchor.UpperLeft;
            leftLabel.normal.textColor = Color.white;

            middleLeftLabel = new GUIStyle(leftLabel);
            middleLeftLabel.alignment = TextAnchor.MiddleLeft;

            middleLeftLabelOrange = new GUIStyle(middleLeftLabel);
            middleLeftLabelOrange.normal.textColor = XKCDColors.BloodOrange;

            targetModeStyle = new GUIStyle();
            targetModeStyle.alignment = TextAnchor.MiddleRight;
            targetModeStyle.fontSize = 9;
            targetModeStyle.normal.textColor = Color.white;

            targetModeStyleSelected = new GUIStyle(targetModeStyle);
            targetModeStyleSelected.normal.textColor = XKCDColors.BloodOrange;

            leftLabelRed = new GUIStyle();
            leftLabelRed.alignment = TextAnchor.UpperLeft;
            leftLabelRed.normal.textColor = Color.red;

            rightLabelRed = new GUIStyle();
            rightLabelRed.alignment = TextAnchor.UpperRight;
            rightLabelRed.normal.textColor = Color.red;

            leftLabelGray = new GUIStyle();
            leftLabelGray.alignment = TextAnchor.UpperLeft;
            leftLabelGray.normal.textColor = Color.gray;

            rippleSliderStyle = new GUIStyle(OrXGUISkin.horizontalSlider);
            rippleThumbStyle = new GUIStyle(OrXGUISkin.horizontalSliderThumb);
            rippleSliderStyle.fixedHeight = rippleThumbStyle.fixedHeight = 0;

            kspTitleLabel = new GUIStyle();
            kspTitleLabel.normal.textColor = OrXGUISkin.window.normal.textColor;
            kspTitleLabel.font = OrXGUISkin.window.font;
            kspTitleLabel.fontSize = OrXGUISkin.window.fontSize;
            kspTitleLabel.fontStyle = OrXGUISkin.window.fontStyle;
            kspTitleLabel.alignment = TextAnchor.UpperCenter;

            redErrorStyle = new GUIStyle(OrXGUISkin.label);
            redErrorStyle.normal.textColor = Color.red;
            redErrorStyle.fontStyle = FontStyle.Bold;
            redErrorStyle.fontSize = 22;
            redErrorStyle.alignment = TextAnchor.UpperCenter;

            redErrorShadowStyle = new GUIStyle(redErrorStyle);
            redErrorShadowStyle.normal.textColor = new Color(0, 0, 0, 0.75f);
            GameEvents.onHideUI.Add(HideGameUI);
            GameEvents.onShowUI.Add(ShowGameUI);
        }

        void OnGUI()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (!GAME_UI_ENABLED || PauseMenu.isOpen) { return; }

                if (TargetGPS && !MapView.fetch.enabled)
                {
                    DrawTextureOnWorldPos(worldPos, instance.HoloTargetTexture, new Vector2(8, 8));
                }

                if (!OrXGPSEnabled) return;
                WindowRectToolbar = GUI.Window(265227765, WindowRectToolbar, OrXGPS, "OrX HoloCache Locations", OrXGUISkin.window);
                UseMouseEventInRect(WindowRectToolbar);
            }
        }

        public static void UseMouseEventInRect(Rect rect)
        {
            if (MouseIsInRect(rect) && Event.current.isMouse && (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseUp))
            {
                Event.current.Use();
            }
        }

        public static bool MouseIsInRect(Rect rect)
        {
            Vector3 inverseMousePos = new Vector3(Input.mousePosition.x, Screen.height - Input.mousePosition.y, 0);
            return rect.Contains(inverseMousePos);
        }


        #region Target Location Spawning

        private float timer = 2;
        public string craftFile = string.Empty;

        private Vector3d _SpawnCoords()
        {
            return FlightGlobals.ActiveVessel.mainBody.GetWorldSurfacePosition((double)lat, (double)lon, (double)alt);
        }

        public static Camera GetMainCamera()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                return FlightCamera.fetch.mainCamera;
            }
            else
            {
                return Camera.main;
            }
        }

        public static string FormattedGeoPosShort(Vector3d geoPos, bool altitude)
        {
            string finalString = string.Empty;
            //lat
            double lat = geoPos.x;
            double latSign = Math.Sign(lat);
            double latMajor = latSign * Math.Floor(Math.Abs(lat));
            double latMinor = 100 * (Math.Abs(lat) - Math.Abs(latMajor));
            string latString = latMajor.ToString("0") + " " + latMinor.ToString("0");
            finalString += "N:" + latString;


            //longi
            double longi = geoPos.y;
            double longiSign = Math.Sign(longi);
            double longiMajor = longiSign * Math.Floor(Math.Abs(longi));
            double longiMinor = 100 * (Math.Abs(longi) - Math.Abs(longiMajor));
            string longiString = longiMajor.ToString("0") + " " + longiMinor.ToString("0");
            finalString += " E:" + longiString;

            if (altitude)
            {
                finalString += " ASL:" + geoPos.z.ToString("0");
            }

            return finalString;
        }

        public static void DrawTextureOnWorldPos(Vector3 worldPos, Texture texture, Vector2 size)
        {
            Vector3 screenPos = GetMainCamera().WorldToViewportPoint(worldPos);
            if (screenPos.z < 0) return; //dont draw if point is behind camera
            if (screenPos.x != Mathf.Clamp01(screenPos.x)) return; //dont draw if off screen
            if (screenPos.y != Mathf.Clamp01(screenPos.y)) return;
            float xPos = screenPos.x * Screen.width - (0.5f * size.x);
            float yPos = (1 - screenPos.y) * Screen.height - (0.5f * size.y);
            Rect iconRect = new Rect(xPos, yPos, size.x, size.y);

            GUI.DrawTexture(iconRect, texture);
        }

        private void TargetDistance()
        {
            if (scanning)
            {
                StartCoroutine(CheckTargetDistance());
            }
        }

        IEnumerator CheckTargetDistance()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                minLoadRange = FlightGlobals.ActiveVessel.vesselRanges.landed.load;
                if (minLoadRange >= 15000)
                {
                    minLoadRange = 15000;
                }
                else
                {
                    if (minLoadRange <= 2000)
                    {
                        minLoadRange = 1500;
                    }
                }

                yield return new WaitForSeconds(2);

                reloadWorldPos = true;

                double targetDistance = Vector3d.Distance(FlightGlobals.ActiveVessel.GetWorldPos3D(), _SpawnCoords());

                if (targetDistance <= minLoadRange)
                {
                    StartCoroutine(HoloSpawn());
                    scanning = false;
                }

                TargetDistance();
            }
        }

        IEnumerator HoloSpawn()
        {
            if (!spawnHoloCache)
            {
                spawnHoloCache = true;
                yield return new WaitForFixedUpdate();
                SpawnOrX_HoloCache.instance.CheckSpawnTimer();
            }
        }

        private void StartSpawn20000()
        {
            if (FlightGlobals.ActiveVessel.LandedOrSplashed)
            {
                if (FlightGlobals.ActiveVessel.Landed)
                {
                }
                else
                {
                }
            }
            else
            {
            }
        }

        private void StartSpawn40000()
        {
            if (FlightGlobals.ActiveVessel.LandedOrSplashed)
            {
                if (FlightGlobals.ActiveVessel.Landed)
                {
                }
                else
                {
                }
            }
            else
            {
            }
        }

        #endregion

        #region GUI

        private void AddToolbarButton()
        {
            string OrXDir = "OrX/Plugin/";

            if (!hasAddedButton)
            {
                Texture buttonTexture = GameDatabase.Instance.GetTexture(OrXDir + "OrX_HoloCache", false); //texture to use for the button
                ApplicationLauncher.Instance.AddModApplication(ShowGameUI, HideGameUI, Dummy, Dummy, Dummy, Dummy,
                    ApplicationLauncher.AppScenes.FLIGHT, buttonTexture);
                hasAddedButton = true;
            }
        }

        private void ToggleGUI()
        {
            if (OrXGPSEnabled)
            {
                //HideGameUI();
            }
            else
            {
                //ShowGameUI();
            }
        }

        private void Dummy() { }

        void HideGameUI()
        {
            OrXGPSEnabled = false;
        }

        void ShowGameUI()
        {
            worldPos = Vector3d.zero;
            TargetGPS = false;
            OrXGPSEnabled = true;
        }

        void OrXGPS(int windowID)
        {
            GUI.DragWindow(new Rect(30, 0, toolWindowWidth - 90, 30));

            float line = 0;
            float leftIndent = 10;
            float contentWidth = (toolWindowWidth) - (2*leftIndent);
            float contentTop = 10;
            float entryHeight = 20;
            float gpsLines = 0;

            line += 0.6f;

            if (!reload)
            {
                GUI.BeginGroup(new Rect(5, contentTop + (line * entryHeight), toolWindowWidth, WindowRectGps.height));
                WindowGPS();
                GUI.EndGroup();

                if (!hide)
                {
                    gpsLines = WindowRectGps.height / entryHeight;

                    gpsHeight = Mathf.Lerp(gpsHeight, gpsLines, 0.15f);
                    line += gpsHeight;

                    line += 0.25f;

                    if (GUI.Button(new Rect(5, contentTop + (line * entryHeight), toolWindowWidth - 5, 20), "Reload HoloCache Data", OrXGUISkin.button))
                    {
                        TargetGPS = false;
                        reload = true;
                        ScreenMsg("<color=#cc4500ff><b>Loading HoloCache Targets</b></color>");
                        OrXTargetManager.instance.LoadHoloCacheTargets();
                    }
                }
            }
            else
            {
                if (!hide)
                {
                    if (GUI.Button(new Rect(5, contentTop + (line * entryHeight), toolWindowWidth - 5, 20), "HoloCache Data Loading", OrXGUISkin.box))
                    {
                        // do nothing ... reload will turn false after OrXHoloCache is finished loading targets
                    }
                }
            }

            if (!hide)
            {
                line += 1.25f;

                if (!spawnHoloCache)
                {
                    if (GUI.Button(new Rect(5, contentTop + (line * entryHeight), toolWindowWidth - 5, 20), "Spawn Empty HoloCache", OrXGUISkin.button))
                    {
                        spawnHoloCache = true;
                        ScreenMsg("<color=#cc4500ff><b>Spawning HoloCache</b></color>");
                        SpawnOrX_HoloCache.instance.SpawnEmptyHoloCache();
                    }
                }
                else
                {
                    if (GUI.Button(new Rect(5, contentTop + (line * entryHeight), toolWindowWidth - 5, 20), "Spawning HoloCache", OrXGUISkin.box))
                    {
                        // do nothing ... spawnHoloCache will turn false after OrXHoloCache is finished spawning empty HoloCache
                    }
                }
                line += 1F;
            }

            toolWindowHeight = Mathf.Lerp(toolWindowHeight, contentTop + (line*entryHeight) + 5, 1);
            WindowRectToolbar.height = toolWindowHeight;
        }

        private bool reloadWorldPos = false;

        public void WindowGPS()
        {
            GUI.Box(WindowRectGps, GUIContent.none, OrXGUISkin.box);
            gpsEntryCount = 0;
            Rect listRect = new Rect(gpsBorder, gpsBorder, WindowRectGps.width - (2 * gpsBorder),
                WindowRectGps.height - (2 * gpsBorder));
            GUI.BeginGroup(listRect);
            string targetLabel = "SOI: " + FlightGlobals.ActiveVessel.mainBody.name;
            GUI.Label(new Rect(0, 0, listRect.width, gpsEntryHeight), targetLabel, kspTitleLabel);

            // Expand/Collapse Target Toggle button
            if (GUI.Button(new Rect(listRect.width - (gpsEntryHeight * 2), 0, gpsEntryHeight, gpsEntryHeight), showTargets ? "-" : "+", OrXGUISkin.button))
                showTargets = !showTargets;

            gpsEntryCount += 1.2f;
            int indexToRemove = -1;
            int index = 0;

            if (showTargets)
            {
                List<OrXHoloCacheinfo>.Enumerator coordinate = OrXTargetManager.HoloCacheTargets[OrXCoords.Kerbin].GetEnumerator();
                while (coordinate.MoveNext())
                {
                    Color origWColor = GUI.color;

                    string label = FormattedGeoPosShort(coordinate.Current.gpsCoordinates, false);
                    float nameWidth = 120;
                    if (scanning)
                    {
                        if (TargetGPS)
                        {
                            if (index == TargetGPSIndex)
                            {
                                if (reloadWorldPos)
                                {
                                    reloadWorldPos = false;
                                    worldPos = coordinate.Current.worldPos;
                                }

                                if (!reload && !hide)
                                {
                                    if (GUI.Button(new Rect(0, gpsEntryCount * gpsEntryHeight, nameWidth, gpsEntryHeight), coordinate.Current.name, OrXGUISkin.box))
                                    {
                                        worldPos = Vector3d.zero;
                                        TargetGPS = false;
                                        reload = true;
                                        ScreenMsg("<color=#cc4500ff><b>Reloading HoloCache Targets</b></color>");
                                        OrXTargetManager.instance.LoadHoloCacheTargets();
                                        scanning = false;
                                    }

                                    if (GUI.Button(new Rect(nameWidth, gpsEntryCount * gpsEntryHeight, listRect.width - gpsEntryHeight - nameWidth, gpsEntryHeight), label, OrXGUISkin.box))
                                    {
                                        worldPos = Vector3d.zero;
                                        reload = true;
                                        ScreenMsg("<color=#cc4500ff><b>Reloading HoloCache Targets</b></color>");
                                        OrXTargetManager.instance.LoadHoloCacheTargets();
                                        TargetGPS = false;
                                        scanning = false;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (TargetGPS)
                        {
                            if (index == TargetGPSIndex)
                            {
                                if (reloadWorldPos)
                                {
                                    reloadWorldPos = false;
                                    worldPos = coordinate.Current.worldPos;
                                }


                                if (!hide)
                                {
                                    if (!reload)
                                    {
                                        if (GUI.Button(new Rect(0, gpsEntryCount * gpsEntryHeight, nameWidth, gpsEntryHeight), coordinate.Current.name, OrXGUISkin.box))
                                        {
                                            worldPos = Vector3d.zero;
                                            TargetGPS = false;
                                            reload = true;
                                            ScreenMsg("<color=#cc4500ff><b>Reloading HoloCache Targets</b></color>");
                                            OrXTargetManager.instance.LoadHoloCacheTargets();
                                            scanning = false;
                                        }

                                        if (GUI.Button(new Rect(nameWidth, gpsEntryCount * gpsEntryHeight, listRect.width - gpsEntryHeight - nameWidth, gpsEntryHeight), label, OrXGUISkin.box))
                                        {
                                            worldPos = Vector3d.zero;
                                            reload = true;
                                            ScreenMsg("<color=#cc4500ff><b>Reloading HoloCache Targets</b></color>");
                                            OrXTargetManager.instance.LoadHoloCacheTargets();
                                            TargetGPS = false;
                                            scanning = false;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (!hide)
                                {
                                    if (GUI.Button(new Rect(0, gpsEntryCount * gpsEntryHeight, nameWidth, gpsEntryHeight), coordinate.Current.name, OrXGUISkin.button))
                                    {
                                        if (HighLogic.LoadedSceneIsFlight)
                                        {
                                            worldPos = coordinate.Current.worldPos;

                                            lat = coordinate.Current.gpsCoordinates.x;
                                            lon = coordinate.Current.gpsCoordinates.y;
                                            alt = coordinate.Current.gpsCoordinates.z;

                                            SpawnOrX_HoloCache.instance.HoloCacheName = coordinate.Current.name;
                                            SpawnOrX_HoloCache.instance.craftFile = coordinate.Current.name;
                                            SpawnOrX_HoloCache.instance._lat = lat;
                                            SpawnOrX_HoloCache.instance._lon = lon;
                                            SpawnOrX_HoloCache.instance._alt = alt;

                                            TargetGPSIndex = index;
                                            TargetGPS = true;
                                            resetTargetGPS = false;
                                            scanning = true;
                                            craftFile = coordinate.Current.name;

                                            TargetDistance();
                                        }
                                    }

                                    if (GUI.Button(new Rect(nameWidth, gpsEntryCount * gpsEntryHeight, listRect.width - gpsEntryHeight - nameWidth, gpsEntryHeight), label, OrXGUISkin.button))
                                    {
                                        if (HighLogic.LoadedSceneIsFlight)
                                        {
                                            lat = coordinate.Current.gpsCoordinates.x;
                                            lon = coordinate.Current.gpsCoordinates.y;
                                            alt = coordinate.Current.gpsCoordinates.z;
                                            worldPos = coordinate.Current.worldPos;

                                            SpawnOrX_HoloCache.instance.HoloCacheName = coordinate.Current.name;
                                            SpawnOrX_HoloCache.instance.craftFile = coordinate.Current.name;
                                            SpawnOrX_HoloCache.instance._lat = lat;
                                            SpawnOrX_HoloCache.instance._lon = lon;
                                            SpawnOrX_HoloCache.instance._alt = alt;

                                            TargetGPSIndex = index;
                                            TargetGPS = true;
                                            resetTargetGPS = false;
                                            scanning = true;
                                            craftFile = coordinate.Current.name;

                                            TargetDistance();
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!hide)
                            {
                                if (GUI.Button(new Rect(0, gpsEntryCount * gpsEntryHeight, nameWidth, gpsEntryHeight), coordinate.Current.name, OrXGUISkin.button))
                                {
                                    if (HighLogic.LoadedSceneIsFlight)
                                    {
                                        worldPos = coordinate.Current.worldPos;

                                        lat = coordinate.Current.gpsCoordinates.x;
                                        lon = coordinate.Current.gpsCoordinates.y;
                                        alt = coordinate.Current.gpsCoordinates.z;

                                        SpawnOrX_HoloCache.instance.HoloCacheName = coordinate.Current.name;
                                        SpawnOrX_HoloCache.instance.craftFile = coordinate.Current.name;
                                        SpawnOrX_HoloCache.instance._lat = lat;
                                        SpawnOrX_HoloCache.instance._lon = lon;
                                        SpawnOrX_HoloCache.instance._alt = alt;

                                        TargetGPSIndex = index;
                                        TargetGPS = true;
                                        resetTargetGPS = false;
                                        scanning = true;
                                        craftFile = coordinate.Current.name;

                                        TargetDistance();
                                    }
                                }

                                if (GUI.Button(new Rect(nameWidth, gpsEntryCount * gpsEntryHeight, listRect.width - gpsEntryHeight - nameWidth, gpsEntryHeight), label, OrXGUISkin.button))
                                {
                                    if (HighLogic.LoadedSceneIsFlight)
                                    {
                                        lat = coordinate.Current.gpsCoordinates.x;
                                        lon = coordinate.Current.gpsCoordinates.y;
                                        alt = coordinate.Current.gpsCoordinates.z;
                                        worldPos = coordinate.Current.worldPos;

                                        SpawnOrX_HoloCache.instance.HoloCacheName = coordinate.Current.name;
                                        SpawnOrX_HoloCache.instance.craftFile = coordinate.Current.name;
                                        SpawnOrX_HoloCache.instance._lat = lat;
                                        SpawnOrX_HoloCache.instance._lon = lon;
                                        SpawnOrX_HoloCache.instance._alt = alt;

                                        TargetGPSIndex = index;
                                        TargetGPS = true;
                                        resetTargetGPS = false;
                                        scanning = true;
                                        craftFile = coordinate.Current.name;

                                        TargetDistance();
                                    }
                                }
                            }
                        }
                    }

                    gpsEntryCount++;
                    index++;
                    GUI.color = origWColor;
                }
                coordinate.Dispose();
            }

            if (resetTargetGPS)
            {
                scanning = false;
                resetTargetGPS = false;
                TargetGPSIndex = 0;
            }

            GUI.EndGroup();
            WindowRectGps.height = (2 * gpsBorder) + (gpsEntryCount * gpsEntryHeight);
        }

        #endregion

        internal void OnDestroy()
        {
            GameEvents.onHideUI.Remove(HideGameUI);
            GameEvents.onShowUI.Remove(ShowGameUI);
        }

        private void ScreenMsg(string msg)
        {
            ScreenMessages.PostScreenMessage(new ScreenMessage(msg, 5, ScreenMessageStyle.UPPER_CENTER));
        }

    }
}