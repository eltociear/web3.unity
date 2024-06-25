using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Numerics;
using System.Reflection;
using System.Threading.Tasks;
using ChainSafe.Gaming.Evm.Signers;
using ChainSafe.Gaming.Evm.Transactions;
using ChainSafe.Gaming.Evm.Utils;
using Nethereum.Hex.HexTypes;

namespace ChainSafe.Gaming.Evm.Contracts.BuiltIn
{
    public class EvmService
    {
        private readonly string abi;
        private readonly IContractBuilder contractBuilder;
        private readonly ISigner signer;

        private readonly Dictionary<string, Tuple<string, EvmContract>> contractCache = new();

        private EvmService()
        {
            abi = AbiHelper.ReadAbiFromResources(Assembly.GetExecutingAssembly(), "ChainSafe.Gaming.Resources.evm-abi.json");
        }

        public EvmService(IContractBuilder contractBuilder)
            : this()
        {
            this.contractBuilder = contractBuilder;
        }

        public EvmService(IContractBuilder contractBuilder, ISigner signer)
            : this(contractBuilder)
        {
            this.signer = signer;
        }

        /// <summary>
        /// Builds an EVM contract instance.
        /// </summary>
        /// <param name="address">The address of the EVM contract.</param>
        /// <returns>An instance of EvmContract.</returns>
        public EvmContract BuildContract(string address)
        {
            if (contractCache.TryGetValue(address, out var cachedContract))
            {
                return cachedContract.Item2;
            }

            var originalContract = contractBuilder.Build(abi, address);
            var contract = new EvmContract(originalContract, signer);
            var tuple = Tuple.Create(string.Empty, contract);
            contractCache.Add(address, tuple);
            return contract;
        }

        /// <summary>
        /// Builds an EVM contract instance with an ABI.
        /// </summary>
        /// <param name="address">The address of the EVM contract.</param>
        /// <param name="abi">The abi of the EVM contract.</param>
        /// <returns>An instance of EvmContract.</returns>
        public EvmContract BuildContract(string address, string abi)
        {
            if (contractCache.TryGetValue(address, out var cachedContract))
            {
                return cachedContract.Item2;
            }

            var originalContract = contractBuilder.Build(abi, address);
            var contract = new EvmContract(originalContract, signer);
            var tuple = Tuple.Create(abi, contract);
            contractCache.Add(address, tuple);
            return contract;
        }

        [Pure]
        public Task<T> ContractCall<T>(string contractAddress, string abi, string method, string arg1) =>
            BuildContract(contractAddress, abi).ContractCall<T>(method, arg1);

        [Pure]
        public Task<T> ContractSend<T>(string contractAddress, string abi, string method, string arg1) =>
            BuildContract(contractAddress, abi).ContractSend<T>(method, arg1);

        [Pure]
        public Task<IEnumerable<T>> GetArray<T>(string contractAddress, string abi, string method, string arg1 = null) =>
            BuildContract(contractAddress, abi).GetArray<T>(method, arg1);

        [Pure]
        public Task<IEnumerable<T>> SendArray<T>(string contractAddress, string abi, string method, string[] arg1 = null) =>
            BuildContract(contractAddress, abi).SendArray<T>(method, arg1);

        [Pure]
        public Task<HexBigInteger> GetBlockNumber() =>
            BuildContract(string.Empty).GetBlockNumber();

        [Pure]
        public Task<IEnumerable<HexBigInteger>> GetGasLimit(string contractAddress, string abi, string method, object[] args) =>
            BuildContract(contractAddress, abi).GetGasLimit(method, args);

        [Pure]
        public Task<HexBigInteger> GetGasPrice() =>
            BuildContract(string.Empty).GetGasPrice();

        [Pure]
        public Task<HexBigInteger> GetNonce() =>
            BuildContract(string.Empty).GetNonce();

        [Pure]
        public Task<TransactionReceipt> GetTransactionStatus() =>
            BuildContract(string.Empty).GetTransactionStatus();

        [Pure]
        public Task<string> SendTransaction(string contractAddress, string abi, string to, BigInteger value) =>
            BuildContract(contractAddress, abi).SendTransaction(to, value);

        [Pure]
        public string Sha3(string message) =>
            BuildContract(string.Empty).Sha3(message);

        [Pure]
        public Task<string> SignMessage(string message) =>
            BuildContract(string.Empty).SignMessage(message);

        [Pure]
        public Task<bool> SignVerify(string message) =>
            BuildContract(string.Empty).SignVerify(message);

        [Pure]
        public string EcdsaSignTransaction(string privateKey, string transaction, string chainId) =>
            BuildContract(string.Empty).EcdsaSignTransaction(privateKey, transaction, chainId);

        [Pure]
        public string EcdsaGetAddress(string privateKey) =>
            BuildContract(string.Empty).EcdsaGetAddress(privateKey);

        [Pure]
        public string EcdsaSignMessage(string privateKey, string message) =>
            BuildContract(string.Empty).EcdsaSignMessage(privateKey, message);

        [Pure]
        public Task<BigInteger> UseRegisteredContract(string contractName, string method) =>
            BuildContract(string.Empty).UseRegisteredContract(contractName, method);
    }
}