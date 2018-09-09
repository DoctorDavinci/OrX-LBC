using KSP.UI.Screens;
using UnityEngine;
using System.Collections;
using OrXBDAc.parts;
using OrXBDAc.missions;
using System.Collections.Generic;

namespace OrXBDAc.missions
{
    [KSPAddon(KSPAddon.Startup.AllGameScenes, true)]
    public class MissionLauncher : MonoBehaviour
    {
        private const float WindowWidth = 220;
        private const float DraggableHeight = 40;
        private const float LeftIndent = 12;
        private const float ContentTop = 20;
        public static MissionLauncher instance;
        public static bool GuiEnabledOrXML;
        public static bool GuiEnabledOrXMLNT;
        public static bool HasAddedButton;
        private readonly float _incrButtonWidth = 26;
        private readonly float contentWidth = WindowWidth - 2 * LeftIndent;
        private readonly float entryHeight = 20;
        private float _contentWidth;
        private bool _gameUiToggle;
        private float _windowHeight = 250;
        private Rect _windowRect;

        /// /////////////////////////////////////////////////////////////////////////////

        public bool guiActive;
        public bool ironKerbal;
        public bool launchSiteChanged = false;
        public bool guiOpen = false;
        private static bool tardisLaunch = false;
        private bool pauseCheck = true;
        public bool distanceCheck = true;
        public bool editorLaunch = false;

        private void Awake()
        {
            DontDestroyOnLoad(this);
            instance = this;
        }

        private void Start()
        {
            _windowRect = new Rect((Screen.width / 2) - (WindowWidth / 2), 250, WindowWidth, _windowHeight);
            GameEvents.onHideUI.Add(GameUiDisableOrXML);
            GameEvents.onShowUI.Add(GameUiEnableOrXML);
            AddToolbarButton();
            _gameUiToggle = true;
        }

        private void OnGUI()
        {
            if (HighLogic.LoadedSceneIsEditor)
            {
                if (GuiEnabledOrXML)
                {
                    _windowRect = GUI.Window(52273316, _windowRect, GuiWindowOrXMLEditor, "");
                }

                if (GuiEnabledOrXMLNT)
                {
                    _windowRect = GUI.Window(23973316, _windowRect, GuiWindowOrXMLFlightNT, "");
                }
            }

            if (HighLogic.LoadedSceneIsFlight)
            {
                if (GuiEnabledOrXML)
                {
                    _windowRect = GUI.Window(621143316, _windowRect, GuiWindowOrXMLFlight, "");
                }

                
                if(GuiEnabledOrXMLNT)
                {
                    _windowRect = GUI.Window(21673316, _windowRect, GuiWindowOrXMLFlightNT, "");
                }
                
            }
        }

        public void Update()
        {
            if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ready)
            {
                if (pauseCheck)
                {
                    if (PauseMenu.isOpen || Time.timeScale == 0)
                    {
                        pauseCheck = false;
                        if (guiOpen)
                        {
                            DisableGui();
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
                        if (guiOpen)
                        {
                            DisableGui();
                        }
                    }
                }
            }
        }

        /// /////////////////////////////////////////////////////////////////////////////

        #region Coords

        // Runway: lat -0.0485890418349364, long 285.276094692895, alt 71.9665353324963
        // Beach: lat -0.039751185006272, long 285.639486693549, alt 1.68487426708452
        // Beach by Island: lat -1.53556797173857, long 287.956960620886, alt 1.56112247915007

        private double latitude = 0;
        private double longitude = 0;

        private double lat = 0;
        private double lon = 0;

        [KSPField(isPersistant = true)]
        public bool Survival = false;

        [KSPField(isPersistant = true)]
        public bool KSC = false;
        public double latKSC = -0.093354877488929;
        public double lonKSC = -74.6521799214026;

        [KSPField(isPersistant = true)]
        public bool WaldosIsland = false;
        public double latWaldosIsland = -1.51812886210386;
        public double lonWaldosIsland = -71.96798623656;

        [KSPField(isPersistant = true)]
        public bool Baikerbanur = false;
        public double latBaikerbanur = 20.6508271202407;
        public double lonBaikerbanur = -146.425097659734;

        [KSPField(isPersistant = true)]
        public bool Pyramids = false;
        public double latPyramids = -6.49869308429184;
        public double lonPyramids = -141.679184195229;

        public double altitude = 200;
        public double alt = 1;
        public float altAdjust = 2;

        #endregion

        /// /////////////////////////////////////////////////////////////////////////////

