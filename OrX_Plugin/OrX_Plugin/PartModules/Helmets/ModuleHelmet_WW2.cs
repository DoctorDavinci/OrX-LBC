

namespace OrX.parts
{
    public class ModuleHelmet_WW2 : PartModule
    {
        private bool triggered = false;

        public override void OnStart(StartState state)
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                triggered = false;
            }
            base.OnStart(state);
        }

        public void Update()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (!triggered && part.vessel.isEVA)
                {
                    SetStats();
                }
            }
        }

        private void SetStats()
        {
            var orx = part.vessel.FindPartModuleImplementing<ModuleOrX>();
            orx.ww2Helm = true;
            triggered = true;
        }
    }
}