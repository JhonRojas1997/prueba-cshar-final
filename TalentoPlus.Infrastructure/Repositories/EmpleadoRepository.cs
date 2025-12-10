using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Interfaces;
using TalentoPlus.Infrastructure.Persistence;

namespace TalentoPlus.Infrastructure.Repositories;

// Esta clase es un ADAPTADOR (Adapter) de infraestructura.
// Implementa el PUERTO (Port) IEmpleadoRepository definido en el Dominio.
public class EmpleadoRepository : IEmpleadoRepository
{
    private readonly TalentoPlusDbContext _context;

    public EmpleadoRepository(TalentoPlusDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Empleado>> GetAllAsync()
    {
        return await _context.Empleados
            .Include(e => e.Cargo)
            .Include(e => e.Departamento)
            .Include(e => e.NivelEducativo)
            .ToListAsync();
    }

    public async Task<Empleado?> GetByIdAsync(int id)
    {
        return await _context.Empleados
            .Include(e => e.Cargo)
            .Include(e => e.Departamento)
            .Include(e => e.NivelEducativo)
            .FirstOrDefaultAsync(e => e.Id == id);
    }
    
    public async Task<Empleado?> GetByDocumentoAsync(string documento)
    {
        return await _context.Empleados
             .Include(e => e.Cargo)
            .Include(e => e.Departamento)
            .Include(e => e.NivelEducativo)
            .FirstOrDefaultAsync(e => e.Documento == documento);
    }

    public async Task<Empleado> AddAsync(Empleado empleado)
    {
        _context.Empleados.Add(empleado);
        await _context.SaveChangesAsync();
        return empleado;
    }

    public async Task UpdateAsync(Empleado empleado)
    {
        _context.Entry(empleado).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Empleado empleado)
    {
        _context.Empleados.Remove(empleado);
        await _context.SaveChangesAsync();
    }

    public async Task<int> GetCountAsync()
    {
        return await _context.Empleados.CountAsync();
    }

    public async Task<int> GetActiveCountAsync()
    {
        return await _context.Empleados.CountAsync(e => e.Estado == EstadoEmpleado.Activo);
    }

    public async Task<int> GetDepartmentsCountAsync()
    {
        return await _context.Departamentos.CountAsync();
    }
    
    public async Task<IEnumerable<Departamento>> GetDepartamentosAsync()
    {
        return await _context.Departamentos.ToListAsync();
    }
    
    public async Task<IEnumerable<Cargo>> GetCargosAsync()
    {
        return await _context.Cargos.ToListAsync();
    }
    
    public async Task<IEnumerable<NivelEducativo>> GetNivelesEducativosAsync()
    {
        return await _context.NivelesEducativos.ToListAsync();
    }
}
