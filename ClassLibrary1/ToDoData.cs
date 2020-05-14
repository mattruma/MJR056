using Newtonsoft.Json;
using System;

namespace ClassLibrary1
{
    public class ToDoData
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("toDoId")]
        public string ToDoId { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("dueOn")]
        public DateTime? DueOn { get; set; }

        [JsonProperty("isCompleted")]
        public bool IsCompleted { get; set; }

        [JsonProperty("completedOn")]
        public DateTime? CompletedOn { get; set; }

        public ToDoData()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Object = "ToDo";
            this.ToDoId = this.Id;
        }
    }
}
