using UnityEngine;

namespace OrXBDAc.chase
{
    /// <summary>
    /// Add the module to all kerbals available. 
    /// </summary>
    [KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
    class AddOrXchase : MonoBehaviour
    {
        public void Awake()
        {
            OrXchaseDebug.DebugLog("Loaded AddOrXchase.");

            ConfigNode EVA = new ConfigNode("MODULE");
            EVA.AddValue("name", "OrXchaseModule");

            try
            {
              PartLoader.getPartInfoByName("kerbalEVA").partPrefab.AddModule(EVA);
            }
            catch { 
			
			}

			EVA = new ConfigNode("MODULE");
            EVA.AddValue("name", "OrXchaseModule");

			try {
				PartLoader.getPartInfoByName("kerbalEVAfemale").partPrefab.AddModule(EVA);
			} catch {

			}
            /*
            EVA = new ConfigNode("MODULE");
            EVA.AddValue("name", "OrXchaseModule");

            try
            {
                PartLoader.getPartInfoByName("OrX_OrxBonusBall").partPrefab.AddModule(EVA);
            }
            catch
            {

            }

            EVA = new ConfigNode("MODULE");
            EVA.AddValue("name", "OrXchaseModule");

            try
            {
                PartLoader.getPartInfoByName("OrX_OrxBonusBall2").partPrefab.AddModule(EVA);
            }
            catch
            {

            }
            */
        }

    }
}
