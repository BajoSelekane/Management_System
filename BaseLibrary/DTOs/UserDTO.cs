using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibrary.DTOs
{
    using System;

    namespace BaseLibrary.DTOs
    {
        public class UserDTO
        {
            public int Id { get; set; }
            public string Email { get; set; }
            public string FullName { get; set; }
            public DateTime CreatedAt { get; set; }
            public bool EmailConfirmed { get; set; }
        }
    }
}
