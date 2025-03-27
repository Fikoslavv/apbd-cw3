namespace apbd_cw3;

class OverfillException : Exception
{
    public OverfillException() { }
    public OverfillException(string message) : base(message) { }
}