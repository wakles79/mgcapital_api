/* 
 *  File: AddressesApplicationService.cs
 *  Copyright (c) 2019 robin
 *  
 *  This source code is Copyright Axzes and MAY NOT be copied, reproduced,
 *  published, distributed or transmitted to or stored in any manner without prior
 *  written consent from Axzes (https://www.axzes.com).
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Geocoding;
using Geocoding.Google;
using MGCap.Business.Abstract.ApplicationServices;
using MGCap.Business.Implementation.ApplicationServices;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.Domain.Models;
using MGCap.Domain.Options;
using Microsoft.Extensions.Options;

public class AddressesApplicationService : BaseApplicationService<MGCap.Domain.Models.Address, int>, IAddressesApplicationService
{

    public readonly IGeocoder Geocoder;
    public AddressesApplicationService(
        IAddressesRepository repository,
        IOptions<GeocoderOptions> geocoderOptions) : base(repository)
    {
        this.Geocoder = new GoogleGeocoder
        {
            ApiKey = geocoderOptions.Value.GoogleGeocoderApiKey
        };
    }

    public async Task<IEnumerable<MGCap.Domain.Models.Address>> GeodecodeAllAsync()
    {
        var objs = await this.Repository.ReadAllAsync(a => true);

        var updated = new List<MGCap.Domain.Models.Address>();

        foreach (var obj in objs)
        {
            var loc = await this.GetCoordinatesAsync($"{obj.AddressLine1} {obj.AddressLine2} {obj.City} {obj.State} {obj.ZipCode} {obj.CountryCode}");

            if (loc != null)
            {
                obj.Latitude = loc.Latitude;
                obj.Longitude = loc.Longitude;
                updated.Add(obj);
            }
        }

        updated = this.Repository.UpdateRange(updated)?.ToList();
        return updated;
    }

    private async Task<Location> GetCoordinatesAsync(string address)
    {
        try
        {
#if DEBUG
            Console.WriteLine($"ADDRESS: {address}");
#endif 
            var addresses = await this.Geocoder.GeocodeAsync(address);
            if (addresses.Count() >= 1)
            {
                var gAddress = addresses.First();
                Console.WriteLine($"SUCCESS: {gAddress.Coordinates.ToJSON()}");
                return gAddress.Coordinates;
            }
        }
        catch (GoogleGeocodingException gEx)
        {
#if DEBUG
            Console.WriteLine(gEx.Message);
#endif
        }
        catch (ArgumentException aEx)
        {
#if DEBUG
            Console.WriteLine(aEx.Message);
#endif
        }
        catch (Exception ex)
        {
#if DEBUG
            Console.WriteLine(ex.Message);
#endif
        }
        return null;
    }
}