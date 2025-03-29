using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Modules.unity.meta.service.Modules.WebProvider
{
    using global::Modules.WebServer;
    
#if UNITY_EDITOR
    using UniModules.Editor;
#endif
    
    /// <summary>
    /// Settings for web API contract generation
    /// </summary>
    [Serializable]
    public class WebApiSettings
    {
        /// <summary>
        /// Path to the Swagger JSON file
        /// </summary>
        [FilePath]
        public string apiJsonPath;

        /// <summary>
        /// Path to the folder where contracts will be generated
        /// </summary>
        public string contractsOutFolder = "Assets/UniGame.Generated/WebContracts/";
        
        /// <summary>
        /// Path to the folder where DTO classes will be generated
        /// </summary>
        public string dtoOutFolder = "Assets/UniGame.Generated/WebContracts/DTO/";
        
        /// <summary>
        /// Namespace for generated contracts and DTOs
        /// </summary>
        public string ContractNamespace = "Game.Generated.WebContracts";
        
        /// <summary>
        /// Template for API URL path
        /// </summary>
        public string apiTemplate = "api/{0}";

        /// <summary>
        /// Allowed API paths to include in generation
        /// Only paths containing these strings will be included
        /// </summary>
        public string[] apiAllowedPaths = Array.Empty<string>();
        
        /// <summary>
        /// If true, output folders will be cleaned before generation
        /// All existing files will be deleted from contracts and DTO folders
        /// </summary>
        [Tooltip("When enabled, all existing files in output folders will be deleted before generating new contracts")]
        public bool cleanUpOnGenerate = false;

        [Button]
        public void GenerateContracts()
        {
            WebApiGenerator.GenerateContracts(this);
            
#if UNITY_EDITOR
            var webProviders = AssetEditorTools
                .GetAssets<WebMetaProviderAsset>();
            foreach (var providerAsset in webProviders)
            {
                providerAsset.LoadContracts();
                providerAsset.MarkDirty();
            }
#endif
        }
    }
}