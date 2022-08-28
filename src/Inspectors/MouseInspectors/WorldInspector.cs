using System.Linq;
using UniverseLib.UI.Panels;

namespace UnityExplorer.Inspectors.MouseInspectors
{
    public class WorldInspector : MouseInspectorBase
    {
        private static Camera MainCamera;
        private static GameObject lastHitObject;
        public static GameObject IgnoreOBJ;
        public static List<System.Type> IgnoreTypes = new();
        public static bool IgnoreHighlights;
        private int LastLayer;
        private const int IgnoreLayer = 29;

        public override void OnBeginMouseInspect()
        {
            if(IgnoreOBJ != null)
            {
                LastLayer = IgnoreOBJ.layer;
                IgnoreOBJ.layer = IgnoreLayer;
            }
            MainCamera = Camera.main;

            if (!MainCamera)
            {
                ExplorerCore.LogWarning("No MainCamera found! Cannot inspect world!");
                return;
            }
            PanelManager.UpdateMouseControls = false;
            Universe.ToggleMouseControls(false);
        }

        public override void ClearHitData()
        {
            lastHitObject = null;
        }

        public override void OnSelectMouseInspect()
        {
            InspectorManager.Inspect(lastHitObject);
        }

        public override void UpdateMouseInspect(Vector2 mousePos)
        {
            if (!MainCamera)
                MainCamera = Camera.main;
            if (!MainCamera)
            {
                ExplorerCore.LogWarning("No Main Camera was found, unable to inspect world!");
                MouseInspector.Instance.StopInspect();
                return;
            }

            Ray ray = MainCamera.ScreenPointToRay(mousePos);
            int layerMask = ~(1 << IgnoreLayer);
            RaycastHit hit;
            if(IgnoreHighlights)
            {
                hit = Physics.RaycastAll(ray, 1000f, layerMask).FirstOrDefault(h => IgnoreTypes.Count == 0 || !h.transform.gameObject.GetComponents<Component>().Any(c => IgnoreTypes.Any(t => t.IsAssignableFrom(c.GetType()))));
            }
            else Physics.Raycast(ray, out hit, 1000f, layerMask);

            if (hit.transform)
                OnHitGameObject(hit.transform.gameObject);
            else if (lastHitObject)
                MouseInspector.Instance.ClearHitData();
        }

        internal void OnHitGameObject(GameObject obj)
        {
            if (obj != lastHitObject)
            {
                lastHitObject = obj;
                MouseInspector.Instance.objNameLabel.text = $"<b>Click to Inspect:</b> <color=cyan>{obj.name}</color>";
                MouseInspector.Instance.objPathLabel.text = $"Path: {obj.transform.GetTransformPath(true)}";
            }
        }

        public override void OnEndInspect()
        {
            if(IgnoreOBJ != null)
                IgnoreOBJ.layer = LastLayer;
            PanelManager.UpdateMouseControls = true;
        }
    }
}
