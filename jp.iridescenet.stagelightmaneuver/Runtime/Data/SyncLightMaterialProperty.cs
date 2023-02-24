namespace StageLightManeuver
{
    public class SyncLightMaterialProperty:SlmAdditionalProperty
    {
        public SlmToggleValue<float> intensitymultiplier;
        public SlmToggleValue<bool> brightnessDecreasesToBlack;
        public SlmToggleValue<float> maxIntensityLimit;
        public SyncLightMaterialProperty()
        {
            propertyName = "Sync Light Material";
            propertyOverride = false;
            intensitymultiplier = new SlmToggleValue<float>() { value = 1f };
            brightnessDecreasesToBlack = new SlmToggleValue<bool>() { value = false };
            maxIntensityLimit = new SlmToggleValue<float>() { value = 3f };
            
        }
        
        public SyncLightMaterialProperty(SyncLightMaterialProperty other)
        {
            propertyName = other.propertyName;
            propertyOverride = other.propertyOverride;
            intensitymultiplier = new SlmToggleValue<float>(){value = other.intensitymultiplier.value};
            brightnessDecreasesToBlack = new SlmToggleValue<bool>() { value = other.brightnessDecreasesToBlack.value };
            maxIntensityLimit = new SlmToggleValue<float>() { value = other.maxIntensityLimit.value };
        }
    }
}