using OrX.missions;
using System.Collections;
using System.Collections.Generic;
using BDArmory.Modules;
using BDArmory.UI;
using UnityEngine;
using OrX.parts;

namespace OrX
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class OrXVesselSwitcher : MonoBehaviour
    {
        private readonly float _buttonGap = 1;
        private readonly float _buttonHeight = 20;

        private int _guiCheckIndex;
        public static OrXVesselSwitcher instance;
        private readonly float _margin = 5;

        private bool _ready;
        private bool _showGui;
        private readonly float _titleHeight = 30;
        private float updateTimer = 0;
        public bool missions = false;
        public bool missionRunning = false;

        //gui params
        private float _windowHeight; //auto adjusting
        private Rect _windowRect
        {
            get { return BDArmorySetup.WindowRectVesselSwitcher; }
            set { BDArmorySetup.WindowRectVesselSwitcher = value; }
        }
        private readonly float _windowWidth = 250;

        private List<MissileFire> _wmgrsA;
        private List<MissileFire> _wmgrsB;

        private List<ModuleOrX> _orxA;
        private List<ModuleOrX> _orxB;

        private void Awake()
        {
            if (instance)
                Destroy(this);
            else
                instance = this;
        }

        private void Start()
        {
            UpdateList();
            GameEvents.onVesselCreate.Add(VesselEventUpdate);
            GameEvents.onVesselDestroy.Add(VesselEventUpdate);
            GameEvents.onVesselGoOffRails.Add(VesselEventUpdate);
            GameEvents.onVesselGoOnRails.Add(VesselEventUpdate);
            MissileFire.OnToggleTeam += MissileFireOnToggleTeam;

            _ready = false;
            StartCoroutine(WaitForBdaSettings());

            // TEST
            FloatingOrigin.fetch.threshold = 20000; //20km
            FloatingOrigin.fetch.thresholdSqr = 20000*20000; //20km
            Debug.Log($"FLOATINGORIGIN: threshold is {FloatingOrigin.fetch.threshold}");

            //_windowRect = new Rect(10, Screen.height / 6f, _windowWidth, 10); // now tied to BDArmorySetup persisted field!
        }

        private void OnDestroy()
        {
                GameEvents.onVesselCreate.Remove(VesselEventUpdate);
                GameEvents.onVesselDestroy.Remove(VesselEventUpdate);
                GameEvents.onVesselGoOffRails.Remove(VesselEventUpdate);
                GameEvents.onVesselGoOnRails.Remove(VesselEventUpdate);
                MissileFire.OnToggleTeam -= MissileFireOnToggleTeam;

                _ready = false;

            // TEST
            Debug.Log($"FLOATINGORIGIN: threshold is {FloatingOrigin.fetch.threshold}");
        }

        private IEnumerator WaitForBdaSettings()
        {
            while (BDArmorySetup.Instance == null)
                yield return null;

            _ready = true;
            BDArmorySetup.Instance.hasVS = true;
            _guiCheckIndex = BDArmory.Misc.Misc.RegisterGUIRect(new Rect());
        }

        private void MissileFireOnToggleTeam(MissileFire wm, BDArmorySetup.BDATeams team)
        {
            if (_showGui)
                UpdateList();
        }

        private void VesselEventUpdate(Vessel v)
        {
            if (_showGui)
                UpdateList();
        }

        private void Update()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (missions)
                {
                    if (missionRunning)
                    {
                        VesselUpdateCheck();
                        Hotkeys();
                    }

                    if (BDArmorySetup.Instance.showVSGUI)
                    {
                        BDArmorySetup.Instance.showVSGUI = false;

                        if (_showGui)
                        {
                            _showGui = false;
                        }
                        else
                        {
                            _showGui = true;
                        }
                    }
                }
                else
                {
                    if (_showGui)
                    {
                        _showGui = false;
                    }
                }
            }
        }

        public void VesselUpdateCheck()
        {
            updateTimer -= Time.fixedDeltaTime;

            if (updateTimer < 0)
            {
                UpdateList();
                updateTimer = 0.5f;    //next update in half a sec only
            }
        }

        private void Hotkeys()
        {
            if (Input.GetKeyDown(KeyCode.PageDown))
                SwitchToNextVessel();
            if (Input.GetKeyDown(KeyCode.PageUp))
                SwitchToPreviousVessel();

            if (Input.GetKeyDown(KeyCode.RightBracket))
                SwitchToNextVessel();
            if (Input.GetKeyDown(KeyCode.LeftBracket))
                SwitchToPreviousVessel();
        }

        private void UpdateList()
        {
            if (_orxA == null) _orxA = new List<ModuleOrX>();
            _orxA.Clear();

            if (_orxB == null) _orxB = new List<ModuleOrX>();
            _orxB.Clear();

            if (_wmgrsA == null) _wmgrsA = new List<MissileFire>();
            _wmgrsA.Clear();

            if (_wmgrsB == null) _wmgrsB = new List<MissileFire>();
            _wmgrsB.Clear();

            List<Vessel>.Enumerator v = FlightGlobals.Vessels.GetEnumerator();
            while (v.MoveNext())
            {
                if (v.Current == null) continue;
                if (!v.Current.loaded || v.Current.packed) continue;
                var orx = v.Current.FindPartModuleImplementing<ModuleOrX>();
                if (orx == null)
                {
                    List<MissileFire>.Enumerator wm = v.Current.FindPartModulesImplementing<MissileFire>().GetEnumerator();
                    while (wm.MoveNext())
                    {
                        if (wm.Current == null) continue;
                        if (!wm.Current.team) _wmgrsA.Add(wm.Current);
                        else _wmgrsB.Add(wm.Current);

                        break;
                    }
                    wm.Dispose();
                }
            }
            v.Dispose();

            List<Vessel>.Enumerator kerb = FlightGlobals.Vessels.GetEnumerator();
            while (kerb.MoveNext())
            {
                if (kerb.Current == null) continue;
                if (!kerb.Current.loaded || kerb.Current.packed) continue;
                List<ModuleOrX>.Enumerator orxKerb = kerb.Current.FindPartModulesImplementing<ModuleOrX>().GetEnumerator();
                while (orxKerb.MoveNext())
                {
                    if (orxKerb.Current == null) continue;
                    if (orxKerb.Current.player) _orxA.Add(orxKerb.Current);
                    else _orxB.Add(orxKerb.Current);
                    break;
                }
                orxKerb.Dispose();
            }
            kerb.Dispose();

            if (missions && missionRunning)
            {
                if (_orxB.Count <= 1 && _wmgrsB.Count <= 1)
                {
                    missionRunning = false;
                    Missions.instance.LevelUp();
                }
            }
        }

        private void OnGUI()
        {
            if (_ready)
            {
                if (_showGui && BDArmorySetup.GAME_UI_ENABLED)
                {
                    SetNewHeight(_windowHeight);
                    _windowRect = GUI.Window(32222504, _windowRect, ListWindow, "OrX Vessel Switcher",
                        HighLogic.Skin.window);
                    BDArmory.Misc.Misc.UpdateGUIRect(_windowRect, _guiCheckIndex);
                }
                else
                {
                    //BDArmory.Misc.Misc.UpdateGUIRect(new Rect(), _guiCheckIndex);
                }
            }
        }

        private void SetNewHeight(float windowHeight)
        {
            BDArmorySetup.WindowRectVesselSwitcher.height = windowHeight;
        }

        private void ListWindow(int id)
        {
            if (_orxA.Count >= 0)
            {
                GUI.DragWindow(new Rect(0, 0, _windowWidth - _buttonHeight - 4, _titleHeight));
                if (GUI.Button(new Rect(_windowWidth - _buttonHeight, 4, _buttonHeight, _buttonHeight), "X",
                    HighLogic.Skin.button))
                {
                    BDArmorySetup.Instance.showVSGUI = false;
                    _showGui = false;
                    return;
                }

                float height = 0;
                float kerbalA = 0;
                float kerbalB = 0;
                float vesselLineA = 0;
                float vesselLineB = 0;

                height += _margin + _titleHeight;
                GUI.Label(new Rect((_windowWidth / 3) - (2 * _margin), height, _windowWidth - 2 * _margin, _buttonHeight), "Player Team", HighLogic.Skin.label);
                height += _buttonHeight;
                float vesselButtonWidth = _windowWidth - 2 * _margin;
                vesselButtonWidth -= 2 * _buttonHeight;

                List<ModuleOrX>.Enumerator orxA = _orxA.GetEnumerator();
                while (orxA.MoveNext())
                {
                    if (orxA.Current == null) continue;

                    if (!orxA.Current.team)
                    {
                        float lineY = height + kerbalA * (_buttonHeight + _buttonGap);
                        Rect buttonRect = new Rect(_margin, lineY, vesselButtonWidth, _buttonHeight);
                        GUIStyle vButtonStyle = orxA.Current.vessel.isActiveVessel ? HighLogic.Skin.box : HighLogic.Skin.button;

                        string status = UpdateKerbalStatus(orxA.Current, vButtonStyle);

                        if (GUI.Button(buttonRect, status + orxA.Current.vessel.GetName(), vButtonStyle))
                        {
                            ForceSwitchVessel(orxA.Current.vessel);
                        }

                        //guard toggle
                        GUIStyle guardStyle = orxA.Current.guard ? HighLogic.Skin.box : HighLogic.Skin.button;
                        Rect guardButtonRect = new Rect(_margin + vesselButtonWidth, lineY, _buttonHeight, _buttonHeight);
                        if (GUI.Button(guardButtonRect, "G", guardStyle))
                        {
                            //orxA.Current.CheckGuard();
                        }
                        /*
                        //AI toggle
                        GUIStyle aiStyle = orxA.Current.player ? HighLogic.Skin.box : HighLogic.Skin.button;
                        Rect aiButtonRect = new Rect(_margin + vesselButtonWidth + _buttonHeight, lineY, _buttonHeight,
                            _buttonHeight);
                        if (GUI.Button(aiButtonRect, "U", aiStyle))
                        {
                            //                        orxA.Current.TogglestayPunkd();
                        }
                        */
                        kerbalA++;
                    }
                }
                orxA.Dispose();

                height += kerbalA * (_buttonHeight + _buttonGap);
                height += _margin;

                List<MissileFire>.Enumerator wma = _wmgrsA.GetEnumerator();
                while (wma.MoveNext())
                {
                    if (wma.Current == null) continue;

                    float lineY = height + vesselLineA * (_buttonHeight + _buttonGap);
                    Rect buttonRect = new Rect(_margin, lineY, vesselButtonWidth, _buttonHeight);
                    GUIStyle vButtonStyle = wma.Current.vessel.isActiveVessel ? HighLogic.Skin.box : HighLogic.Skin.button;

                    string status = UpdateVesselStatus(wma.Current, vButtonStyle);

                    if (GUI.Button(buttonRect, status + wma.Current.vessel.GetName(), vButtonStyle))
                    {
                        ForceSwitchVessel(wma.Current.vessel);
                    }

                    //guard toggle
                    GUIStyle guardStyle = wma.Current.guardMode ? HighLogic.Skin.box : HighLogic.Skin.button;
                    Rect guardButtonRect = new Rect(_margin + vesselButtonWidth, lineY, _buttonHeight, _buttonHeight);
                    if (GUI.Button(guardButtonRect, "G", guardStyle))
                        wma.Current.ToggleGuardMode();

                    //AI toggle
                    if (wma.Current.AI != null)
                    {
                        GUIStyle aiStyle = wma.Current.AI.pilotEnabled ? HighLogic.Skin.box : HighLogic.Skin.button;
                        Rect aiButtonRect = new Rect(_margin + vesselButtonWidth + _buttonHeight, lineY, _buttonHeight,
                            _buttonHeight);
                        if (GUI.Button(aiButtonRect, "P", aiStyle))
                            wma.Current.AI.TogglePilot();
                    }
                    vesselLineA++;
                }
                wma.Dispose();

                height += vesselLineA * (_buttonHeight + _buttonGap);

                GUI.Label(new Rect((_windowWidth / 3) - (2 * _margin), height, _windowWidth - 2 * _margin, _buttonHeight), "OrX Team", HighLogic.Skin.label);
                height += _buttonHeight;

                List<MissileFire>.Enumerator wmb = _wmgrsB.GetEnumerator();
                while (wmb.MoveNext())
                {
                    if (wmb.Current == null) continue;

                    float lineY = height + vesselLineB * (_buttonHeight + _buttonGap);

                    Rect buttonRect = new Rect(_margin, lineY, vesselButtonWidth, _buttonHeight);
                    GUIStyle vButtonStyle = wmb.Current.vessel.isActiveVessel ? HighLogic.Skin.box : HighLogic.Skin.button;

                    string status = UpdateVesselStatus(wmb.Current, vButtonStyle);


                    if (GUI.Button(buttonRect, status + wmb.Current.vessel.GetName(), vButtonStyle))
                    {
                        //ForceSwitchVessel(wmb.Current.vessel);
                    }


                    //guard toggle
                    GUIStyle guardStyle = wmb.Current.guardMode ? HighLogic.Skin.box : HighLogic.Skin.button;
                    Rect guardButtonRect = new Rect(_margin + vesselButtonWidth, lineY, _buttonHeight, _buttonHeight);
                    if (GUI.Button(guardButtonRect, "G", guardStyle))
                        //wmb.Current.ToggleGuardMode();

                        //AI toggle
                        if (wmb.Current.AI != null)
                        {
                            GUIStyle aiStyle = wmb.Current.AI.pilotEnabled ? HighLogic.Skin.box : HighLogic.Skin.button;
                            Rect aiButtonRect = new Rect(_margin + vesselButtonWidth + _buttonHeight, lineY, _buttonHeight,
                                _buttonHeight);
                            if (GUI.Button(aiButtonRect, "P", aiStyle))
                            {
                                ;
                            }
                            //wmb.Current.AI.TogglePilot();
                        }
                        /*
                        else
                        {
                            var OrXkerbal = wmb.Current.vessel.FindPartModuleImplementing<ModuleOrX>();
                            GUIStyle aiStyle = OrXkerbal.attacking ? HighLogic.Skin.box : HighLogic.Skin.button;
                            Rect aiButtonRect = new Rect(_margin + vesselButtonWidth + _buttonHeight, lineY, _buttonHeight,
                                _buttonHeight);
                            if (GUI.Button(aiButtonRect, "A", aiStyle))
                            {
                                //orx.Current.TogglestayPunkd();
                            }
                        }*/
                    vesselLineB++;
                }
                height += vesselLineB * (_buttonHeight + _buttonGap);

                
                List<ModuleOrX>.Enumerator orxB = _orxB.GetEnumerator();
                while (orxB.MoveNext())
                {
                    if (orxB.Current == null) continue;

                    if (orxB.Current.team)
                    {
                        float lineY = height + kerbalB * (_buttonHeight + _buttonGap);
                        Rect buttonRect = new Rect(_margin, lineY, vesselButtonWidth, _buttonHeight);
                        GUIStyle vButtonStyle = orxB.Current.vessel.isActiveVessel ? HighLogic.Skin.box : HighLogic.Skin.button;

                        string status = UpdateKerbalStatus(orxB.Current, vButtonStyle);

                        if (GUI.Button(buttonRect, status + orxB.Current.vessel.GetName(), vButtonStyle))
                        {
                            //ForceSwitchVessel(orxB.Current.vessel);
                        }

                        /* 
                        //guard toggle
                        GUIStyle guardStyle = orxB.Current.guard ? HighLogic.Skin.box : HighLogic.Skin.button;
                        Rect guardButtonRect = new Rect(_margin + vesselButtonWidth, lineY, _buttonHeight, _buttonHeight);
                        if (GUI.Button(guardButtonRect, "G", guardStyle))
                        {
                            //orx.Current.ToggleBrute();
                        }

                        //AI toggle
                        GUIStyle aiStyle = orxB.Current.attacking ? HighLogic.Skin.box : HighLogic.Skin.button;
                        Rect aiButtonRect = new Rect(_margin + vesselButtonWidth + _buttonHeight, lineY, _buttonHeight,
                            _buttonHeight);
                        if (GUI.Button(aiButtonRect, "A", aiStyle))
                        {
                            //orx.Current.TogglestayPunkd();
                        }
                        */
                        kerbalB++;
                    }
                }
                orxB.Dispose();
                

                height += kerbalB * (_buttonHeight + _buttonGap);
                height += _margin;

                _windowHeight = height;

            }
        }

        private string UpdateKerbalStatus(ModuleOrX orx, GUIStyle vButtonStyle)
        {
            string status = "";
            if (orx.vessel.LandedOrSplashed)
            {
                if (orx.vessel.Landed)
                    status = "(L) ";
                else
                    status = "(S) ";
                vButtonStyle.fontStyle = FontStyle.Italic;
            }
            else
            {
                vButtonStyle.fontStyle = FontStyle.Italic;
            }
            return status;
        }

        private string UpdateVesselStatus(MissileFire wm, GUIStyle vButtonStyle)
        {
            string status = "";
            if (wm.vessel.LandedOrSplashed)
            {
                if (wm.vessel.Landed)
                    status = "(L) ";
                else
                    status = "(S) ";
                vButtonStyle.fontStyle = FontStyle.Normal;
            }
            else
            {
                vButtonStyle.fontStyle = FontStyle.Normal;
            }
            return status;
        }

        private void SwitchToNextVessel()
        {
            bool switchNext = false;

            List<MissileFire>.Enumerator wma = _wmgrsA.GetEnumerator();
            while (wma.MoveNext())
            {
                if (wma.Current == null) continue;
                if (switchNext)
                {
                    ForceSwitchVessel(wma.Current.vessel);
                    return;
                }
                if (wma.Current.vessel.isActiveVessel) switchNext = true;
            }
            wma.Dispose();
            
            List<ModuleOrX>.Enumerator orx = _orxA.GetEnumerator();
            while (orx.MoveNext())
            {
                if (orx.Current == null) continue;
                if (switchNext)
                {
                    ForceSwitchVessel(orx.Current.vessel);
                    return;
                }
                if (orx.Current.vessel.isActiveVessel) switchNext = true;
            }
            orx.Dispose();
            
            if (_wmgrsA.Count > 0 && _wmgrsA[0] && !_wmgrsA[0].vessel.isActiveVessel)
            {
                ForceSwitchVessel(_wmgrsA[0].vessel);
            }
            else if (_orxA.Count > 0 && _orxA[0] && !_orxA[0].vessel.isActiveVessel)
            {
                ForceSwitchVessel(_orxA[0].vessel);
            }
        }

        private void SwitchToPreviousVessel()
        {
            if (_orxA.Count > 0)
                for (int i = _wmgrsB.Count - 1; i >= 0; i--)
                    if (_orxA[i].vessel.isActiveVessel)
                        if (i > 0)
                        {
                            ForceSwitchVessel(_orxA[i - 1].vessel);
                            return;
                        }
                        else if (_wmgrsA.Count > 0)
                        {
                            ForceSwitchVessel(_wmgrsA[_wmgrsA.Count - 1].vessel);
                            return;
                        }
                        else if (_orxA.Count > 0)
                        {
                            ForceSwitchVessel(_orxA[_wmgrsB.Count - 1].vessel);
                            return;
                        }
            

            if (_wmgrsA.Count > 0)
                for (int i = _wmgrsA.Count - 1; i >= 0; i--)
                    if (_wmgrsA[i].vessel.isActiveVessel)
                        if (i > 0)
                        {
                            ForceSwitchVessel(_wmgrsA[i - 1].vessel);
                            return;
                        }
                        else if (_orxA.Count > 0)
                        {
                            ForceSwitchVessel(_orxA[_orxA.Count - 1].vessel);
                            return;
                        }
                        else if (_wmgrsA.Count > 0)
                        {
                            ForceSwitchVessel(_wmgrsA[_wmgrsA.Count - 1].vessel);
                            return;
                        }
        }


        // Extracted method, so we dont have to call these two lines everywhere
        private void ForceSwitchVessel(Vessel v)
        {
            if (missions)
            {
                FlightGlobals.ForceSetActiveVessel(v);
                FlightInputHandler.ResumeVesselCtrlState(v);
            }
        }
    }
}