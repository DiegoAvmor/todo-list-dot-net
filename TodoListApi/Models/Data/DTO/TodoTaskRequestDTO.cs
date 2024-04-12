using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoListApi.Models.Data.DTO
{
    public class TodoTaskRequestDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
}