using System.Windows;

namespace KooliProjekt.WpfApplication
{
    public class DialogProvider : IDialogProvider
    {
        public void ShowError(string message)
        {
            MessageBox.Show(message, "Cars API", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public bool ConfirmDelete(string numberPlate)
        {
            return MessageBox.Show(
                $"Are you sure you want to delete car {numberPlate}?",
                "Confirm deletion",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning) == MessageBoxResult.Yes;
        }
    }
}
