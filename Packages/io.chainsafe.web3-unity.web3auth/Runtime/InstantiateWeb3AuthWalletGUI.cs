using System.Collections;
using System.Collections.Generic;
using ChainSafe.Gaming.InProcessSigner;
using ChainSafe.Gaming.UnityPackage;
using ChainSafe.Gaming.UnityPackage.Connection;
using UnityEngine;

public class InstantiateWeb3AuthWalletGUI : MonoBehaviour, IWeb3InitializedHandler
{
    [SerializeField] private Web3AuthWalletGUI gui;
    public void OnWeb3Initialized()
    {
        if (Web3Accessor.Web3.Signer is InProcessSigner)
        {
            var wallet = Instantiate(gui);
            DontDestroyOnLoad(wallet.gameObject);
        }
        
    }
}
