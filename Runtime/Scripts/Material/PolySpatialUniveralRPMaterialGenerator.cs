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

        static Shader s_UnlitCutoutShader;
        static Shader s_UnlitTransparentShader;

        static Shader s_SpecularCutoutShader;
        static Shader s_SpecularTransparentShader;

        public const string MetallicCutoutShader = "PolySpatial/glTF-pbrMetallicRoughness-cutout";
        public const string MetallicClearcoatCutoutShader = "PolySpatial/URP/glTF-pbrMetallicRoughness-Clearcoat-cutout";
        public const string MetallicTransparentShader = "PolySpatial/glTF-pbrMetallicRoughness-transparent";
        public const string MetallicClearcoatTransparentShader = "PolySpatial/URP/glTF-pbrMetallicRoughness-Clearcoat-transparent";

        public const string UnlitCutoutShader = "PolySpatial/glTF-Unlit-cutout";
        public const string UnlitTransparentShader = "PolySpatial/glTF-Unlit-transparent";

        public const string SpecularCutoutShader = "PolySpatial/glTF-pbrSpecularGlossiness-cutout";
        public const string SpecularTransparentShader = "PolySpatial/glTF-pbrSpecularGlossiness-transparent";

        private enum ShaderType
        {
            Metallic,
            MetallicClearCoat,
            MetallicCutout,
            MetallicClearCoatCutout,
            MetallicTransparent,
            MetallicClearCoatTransparent,

            Unlit,
            UnlitCutout,
            UnlitTransparent,

            Specular,
            SpecularCutout,
            SpecularTransparent,
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

        protected override Shader GetUnlitShader(MaterialBase gltfMaterial)
        {
            var alphaMode = gltfMaterial.GetAlphaMode();
            var shaderType = alphaMode switch
            {
                MaterialBase.AlphaMode.Mask => ShaderType.UnlitCutout,
                MaterialBase.AlphaMode.Blend => ShaderType.UnlitTransparent,
                _ => ShaderType.Unlit,
            };

            var shader = GetShader(shaderType);
            if (shader != null)
            {
                return shader;
            }
            else
            {
                return base.GetUnlitShader(gltfMaterial);
            }
        }

        protected override Shader GetSpecularShader(SpecularShaderFeatures features, MaterialBase gltfMaterial = null)
        {
            var alphaMode = MaterialBase.AlphaMode.Opaque;
            if (gltfMaterial != null)
            {
                alphaMode = gltfMaterial.GetAlphaMode();
            }

            var shaderType = alphaMode switch
            {
                MaterialBase.AlphaMode.Mask => ShaderType.SpecularCutout,
                MaterialBase.AlphaMode.Blend => ShaderType.SpecularTransparent,
                _ => ShaderType.Specular,
            };

            var shader = GetShader(shaderType);

            if (shader != null)
            {
                return shader;
            }
            else
            {
                return base.GetSpecularShader(features, gltfMaterial);
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

                case ShaderType.UnlitCutout:
                    if (s_UnlitCutoutShader == null)
                    {
                        s_UnlitCutoutShader = LoadShaderByName(UnlitCutoutShader);
                    }
                    return s_UnlitCutoutShader;
                case ShaderType.UnlitTransparent:
                    if (s_UnlitTransparentShader == null)
                    {
                        s_UnlitTransparentShader = LoadShaderByName(UnlitTransparentShader);
                    }
                    return s_UnlitTransparentShader;

                case ShaderType.SpecularCutout:
                    if (s_SpecularCutoutShader == null)
                    {
                        s_SpecularCutoutShader = LoadShaderByName(SpecularCutoutShader);
                    }
                    return s_SpecularCutoutShader;
                case ShaderType.SpecularTransparent:
                    if (s_SpecularTransparentShader == null)
                    {
                        s_SpecularTransparentShader = LoadShaderByName(SpecularTransparentShader);
                    }
                    return s_SpecularTransparentShader;
            }

            return null;
        }
    }
}
#endif // USING_URP
