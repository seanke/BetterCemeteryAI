using ColossalFramework;
using ICities;
using System;
using System.Reflection;

namespace BetterCemeteryAI
{
    public class ThreadingExtension : ThreadingExtensionBase
    {
        int b = Singleton<SimulationManager>.instance.m_randomizer.Int32(10000u);
        int a = 0;

        public override void OnAfterSimulationTick()
        {
            ReloadIfRequired();
            //LogNumberOfStored();
        }

        private void LogNumberOfStored()
        {
            if (threadingManager.simulationTick % 512 == 0 && !threadingManager.simulationPaused)
            {
                int count = 0;

                Building[] buffer = Singleton<BuildingManager>.instance.m_buildings.m_buffer;
                for (ushort i = 0; i < buffer.Length; i++)
                {

                    if (buffer[i].Info == null) continue;

                    if (buffer[i].Info.name.Equals("Cemetery"))
                    {
                        var b = Singleton<BuildingManager>.instance.m_buildings.m_buffer[i];

                        count += b.m_customBuffer1;
                    }

                }
                LogHelper.Information(count.ToString());
            }
        }

        private void ReloadIfRequired()
        {
            if (a == b) return;
            a = b;

            try
            {
                Building[] buffer = Singleton<BuildingManager>.instance.m_buildings.m_buffer;
                for (ushort i = 0; i < buffer.Length; i++)
                {

                    if (buffer[i].Info == null) continue;

                    if (buffer[i].Info.name.Equals("Cemetery"))
                    {
                        var b = Singleton<BuildingManager>.instance.m_buildings.m_buffer[i];
                        AIHelper.ChangeBuildingAI(b, typeof(NewCemetaryAI));
                    }
                }

                LogHelper.Information("Reloaded Mod");
            }
            catch (Exception e)
            {
                LogHelper.Information(e.ToString());
            }
        }
    } 
}
