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
                { value = new BpmOverrideToggleValueBase()
                {
                    bpmOverride =   materialTextureProperty.bpmOverrideData.value.bpmOverride,
                    bpmScale =    materialTextureProperty.bpmOverrideData.value.bpmScale,
                    childStagger =  materialTextureProperty.bpmOverrideData.value.childStagger,
                    loopType =  materialTextureProperty.bpmOverrideData.value.loopType,
                    offsetTime =    materialTextureProperty.bpmOverrideData.value.offsetTime,
                    propertyOverride =  materialTextureProperty.bpmOverrideData.value.propertyOverride,
                } };
            texturePropertyName = new SlmToggleValue<string>(materialTextureProperty.texturePropertyName);
            materialindex = new SlmToggleValue<int>(materialTextureProperty.materialindex);
            texture = new SlmToggleValue<Texture2D>()
            {
                value = materialTextureProperty.texture.value
            };
        }
    }
}