        #region Launch Tardis

        public float alt01 = 5000;
        public float alt02 = 20000;
        public float alt03 = 40000;

        public float alt04 = 5000;
        public float alt05 = 5000;

        public Vector3d _SpawnCoords()
        {
            return FlightGlobals.ActiveVessel.mainBody.GetWorldSurfacePosition((double)lat, (double)lon, (double)alt);
        }

        public Vector3d LaunchPosition()
        {
            return FlightGlobals.ActiveVessel.mainBody.GetWorldSurfacePosition((double)latitude, (double)longitude, (double)altitude);
        }

        public void LaunchSurvival()
        {
            WaldosIsland = false;
            KSC = false;
            Pyramids = false;
            Baikerbanur = false;

            OrX_Log.instance.survival = true;
            KerbinMissions.instance.ResetKM();
            OrXVesselSwitcher.instance.missionPaused = true;
            Survival = true;
            StartCoroutine(MissionDelay());
        }

        public void LaunchToKSC()
        {
            lat = latKSC;
            lon = lonKSC;
            alt = 72;

            WaldosIsland = false;
            Survival = false;
            Pyramids = false;
            Baikerbanur = false;

            KerbinMissions.instance.ResetKM();
            OrXVesselSwitcher.instance.missionPaused = true;
            KSC = true;
            StartCoroutine(Launch());
        }

        public void LaunchToWaldosIsland()
        {
            alt = 138;
            lat = latWaldosIsland;
            lon = lonWaldosIsland;

            KSC = false;
            Survival = false;
            Pyramids = false;
            Baikerbanur = false;

            KerbinMissions.instance.ResetKM();
            OrXVesselSwitcher.instance.missionPaused = true;
            WaldosIsland = true;
            StartCoroutine(Launch());
        }

        public void LaunchToBaikerbanur()
        {
            KSC = false;
            Survival = false;
            Pyramids = false;
            WaldosIsland = false;

            KerbinMissions.instance.ResetKM();
            OrXVesselSwitcher.instance.missionPaused = true;
            Baikerbanur = true;
            alt = 430;
            lat = latBaikerbanur;
            lon = lonBaikerbanur;
            StartCoroutine(Launch());
        }

        public void LaunchToPyramids()
        {
            KSC = false;
            Survival = false;
            Baikerbanur = false;
            WaldosIsland = false;

            KerbinMissions.instance.ResetKM();
            OrXVesselSwitcher.instance.missionPaused = true;
            Pyramids = true;
            alt = 1304;
            lat = latPyramids;
            lon = lonPyramids;
            StartCoroutine(Launch());
        }

