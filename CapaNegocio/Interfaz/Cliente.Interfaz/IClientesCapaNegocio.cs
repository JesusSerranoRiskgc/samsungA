using CapaDTO.Peticiones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio.Interfaz.Cliente.Interfaz
{
    public interface IClientesCapaNegocio
    {
        Task<List<ClienteDto>> ListaClientes(int IdArea);
        Task<bool> CrearCliente(ClienteDto cliente);
        Task<bool> EditarCliente(ClienteDto cliente);
        Task<bool> DeleteCliente(int IdCliente);

        Task<List<ClienteDto>> ListaTodosClientes();

    }
}
