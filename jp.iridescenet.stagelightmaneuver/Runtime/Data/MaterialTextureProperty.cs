using UnityEngine;

namespace StageLightManeuver
{
    public class MaterialTextureProperty:SlmAdditionalProperty
    {
        public SlmToggleValue<string> texturePropertyName;
        public SlmToggleValue<int> materialindex;
        public SlmToggleValue<Texture2D> texture;
        
        public  MaterialTextureProperty()
        {
            propertyName = "Material Texture";
            bpmOverrideData = new SlmToggleValue<BpmOverrideToggleValueBase>()
                { value = new BpmOverrideToggleValueBase() };
            texturePropertyName = new SlmToggleValue<string>(){value = "_Texture"};
            materialindex = new SlmToggleValue<int>() {value = 0};
            texture = new SlmToggleValue<Texture2D>(){value = Texture2D.whiteTexture};
        }

        public MaterialTextureProperty(MaterialTextureProperty materialTextureProperty)
        {
            propertyName = materialTextureProperty.propertyName;
            bpmOverrideData = new SlmToggleValue<BpmOverrideToggleValueBase>()
            {
                propertyOverride =  materialTextureProperty.bpmOverrideData.propertyOverride,
                value = new BpmOverrideToggleValueBase(materialTextureProperty.bpmOverrideData.value)
            };
            texturePropertyName = new SlmToggleValue<string>(materialTextureProperty.texturePropertyName);
            materialindex = new SlmToggleValue<int>(materialTextureProperty.materialindex);
            texture = new SlmToggleValue<Texture2D>()
            {
                value = materialTextureProperty.texture.value
            };
        }
    }
}