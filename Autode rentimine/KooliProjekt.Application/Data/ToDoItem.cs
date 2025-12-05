namespace KooliProjekt.Application.Data
{
    public class ToDoItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsDone { get; set; }

        public ToDoList ToDoList { get; set; }
        public int ToDoListId { get; set; }
    }
}
