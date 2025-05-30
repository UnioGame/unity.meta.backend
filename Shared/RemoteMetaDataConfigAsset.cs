﻿namespace Game.Modules.ModelMapping
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Meta.Runtime;
    using MetaService.Runtime;
    using UniGame.MetaBackend.Shared;
    using Sirenix.OdinInspector;
    using UniModules;
    using UniModules.UniCore.Runtime.Utils;
    using UnityEngine;

#if UNITY_EDITOR
    using UniModules.Editor;
    using UnityEditor;
#endif

    [CreateAssetMenu(menuName = "UniGame/Services/MetaBackend/Remote Meta Data Config")]
    public class RemoteMetaDataConfigAsset : ScriptableObject
    {
        [TabGroup("meta contracts")]
        [HideLabel]
        [InlineProperty]
        public RemoteMetaDataConfig configuration = new();

        [TabGroup(nameof(settings))]
        [InlineProperty]
        [HideLabel]
        public BackendMetaSettings settings = new();

        
        #region IdGenerator

#if UNITY_EDITOR

        [PropertyOrder(-1)]
        [Button(icon: SdfIconType.ImageFill)]
        [ButtonGroup("Providers")]
        public void SetDefaultTypeForAll()
        {
            foreach (var metaCallData in configuration.remoteMetaData)
            {
                metaCallData.provider = settings.backendType;
            }
        }

        [Button(icon: SdfIconType.ArrowClockwise, "Update Remote Meta Data")]
        [PropertyOrder(-1)]
        [ButtonGroup("Providers")]
        public void UpdateRemoteMetaData()
        {
            var remoteItems = LoadRemoteMetaData();
            var sourceItems = new Dictionary<int, RemoteMetaData>();
            
            foreach (var metaData in configuration.remoteMetaData)
            {
                var contract = metaData.contract;
                
                if(contract == null) continue;
                if(contract.OutputType == null) continue;
                if(contract.InputType == null) continue;
                
                var id = configuration.CalculateMetaId(contract);
                metaData.id = id;
                
                sourceItems[metaData.id] = metaData;
            }
            
            foreach (var item in remoteItems)
                sourceItems.TryAdd(item.Key, item.Value);

            configuration.remoteMetaData = sourceItems.Values.ToArray();

            UpdateRemoteMetas(configuration.remoteMetaData);

            this.SaveAsset();
            
            AssetDatabase.SaveAssets();
        }

        private void UpdateRemoteMetas(IEnumerable<RemoteMetaData> data)
        {
            foreach (var metaCallData in data)
            {
                var method = BackendMetaTools.GetContractName(metaCallData.contract);
                metaCallData.method = method;
            }
        }

        public Dictionary<int, RemoteMetaData> LoadRemoteMetaData()
        {
            var remoteCallContractType = typeof(IRemoteMetaContract);
            var contractTypes = TypeCache.GetTypesDerivedFrom(remoteCallContractType);
            var remoteModels = new Dictionary<int, RemoteMetaData>();

            foreach (var typeItem in contractTypes)
            {
                if (!ValidateType(typeItem)) continue;

                var contract = typeItem.CreateWithDefaultConstructor() as IRemoteMetaContract;
                if (contract == null) continue;

                var method = configuration.GetRemoteMethodName(contract);
                var id = configuration.CalculateMetaId(contract);

                var remoteItem = new RemoteMetaData()
                {
                    id = id,
                    method = method,
                    contract = contract,
                    provider = settings.backendType,
                    overriderDataConverter = false,
                    converter = configuration.defaultConverter,
                };

                remoteModels[id] = remoteItem;
            }

            return remoteModels;
        }

        public bool ValidateType(Type type)
        {
            if (type == null) return false;
            if (type.IsAbstract || type.IsInterface) return false;
            if (type.IsGenericType) return false;
            if (type.HasDefaultConstructor() == false) return false;

            return true;
        }

        [PropertyOrder(-1)]
        [Button("Generate Static Properties")]
        public void GenerateProperties()
        {
            GenerateStaticProperties(this);
        }

        public static void GenerateStaticProperties(RemoteMetaDataConfigAsset dataAsset)
        {
            var idType = typeof(RemoteMetaId);
            var typeName = nameof(RemoteMetaId);
            
            var outputPath = $"/UniGame.Generated/RemoteMetaService/"
                .FixUnityPath()
                .ToProjectPath();

            var outputFileName = "RemoteMetaId.Generated.cs";

            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            var namespaceName = idType.Namespace;

            var filePath = outputPath.CombinePath(outputFileName);

            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                writer.WriteLine($"namespace {namespaceName}");
                writer.WriteLine("{");
                writer.WriteLine($"    public struct RemoteMetaContracts");
                writer.WriteLine("    {");

                var items = dataAsset.configuration.remoteMetaData;

                foreach (var item in items)
                {
                    var name = item.method;
                    if (name == null) continue;

                    var propertyName = name.Replace(" ", "");
                    writer.WriteLine(
                        $"        public static {typeName} {propertyName} = new {typeName} {{ value = {item.id} }};");
                }

                writer.WriteLine("    }");
                writer.WriteLine("}");
            }

            AssetDatabase.Refresh();
            Debug.Log("Partial class with static properties generated successfully.");
        }

#endif

        #endregion
    }
}