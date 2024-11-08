﻿namespace Modules.WebServer
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UniGame.MetaBackend.Shared;
    using UniGame.MetaBackend.Shared.Data;

    public interface IWebMetaProvider : IRemoteMetaProvider
    {
        void SetToken(string token);
        Dictionary<string, string> SerializeToQuery(object payload);
        UniTask<RemoteMetaResult> ExecuteAsync(IRemoteMetaContract contract);
    }
}