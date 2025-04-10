﻿namespace MetaService.Runtime
{
    using System;
    using Game.Modules.ModelMapping;
    using UniGame.MetaBackend.Shared;

    [Serializable]
    public struct MetaContractData
    {
        public int id;
        public string contractName;
        public IRemoteMetaProvider provider;
        public RemoteMetaData metaData;
        public IRemoteMetaContract contract;
    }
}