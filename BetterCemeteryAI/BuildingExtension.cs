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
                if (!b.Info.name.Equals("Cemetery")) return;

                AIHelper.ChangeBuildingAI(b, typeof(NewCemetaryAI));
            }
            catch (Exception e)
            {
                LogHelper.Information(e.ToString());
            }
        }
    }
}
 