using ColossalFramework;
using ICities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BetterCemeteryAI
{
    public class LoadingExtension : LoadingExtensionBase
    {
        public override void OnLevelLoaded(LoadMode mode)
        {
            try
            {
                Building[] buffer = Singleton<BuildingManager>.instance.m_buildings.m_buffer;
                for (ushort i = 0; i < buffer.Length; i++)
                {

                    if (buffer[i].Info == null) continue;

                    if (buffer[i].Info.m_class.name.Equals("DeathCare Facility"))
                    {
                        AIHelper.ApplyNewAIToBuilding(Singleton<BuildingManager>.instance.m_buildings.m_buffer[i]);
                    }
                }

                LogHelper.Information("Reloaded Mod");
            }
            catch (Exception e)
            {
                LogHelper.Information(e.ToString());
            }

            LogHelper.Information("Loaded Mod");
        }
    }
}
