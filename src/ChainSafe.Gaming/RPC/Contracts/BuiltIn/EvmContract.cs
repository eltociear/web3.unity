using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ChainSafe.Gaming.Evm.Providers;
using ChainSafe.Gaming.Evm.Signers;
using ChainSafe.Gaming.Evm.Transactions;
using ChainSafe.Gaming.Web3;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.Signer;
using Nethereum.Util;

namespace ChainSafe.Gaming.Evm.Contracts.BuiltIn
{
    public class EvmContract : BuiltInContract
    {
        private readonly ISigner signer;

        internal EvmContract(Contract contract, ISigner signer)
            : base(contract)
        {
            this.signer = signer;
        }

        [Pure]
        public async Task<T> ContractCall<T>(string method, string arg1)
        {
            EnsureSigner();
            var result = await Original.CallSingle<T, string>(method, arg1);
            return result;
        }

        [Pure]
        public async Task<T> ContractSend<T>(string method, string arg1, HexBigInteger value = null)
        {
            EnsureSigner();
            var overwrite = value != null ? new TransactionRequest { Value = value } : null;
            var result = await Original.SendSingle<T, string>(method, arg1, overwrite);
            return result;
        }

        [Pure]
        public async Task<IEnumerable<T>> GetArray<T>(string method, string arg1 = null)
        {
            EnsureSigner();
            if (arg1 == null)
            {
                var result = await Original.SendMany<T>(method);
                return result;
            }
            else
            {
                var result = await Original.SendMany<T, string>(method, arg1);
                return result;
            }
        }

        [Pure]
        public async Task<IEnumerable<T>> SendArray<T>(string method, string[] arg1 = null)
        {
            EnsureSigner();
            var result = await Original.SendMany<T, string[]>(method, arg1);
            return result;
        }

        [Pure]
        public async Task<HexBigInteger> GetBlockNumber()
        {
            var result = await Original.SendSingle<HexBigInteger>("eth_blockNumber");
            return result;
        }

        [Pure]
        public async Task<IEnumerable<HexBigInteger>> GetGasLimit(string method, object[] args)
        {
            var result = await Original.SendMany<HexBigInteger, object[]>(method, args);
            return result;
        }

        [Pure]
        public async Task<HexBigInteger> GetGasPrice()
        {
            var gasPrice = await Original.SendSingle<HexBigInteger>("eth_gasPrice");
            return gasPrice;
        }

        [Pure]
        public async Task<HexBigInteger> GetNonce()
        {
            EnsureSigner();
            var transactionRequest = new TransactionRequest
            {
                To = signer.PublicAddress,
                Value = new HexBigInteger(100000),
            };
            var transactionResponse =
                await Original.SendSingle<TransactionResponse>("eth_sendTransaction", transactionRequest);
            return transactionResponse.Nonce;
        }

        [Pure]
        public async Task<TransactionReceipt> GetTransactionStatus()
        {
            EnsureSigner();
            var transactionRequest = new TransactionRequest
            {
                To = signer.PublicAddress,
                Value = new HexBigInteger(10000000),
            };
            var transactionResponse =
                await Original.SendSingle<TransactionResponse>("eth_sendTransaction", transactionRequest);
            var receipt =
                await Original.SendSingle<TransactionReceipt, TransactionResponse>("eth_getTransactionReceipt", transactionResponse);
            return receipt;
        }

        [Pure]
        public async Task<string> SendTransaction(string to, BigInteger value)
        {
            EnsureSigner();
            var feeData = await Original.SendSingle<FeeData>("eth_feeHistory");
            var txRequest = new TransactionRequest
            {
                To = to,
                Value = new HexBigInteger(value.ToString("X")),
                MaxFeePerGas = new HexBigInteger(feeData.MaxFeePerGas),
            };
            var response = await Original.SendSingle<TransactionResponse>("eth_sendTransaction", txRequest);
            return response.Hash;
        }

        [Pure]
        public string Sha3(string message)
        {
            return new Sha3Keccack().CalculateHash(message);
        }

        [Pure]
        public async Task<string> SignMessage(string message)
        {
            EnsureSigner();
            var signature = await signer.SignMessage(message);
            return signature;
        }

        [Pure]
        public async Task<bool> SignVerify(string message)
        {
            EnsureSigner();
            var playerAccount = signer.PublicAddress;
            var signatureString = await signer.SignMessage(message);
            var msg = "\x19" + "Ethereum Signed Message:\n" + message.Length + message;
            var msgHash = new Sha3Keccack().CalculateHash(Encoding.UTF8.GetBytes(msg));
            var signature = MessageSigner.ExtractEcdsaSignature(signatureString);
            var key = EthECKey.RecoverFromSignature(signature, msgHash);
            return key.GetPublicAddress().ToLower() == playerAccount.ToLower();
        }

        [Pure]
        public string EcdsaSignTransaction(string privateKey, string transaction, string chainId)
        {
            const int MATIC_MAIN = 137;
            const int MATIC_MUMBAI = 80001;
            const int HARMONY_MAINNET = 1666600000;
            const int HARMONY_TESTNET = 1666700000;
            const int CRONOS_MAINNET = 25;
            const int CRONOS_TESTNET = 338;
            const int FTM_MAINNET = 250;
            const int FTM_TESTNET = 0xfa2;
            const int AVA_MAINNET = 43114;
            const int AVA_TESTNET = 43113;
            int chainID = Convert.ToInt32(chainId);
            string signature;
            EthECKey key = new EthECKey(privateKey);
            byte[] hashByteArr = HexByteConvertorExtensions.HexToByteArray(transaction);
            BigInteger chainIdBigInt = BigInteger.Parse(chainId);
            if ((chainID == MATIC_MAIN) || (chainID == MATIC_MUMBAI) || (chainID == HARMONY_MAINNET) ||
                (chainID == HARMONY_TESTNET) || (chainID == CRONOS_MAINNET) || (chainID == CRONOS_TESTNET) || (chainID == FTM_MAINNET) || (chainID == FTM_TESTNET) || (chainID == AVA_MAINNET) || (chainID == AVA_TESTNET))
            {
                signature = EthECDSASignature.CreateStringSignature(key.SignAndCalculateYParityV(hashByteArr));
                return signature;
            }

            signature = EthECDSASignature.CreateStringSignature(key.SignAndCalculateV(hashByteArr, chainIdBigInt));
            return signature;
        }

        [Pure]
        public string EcdsaGetAddress(string privateKey)
        {
            EthECKey key = new EthECKey(privateKey);
            return key.GetPublicAddress();
        }

        [Pure]
        public string EcdsaSignMessage(string privateKey, string message)
        {
            var signer = new EthereumMessageSigner();
            string signature = signer.HashAndSign(message, privateKey);
            return signature;
        }

        [Pure]
        public async Task<BigInteger> UseRegisteredContract(string contractName, string method)
        {
            var response = await Original.CallSingle<object[], string>(method, contractName, null);
            var balance = BigInteger.Parse(response[0].ToString());
            return balance;
        }

        private void EnsureSigner()
        {
            if (signer is not null)
            {
                return;
            }

            throw new Web3Exception("Can't get player address. No Signer was provided during construction.");
        }
    }
}