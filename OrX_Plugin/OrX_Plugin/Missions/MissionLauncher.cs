using System;
using KSP.UI.Screens;
using UnityEngine;
using System.Collections;
using OrX.parts;
using System.IO;
using System.Reflection;
using OrX.missions;
using System.Collections.Generic;

namespace OrX
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
    public class MissionLauncher : MonoBehaviour
    {
        private const float WindowWidth = 220;
        private const float DraggableHeight = 40;
        private const float LeftIndent = 12;
        private const float ContentTop = 20;
        public static MissionLauncher instance;
        public static bool GuiEnabledOrXML;
        public static bool HasAddedButton;
        private readonly float _incrButtonWidth = 26;
        private readonly float contentWidth = WindowWidth - 2 * LeftIndent;
        private readonly float entryHeight = 20;
        private float _contentWidth;
        private bool _gameUiToggle;
        private float _windowHeight = 250;
        private Rect _windowRect;

        private bool launchSiteChanged = false;

        //////////////////////////////////////////////////////////////////////////////////////////

        // Runway: lat -0.0485890418349364, long 285.276094692895, alt 71.9665353324963
        // Beach: lat -0.039751185006272, long 285.639486693549, alt 1.68487426708452
        // Beach by Island: lat -1.53556797173857, long 287.956960620886, alt 1.56112247915007

        private double latitude = 0;
        private double longitude = 0;

        private double lat = 0;
        private double lon = 0;

        [KSPField(isPersistant = true)]
        private bool KSC = false;
        public double latKSC = -0.093354877488929;
        public double lonKSC = -74.6521799214026;

        [KSPField(isPersistant = true)]
        private bool WaldosIsland = false;
        public double latWaldosIsland = -1.51812886210386;
        public double lonWaldosIsland = -71.96798623656;

        [KSPField(isPersistant = true)]
        private bool Baikerbanur = false;
        public double latBaikerbanur = 20.6635562459104;
        public double lonBaikerbanur = -146.420941786273;

        [KSPField(isPersistant = true)]
        private bool Pyramids = false;
        public double latPyramids = -6.49869308429184;
        public double lonPyramids = -141.679184195229;

        private double altitude = 200;
        public double alt = 1;
        public float altAdjust = 2;

        public bool guiOpen = false;

        /// <summary>
        /// ///////////////////////////////////////////////////////////////////////////////////////
        /// </summary>

        //private string tardisCraft = string.Empty;

        private void Awake()
        {
            //var _OrXdir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //tardisCraft = _OrXdir + "\\PluginData" + "\\VesselData" + "\\Player" + "\\Tardis.craft";

            //if (instance) Destroy(instance);
            DontDestroyOnLoad(this);
            instance = this;
        }

        private void Start()
        {
            _windowRect = new Rect((Screen.width / 2) - (WindowWidth / 2), 160, WindowWidth, _windowHeight);
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
                    _windowRect = GUI.Window(622253316, _windowRect, GuiWindowOrXMLEditor, "");
                }
            }

            if (HighLogic.LoadedSceneIsFlight)
            {
                if (GuiEnabledOrXML)
                {
                    _windowRect = GUI.Window(621143316, _windowRect, GuiWindowOrXMLFlight, "");
                }
            }
        }

        #region Launch

        public Vector3d LaunchPosition()
        {
            return FlightGlobals.ActiveVessel.mainBody.GetWorldSurfacePosition((double)latitude, (double)longitude, (double)altitude);
        }

        public void LaunchToKSC()
        {
            KSC = true;
            lat = latKSC;
            lon = lonKSC;
            alt = 69;
            StartCoroutine(Launch());
        }

        public void LaunchToWaldosIsland()
        {
            WaldosIsland = true;
            alt = 136;
            lat = latWaldosIsland;
            lon = lonWaldosIsland;
            StartCoroutine(Launch());
        }

        public void LaunchToBaikerbanur()
        {
            Baikerbanur = true;
            alt = 136;
            lat = latBaikerbanur;
            lon = lonBaikerbanur;
            StartCoroutine(Launch());
        }

        public void LaunchToPyramids()
        {
            Pyramids = true;
            alt = 136;
            lat = latPyramids;
            lon = lonPyramids;
            StartCoroutine(Launch());
        }

        IEnumerator Launch()
        {
            FlightGlobals.ActiveVessel.GetComponent<Rigidbody>().isKinematic = true;

            latitude = FlightGlobals.ActiveVessel.latitude;
            longitude = FlightGlobals.ActiveVessel.longitude;
            altitude = 65000;
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
            altitude = 65000;
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
            altitude = 65000;
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
            altitude = 65000;
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
            altitude =  alt + altAdjust;
            FlightGlobals.ActiveVessel.SetPosition(LaunchPosition(), true);
            FlightGlobals.ActiveVessel.GetComponent<Rigidbody>().velocity = Vector3.zero;
            FlightGlobals.ActiveVessel.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            FlightGlobals.ActiveVessel.geeForce = 0;
            FlightGlobals.ActiveVessel.geeForce_immediate = 0;
            FlightGlobals.ActiveVessel.GetComponent<Rigidbody>().isKinematic = false;
            yield return new WaitForFixedUpdate();
            StartCoroutine(MissionDelay());
        }

        IEnumerator MissionDelay()
        {
            if (FlightGlobals.ActiveVessel.LandedOrSplashed)
            {
                OrX_Controls.instance.OpenGUI();

                yield return new WaitForSeconds(15);

                if (KSC)
                {
                    Missions.instance.spawnCount = 0;
                    Missions.instance.LootBoxControversy();
                }

                if (WaldosIsland)
                {
                    Missions.instance.spawnCount = 0;
                    Missions.instance.WaldosIsland();
                }

                if (Pyramids)
                {
                    Missions.instance.spawnCount = 0;
                    Missions.instance.KillerTomatoes();
                }

                if (Baikerbanur)
                {
                    Missions.instance.spawnCount = 0;
                    Missions.instance.TutenKerman();
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
            launchSiteChanged = false;
            Pyramids = false;
            WaldosIsland = false;
            KSC = false;
            Baikerbanur = false;
        }

        #endregion

        #region GUI
        /// <summary>
        /// GUI
        /// </summary>

        private void GuiWindowOrXMLFlight(int OrXMLF)
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
                Texture buttonTexture = GameDatabase.Instance.GetTexture(textureDir + "OrX_missions", false); //texture to use for the button
                ApplicationLauncher.Instance.AddModApplication(ToggleGUI, ToggleGUI, Dummy, Dummy, Dummy, Dummy,
                    ApplicationLauncher.AppScenes.SPH | ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.FLIGHT, buttonTexture);
                HasAddedButton = true;
            }
        }

        public void ToggleGUI()
        {
            if (GuiEnabledOrXML)
            {
                DisableGui();
            }
            else
            {
                EnableGui();
            }
        }

        public void EnableGui()
        {
            if (HighLogic.LoadedSceneIsEditor)
            {
                var count = 0;

                foreach (Part p in EditorLogic.fetch.ship.parts)
                {
                    if (p != null)
                    {
                        var tardis = p.FindModuleImplementing<ModuleOrXTardis>();
                        if (tardis != null)
                        {
                            count += 1;
                            guiOpen = true;
                            GuiEnabledOrXML = true;
                            Debug.Log("[OrX Kerbal Launch]: Showing GUI");
                            ResetMissions();
                        }
                    }
                }

                if (count == 0)
                {
                    ScreenMsg("<b>OrX Missions require the use of the Tardis</b>");
                }
            }

            if (HighLogic.LoadedSceneIsFlight)
            {
                var tardis = FlightGlobals.ActiveVessel.FindPartModuleImplementing<ModuleOrXTardis>();
                if (tardis != null)
                {
                    guiOpen = true;
                    GuiEnabledOrXML = true;
                    Debug.Log("[OrX Kerbal Launch]: Showing GUI");
                    ResetMissions();
                }
                else
                {
                    ScreenMsg("<b>OrX Missions require the use of the Tardis</b>");
                }
            }
        }

        public void DisableGui()
        {
            guiOpen = false;
            GuiEnabledOrXML = false;
            Debug.Log("[OrX Kerbal Launch]: Hiding GUI");
            ResetMissions();
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

        private void DrawKSC(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);
            if (GUI.Button(saveRect, "Loot Box Conspiracy"))
            {
                KSC = true;

                if (HighLogic.LoadedSceneIsEditor)
                {
                    var count = 0;

                    if (!WaldosIsland && !Baikerbanur && !Pyramids)
                    {
                        Debug.Log("[DEFEND KSC - LOOT BOX CONSPIRACY] ........ DefendKSC");

                        foreach (Part p in EditorLogic.fetch.ship.parts)
                        {
                            var tardis = p.FindModuleImplementing<ModuleOrXTardis>();
                            if (tardis != null && count >= 0)
                            {
                                count += 1;
                                launchSiteChanged = true;
                                ScreenMsg("<b>PREPARE YOURSELF</b>");
                                tardis.KSC = true;
                                tardis.launchSiteChanged = true;
                                tardis.triggered = false;
                                break;
                            }
                            else
                            {
                                ScreenMsg("<b>OrX Missions require the use of the Tardis</b>");
                                break;
                            }
                        }
                        DisableGui();
                    }
                }
                else
                {
                    var tardis = FlightGlobals.ActiveVessel.FindPartModuleImplementing<ModuleOrXTardis>();
                    if (tardis != null)
                    {
                        Missions.instance.startMission = true;
                        Missions.instance.KSC = true;
                        launchSiteChanged = true;
                        tardis.launchSiteChanged = true;
                        tardis.KSC = true;
                        tardis.triggered = false;
                    }
                    else
                    {
                        ScreenMsg("<b>OrX Missions require the use of the Tardis</b>");
                    }
                    DisableGui();
                }
            }
        }

        private void DrawWaldosIsland(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);
            if (GUI.Button(saveRect, "Waldo's Island"))
            {
                ScreenMsg("<color=#cfc100ff><b>Sorry, this mission is unavailable at this time</b></color>");
                ScreenMsg("<color=#cfc100ff><b>Will be included in a future OrX release</b></color>");
                /*
                if (HighLogic.LoadedSceneIsEditor)
                {
                    if (!KSC && !Baikerbanur && !Pyramids)
                    {
                        WaldosIsland = true;
                        EditorLogic.LoadShipFromFile(tardisCraft);
                        Missions.instance.startMission = true;
                        launchSiteChanged = true;
                        Missions.instance.waldosIsland = true;
                        Pyramids = false;
                        KSC = false;
                        Baikerbanur = false;
                        ScreenMsg("<b>PREPARE YOURSELF</b>");
                        DefendWaldosIsland();
                        DisableGui();
                    }
                }
                else
                {
                    var tardis = FlightGlobals.ActiveVessel.FindPartModuleImplementing<ModuleOrXTardis>();
                    if (tardis != null)
                    {
                        Missions.instance.startMission = true;
                        Missions.instance.waldosIsland = true;
                        tardis.WaldosIsland = true;
                        tardis.triggered = false;
                        tardis.travel = true;
                        DisableGui();
                    }
                    else
                    {
                        ScreenMsg("<b>OrX Missions require the use of the Tardis</b>");
                        DisableGui();
                    }
                }*/
            }
        }

        private void DefendWaldosIsland()
        {
            var tardis = EditorLogic.RootPart.FindModuleImplementing<ModuleOrXTardis>();
            if (tardis != null)
            {
                tardis.WaldosIsland = true;
                tardis.launchSiteChanged = true;
                tardis.launchSiteChanged = true;
                tardis.triggered = false;
                DisableGui();
            }
            else
            {
                ScreenMsg("<b>OrX Missions require the use of the Tardis</b>");
                DisableGui();
            }
        }

        private void DrawBaikerbanur(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);
            if (GUI.Button(saveRect, "Attack of the Killer Tomatoes"))
            {
                ScreenMsg("<color=#cfc100ff><b>Sorry, this mission is unavailable at this time</b></color>");
                ScreenMsg("<color=#cfc100ff><b>Will be included in a future OrX release</b></color>");
                /*
                if (HighLogic.LoadedSceneIsEditor)
                {

                    //Baikerbanur = true;

                    if (!WaldosIsland && !KSC && !Pyramids)
                    {

                        //EditorLogic.LoadShipFromFile(tardisCraft);
                        //launchSiteChanged = true;
                        //ScreenMsg("<b>PREPARE YOURSELF</b>");
                        //DefendBaikerbanur();
                        //DisableGui();
                    }
                }
                else
                {

                }*/
            }
        }

        private void DefendBaikerbanur()
        {
            var tardis = EditorLogic.RootPart.FindModuleImplementing<ModuleOrXTardis>();
            if (tardis != null)
            {
                tardis.Baikerbanur = true;
                tardis.launchSiteChanged = true;
                tardis.triggered = false;
                DisableGui();
            }
            else
            {
                ScreenMsg("<b>OrX Missions require the use of the Tardis</b>");
                DisableGui();
            }
        }

        private void DrawPyramids(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);
            if (GUI.Button(saveRect, "Tuten-Kerman Uldum"))
            {
                ScreenMsg("<color=#cfc100ff><b>Sorry, this mission is unavailable at this time</b></color>");
                ScreenMsg("<color=#cfc100ff><b>Will be included in a future OrX release</b></color>");
                /*
                if (HighLogic.LoadedSceneIsEditor)
                {

                    //Pyramids = true;

                    if (!WaldosIsland && !KSC && !Baikerbanur)
                    {

                        //EditorLogic.LoadShipFromFile(tardisCraft);
                        //launchSiteChanged = true;
                        //ScreenMsg("<b>PREPARE YOURSELF</b>");
                        //DefendPyramids();
                        //DisableGui();
                    }
                }
                else
                {

                }*/
            }
        }

        private void DefendPyramids()
        {
            var tardis = EditorLogic.RootPart.FindModuleImplementing<ModuleOrXTardis>();
            if (tardis != null)
            {
                tardis.Pyramids = true;
                tardis.launchSiteChanged = true;
                tardis.triggered = false;
                DisableGui();
            }
            else
            {
                ScreenMsg("<b>OrX Missions require the use of the Tardis</b>");
                DisableGui();
            }
        }

        #endregion

        private void ScreenMsg(string msg)
        {
            ScreenMessages.PostScreenMessage(new ScreenMessage(msg, 4, ScreenMessageStyle.UPPER_CENTER));
        }

        private void Dummy() { }
    }
}