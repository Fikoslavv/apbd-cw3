namespace apbd_cw3;

class GasContainer : AbstractContainer, IHazardNotifier
{
    public double Pressure { get => this.Freight is not null ? this.Freight.Weight / (double)this.MaxNetWeight : 1; }

    public void CheckCondition()
    {
        try
        {
            if (this.Freight is not GasFreight lFreight) throw new InvalidOperationException("This container doesn't support non-gas freight !");

            if (lFreight.FreightMassMaxLimitFactor * this.MaxNetWeight < lFreight.Weight) throw new OverfillException("This containter cannot fit this much freight");
        }
        catch
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine($"[{this.SerialNumber}] Hazardous conditions detected !");
            System.Console.ResetColor();
            throw;
        }
    }

    public override string GetSerialNumberIdentifier() => "G";

    public override void Load(AbstractFreight freight)
    {
        if (this.Freight is not null && this.Freight.Weight > 0.5) throw new InvalidOperationException("The container has not been unloaded yet !");

        var prevFreight = this.Freight;
        this.Freight = freight;

        try { this.CheckCondition(); }
        catch
        {
            this.Freight = prevFreight;
            throw;
        }
    }

    public override void Unload()
    {
        if (this.Freight is null || this.Freight is not GasFreight freight) return;

        this.Freight = freight.Weight * freight.FreightMassMinLimitFactor > 0.5 ? new GasFreight() { IsHazardous = freight.IsHazardous, Weight = (long)(freight.Weight * freight.FreightMassMinLimitFactor) } : null;
    }

    public override string ToString()
    {
        return $"Container {this.SerialNumber} (tare weight = {this.TareWeight} kg, max freight weight = {this.MaxNetWeight} kg, pressure = {this.Pressure} atm) with freight [{(this.Freight is not null ? this.Freight : "N/A")}]";
    }
}
