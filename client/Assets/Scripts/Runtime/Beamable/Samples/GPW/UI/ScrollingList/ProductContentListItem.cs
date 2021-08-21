using System.Text;
using AirFishLab.ScrollingList;
using Beamable.Samples.GPW.Content;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Beamable.Samples.GPW.UI.ScrollingList
{
    public class ProductContentEvent : UnityEvent<ProductContent>{}
	
    /// <summary>
    /// Renderer of ScrollingList's <see cref="ListBox"/>.
    /// </summary>
    public class ProductContentListItem : ListBox
    {
        //  Events  --------------------------------------
        public ProductContentEvent OnBuy = new ProductContentEvent();
        public ProductContentEvent OnSell = new ProductContentEvent();
		
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

        private Content.ProductContent _productContent = null;
        
        //  Other Methods  --------------------------------
        protected override void UpdateDisplayContent(object content)
        {
            _productContent = (Content.ProductContent)content;
            GPWHelper.AddressablesLoadAssetAsync(_productContent.icon, _iconImage);
            _titleText.text = _productContent.Title;

            StringBuilder stringBuilder = new StringBuilder();

            int countThem = 10;
            int countMe = 05;
            float priceThem = 100;
            float priceMe = 20;
            stringBuilder.AppendLine($"I: {countThem}     P:{priceThem}");
            stringBuilder.AppendLine($"I: {countMe}     P:{priceMe}");
            _detailsText.text = stringBuilder.ToString();
            _buyButton.onClick.AddListener(() =>
            {
                OnBuy.Invoke(_productContent);   
            });
            _sellButton.onClick.AddListener(() =>
            {
                OnSell.Invoke(_productContent);   
            });
        }
        
        //  Event Handlers  -------------------------------
    }
}

