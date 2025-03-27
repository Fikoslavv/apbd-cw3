namespace apbd_cw3;

class LiquidContainer : AbstractContainer, IHazardNotifier
{
    public void CheckCondition()
    {
        try
        {
            if (this.Freight is not LiquidFreight lFreight) throw new InvalidOperationException("This container doesn't support non-liquid freight !");
    
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

    public override string GetSerialNumberIdentifier() => "L";

    public override void Load(AbstractFreight freight)
    {
        if (this.Freight is not null) throw new InvalidOperationException("The container has not been unloaded yet !");

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
        this.Freight = null;
    }
}
