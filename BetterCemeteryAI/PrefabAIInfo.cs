/** PrefabAIChanger
 * @author Stefan Kaufhold
 * @author D Lue Choy
 * 
 * Copyright (c) 2015, All rights reserved.
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either 
 * version 3.0 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  
 * See the GNU Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser 
 * General Public License along with this library.
 **/

using System;
using System.Collections.Generic;
using System.Reflection;

namespace BetterCemeteryAI
{
    public class PrefabAIInfo
    {
        public PrefabAIInfo(Type aiType)
        {
            type = aiType;

            if (!type.IsSubclassOf(typeof(PrefabAI))) throw new PrefabAITypeException();

            if (type.IsSubclassOf(typeof(BuildingAI)))
            {
                aiGroup = PrefabAIGroup.BuildingAI;
            }
            else if (type.IsSubclassOf(typeof(CitizenAI)))
            {
                aiGroup = PrefabAIGroup.CitizenAI;
            }
            else if (type.IsSubclassOf(typeof(NetAI)))
            {
                aiGroup = PrefabAIGroup.NetAI;
            }
            else if (type.IsSubclassOf(typeof(VehicleAI)))
            {
                aiGroup = PrefabAIGroup.VehicleAI;
            }
            else
            {
                aiGroup = PrefabAIGroup.Other;
            }

            /*var derived = type;
            do
            {
                derived = derived.BaseType;
                if (derived != null)
                {
                    // temp.fullname.Insert(0,derived.FullName);
                    temp.fullname = String.Concat(derived.FullName, ".", temp.fullname);
                }
            } while (derived != null);*/
        }

        public readonly Type type; //type of Prefab AI
        public readonly string fullname; //not an actual valid fullname, but an approximation used for sorting
        public readonly PrefabAIGroup aiGroup;

        public static void TryCopyAttributes(PrefabAI src, PrefabAI dst, bool safe = true)
        {
            var oldAIFields = src.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            var newAIFields = dst.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);

            var newAIFieldDic = new Dictionary<string, FieldInfo>(newAIFields.Length);
            foreach (var field in newAIFields)
            {
                newAIFieldDic.Add(field.Name, field);
            }

            foreach (var fieldInfo in oldAIFields)
            {
                // do not copy attributes marked NonSerialized
                bool copyField = !fieldInfo.IsDefined(typeof(NonSerializedAttribute), true);
                
                if (safe && !fieldInfo.IsDefined(typeof(CustomizablePropertyAttribute), true)) copyField = false;

                if (copyField)
                {
                    FieldInfo newAIField;
                    newAIFieldDic.TryGetValue(fieldInfo.Name, out newAIField);
                    try
                    {
                        if (newAIField != null && newAIField.GetType().Equals(fieldInfo.GetType()))
                        {
                            newAIField.SetValue(dst, fieldInfo.GetValue(src));
                        }
                    }
                    catch (NullReferenceException)
                    {
                    }
                }
            }
        }
    }

    /// <summary>
    /// Thrown when a Type is unexpectedly not a subclass of PrefabAI
    /// </summary>
    public class PrefabAITypeException : Exception { }

    public enum PrefabAIGroup
    {
        Other,
        BuildingAI,
        CitizenAI,
        NetAI,
        VehicleAI
    }
}