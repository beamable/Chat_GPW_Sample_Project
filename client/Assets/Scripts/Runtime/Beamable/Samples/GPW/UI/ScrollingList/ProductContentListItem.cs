﻿using System.Text;
using AirFishLab.ScrollingList;
using Beamable.Samples.GPW.Content;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Beamable.Samples.GPW.UI.ScrollingList
{
    public class ProductContentViewEvent : UnityEvent<ProductContentView>{}
	
    /// <summary>
    /// Renderer of ScrollingList's <see cref="ListBox"/>.
    /// </summary>
    public class ProductContentListItem : ListBox
    {
        //  Events  --------------------------------------
        public ProductContentViewEvent OnBuy = new ProductContentViewEvent();
        public ProductContentViewEvent OnSell = new ProductContentViewEvent();
		
        //  Properties  ----------------------------------

        //  Fields  --------------------------------------
        [SerializeField]
        private Image _iconImage = null;
        
        [SerializeField]
        private TMP_Text _titleText = null;
                
        [SerializeField]
        private TMP_Text _detailsText = null;

        [SerializeField]
        private Button _buyButton = null;

        [SerializeField]
        private Button _sellButton = null;

        private ProductContentView _productContentView = null;
        
        //  Other Methods  --------------------------------
        protected override void UpdateDisplayContent(object content)
        {
            _productContentView = (ProductContentView)content;
            GPWHelper.AddressablesLoadAssetAsync(_productContentView.ProductContent.icon, _iconImage);
            _titleText.text = _productContentView.ProductContent.Title;

            StringBuilder stringBuilder = new StringBuilder();
            int countThem = _productContentView.Quantity;
            int countMe = 01;
            int priceThem = _productContentView.Price;
            int priceMe = 02;
            
            stringBuilder.AppendLine($"Avail: #{countThem:000}   ${priceThem:000}");
            stringBuilder.AppendLine($"Owned: #{countMe:000}   ${priceMe:000}");
            
            _detailsText.text = stringBuilder.ToString();
            _buyButton.onClick.RemoveAllListeners();
            _buyButton.onClick.AddListener(() =>
            {
                OnBuy.Invoke(_productContentView);   
            });
            
            _sellButton.onClick.RemoveAllListeners();
            _sellButton.onClick.AddListener(() =>
            {
                OnSell.Invoke(_productContentView);   
            });
        }
        
        //  Event Handlers  -------------------------------
    }
}
