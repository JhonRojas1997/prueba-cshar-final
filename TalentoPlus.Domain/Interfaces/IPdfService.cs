using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Domain.Interfaces;

public interface IPdfService
{
    byte[] GenerateHojaDeVida(Empleado empleado);
}
