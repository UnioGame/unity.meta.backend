namespace Game.Modules.Meta.Runtime
{
    using System;
    using UniGame.MetaBackend.Shared;

    public static class BackendMetaTools
    {
        public const string ContractKey = "Contract";
        
        public static string GetContractName(IRemoteMetaContract contract)
        {
            if(contract == null) return string.Empty;
            if(!string.IsNullOrEmpty(contract.Path))
                return contract.Path;

            var typeName = GetContractName(contract.GetType());
            return typeName;
        }
        
        public static string GetContractName(Type contractType)
        {
            if(contractType == null) return string.Empty;
            var typeName = contractType.Name;
            if (typeName.EndsWith(ContractKey, StringComparison.OrdinalIgnoreCase) && 
                typeName.Length > ContractKey.Length)
            {
                return typeName.Substring(0,typeName.Length - ContractKey.Length);
            }

            return typeName;
        }
        
        public static int CalculateMetaId(IRemoteMetaContract contract)
        {
            var contractType = contract.GetType().Name;
            var inputType = contract.InputType?.GetHashCode() ?? 0;
            var outputType = contract.OutputType?.GetHashCode() ?? 0;
            var id = HashCode.Combine(contractType, inputType, outputType);
            return id;
        }
        
    }
}