using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;


namespace StageLightManeuver
{
    public static class SlmSettingsUtility
    {
        /// <summary>
        ///  <see cref="StageLightProfile"/>のデフォルトのエクスポート先のパス
        /// </summary>
        public const string BaseExportProfilePath = "Assets/StageLightManeuver/Profiles/<Scene>/<ClipName>";
        /// <summary>
        /// 全ての<see cref="SlmProperty"/>を含むリストを返す。このリストの順番が<see cref="StageLightProfile"/>のデフォルトの順番になる。
        /// </summary>
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
#if USE_VLB_ALTER
            typrof(GoboProperty),
#endif
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

        private static string? stageLightManeuverSettingsPath = _defaultStageLightManeuverSettingsPath;
        private const string _defaultStageLightManeuverSettingsPath = "Assets/StageLightManeuverSettings.asset";

        /// <summary>
        /// <see cref="StageLightManeuverSettings"/>のアセットを返す。無ければ作成する。
        /// </summary>
        public static StageLightManeuverSettings GetStageLightManeuverSettingsAsset()
        {
            var stageLightManeuverSettingsAsset =
                AssetDatabase.LoadAssetAtPath<StageLightManeuverSettings>(stageLightManeuverSettingsPath);
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
                    return stageLightManeuverSettingsAsset;
                }

                stageLightManeuverSettingsPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                stageLightManeuverSettingsAsset =
                    AssetDatabase.LoadAssetAtPath<StageLightManeuverSettings>(stageLightManeuverSettingsPath);
            }

            return stageLightManeuverSettingsAsset;
        }


        /// <summary>
        /// <paramref name="stageLightProperties"/>の順番を<paramref name="slmPropertyOrder"/>に従って登録する
        /// </summary>
        /// <returns>y</returns>
        private static List<SlmProperty> SetPropertyOrder(List<SlmProperty> stageLightProperties, Dictionary<Type, int> slmPropertyOrder)
        {
            // stageLightProperties.RemoveAll(x => x == null);
            foreach (var slmProperty in stageLightProperties)
            {
                if (slmProperty == null) continue;
                slmProperty.propertyOrder = slmPropertyOrder[slmProperty.GetType()];
            }

            return stageLightProperties;
        }

        private static Dictionary<Type, int> GetPropertyOrders()
        {
            Dictionary<Type, int> slmPropertyOrder = null;
            var stageLightManeuverSettingsAsset = GetStageLightManeuverSettingsAsset();
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