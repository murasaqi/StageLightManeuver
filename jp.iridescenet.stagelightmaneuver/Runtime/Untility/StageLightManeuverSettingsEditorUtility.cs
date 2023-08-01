using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;


namespace StageLightManeuver
{
    public static class StageLightManeuverSettingsUtility
    {
        /// <summary>
        /// 全ての<see cref="SlmProperty"/>を含むリストを返す。このリストの順番が<see cref="StageLightProfile"/>のデフォルトの順番になる。
        /// </summary>
        // Clock->StageLightOrder->Light自体の設定->ライトの動き の順に
        public static List<Type> SlmPropertys = new List<Type>
        {
            typeof(ClockProperty),
            typeof(StageLightOrderProperty),
            typeof(LightProperty),
            typeof(LightIntensityProperty),
            typeof(LightColorProperty),
            typeof(MaterialColorProperty),
            typeof(MaterialTextureProperty),
            typeof(SyncLightMaterialProperty),
            typeof(DecalProperty),
            typeof(XTransformProperty),
            typeof(YTransformProperty),
            typeof(ZTransformProperty),
            typeof(RotationProperty),
            typeof(PanProperty),
            typeof(TiltProperty),
            typeof(ManualPanTiltProperty),
            typeof(LookAtProperty),
            typeof(SmoothLookAtProperty),
            typeof(EnvironmentProperty),
            typeof(ReflectionProbeProperty),
            typeof(SlmBarLightProperty),
            typeof(BarLightIntensityProperty),
            typeof(BarLightPanProperty),
            typeof(LightArrayProperty),
            typeof(ManualLightArrayProperty),
            typeof(ManualColorArrayProperty),
            typeof(SlmAdditionalProperty),
        };

        public static string? stageLightManeuverSettingsPath = _defaultStageLightManeuverSettingsPath;
        private const string _defaultStageLightManeuverSettingsPath = "Assets/StageLightManeuverSettings.asset";

        /// <summary>
        /// <paramref name="stageLightProperties"/>の順番を<paramref name="slmPropertyOrder"/>に従って登録する
        /// </summary>
        public static List<SlmProperty> SetPropertyOrder(List<SlmProperty> stageLightProperties, Dictionary<Type, int> slmPropertyOrder)
        {
            for (int i = 0; i < stageLightProperties.Count; i++)
            {
                var slmProperty = stageLightProperties[i];
                slmProperty.propertyOrder = slmPropertyOrder[slmProperty.GetType()];
                // Debug.Log(slmProperty.propertyName + "'s order is " + slmProperty.propertyOrder);
            }
            return stageLightProperties;
        }

        public static Dictionary<Type, int> GetPropertyOrders()
        {
            Dictionary<Type, int> slmPropertyOrder = null;
            var stageLightManeuverSettingsAsset = AssetDatabase.LoadAssetAtPath<StageLightManeuverSettings>(stageLightManeuverSettingsPath);
            if (stageLightManeuverSettingsAsset == null)
            {
                var guids = AssetDatabase.FindAssets("t:StageLightManeuverSettings");
                // Debug.Log([StageLightManeuverSettings] Search Settings asset");
                if (guids.Length <= 0)
                {
                    var slmSettings = StageLightManeuverSettings.CreateInstance<StageLightManeuverSettings>();
                    stageLightManeuverSettingsPath = _defaultStageLightManeuverSettingsPath;
                    AssetDatabase.CreateAsset(slmSettings, _defaultStageLightManeuverSettingsPath);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    return slmPropertyOrder = slmSettings.SlmPropertyOrder;
                }
                stageLightManeuverSettingsPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                stageLightManeuverSettingsAsset = AssetDatabase.LoadAssetAtPath<StageLightManeuverSettings>(stageLightManeuverSettingsPath);
            }
            slmPropertyOrder = stageLightManeuverSettingsAsset.SlmPropertyOrder;
            return slmPropertyOrder;
        }

        /// <summary>
        /// <paramref name="stageLightProperties"/>に<see cref="StageLightManeuverSettings"/>の設定を適用して並び変えたリストを返す
        /// </summary>
        public static List<SlmProperty> SortByPropertyOrder(List<SlmProperty> stageLightProperties, in Dictionary<Type, int> slmPropertyOrder)
        {
            stageLightProperties = SetPropertyOrder(stageLightProperties, slmPropertyOrder);
            stageLightProperties.Sort((x, y) => x.propertyOrder.CompareTo(y.propertyOrder));
            return stageLightProperties;
        }

        /// <summary>
        /// <paramref name="stageLightProperties"/>に<see cref="StageLightManeuverSettings"/>の設定を適用して並び変えたリストを返す
        /// </summary>
        public static List<SlmProperty> SortByPropertyOrder(List<SlmProperty> stageLightProperties)
        {
            var slmPropertyOrder = GetPropertyOrders();
            stageLightProperties = SetPropertyOrder(stageLightProperties, slmPropertyOrder);
            stageLightProperties.Sort((x, y) => x.propertyOrder.CompareTo(y.propertyOrder));
            return stageLightProperties;
        }
    }
}