        IEnumerator Launch()
        {
            double targetDistance = Vector3d.Distance(FlightGlobals.ActiveVessel.GetWorldPos3D(), _SpawnCoords());

            if (targetDistance >= 150000 || editorLaunch)
            {
                editorLaunch = false;
                OrX_Log.instance.PlayOrders();
                List<Vessel>.Enumerator v = FlightGlobals.Vessels.GetEnumerator();
                while (v.MoveNext())
                {
                    if (v.Current == null) continue;
                    if (!v.Current.loaded || v.Current.packed) continue;

                    var tardis = v.Current.FindPartModuleImplementing<ModuleOrXTardis>();
                    if (!v.Current.isActiveVessel && tardis == null)
                    {
                        v.Current.rootPart.AddModule("ModuleOrXDestroyVessel", true);
                    }
                }
                v.Dispose();

                yield return new WaitForSeconds(2);

                FlightGlobals.ActiveVessel.GetComponent<Rigidbody>().isKinematic = true;

                latitude = FlightGlobals.ActiveVessel.latitude;
                longitude = FlightGlobals.ActiveVessel.longitude;
                altitude = alt01;
                FlightGlobals.ActiveVessel.geeForce = 0;
                FlightGlobals.ActiveVessel.geeForce_immediate = 0;
                FlightGlobals.ActiveVessel.SetPosition(LaunchPosition(), true);
                FlightGlobals.ActiveVessel.GetComponent<Rigidbody>().velocity = Vector3.zero;
                FlightGlobals.ActiveVessel.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                FlightGlobals.ActiveVessel.geeForce = 0;
                FlightGlobals.ActiveVessel.geeForce_immediate = 0;
                yield return new WaitForFixedUpdate();

                latitude = lat / 2 * -1;
                longitude = lon / 2 * -1;
                altitude = alt02;
                FlightGlobals.ActiveVessel.geeForce = 0;
                FlightGlobals.ActiveVessel.geeForce_immediate = 0;
                FlightGlobals.ActiveVessel.SetPosition(LaunchPosition(), true);
                FlightGlobals.ActiveVessel.GetComponent<Rigidbody>().velocity = Vector3.zero;
                FlightGlobals.ActiveVessel.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                FlightGlobals.ActiveVessel.geeForce = 0;
                FlightGlobals.ActiveVessel.geeForce_immediate = 0;
                yield return new WaitForFixedUpdate();

                latitude = 0;
                longitude = 0;
                altitude = alt03;
                FlightGlobals.ActiveVessel.geeForce = 0;
                FlightGlobals.ActiveVessel.geeForce_immediate = 0;
                FlightGlobals.ActiveVessel.SetPosition(LaunchPosition(), true);
                FlightGlobals.ActiveVessel.GetComponent<Rigidbody>().velocity = Vector3.zero;
                FlightGlobals.ActiveVessel.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                FlightGlobals.ActiveVessel.geeForce = 0;
                FlightGlobals.ActiveVessel.geeForce_immediate = 0;
                yield return new WaitForFixedUpdate();

                latitude = lat / 2;
                longitude = lon / 2;
                altitude = alt02;
                FlightGlobals.ActiveVessel.geeForce = 0;
                FlightGlobals.ActiveVessel.geeForce_immediate = 0;
                FlightGlobals.ActiveVessel.SetPosition(LaunchPosition(), true);
                FlightGlobals.ActiveVessel.GetComponent<Rigidbody>().velocity = Vector3.zero;
                FlightGlobals.ActiveVessel.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                FlightGlobals.ActiveVessel.geeForce = 0;
                FlightGlobals.ActiveVessel.geeForce_immediate = 0;
                yield return new WaitForFixedUpdate();

                latitude = lat;
                longitude = lon;
                FlightGlobals.ActiveVessel.geeForce = 0;
                FlightGlobals.ActiveVessel.geeForce_immediate = 0;
                altitude = alt + altAdjust;
                FlightGlobals.ActiveVessel.SetPosition(LaunchPosition(), true);
                FlightGlobals.ActiveVessel.GetComponent<Rigidbody>().velocity = Vector3.zero;
                FlightGlobals.ActiveVessel.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                FlightGlobals.ActiveVessel.geeForce = 0;
                FlightGlobals.ActiveVessel.geeForce_immediate = 0;
                FlightGlobals.ActiveVessel.GetComponent<Rigidbody>().isKinematic = false;
                yield return new WaitForFixedUpdate();
                StartCoroutine(MissionDelay());
                lat = 0;
                lon = 0;
                alt = 0;
            }
            else
            {
                ScreenMsg("<color=#cc4500ff><b>Selected mission is too close to your current position</b></color>");
                ScreenMsg("<color=#cc4500ff><b>Please select another destination</b></color>");
                EnableGui();
            }
        }

        IEnumerator LaunchToResetLoc()
        {
            List<Vessel>.Enumerator v = FlightGlobals.Vessels.GetEnumerator();
            while (v.MoveNext())
            {
                if (v.Current == null) continue;
                if (!v.Current.loaded || v.Current.packed) continue;
                if (!v.Current.isActiveVessel && !v.Current.vesselName.Contains("Tardis"))
                {
                    v.Current.Die();
                }
            }
            v.Dispose();

            FlightGlobals.ActiveVessel.GetComponent<Rigidbody>().isKinematic = true;

            latitude = FlightGlobals.ActiveVessel.latitude;
            longitude = FlightGlobals.ActiveVessel.longitude;
            altitude = 500000;
            FlightGlobals.ActiveVessel.geeForce = 0;
            FlightGlobals.ActiveVessel.geeForce_immediate = 0;
            FlightGlobals.ActiveVessel.SetPosition(LaunchPosition(), true);
            FlightGlobals.ActiveVessel.GetComponent<Rigidbody>().velocity = Vector3.zero;
            FlightGlobals.ActiveVessel.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            FlightGlobals.ActiveVessel.geeForce = 0;
            FlightGlobals.ActiveVessel.geeForce_immediate = 0;
            yield return new WaitForFixedUpdate();
            FlightGlobals.ActiveVessel.GetComponent<Rigidbody>().isKinematic = false;

            yield return new WaitForSeconds(10);
            distanceCheck = false;

            var tardis = FlightGlobals.ActiveVessel.FindPartModuleImplementing<ModuleOrXTardis>();
            if (tardis != null)
            {
                tardis.StartTravel();
            }
            else
            {

            }
        }

