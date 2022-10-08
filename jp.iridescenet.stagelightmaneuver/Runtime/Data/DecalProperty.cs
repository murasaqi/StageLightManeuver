using System;
using UnityEngine;

namespace StageLightManeuver
{
    [Serializable]
    public class DecalProperty : SlmAdditionalProperty
    {
        public SlmToggleValue<Texture2D> decalTexture;
        public SlmToggleValue<float> decalSizeScaler;
        public SlmToggleValue<float> floorHeight;
        public SlmToggleValue<float> decalDepthScaler;
        public SlmToggleValue<float> fadeFactor;
        public SlmToggleValue<float> opacity;
        public DecalProperty()
        {
            propertyOverride = false;
            bpmOverrideData = new SlmToggleValue<BpmOverrideToggleValueBase>(){value = new BpmOverrideToggleValueBase()};
            propertyName = "Decal";
            decalTexture = new SlmToggleValue<Texture2D>();
            decalSizeScaler = new SlmToggleValue<float>(){value = 0.8f};
            floorHeight = new SlmToggleValue<float> { value = 0f };
            decalDepthScaler = new SlmToggleValue<float> { value = 1f };
            fadeFactor = new SlmToggleValue<float> { value = 1f };
            opacity = new SlmToggleValue<float> { value = 1f };
        }

        public override void ToggleOverride(bool toggle)
        {
            base.ToggleOverride(toggle);
       
            propertyOverride = toggle;
            decalTexture.propertyOverride = toggle;
            decalSizeScaler.propertyOverride = toggle;
            floorHeight.propertyOverride = toggle;
            decalDepthScaler.propertyOverride = toggle;
            fadeFactor.propertyOverride = toggle;
            opacity.propertyOverride = toggle;
            
        }

        public DecalProperty(DecalProperty other)
        {
            propertyOverride = other.propertyOverride;
            bpmOverrideData = other.bpmOverrideData;
            decalTexture = new SlmToggleValue<Texture2D>(other.decalTexture);
            propertyName = other.propertyName;
            decalSizeScaler = new SlmToggleValue<float>(other.decalSizeScaler);
            floorHeight = new SlmToggleValue<float>(other.floorHeight);
            decalDepthScaler = new SlmToggleValue<float>(other.decalDepthScaler);
            fadeFactor = new SlmToggleValue<float>(other.fadeFactor);
            opacity = new SlmToggleValue<float>(other.opacity);
        }

    }
}