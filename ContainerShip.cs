namespace apbd_cw3;

class ContainerShip : IContainerShip
{
    private IDictionary<string, IContainer> containers = new Dictionary<string, IContainer>();
    private uint maxSpeed;
    private uint containersCapacity;
    private long tareWeight;
    private long maxNetWeight;

    public string Id { get; init; }
    public IEnumerable<IContainer> Containers { get => this.containers.Values; }
    public uint MaxSpeed { get => this.maxSpeed; init => this.maxSpeed = value; }
    public uint ContainersCapacity { get => this.containersCapacity; init => this.containersCapacity = value; }
    public long TareWeight { get => this.tareWeight; init => this.tareWeight = value; }
    public long MaxNetWeight { get => this.maxNetWeight; init => this.maxNetWeight = value; }
    public long NetWeight { get => containers.Values.Select(c => c.GrossWeight).Sum(); }
    public long GrossWeight { get => this.NetWeight + (this.TareWeight * 1000); }

    public IContainer this[string serialNumber] => this.containers[serialNumber];

    public ContainerShip()
    {
        this.Id = IContainerShip.GetNextShipId();
    }

    public bool TryGetContainer(string serialNumber, out IContainer? container) => this.containers.TryGetValue(serialNumber, out container);

    public void Load(IContainer container)
    {
        if (this.containers.Count >= 30 || this.NetWeight + container.GrossWeight > this.maxNetWeight * 1000) throw new OverfillException("This ship cannot fit more containers onboard !");

        this.containers.Add(container.SerialNumber, container);
    }

    public void Unload(string serialNumber)
    {
        this.containers.Remove(serialNumber);
    }

    public override string ToString()
    {
        var builder = new System.Text.StringBuilder($"Cargo ship {this.Id} (max speed = {this.MaxNetWeight} kts, containers capacity = {this.ContainersCapacity}, tare weight = {this.TareWeight} t, max net weight = {this.MaxNetWeight} t, net weight = {this.NetWeight} kg, gross weight = {this.GrossWeight} kg), containers ");

        if (this.containers.Any())
        {
            builder.Append('↓');
            foreach (var str in this.containers.Select(c => c.Value.ToString())) builder.Append("\n · ").Append(str);
        }
        else builder.Append("=> [N/A]");

        return builder.ToString();
    }
}
