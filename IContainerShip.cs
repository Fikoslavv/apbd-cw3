namespace apbd_cw3;

interface IContainerShip
{
    private static uint nextShipId = 0;

    public string Id { get; init; }
    public abstract IEnumerable<IContainer> Containers { get; }
    public uint MaxSpeed { get; init; }
    public uint ContainersCapacity { get; init; }
    public long TareWeight { get; init; }
    public abstract long MaxNetWeight { get; }
    public abstract long NetWeight { get; }
    public abstract long GrossWeight { get; }

    public abstract IContainer this[string serialNumber] { get; }

    public abstract bool TryGetContainer(string serialNumber, out IContainer? container);

    public void Load(IContainer container);

    public void Unload(string serialNumber);

    protected sealed static string GetNextShipId()
    {
        return $"CS-{IContainerShip.nextShipId++}";
    }
}
