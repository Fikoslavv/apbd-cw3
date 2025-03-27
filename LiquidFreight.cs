namespace apbd_cw3;

class LiquidFreight : AbstractFreight
{
    public override double FreightMassMaxLimitFactor { get => this.IsHazardous ? 0.5 : 0.9; }

    public override double FreightMassMinLimitFactor { get => 0; }
    public override string Name { get; init; } = string.Empty;
    public override bool IsHazardous { get; init; }
    public override long Weight { get; init; }
}
