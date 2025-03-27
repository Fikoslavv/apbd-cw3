namespace apbd_cw3;

class FreezerFreight : AbstractFreight
{
    public override double FreightMassMaxLimitFactor { get => 1; }

    public override double FreightMassMinLimitFactor { get => 0; }
    public override string Name { get; init; } = string.Empty;
    public override bool IsHazardous { get; init; }
    public override long Weight { get; init; }
}