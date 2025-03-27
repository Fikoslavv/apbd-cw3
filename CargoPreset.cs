namespace apbd_cw3;

class CargoPreset
{
    [System.Text.Json.Serialization.JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [System.Text.Json.Serialization.JsonPropertyName("is-hazardous")]
    public bool IsHazardous { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("min-temperature")]
    public double MinTemperature { get; set; }

    public override string ToString()
    {
        return $"Cargo preset => {this.Name}; {(this.IsHazardous ? "" : "non-")}hazardous; min temperature => {this.MinTemperature}";
    }
}
