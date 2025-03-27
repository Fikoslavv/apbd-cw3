namespace apbd_cw3;

interface IContainer
{
    public static IDictionary<Type, ulong> serialNumsSequence = new Dictionary<Type, ulong>();

    public AbstractFreight? Freight { get; protected set; }
    public long TareWeight { get; init; }
    public long MaxNetWeight { get; }
    public long GrossWeight { get => this.TareWeight + (this.Freight is not null ? this.Freight.Weight : 0); }
    public double Height { get; set; }
    public double Depth { get; set; }
    public string SerialNumber { get; }

    public void Unload();
    public void Load(AbstractFreight freight);
    protected string GetSerialNumberIdentifier();

    protected static string GetNextSerialNumber(IContainer container)
    {
        var containerType = container.GetType();
        if (!IContainer.serialNumsSequence.TryGetValue(containerType, out var lastId))
        {
            IContainer.serialNumsSequence.Add(containerType, 0);
            lastId = 0;
        }
        else
        {
            lastId++;
            IContainer.serialNumsSequence[containerType] = lastId;
        }

        return $"KON-{container.GetSerialNumberIdentifier()}-{lastId}";
    }
}
