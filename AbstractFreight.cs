namespace apbd_cw3;

abstract class AbstractFreight
{
    public abstract string Name { get; init; }
    public abstract bool IsHazardous { get; init; }
    public abstract long Weight { get; init; }
    public abstract double FreightMassMaxLimitFactor { get; }
    public abstract double FreightMassMinLimitFactor { get; }
    public double MinTemperature { get; init; }

    public override string ToString()
    {
        return $"{(this.IsHazardous ? "H" : "Non-h")}azardous freight {this.Name} ({this.Weight} kg)";
    }
}
