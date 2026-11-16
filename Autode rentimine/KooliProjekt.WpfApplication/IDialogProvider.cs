namespace KooliProjekt.WpfApplication
{
    public interface IDialogProvider
    {
        void ShowError(string message);
        bool ConfirmDelete(string numberPlate);
    }
}
