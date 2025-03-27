namespace apbd_cw3;

abstract class AbstractContainer : IContainer
{
    protected long tareWeight;
    protected long maxNetWeight;
    protected double height;
    protected double depth;
    protected string serialNumber = string.Empty;
    protected AbstractFreight? freight;

    public long TareWeight { get => this.tareWeight; init => this.tareWeight = value; }

    public long MaxNetWeight { get => this.maxNetWeight; init => this.maxNetWeight = value; }

    public double Height { get => this.height; set => this.height = value; }
    public double Depth { get => this.depth; set => this.depth = value; }

    public string SerialNumber => this.serialNumber;

    public AbstractFreight? Freight { get => this.freight; set => this.freight = value; }

    public AbstractContainer()
    {
        this.serialNumber = IContainer.GetNextSerialNumber(this);
    }

    public abstract void Load(AbstractFreight freight);

    public abstract void Unload();

    public abstract string GetSerialNumberIdentifier();

    public override string ToString()
    {
        return $"Container {this.SerialNumber} (tare weight = {this.TareWeight} kg, max freight weight = {this.MaxNetWeight} kg) with freight [{(this.Freight is not null ? this.Freight : "N/A")}]";
    }
}