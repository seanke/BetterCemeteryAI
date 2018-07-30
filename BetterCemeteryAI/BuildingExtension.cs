using ColossalFramework;
using ICities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BetterCemeteryAI
{
    public class BuildingExtension : BuildingExtensionBase
    {
        public override void OnBuildingCreated(ushort id)
        {
            base.OnBuildingCreated(id);

            try
            {
                var b = Singleton<BuildingManager>.instance.m_buildings.m_buffer[id];
                if (!b.Info.m_class.name.Equals("DeathCare Facility")) return;

                AIHelper.ApplyNewAIToBuilding(b);
            }
            catch (Exception e)
            {
                LogHelper.Information(e.ToString());
            }
        }
    }
}
 