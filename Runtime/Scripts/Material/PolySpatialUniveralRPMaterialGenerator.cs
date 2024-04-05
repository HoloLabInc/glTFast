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
        static Shader s_MetallicDoubleShader;
        static Shader s_MetallicCutoutShader;
        static Shader s_MetallicCutoutDoubleShader;
        static Shader s_MetallicTransparentShader;
        static Shader s_MetallicTransparentDoubleShader;

        static Shader s_MetallicClearcoatDoubleShader;
        static Shader s_MetallicClearcoatCutoutShader;
        static Shader s_MetallicClearcoatCutoutDoubleShader;
        static Shader s_MetallicClearcoatTransparentShader;
        static Shader s_MetallicClearcoatTransparentDoubleShader;

        static Shader s_UnlitDoubleShader;
        static Shader s_UnlitCutoutShader;
        static Shader s_UnlitCutoutDoubleShader;
        static Shader s_UnlitTransparentShader;
        static Shader s_UnlitTransparentDoubleShader;

        static Shader s_SpecularDoubleShader;
        static Shader s_SpecularCutoutShader;
        static Shader s_SpecularCutoutDoubleShader;
        static Shader s_SpecularTransparentShader;
        static Shader s_SpecularTransparentDoubleShader;

        public const string MetallicDoubleShader = "PolySpatial/glTF-pbrMetallicRoughness-double";
        public const string MetallicCutoutShader = "PolySpatial/glTF-pbrMetallicRoughness-cutout";
        public const string MetallicCutoutDoubleShader = "PolySpatial/glTF-pbrMetallicRoughness-cutout-double";
        public const string MetallicTransparentShader = "PolySpatial/glTF-pbrMetallicRoughness-transparent";
        public const string MetallicTransparentDoubleShader = "PolySpatial/glTF-pbrMetallicRoughness-transparent-double";

        public const string MetallicClearCoatDoubleShader = "PolySpatial/URP/glTF-pbrMetallicRoughness-Clearcoat-double";
        public const string MetallicClearcoatCutoutShader = "PolySpatial/URP/glTF-pbrMetallicRoughness-Clearcoat-cutout";
        public const string MetallicClearcoatCutoutDoubleShader = "PolySpatial/URP/glTF-pbrMetallicRoughness-Clearcoat-cutout-double";
        public const string MetallicClearcoatTransparentShader = "PolySpatial/URP/glTF-pbrMetallicRoughness-Clearcoat-transparent";
        public const string MetallicClearcoatTransparentDoubleShader = "PolySpatial/URP/glTF-pbrMetallicRoughness-Clearcoat-transparent-double";

        public const string UnlitDoubleShader = "PolySpatial/glTF-Unlit-double";
        public const string UnlitCutoutShader = "PolySpatial/glTF-Unlit-cutout";
        public const string UnlitCutoutDoubleShader = "PolySpatial/glTF-Unlit-cutout-double";
        public const string UnlitTransparentShader = "PolySpatial/glTF-Unlit-transparent";
        public const string UnlitTransparentDoubleShader = "PolySpatial/glTF-Unlit-transparent-double";

        public const string SpecularDoubleShader = "PolySpatial/glTF-pbrSpecularGlossiness-double";
        public const string SpecularCutoutShader = "PolySpatial/glTF-pbrSpecularGlossiness-cutout";
        public const string SpecularCutoutDoubleShader = "PolySpatial/glTF-pbrSpecularGlossiness-cutout-double";
        public const string SpecularTransparentShader = "PolySpatial/glTF-pbrSpecularGlossiness-transparent";
        public const string SpecularTransparentDoubleShader = "PolySpatial/glTF-pbrSpecularGlossiness-transparent-double";

        private enum ShaderType
        {
            Metallic,
            MetallicDouble,
            MetallicCutout,
            MetallicCutoutDouble,
            MetallicTransparent,
            MetallicTransparentDouble,

            MetallicClearCoat,
            MetallicClearCoatDouble,
            MetallicClearCoatCutout,
            MetallicClearCoatCutoutDouble,
            MetallicClearCoatTransparent,
            MetallicClearCoatTransparentDouble,

            Unlit,
            UnlitDouble,
            UnlitCutout,
            UnlitCutoutDouble,
            UnlitTransparent,
            UnlitTransparentDouble,

            Specular,
            SpecularDouble,
            SpecularCutout,
            SpecularCutoutDouble,
            SpecularTransparent,
            SpecularTransparentDouble,
        }

        public PolySpatialUniversalRPMaterialGenerator(UniversalRenderPipelineAsset renderPipelineAsset) : base(renderPipelineAsset) { }

        protected override Shader GetMetallicShader(MetallicShaderFeatures features, MaterialBase gltfMaterial = null)
        {
            var alphaMode = MaterialBase.AlphaMode.Opaque;
            if (gltfMaterial != null)
            {
                alphaMode = gltfMaterial.GetAlphaMode();
            }

            var doubleSided = (features & MetallicShaderFeatures.DoubleSided) != 0;
            var isClearCoat = (features & MetallicShaderFeatures.ClearCoat) != 0;

            ShaderType shaderType;
            if (isClearCoat)
            {
                shaderType = alphaMode switch
                {
                    MaterialBase.AlphaMode.Mask => doubleSided ? ShaderType.MetallicCutoutDouble : ShaderType.MetallicCutout,
                    MaterialBase.AlphaMode.Blend => doubleSided ? ShaderType.MetallicTransparentDouble : ShaderType.MetallicTransparent,
                    _ => doubleSided ? ShaderType.MetallicDouble : ShaderType.Metallic,
                };
            }
            else
            {
                shaderType = alphaMode switch
                {
                    MaterialBase.AlphaMode.Mask => doubleSided ? ShaderType.MetallicClearCoatCutoutDouble : ShaderType.MetallicClearCoatCutout,
                    MaterialBase.AlphaMode.Blend => doubleSided ? ShaderType.MetallicClearCoatTransparentDouble : ShaderType.MetallicClearCoatTransparent,
                    _ => doubleSided ? ShaderType.MetallicClearCoatDouble : ShaderType.MetallicClearCoat,
                };
            }

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
            var doubleSided = gltfMaterial.doubleSided;
            var shaderType = alphaMode switch
            {
                MaterialBase.AlphaMode.Mask => doubleSided ? ShaderType.UnlitCutoutDouble : ShaderType.UnlitCutout,
                MaterialBase.AlphaMode.Blend => doubleSided ? ShaderType.UnlitTransparentDouble : ShaderType.UnlitTransparent,
                _ => doubleSided ? ShaderType.UnlitDouble : ShaderType.Unlit,
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

            var doubleSided = (features & SpecularShaderFeatures.DoubleSided) != 0;

            var shaderType = alphaMode switch
            {
                MaterialBase.AlphaMode.Mask => doubleSided ? ShaderType.SpecularCutoutDouble : ShaderType.SpecularCutout,
                MaterialBase.AlphaMode.Blend => doubleSided ? ShaderType.SpecularTransparentDouble : ShaderType.SpecularTransparent,
                _ => doubleSided ? ShaderType.SpecularDouble : ShaderType.Specular,
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
                case ShaderType.Metallic:
                    return null;

                case ShaderType.MetallicDouble:
                    if (s_MetallicDoubleShader == null)
                    {
                        s_MetallicDoubleShader = LoadShaderByName(MetallicDoubleShader);
                    }
                    return s_MetallicDoubleShader;

                case ShaderType.MetallicCutout:
                    if (s_MetallicCutoutShader == null)
                    {
                        s_MetallicCutoutShader = LoadShaderByName(MetallicCutoutShader);
                    }
                    return s_MetallicCutoutShader;

                case ShaderType.MetallicCutoutDouble:
                    if (s_MetallicCutoutDoubleShader == null)
                    {
                        s_MetallicCutoutDoubleShader = LoadShaderByName(MetallicCutoutDoubleShader);
                    }
                    return s_MetallicCutoutDoubleShader;

                case ShaderType.MetallicTransparent:
                    if (s_MetallicTransparentShader == null)
                    {
                        s_MetallicTransparentShader = LoadShaderByName(MetallicTransparentShader);
                    }
                    return s_MetallicTransparentShader;

                case ShaderType.MetallicTransparentDouble:
                    if (s_MetallicTransparentDoubleShader == null)
                    {
                        s_MetallicTransparentDoubleShader = LoadShaderByName(MetallicTransparentDoubleShader);
                    }
                    return s_MetallicTransparentDoubleShader;

                case ShaderType.MetallicClearCoat:
                    return null;

                case ShaderType.MetallicClearCoatDouble:
                    if (s_MetallicClearcoatDoubleShader == null)
                    {
                        s_MetallicClearcoatDoubleShader = LoadShaderByName(MetallicClearCoatDoubleShader);
                    }
                    return s_MetallicClearcoatDoubleShader;

                case ShaderType.MetallicClearCoatCutout:
                    if (s_MetallicClearcoatCutoutShader == null)
                    {
                        s_MetallicClearcoatCutoutShader = LoadShaderByName(MetallicClearcoatCutoutShader);
                    }
                    return s_MetallicClearcoatCutoutShader;

                case ShaderType.MetallicClearCoatCutoutDouble:
                    if (s_MetallicClearcoatCutoutDoubleShader == null)
                    {
                        s_MetallicClearcoatCutoutDoubleShader = LoadShaderByName(MetallicClearcoatCutoutDoubleShader);
                    }
                    return s_MetallicClearcoatCutoutDoubleShader;

                case ShaderType.MetallicClearCoatTransparent:
                    if (s_MetallicClearcoatTransparentShader == null)
                    {
                        s_MetallicClearcoatTransparentShader = LoadShaderByName(MetallicClearcoatTransparentShader);
                    }
                    return s_MetallicClearcoatTransparentShader;

                case ShaderType.MetallicClearCoatTransparentDouble:
                    if (s_MetallicClearcoatTransparentDoubleShader == null)
                    {
                        s_MetallicClearcoatTransparentDoubleShader = LoadShaderByName(MetallicClearcoatTransparentDoubleShader);
                    }
                    return s_MetallicClearcoatTransparentDoubleShader;

                case ShaderType.Unlit:
                    return null;

                case ShaderType.UnlitDouble:
                    if (s_UnlitDoubleShader == null)
                    {
                        s_UnlitDoubleShader = LoadShaderByName(UnlitDoubleShader);
                    }
                    return s_UnlitDoubleShader;

                case ShaderType.UnlitCutout:
                    if (s_UnlitCutoutShader == null)
                    {
                        s_UnlitCutoutShader = LoadShaderByName(UnlitCutoutShader);
                    }
                    return s_UnlitCutoutShader;

                case ShaderType.UnlitCutoutDouble:
                    if (s_UnlitCutoutDoubleShader == null)
                    {
                        s_UnlitCutoutDoubleShader = LoadShaderByName(UnlitCutoutDoubleShader);
                    }
                    return s_UnlitCutoutDoubleShader;

                case ShaderType.UnlitTransparent:
                    if (s_UnlitTransparentShader == null)
                    {
                        s_UnlitTransparentShader = LoadShaderByName(UnlitTransparentShader);
                    }
                    return s_UnlitTransparentShader;

                case ShaderType.UnlitTransparentDouble:
                    if (s_UnlitTransparentDoubleShader == null)
                    {
                        s_UnlitTransparentDoubleShader = LoadShaderByName(UnlitTransparentDoubleShader);
                    }
                    return s_UnlitTransparentDoubleShader;

                case ShaderType.Specular:
                    return null;

                case ShaderType.SpecularDouble:
                    if (s_SpecularDoubleShader == null)
                    {
                        s_SpecularDoubleShader = LoadShaderByName(SpecularDoubleShader);
                    }
                    return s_SpecularDoubleShader;

                case ShaderType.SpecularCutout:
                    if (s_SpecularCutoutShader == null)
                    {
                        s_SpecularCutoutShader = LoadShaderByName(SpecularCutoutShader);
                    }
                    return s_SpecularCutoutShader;

                case ShaderType.SpecularCutoutDouble:
                    if (s_SpecularCutoutDoubleShader == null)
                    {
                        s_SpecularCutoutDoubleShader = LoadShaderByName(SpecularCutoutDoubleShader);
                    }
                    return s_SpecularCutoutDoubleShader;

                case ShaderType.SpecularTransparent:
                    if (s_SpecularTransparentShader == null)
                    {
                        s_SpecularTransparentShader = LoadShaderByName(SpecularTransparentShader);
                    }
                    return s_SpecularTransparentShader;

                case ShaderType.SpecularTransparentDouble:
                    if (s_SpecularTransparentDoubleShader == null)
                    {
                        s_SpecularTransparentDoubleShader = LoadShaderByName(SpecularTransparentDoubleShader);
                    }
                    return s_SpecularTransparentDoubleShader;
            }

            return null;
        }
    }
}
#endif // USING_URP
