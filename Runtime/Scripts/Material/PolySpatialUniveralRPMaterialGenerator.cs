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
        static bool s_MetallicClearcoatTransparentShaderQueried;
        static Shader s_MetallicClearcoatTransparentShader;

        static Shader s_MetallicTransparentShader;

        public const string MetallicTransparentShader = "glTF-pbrMetallicRoughness-transparent";

        public const string MetallicClearcoatTransparentShader = "URP/glTF-pbrMetallicRoughness-Clearcoat-transparent";

        public PolySpatialUniversalRPMaterialGenerator(UniversalRenderPipelineAsset renderPipelineAsset) : base(renderPipelineAsset) { }

        protected override Shader GetMetallicShader(MetallicShaderFeatures features)
        {
            if ((features & MetallicShaderFeatures.ModeTransparent) != 0)
            {
                if ((features & MetallicShaderFeatures.ClearCoat) != 0)
                {
                    if (!s_MetallicClearcoatTransparentShaderQueried)
                    {
                        s_MetallicClearcoatTransparentShader = LoadShaderByName(MetallicClearcoatShader);
                        s_MetallicClearcoatTransparentShaderQueried = true;
                    }

                    if (s_MetallicClearcoatTransparentShader != null)
                    {
                        return s_MetallicClearcoatTransparentShader;
                    }
                }

                if (s_MetallicTransparentShader == null)
                {
                    s_MetallicTransparentShader = LoadShaderByName(MetallicTransparentShader);
                }

                return s_MetallicTransparentShader;
            }

            return base.GetMetallicShader(features);
        }
    }
}
#endif // USING_URP
