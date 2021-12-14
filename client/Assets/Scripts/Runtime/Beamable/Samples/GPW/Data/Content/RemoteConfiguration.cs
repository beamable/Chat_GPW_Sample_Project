using System;
using System.Collections.Generic;
using Beamable.Common.Content;
using UnityEngine;

namespace Beamable.Samples.GPW.Data.Content
{
    [Serializable]
    public class RemoteConfigurationRef : ContentRef<RemoteConfiguration> {}
    
    /// <summary>
    /// Store the data: Represents core game data
    /// </summary>
    [ContentType("remote_configuration")]
    public class RemoteConfiguration : ContentObject
    {
        //  Fields ---------------------------------------
        [Header("Debt")]
        public int DebtAmountInitial = 1000;
        public float DebtInterestMin = 0.01f;
        public float DebtInterestMax = 0.1f;
      
        [Header("Cash")]
        public int CashAmountInitial = 1000;
      
        [Header("Bank")]
        public int BankAmountInitial = 0;
        public float BankInterestMin = 0.01f;
        public float BankInterestMax = 0.1f;
        public int CashTransactionMin = 100;
        
        [Header("Other")]
        public int RandomSeed = 1;
        public int TurnsTotal = 30;
        public int ItemsMax = 100;
        
        [Header("Lists")]
        public List<ProductContentRef> ProductContentRefs = new List<ProductContentRef>();
        public List<LocationContentRef> LocationContentRefs = new List<LocationContentRef>();
    }
}
