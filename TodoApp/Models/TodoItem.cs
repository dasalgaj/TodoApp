using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApp.Models
{
    public class TodoItem
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } = "new";

        [ForeignKey("TodoListId")]
        public TodoList TodoList { get; set; }
        public int TodoListId { get; set; }
    }
}
