using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadHub.Application.Dtos
{
    public class AssignRoleDto
    {
        public string UserEmail { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
    }
}
