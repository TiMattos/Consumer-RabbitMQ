using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consumer_RabbitMQ.DTO
{
    internal class QueueDTO
    {
        
        public string filiacao { get; set; }
        public string nome { get; set; }
        public string cpf { get; set; }
        public string data_nascimento { get; set; }

    }
}
