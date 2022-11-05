using System;
using System.Collections.Generic;
using UnityEngine;

namespace StageLightManeuver
{
    
  

    [Serializable]
    public class LightPrimitiveData
    {
        public float intensity;
        public Color lightColor;
        public float outerSpotAngle;
        public float innerSpotAngle;
        public float range;

        public LightPrimitiveData()
        {
            intensity = 1;
            lightColor = new Color(1, 1, 1, 1);
            outerSpotAngle = 30;
            innerSpotAngle = 10;
            range = 10;
        }

        public void Clear()
        {
            intensity = 0;
            lightColor = new Color(0, 0, 0, 0);
            outerSpotAngle = 0;
            innerSpotAngle = 0;
            range = 0;
        }
        public LightPrimitiveData(LightPrimitiveData lightPrimitiveData)
        {
            intensity = lightPrimitiveData.intensity;
            lightColor = new Color(lightPrimitiveData.lightColor.r, lightPrimitiveData.lightColor.g, lightPrimitiveData.lightColor.b, lightPrimitiveData.lightColor.a);
            outerSpotAngle = lightPrimitiveData.outerSpotAngle;
            innerSpotAngle = lightPrimitiveData.innerSpotAngle;
            range = lightPrimitiveData.range;
        }
    }
    
    [ExecuteAlways]
    public class ManualLightArrayFixture:StageLightFixtureBase
    {
        public List<LightPrimitiveData> lightPrimitiveDataList = new List<LightPrimitiveData>();
        public List<Light> lights = new List<Light>();

        public override void Init()
        {
            lightPrimitiveDataList.Clear();
            foreach (var light in lights)
            {
               lightPrimitiveDataList.Add(new LightPrimitiveData());
            }
            base.Init();
        }

        private void OnValidate()
        {
        }

        private void ClearLightPrimitiveProperties()
        {
            foreach (var lightPrimitiveProperty in lightPrimitiveDataList)
            {
                lightPrimitiveProperty.Clear();
            }
        }
        public override void EvaluateQue(float currentTime)
        {
            base.EvaluateQue(currentTime);

            while (stageLightDataQueue.Count > 0)
            {
                ClearLightPrimitiveProperties();
                var data = stageLightDataQueue.Dequeue();
                var timeProperty = data.TryGet<TimeProperty>() as TimeProperty;
                var lightArrayProperty = data.TryGet<LightArrayProperty>() as LightArrayProperty;

                var weight = data.weight;
                if (lightArrayProperty == null || timeProperty == null) continue;

                var baseTime = GetNormalizedTime(currentTime, timeProperty.bpm.value, timeProperty.offsetTime.value,
                    timeProperty.bpmScale.value, timeProperty.clipProperty, timeProperty.loopType.value);

                var t = lightArrayProperty.bpmOverrideData.value.bpmOverride
                    ? GetNormalizedTime(currentTime, data, typeof(LightArrayProperty))
                    : baseTime;

                if (lightPrimitiveDataList.Count == lightArrayProperty.lightToggleValues.value.Count)
                {
                    for (int i = 0; i < lightPrimitiveDataList.Count; i++)
                    {
                        var lightPrimitiveData = lightPrimitiveDataList[i];
                        var lightToggleValue = lightArrayProperty.lightToggleValues.value[i];
                        lightPrimitiveData.intensity += lightToggleValue.intensity * weight;
                        lightPrimitiveData.lightColor += lightToggleValue.lightColor * weight;
                        lightPrimitiveData.outerSpotAngle += lightToggleValue.outerSpotAngle * weight;
                        lightPrimitiveData.innerSpotAngle += lightToggleValue.innerSpotAngle * weight;
                        lightPrimitiveData.range += lightToggleValue.range * weight;
                    }
                }
            }
        }

        public void Update()
        {
            if (lights.Count != lightPrimitiveDataList.Count)
            {
                var diff = lights.Count - lightPrimitiveDataList.Count;
                if (diff > 0)
                {
                    for (int i = 0; i < diff; i++)
                    {
                        lightPrimitiveDataList.Add(new LightPrimitiveData());
                    }
                }
                else
                {
                    for (int i = 0; i < -diff; i++)
                    {
                        lightPrimitiveDataList.RemoveAt(lightPrimitiveDataList.Count - 1);
                    }
                }
            }
        }

        public override void UpdateFixture()
        {
            if(lights.Count == lightPrimitiveDataList.Count)
            {
                for (int i = 0; i < lights.Count; i++)
                {
                    var light = lights[i];
                    var lightPrimitiveData = lightPrimitiveDataList[i];
                    light.intensity = lightPrimitiveData.intensity;
                    light.color = lightPrimitiveData.lightColor;
                    light.spotAngle = lightPrimitiveData.outerSpotAngle;
                    light.innerSpotAngle = lightPrimitiveData.innerSpotAngle;
                    light.range = lightPrimitiveData.range;
                }
            }
        }
    }
}