        IEnumerator MissionDelay()
        {
            if (FlightGlobals.ActiveVessel.LandedOrSplashed)
            {
                OrX_Controls.instance.OpenGUI();

                yield return new WaitForSeconds(15);

                if (Survival)
                {
                    KerbinMissions.instance.SurvivalMode();
                }

                if (KSC)
                {
                    KerbinMissions.instance.LootBoxControversy();
                }

                if (WaldosIsland)
                {
                    KerbinMissions.instance.WaldosIsland();
                }

                if (Pyramids)
                {
                    KerbinMissions.instance.TutenKermanUldum();
                }

                if (Baikerbanur)
                {
                    KerbinMissions.instance.KillerTomatoes();
                }

                ResetMissions();
            }
            else
            {
                yield return new WaitForSeconds(5);
                StartCoroutine(MissionDelay());
            }
        }

        public void ResetMissions()
        {
            Survival = false;
            launchSiteChanged = false;
            Pyramids = false;
            WaldosIsland = false;
            KSC = false;
            Baikerbanur = false;

            if (HighLogic.LoadedSceneIsEditor)
            {
                var tardis = EditorLogic.RootPart.FindModuleImplementing<ModuleOrXTardis>();
                if (tardis != null)
                {
                    tardis.Survival = false;
                    tardis.WaldosIsland = false;
                    tardis.Baikerbanur = false;
                    tardis.KSC = false;
                    tardis.Pyramids = false;
                    tardis.launchSiteChanged = false;
                    tardis.triggered = false;
                }
            }

            if (HighLogic.LoadedSceneIsFlight)
            {
                var tardis = FlightGlobals.ActiveVessel.FindPartModuleImplementing<ModuleOrXTardis>();
                if (tardis != null)
                {
                    tardis.Survival = false;
                    tardis.WaldosIsland = false;
                    tardis.Baikerbanur = false;
                    tardis.KSC = false;
                    tardis.Pyramids = false;
                    tardis.launchSiteChanged = false;
                    tardis.triggered = false;
                }
            }
        }

        #endregion

        /// /////////////////////////////////////////////////////////////////////////////

        #region Missions GUI

        private void GuiWindowOrXMLFlightNT(int OrXMLFNT)
        {
            GUI.DragWindow(new Rect(0, 0, WindowWidth, DraggableHeight));
            float line = 0;
            _contentWidth = WindowWidth - 2 * LeftIndent;

            DrawTitle(line);
            line++;
            DrawIronKerbal(line);

            _windowHeight = ContentTop + line * entryHeight + entryHeight + (entryHeight / 2);
            _windowRect.height = _windowHeight;
        }

        private void GuiWindowOrXMLFlight(int OrXMLF)
        {
            GUI.DragWindow(new Rect(0, 0, WindowWidth, DraggableHeight));
            float line = 0;
            _contentWidth = WindowWidth - 2 * LeftIndent;

            DrawTitle(line);
            line++;
            DrawIronKerbal(line);
            line++;
            DrawKSC(line);
            line++;
            DrawWaldosIsland(line);
            line++;
            DrawBaikerbanur(line);
            line++;
            DrawPyramids(line);

            _windowHeight = ContentTop + line * entryHeight + entryHeight + (entryHeight / 2);
            _windowRect.height = _windowHeight;
        }

        private void GuiWindowOrXMLEditor(int OrXMLE)
        {
            GUI.DragWindow(new Rect(0, 0, WindowWidth, DraggableHeight));
            float line = 0;
            _contentWidth = WindowWidth - 2 * LeftIndent;

            DrawTitle(line);
            line++;
            DrawKSC(line);
            line++;
            DrawWaldosIsland(line);
            line++;
            DrawBaikerbanur(line);
            line++;
            DrawPyramids(line);

            _windowHeight = ContentTop + line * entryHeight + entryHeight + (entryHeight / 2);
            _windowRect.height = _windowHeight;
        }

        private void AddToolbarButton()
        {
            string textureDir = "OrX/Plugin/";

            if (!HasAddedButton)
            {
                Texture buttonTexture = GameDatabase.Instance.GetTexture(textureDir + "OrXSMSA_normal", false); //texture to use for the button
                ApplicationLauncher.Instance.AddModApplication(EnableGui, DisableGui, Dummy, Dummy, Dummy, Dummy,
                    ApplicationLauncher.AppScenes.SPH | ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.FLIGHT, buttonTexture);
                HasAddedButton = true;
            }
        }

