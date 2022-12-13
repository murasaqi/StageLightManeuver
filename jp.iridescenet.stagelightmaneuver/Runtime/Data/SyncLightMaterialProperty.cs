namespace StageLightManeuver
{
    public class SyncLightMaterialProperty:SlmAdditionalProperty
    {
        public SlmToggleValue<float> intensitymultiplier;
        public SlmToggleValue<bool> brightnessDecreasesToBlack;
        public SyncLightMaterialProperty()
        {
            propertyName = "Sync Light Material";
            propertyOverride = false;
            intensitymultiplier = new SlmToggleValue<float>() { value = 1f };
            brightnessDecreasesToBlack = new SlmToggleValue<bool>() { value = false };
            
        }
        
        public SyncLightMaterialProperty(SyncLightMaterialProperty other)
        {
            propertyName = other.propertyName;
            propertyOverride = other.propertyOverride;
            intensitymultiplier = new SlmToggleValue<float>(){value = other.intensitymultiplier.value};
            brightnessDecreasesToBlack = new SlmToggleValue<bool>() { value = other.brightnessDecreasesToBlack.value };
        }
    }
}