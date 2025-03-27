namespace apbd_cw3;

class FreezerContainer : AbstractContainer
{
    public int Temperature { get; init; }
    
    public override string GetSerialNumberIdentifier() => "C";

    public override void Load(AbstractFreight freight)
    {
        if (this.Freight is not null && this.Freight.Weight > 0.5) throw new InvalidOperationException("The container has not been unloaded yet !");
        if (freight is not FreezerFreight lFreight) throw new InvalidOperationException("This container doesn't support non-freezer freight !");

        if (lFreight.FreightMassMaxLimitFactor * this.MaxNetWeight < lFreight.Weight) throw new OverfillException("This containter cannot fit this much freight");
        if (lFreight.MinTemperature < this.Temperature) throw new InvalidOperationException("Cargo for this container requires higher temperature than this container maintains !");

        this.Freight = freight;
    }

    public override void Unload()
    {
        this.Freight = null;
    }

    public override string ToString()
    {
        return $"Container {this.SerialNumber} (tare weight = {this.TareWeight} kg, max freight weight = {this.MaxNetWeight} kg, temperature = {this.Temperature} °C) with freight [{(this.Freight is not null ? this.Freight : "N/A")}]";
    }
}