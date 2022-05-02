using MGCap.Domain.ViewModels.Common;
using Newtonsoft.Json;

public class BuildingWithLocationViewModel : ListBoxViewModel
{
    [JsonProperty("lat")]
    public virtual double Latitude { get; set; }
    [JsonProperty("lng")]
    public virtual double Longitude { get; set; }
    public int IsActive { get; set; }
}