        public void EnableGui()
        {
            KerbinMissions.instance.KillMission();

            if (HighLogic.LoadedSceneIsEditor)
            {
                var tardis = EditorLogic.RootPart.FindModuleImplementing<ModuleOrXTardis>();
                if (tardis != null)
                {
                    GuiEnabledOrXMLNT = false;
                    tardisLaunch = true;
                    guiOpen = true;
                    GuiEnabledOrXML = true;
                    Debug.Log("[OrX Mission Launch]: Showing Tardis GUI");
                    ResetMissions();
                }
                else
                {
                    GuiEnabledOrXMLNT = true;
                    tardisLaunch = false;
                    guiOpen = true;
                    GuiEnabledOrXML = false;
                    Debug.Log("[OrX Mission Launch]: Showing Non-Tardis GUI");
                    ResetMissions();
                }
            }

            if (HighLogic.LoadedSceneIsFlight)
            {
                var tardis = FlightGlobals.ActiveVessel.FindPartModuleImplementing<ModuleOrXTardis>();
                if (tardis != null)
                {
                    guiOpen = true;
                    GuiEnabledOrXMLNT = false;
                    GuiEnabledOrXML = true;
                    Debug.Log("[OrX Mission Launch]: Showing Tardis GUI");
                    ResetMissions();
                }
                else
                {
                    guiOpen = true;
                    GuiEnabledOrXMLNT = true;
                    GuiEnabledOrXML = false;
                    Debug.Log("[OrX Mission Launch]: Showing Non-Tardis GUI");
                    ResetMissions();
                }
            }
        }

        public void DisableGui()
        {
            guiOpen = false;
            GuiEnabledOrXML = false;
            GuiEnabledOrXMLNT = false;
            Debug.Log("[OrX Mission Launch]: Hiding GUI");
        }

        private void GameUiEnableOrXML()
        {
            _gameUiToggle = true;
        }

        private void GameUiDisableOrXML()
        {
            _gameUiToggle = false;
        }

        private void DrawTitle(float line)
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
            GUI.Label(new Rect(0, 0, WindowWidth, 20), "SELECT MISSION", titleStyle);
        }

        private void DrawIronKerbal(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);
            if (!ironKerbal)
            {
                if (GUI.Button(saveRect, "Iron Kerbal Mode"))
                {
                    if (!WaldosIsland && !Pyramids && !Baikerbanur && !KSC)
                    {
                        OrX_Log.instance.survival = true;
                        Survival = true;
                        DisableGui();
                        KerbinMissions.instance.EnableSurvivalGui();
                    }
                }
            }
        }

        private void DrawKSC(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);
            if (!ironKerbal)
            {
                if (GUI.Button(saveRect, "Loot Box Controversy"))
                {
                    if (!WaldosIsland && !Pyramids && !Baikerbanur)
                    {
                        KSC = true;
                        DisableGui();
                        KerbinMissions.instance.EnableLBCGui();
                    }
                }
            }
        }

        private void DrawWaldosIsland(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);
            if (!ironKerbal)
            {
                if (GUI.Button(saveRect, "Waldo's Island"))
                {
                    if (!KSC && !Pyramids && !Baikerbanur && !Survival)
                    {
                        WaldosIsland = true;
                        KerbinMissions.instance.EnableIWIGui();
                        DisableGui();
                    }
                }
            }
        }

        private void DrawBaikerbanur(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);
            if (!ironKerbal)
            {
                if (GUI.Button(saveRect, "Killer Tomatoes"))
                {
                    if (!KSC && !Pyramids && !WaldosIsland && !Survival)
                    {
                        Baikerbanur = true;
                        KerbinMissions.instance.EnableATKGui();
                        DisableGui();
                    }
                }

            }
        }

        private void DrawPyramids(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);
            if (!ironKerbal)
            {
                if (GUI.Button(saveRect, "Tuten-Kerman Uldum"))
                {
                    if (!KSC && !WaldosIsland && !Baikerbanur && !Survival)
                    {
                        Pyramids = true;
                        KerbinMissions.instance.EnableTKUGui();
                        DisableGui();
                    }
                }
            }
        }

        #endregion

        /// /////////////////////////////////////////////////////////////////////////////

        private void ScreenMsg(string msg)
        {
            ScreenMessages.PostScreenMessage(new ScreenMessage(msg, 6, ScreenMessageStyle.UPPER_CENTER));
        }

        private void Dummy() { }
    }
}