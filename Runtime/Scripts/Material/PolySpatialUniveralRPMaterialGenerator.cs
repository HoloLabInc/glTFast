// SPDX-FileCopyrightText: 2023 Unity Technologies and the glTFast authors
// SPDX-License-Identifier: Apache-2.0

#if USING_URP

using System;

using GLTFast.Schema;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

using Material = UnityEngine.Material;

namespace GLTFast.Materials
{

    public class PolySpatialUniversalRPMaterialGenerator : UniversalRPMaterialGenerator
    {
        static Shader s_MetallicCutoutShader;
        static Shader s_MetallicClearcoatCutoutShader;
        static Shader s_MetallicTransparentShader;
        static Shader s_MetallicClearcoatTransparentShader;

        public const string MetallicCutoutShader = "PolySpatial/glTF-pbrMetallicRoughness-cutout";
        public const string MetallicClearcoatCutoutShader = "PolySpatial/URP/glTF-pbrMetallicRoughness-Clearcoat-cutout";
        public const string MetallicTransparentShader = "PolySpatial/glTF-pbrMetallicRoughness-transparent";
        public const string MetallicClearcoatTransparentShader = "PolySpatial/URP/glTF-pbrMetallicRoughness-Clearcoat-transparent";

        private enum ShaderType
        {
            Metallic,
            MetallicClearCoat,
            MetallicCutout,
            MetallicClearCoatCutout,
            MetallicTransparent,
            MetallicClearCoatTransparent
        }

        public PolySpatialUniversalRPMaterialGenerator(UniversalRenderPipelineAsset renderPipelineAsset) : base(renderPipelineAsset) { }

        protected override Shader GetMetallicShader(MetallicShaderFeatures features, MaterialBase gltfMaterial = null)
        {
            var alphaMode = MaterialBase.AlphaMode.Opaque;
            if (gltfMaterial != null)
            {
                alphaMode = gltfMaterial.GetAlphaMode();
            }

            var isClearCoat = (features & MetallicShaderFeatures.ClearCoat) != 0;
            var shaderType = alphaMode switch
            {
                MaterialBase.AlphaMode.Mask => isClearCoat ? ShaderType.MetallicClearCoatCutout : ShaderType.MetallicCutout,
                MaterialBase.AlphaMode.Blend => isClearCoat ? ShaderType.MetallicClearCoatTransparent : ShaderType.MetallicTransparent,
                _ => isClearCoat ? ShaderType.MetallicClearCoat : ShaderType.Metallic,
            };

            var shader = GetShader(shaderType);

            if (shader != null)
            {
                return shader;
            }
            else
            {
                return base.GetMetallicShader(features, gltfMaterial);
            }
        }

        private Shader GetShader(ShaderType shaderType)
        {
            switch (shaderType)
            {
                case ShaderType.MetallicCutout:
                    if (s_MetallicCutoutShader == null)
                    {
                        s_MetallicCutoutShader = LoadShaderByName(MetallicCutoutShader);
                    }
                    return s_MetallicCutoutShader;

                case ShaderType.MetallicTransparent:
                    if (s_MetallicTransparentShader == null)
                    {
                        s_MetallicTransparentShader = LoadShaderByName(MetallicTransparentShader);
                    }
                    return s_MetallicTransparentShader;
                case ShaderType.MetallicClearCoatCutout:
                    if (s_MetallicClearcoatCutoutShader == null)
                    {
                        s_MetallicClearcoatCutoutShader = LoadShaderByName(MetallicClearcoatCutoutShader);
                    }
                    break;
                case ShaderType.MetallicClearCoatTransparent:
                    if (s_MetallicClearcoatTransparentShader == null)
                    {
                        s_MetallicClearcoatTransparentShader = LoadShaderByName(MetallicClearcoatTransparentShader);
                    }
                    break;
            }

            return null;
        }
    }
}
#endif // USING_URP
