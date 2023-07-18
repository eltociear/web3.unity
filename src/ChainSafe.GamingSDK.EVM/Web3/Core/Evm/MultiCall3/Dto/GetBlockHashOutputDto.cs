﻿using Nethereum.ABI.FunctionEncoding.Attributes;

namespace ChainSafe.GamingSDK.EVM.Web3.Core.Evm.MultiCall3.Dto
{
    /// <summary>
    /// Returns the block hash for the given block number.
    /// </summary>
    public partial class GetBlockHashOutputDto : GetBlockHashOutputDtoBase
    {
    }

    [FunctionOutput]
    public class GetBlockHashOutputDtoBase : IFunctionOutputDTO
    {
        [Parameter("bytes32", "blockHash", 1)]
        public virtual byte[] BlockHash { get; set; }
    }
}