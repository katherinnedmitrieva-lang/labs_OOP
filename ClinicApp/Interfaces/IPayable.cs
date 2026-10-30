namespace ClinicApp.Interfaces
{
    public interface IPayable
    {
        decimal Cost { get; }
        bool IsPaid { get; }
        void MarkPaid();
    }
}