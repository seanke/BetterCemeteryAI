using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BetterCemeteryAI
{
    public static class AIHelper
    {
        public static void ChangeBuildingAI(Building b, Type AIType)
        {
            var oldAI = b.Info.gameObject.GetComponent<PrefabAI>();
            UnityEngine.Object.DestroyImmediate(oldAI);
            var newAIInfo = new PrefabAIInfo(AIType);
            var newAI = (PrefabAI)b.Info.gameObject.AddComponent(newAIInfo.type);
            PrefabAIInfo.TryCopyAttributes(oldAI, newAI, false);
            b.Info.InitializePrefab();
        }
    }
}
