

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OrXBDAc.parts
{
    public class ModuleStayPunkdHead : PartModule
    {

        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = true, guiName = "ENLARGE HEAD"),
         UI_Toggle(controlEnabled = true, scene = UI_Scene.All, disabledText = "False", enabledText = "True")]
        public bool enlarge = false;

        private bool enlarged = false;
        GameObject kerbalLarge;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            if (HighLogic.LoadedSceneIsFlight)
            {
                part.force_activate();
            }
        }

        public void Update()
        {
            if (enlarge && !enlarged)
            {
                enlarged = true;
                Size1Routine();
            }
        }

        public void Size1Routine()
        {
            Debug.Log("[OrX Stay Puffd Head] Resizing " + part.name + " ............. ");

            kerbalLarge = this.part.gameObject;

            kerbalLarge.transform.localScale += new Vector3(7, 7, 7);

            Debug.Log("[OrX Stay Puffd Head] STAY PUNKD MARSHMALLOW KERBAL ........layer " + kerbalLarge.layer);
            Debug.Log("[OrX Stay Puffd Head] STAY PUNKD MARSHMALLOW KERBAL ........transform " + kerbalLarge.transform);
            Debug.Log("[OrX Stay Puffd Head] STAY PUNKD MARSHMALLOW KERBAL ........transform.parent " + kerbalLarge.transform.parent);
            Debug.Log("[OrX Stay Puffd Head] STAY PUNKD MARSHMALLOW KERBAL ........transform.position " + kerbalLarge.transform.position);
            Debug.Log("[OrX Stay Puffd Head] STAY PUNKD MARSHMALLOW KERBAL ........transform.right " + kerbalLarge.transform.right);
            Debug.Log("[OrX Stay Puffd Head] STAY PUNKD MARSHMALLOW KERBAL ........transform.root " + kerbalLarge.transform.root);
            Debug.Log("[OrX Stay Puffd Head] STAY PUNKD MARSHMALLOW KERBAL ........transform.rotation " + kerbalLarge.transform.rotation);
            Debug.Log("[OrX Stay Puffd Head] STAY PUNKD MARSHMALLOW KERBAL ........transform.localPosition " + kerbalLarge.transform.localPosition);
            Debug.Log("[OrX Stay Puffd Head] STAY PUNKD MARSHMALLOW KERBAL ........transform.localRotation " + kerbalLarge.transform.localRotation);
            Debug.Log("[OrX Stay Puffd Head] STAY PUNKD MARSHMALLOW KERBAL ........transform.localScale " + kerbalLarge.transform.localScale);
            Debug.Log("[OrX Stay Puffd Head] STAY PUNKD MARSHMALLOW KERBAL ........transform.up " + kerbalLarge.transform.up);

        }
    }
}
