namespace apbd_cw3;

class ContainerTypePreset
{
    [System.Text.Json.Serialization.JsonPropertyName("container-type")]
    public string ContainerType { get; set; } = string.Empty;

    [System.Text.Json.Serialization.JsonPropertyName("presets")]
    public CargoPreset[] Presets { get; set; } = [];

    public override string ToString()
    {
        var builder = new System.Text.StringBuilder($"Presets for container of type {this.ContainerType} ↓");
        foreach (var str in this.Presets.Select(p => p.ToString())) builder.Append("\n · ").Append(str);
        return builder.ToString();
    }
}
