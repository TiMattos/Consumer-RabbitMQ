using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Worker_RabbitMQ.DTO
{
    internal class QueueDTO
    {
        
        public string address { get; set; }
        public string name { get; set; }
        public string cpf { get; set; }
        public string birthDate { get; set; }

    }
}
