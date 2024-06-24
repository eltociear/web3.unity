using ChainSafe.Gaming.Evm.Signers;
using ChainSafe.Gaming.Web3;

namespace ChainSafe.Gaming.Evm.Contracts.BuiltIn
{
    /// <summary>
    /// Represents a default EVM contract.
    /// </summary>
    public class GelatoContract : BuiltInContract
    {
        private readonly ISigner signer;

        internal GelatoContract(Contract contract, ISigner signer)
            : base(contract)
        {
            this.signer = signer;
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