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
            clockOverride = new SlmToggleValue<ClockOverride>();
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
            clockOverride = new SlmToggleValue<ClockOverride>(other.clockOverride);
            decalTexture = new SlmToggleValue<Texture2D>()
            {
                propertyOverride = other.decalTexture.propertyOverride,
                value = other.decalTexture.value,
            };
            propertyName = other.propertyName;
            decalSizeScaler = new SlmToggleValue<float>(other.decalSizeScaler);
            floorHeight = new SlmToggleValue<float>(other.floorHeight);
            decalDepthScaler = new SlmToggleValue<float>(other.decalDepthScaler);
            fadeFactor = new SlmToggleValue<float>(other.fadeFactor);
            opacity = new SlmToggleValue<float>(other.opacity);
            
            Debug.Log("DecalProperty Created");
        }


        public override void OverwriteProperty(SlmProperty other)
        {
            base.OverwriteProperty(other);
            var decalProperty = other as DecalProperty;
            if (decalProperty == null) return;
            if (decalProperty.propertyOverride)
            {
                if(decalProperty.decalTexture.propertyOverride) decalTexture.value = decalProperty.decalTexture.value;
                if(decalProperty.decalSizeScaler.propertyOverride) decalSizeScaler.value = decalProperty.decalSizeScaler.value;
                if(decalProperty.floorHeight.propertyOverride) floorHeight.value = decalProperty.floorHeight.value;
                if(decalProperty.decalDepthScaler.propertyOverride) decalDepthScaler.value = decalProperty.decalDepthScaler.value;
                if(decalProperty.fadeFactor.propertyOverride) fadeFactor.value = decalProperty.fadeFactor.value;
                if(decalProperty.opacity.propertyOverride) opacity.value = decalProperty.opacity.value;
                
            }
        }
    }
}