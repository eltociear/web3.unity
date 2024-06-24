using ChainSafe.Gaming.Evm.Signers;
using ChainSafe.Gaming.Web3;

namespace ChainSafe.Gaming.Evm.Contracts.BuiltIn
{
    /// <summary>
    /// Represents a default Marketplace contract.
    /// </summary>
    public class MarketplaceContract : BuiltInContract
    {
        private readonly ISigner signer;

        internal MarketplaceContract(Contract contract, ISigner signer)
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