using System.Linq;
using Verse;

namespace MountainMiner
{
    public class PlaceWorker_ShowRockRoof : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null)
        {
            if (loc != null)
            {
                for (int i = 0; i < 9; i++)
                {
                    IntVec3 intVec = loc + GenRadial.RadialPattern[i];
                    if (intVec.InBounds(map) && map.roofGrid.RoofAt(intVec) != null && map.roofGrid.RoofAt(intVec).isThickRoof)
                            return true;
                }
                return new AcceptanceReport("Must be placed under overhead Mountain");
            }
            return false;
        }

        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot)
        {
            foreach (IntVec3 current in from cur in Find.VisibleMap.AllCells where Find.VisibleMap.roofGrid.RoofAt(cur) != null && Find.VisibleMap.roofGrid.RoofAt(cur).isThickRoof select cur)
                CellRenderer.RenderCell(current);
        }
    }
}