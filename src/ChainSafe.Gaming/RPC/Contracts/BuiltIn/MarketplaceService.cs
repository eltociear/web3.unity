using System.Collections.Generic;
using System.Reflection;
using ChainSafe.Gaming.Evm.Signers;
using ChainSafe.Gaming.Evm.Utils;

namespace ChainSafe.Gaming.Evm.Contracts.BuiltIn
{
    public class MarketplaceService
    {
        private readonly string abi;
        private readonly IContractBuilder contractBuilder;
        private readonly ISigner signer;

        private readonly Dictionary<string, MarketplaceContract> contractCache = new();

        private MarketplaceService()
        {
            abi = AbiHelper.ReadAbiFromResources(Assembly.GetExecutingAssembly(), "ChainSafe.Gaming.Resources.marketplace-abi.json");
        }

        public MarketplaceService(IContractBuilder contractBuilder)
            : this()
        {
            this.contractBuilder = contractBuilder;
        }

        public MarketplaceService(IContractBuilder contractBuilder, ISigner signer)
            : this(contractBuilder)
        {
            this.signer = signer;
        }

        /// <summary>
        /// Builds an EVM contract instance.
        /// </summary>
        /// <param name="address">The address of the EVM contract.</param>
        /// <returns>An instance of EvmContract.</returns>
        public MarketplaceContract BuildContract(string address)
        {
            if (contractCache.TryGetValue(address, out var cachedContract))
            {
                return cachedContract;
            }

            var originalContract = contractBuilder.Build(abi, address);
            var contract = new MarketplaceContract(originalContract, signer);
            contractCache.Add(address, contract);
            return contract;
        }
    }
}