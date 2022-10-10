namespace StageLightManeuver
{
    public class SyncLightMaterialProperty:SlmAdditionalProperty
    {
        public SlmToggleValue<float> intensitymultiplier;

        public SyncLightMaterialProperty()
        {
            propertyName = "Sync Light Material";
            propertyOverride = false;
            intensitymultiplier = new SlmToggleValue<float>() { value = 1f };
        }
        
        public SyncLightMaterialProperty(SyncLightMaterialProperty other)
        {
            propertyName = other.propertyName;
            propertyOverride = other.propertyOverride;
            intensitymultiplier = new SlmToggleValue<float>(){value = other.intensitymultiplier.value};
        }
    }
}