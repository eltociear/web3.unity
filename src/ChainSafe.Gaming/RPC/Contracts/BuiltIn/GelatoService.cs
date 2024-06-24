using System.Collections.Generic;
using System.Reflection;
using ChainSafe.Gaming.Evm.Signers;
using ChainSafe.Gaming.Evm.Utils;

namespace ChainSafe.Gaming.Evm.Contracts.BuiltIn
{
    public class GelatoService
    {
        private readonly string abi;
        private readonly IContractBuilder contractBuilder;
        private readonly ISigner signer;

        private readonly Dictionary<string, GelatoContract> contractCache = new();

        private GelatoService()
        {
            abi = AbiHelper.ReadAbiFromResources(Assembly.GetExecutingAssembly(), "ChainSafe.Gaming.Resources.gelato-abi.json");
        }

        public GelatoService(IContractBuilder contractBuilder)
            : this()
        {
            this.contractBuilder = contractBuilder;
        }

        public GelatoService(IContractBuilder contractBuilder, ISigner signer)
            : this(contractBuilder)
        {
            this.signer = signer;
        }

        /// <summary>
        /// Builds a Gelato contract instance.
        /// </summary>
        /// <param name="address">The address of the Gelato contract.</param>
        /// <returns>An instance of GelatoContract.</returns>
        public GelatoContract BuildContract(string address)
        {
            if (contractCache.TryGetValue(address, out var cachedContract))
            {
                return cachedContract;
            }

            var originalContract = contractBuilder.Build(abi, address);
            var contract = new GelatoContract(originalContract, signer);
            contractCache.Add(address, contract);
            return contract;
        }
    }
}