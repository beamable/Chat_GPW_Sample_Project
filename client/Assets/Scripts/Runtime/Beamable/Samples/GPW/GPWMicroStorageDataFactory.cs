using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Samples.GPW.Data;
using Beamable.Samples.GPW.Data.Factories;
using Beamable.Server.Clients;
using UnityEngine;

#pragma warning disable 1998
namespace Beamable.Samples.GPW
{
    /// <summary>
    /// Client-side factory for the loaded data content
    /// </summary>
    public class GPWMicroStorageDataFactory : IDataFactory
    {
        //  Fields ---------------------------------------

        //  Other Methods -----------------------------------
        public async Task<List<LocationContentView>> GetLocationContentViews(
            List<LocationData> locationDatas, List<ProductData> productDatas)
        {
            GPWDataServiceClient gpwDataServiceClient = new GPWDataServiceClient();

            // Check stability
            bool isMicroServiceReady = await gpwDataServiceClient.IsMicroServiceReady();
            if (!isMicroServiceReady)
            {
                Debug.LogError($"GetLocationContentViews() failed. isMicroServiceReady = {isMicroServiceReady}");
                return null;
            }

            // Check stability
            bool isMicroStorageReady = await gpwDataServiceClient.IsMicroStorageReady();
            if (!isMicroStorageReady)
            {
                Debug.LogError($"GetLocationContentViews() failed. isMicroStorageReady = {isMicroStorageReady}");
                return null;
            }

            // Check existing data
            bool hasLocationContentViews = await gpwDataServiceClient.HasLocationContentViews();
            if (!hasLocationContentViews)
            {
                // Populate existing data
                bool isSuccess = await gpwDataServiceClient.CreateLocationContentViews(locationDatas, productDatas);
                if (!isSuccess)
                {
                    Debug.LogError($"CreateLocationContentViews() failed. isSuccess = {isMicroServiceReady}");
                    return null;
                }
            }

            // Fetch existing data
            LocationContentViewCollection locationContentViewCollection =
                await gpwDataServiceClient.GetLocationContentViews();

            //  Sort list: A to Z
            locationContentViewCollection.LocationContentViews.Sort((p1, p2) =>
            {
                return string.Compare(p2.LocationData.Title, p2.LocationData.Title,
                    StringComparison.InvariantCulture);
            });

            // Return List
            Debug.Log($"GetLocationContentViews() success. LocationContentViews.Count = " +
                $"{locationContentViewCollection.LocationContentViews.Count}");
            return locationContentViewCollection.LocationContentViews;
        }
    }
}