using System.Collections.Generic;
using KooliProjekt.WindowsForms.Api;

namespace KooliProjekt.WindowsForms
{
    public interface IMainView
    {
        IList<Car> DataSource { get; set; }
        Car SelectedItem { get; set; }
        string Id { get; set; }
        string NumberPlate { get; set; }
        string TypeId { get; set; }
        string Kmrate { get; set; }
        string TimeRate { get; set; }
        bool IsAvailable { get; set; }
        bool ConfirmDelete();
        void FocusNumberPlate();
        void ShowError(string message);
    }
}
