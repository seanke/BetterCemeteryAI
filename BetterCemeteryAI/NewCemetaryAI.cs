using ColossalFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BetterCemeteryAI
{
    public class NewCemetaryAI : CemeteryAI
    {
        protected override void ProduceGoods(ushort buildingID, ref Building buildingData, ref Building.Frame frameData, int productionRate, int finalProductionRate, ref Citizen.BehaviourData behaviour, int aliveWorkerCount, int totalWorkerCount, int workPlaceCount, int aliveVisitorCount, int totalVisitorCount, int visitPlaceCount)
        {
            base.ProduceGoods(buildingID, ref buildingData, ref frameData, productionRate, finalProductionRate, ref behaviour, aliveWorkerCount, totalWorkerCount, workPlaceCount, aliveVisitorCount, totalVisitorCount, visitPlaceCount);
            int num = productionRate * m_deathCareAccumulation / 100;
            if (num != 0)
            {
                Singleton<ImmaterialResourceManager>.instance.AddResource(ImmaterialResourceManager.Resource.DeathCare, num, buildingData.m_position, m_deathCareRadius);
            }
            if (finalProductionRate != 0)
            {
                int num2 = buildingData.m_customBuffer1;
                int num3 = (m_burialRate * finalProductionRate * 100 + m_corpseCapacity - 1) / m_corpseCapacity;
                CitizenManager instance = Singleton<CitizenManager>.instance;
                uint num4 = buildingData.m_citizenUnits;
                int num5 = 0;
                int num6 = 0;
                int num7 = 0;
                while (num4 != 0)
                {
                    uint nextUnit = instance.m_units.m_buffer[num4].m_nextUnit;
                    if ((instance.m_units.m_buffer[num4].m_flags & CitizenUnit.Flags.Visit) != 0)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            uint citizen = instance.m_units.m_buffer[num4].GetCitizen(i);
                            if (citizen != 0)
                            {
                                if (instance.m_citizens.m_buffer[citizen].Dead)
                                {
                                    if (instance.m_citizens.m_buffer[citizen].CurrentLocation == Citizen.Location.Visit)
                                    {
                                        if (Singleton<SimulationManager>.instance.m_randomizer.Int32(10000u) < num3)
                                        {
                                            instance.ReleaseCitizen(citizen);
                                            num7++;
                                            num2++;
                                        }
                                        else
                                        {
                                            num6++;
                                        }
                                    }
                                    else
                                    {
                                        num6++;
                                    }
                                }
                                else if (instance.m_citizens.m_buffer[citizen].Sick && instance.m_citizens.m_buffer[citizen].CurrentLocation == Citizen.Location.Visit)
                                {
                                    instance.m_citizens.m_buffer[citizen].Sick = false;
                                }
                            }
                        }
                    }
                    num4 = nextUnit;
                    if (++num5 > 524288)
                    {
                        CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                        break;
                    }
                }
                behaviour.m_deadCount += num6;
                if (m_graveCount == 0)
                {
                    for (int j = num7; j < num2; j++)
                    {
                        if (Singleton<SimulationManager>.instance.m_randomizer.Int32(10000u) < num3)
                        {
                            num7++;
                            num2--;
                        }
                        else
                        {
                            num6++;
                        }
                    }
                }
                buildingData.m_tempExport = (byte)Mathf.Min(buildingData.m_tempExport + num7, 255);
                DistrictManager instance2 = Singleton<DistrictManager>.instance;
                byte district = instance2.GetDistrict(buildingData.m_position);
                bool flag = IsFull(buildingID, ref buildingData);
                if (m_graveCount != 0)
                {
                    num2 = Mathf.Min(num2, m_graveCount);
                    buildingData.m_customBuffer1 = (ushort)num2;
                    instance2.m_districts.m_buffer[district].m_productionData.m_tempDeadAmount += (uint)num2;
                    instance2.m_districts.m_buffer[district].m_productionData.m_tempDeadCapacity += (uint)m_graveCount;
                }
                else
                {
                    buildingData.m_customBuffer1 = (ushort)num2;
                    instance2.m_districts.m_buffer[district].m_productionData.m_tempCremateCapacity += (uint)m_corpseCapacity;
                }
                bool flag2 = IsFull(buildingID, ref buildingData);
                if (flag != flag2)
                {
                    if (flag2)
                    {
                        if ((object)m_fullPassMilestone != null)
                        {
                            m_fullPassMilestone.Unlock();
                        }
                    }
                    else if ((object)m_fullPassMilestone != null)
                    {
                        m_fullPassMilestone.Relock();
                    }
                }
                int count = 0;
                int count2 = 0;
                int cargo = 0;
                int capacity = 0;
                int outside = 0;
                CalculateOwnVehicles(buildingID, ref buildingData, TransferManager.TransferReason.Dead, ref count, ref cargo, ref capacity, ref outside);
                CalculateGuestVehicles(buildingID, ref buildingData, TransferManager.TransferReason.DeadMove, ref count2, ref cargo, ref capacity, ref outside);
                int num8 = m_corpseCapacity - num6 - capacity;
                int num9 = (finalProductionRate * m_hearseCount + 99) / 100;
                if ((buildingData.m_flags & Building.Flags.Downgrading) != 0)
                {
                    if (m_graveCount != 0)
                    {
                        int count3 = 0;
                        int cargo2 = 0;
                        int capacity2 = 0;
                        int outside2 = 0;
                        CalculateOwnVehicles(buildingID, ref buildingData, TransferManager.TransferReason.DeadMove, ref count3, ref cargo2, ref capacity2, ref outside2);
                        if (num2 > 0 && count3 < num9)
                        {
                            TransferManager.TransferOffer offer = default(TransferManager.TransferOffer);
                            offer.Priority = 7;
                            offer.Building = buildingID;
                            offer.Position = buildingData.m_position;
                            offer.Amount = 1;
                            offer.Active = true;
                            Singleton<TransferManager>.instance.AddOutgoingOffer(TransferManager.TransferReason.DeadMove, offer);
                        }
                    }
                }
                else if (m_graveCount != 0)
                {
                    num8 = Mathf.Min(num8, m_graveCount - num2 - num6 - capacity + 9);
                    int num10 = num8 / 10;
                    if (count != 0)
                    {
                        num10--;
                    }
                    bool flag4 = num10 >= 1 && count < num9 && (num10 > 1);

                    if (flag4)
                    {
                        TransferManager.TransferOffer offer3 = default(TransferManager.TransferOffer);
                        offer3.Priority = 2 - count;
                        offer3.Building = buildingID;
                        offer3.Position = buildingData.m_position;
                        offer3.Amount = 1;
                        offer3.Active = true;
                        Singleton<TransferManager>.instance.AddIncomingOffer(TransferManager.TransferReason.Dead, offer3);
                    }

                    if(buildingData.m_customBuffer1 > 2000)
                    {
                        TransferManager.TransferOffer offer = default(TransferManager.TransferOffer);
                        offer.Priority = 0;
                        offer.Building = buildingID;
                        offer.Position = buildingData.m_position;
                        offer.Amount = 1;
                        offer.Active = true;
                        Singleton<TransferManager>.instance.AddOutgoingOffer(TransferManager.TransferReason.DeadMove, offer);
                    }
                }
                else
                {
                    int num11 = num8 / 10;
                    bool flag5 = num11 >= 1 && (num11 > 1 || count >= num9 || Singleton<SimulationManager>.instance.m_randomizer.Int32(2u) == 0);
                    bool flag6 = num11 >= 1 && count < num9 && (num11 > 1 || !flag5);
                    if (flag5)
                    {
                        TransferManager.TransferOffer offer4 = default(TransferManager.TransferOffer);
                        offer4.Priority = Mathf.Max(1, num8 * 6 / m_corpseCapacity);
                        offer4.Building = buildingID;
                        offer4.Position = buildingData.m_position;
                        offer4.Amount = Mathf.Max(1, num11 - 1);
                        offer4.Active = false;
                        Singleton<TransferManager>.instance.AddIncomingOffer(TransferManager.TransferReason.DeadMove, offer4);
                    }
                    if (flag6)
                    {
                        TransferManager.TransferOffer offer5 = default(TransferManager.TransferOffer);
                        offer5.Priority = 2 - count;
                        offer5.Building = buildingID;
                        offer5.Position = buildingData.m_position;
                        offer5.Amount = 1;
                        offer5.Active = true;
                        Singleton<TransferManager>.instance.AddIncomingOffer(TransferManager.TransferReason.Dead, offer5);
                    }
                }
            }
        }
    }
}
