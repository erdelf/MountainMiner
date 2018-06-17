using System.Linq;
using Verse;

namespace MountainMiner
{
    using JetBrains.Annotations;
    using UnityEngine;

    [UsedImplicitly]
    public class PlaceWorker_ShowRockRoof : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null)
        {
            for (int i = 0; i < 9; i++)
            {
                IntVec3 intVec = loc + GenRadial.RadialPattern[i];
                if (intVec.InBounds(map: map) && map.roofGrid.RoofAt(c: intVec) != null && map.roofGrid.RoofAt(c: intVec).isThickRoof)
                    return true;
            }
            return new AcceptanceReport(reasonText: "Must be placed under overhead Mountain");
        }
        
        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol)
        {
            foreach (IntVec3 current in from cur in Find.CurrentMap.AllCells where Find.CurrentMap.roofGrid.RoofAt(c: cur) != null && Find.CurrentMap.roofGrid.RoofAt(c: cur).isThickRoof select cur)
                CellRenderer.RenderCell(c: current);
        }
    }
}