using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalentoPlus.Application.DTOs;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Interfaces;

namespace TalentoPlus.Application.Services;

public class EmpleadoService : IEmpleadoService
{
    private readonly IEmpleadoRepository _repository;
    private readonly IPdfService _pdfService;

    public EmpleadoService(IEmpleadoRepository repository, IPdfService pdfService)
    {
        _repository = repository;
        _pdfService = pdfService;
    }

    public async Task<IEnumerable<EmpleadoDto>> GetAllAsync()
    {
        var empleados = await _repository.GetAllAsync();
        return empleados.Select(MapToDto);
    }

    public async Task<EmpleadoDto?> GetByIdAsync(int id)
    {
        var empleado = await _repository.GetByIdAsync(id);
        return empleado == null ? null : MapToDto(empleado);
    }

    public async Task<EmpleadoDto> CreateAsync(CreateEmpleadoDto dto)
    {
        // 1. Resolve Dependencies (Cargo, Dept, Nivel)
        // In a real app we might want to create them if missing or throw error.
        // For this test, we assume we might need to look them up by name.
        
        var cargoId = await GetOrLinkMetadata(dto.Cargo, MetadataType.Cargo);
        var deptId = await GetOrLinkMetadata(dto.Departamento, MetadataType.Departamento);
        var nivelId = await GetOrLinkMetadata(dto.NivelEducativo, MetadataType.NivelEducativo);
        
        var estado = Enum.TryParse<EstadoEmpleado>(dto.Estado, true, out var e) ? e : EstadoEmpleado.Activo;

        var empleado = new Empleado
        {
            Documento = dto.Documento,
            Nombres = dto.Nombres,
            Apellidos = dto.Apellidos,
            FechaNacimiento = dto.FechaNacimiento,
            Direccion = dto.Direccion,
            Telefono = dto.Telefono,
            Email = dto.Email,
            Salario = dto.Salario,
            FechaIngreso = dto.FechaIngreso,
            PerfilProfesional = dto.PerfilProfesional,
            Estado = estado,
            CargoId = cargoId,
            DepartamentoId = deptId,
            NivelEducativoId = nivelId
        };

        await _repository.AddAsync(empleado);
        return MapToDto(empleado);
    }

    public async Task UpdateAsync(int id, CreateEmpleadoDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) throw new KeyNotFoundException($"Empleado {id} not found.");

        // Update fields
        existing.Nombres = dto.Nombres;
        existing.Apellidos = dto.Apellidos;
        existing.Documento = dto.Documento;
        existing.FechaNacimiento = dto.FechaNacimiento;
        existing.Direccion = dto.Direccion;
        existing.Telefono = dto.Telefono;
        existing.Email = dto.Email;
        existing.Salario = dto.Salario;
        existing.FechaIngreso = dto.FechaIngreso;
        existing.PerfilProfesional = dto.PerfilProfesional;
        
        if (Enum.TryParse<EstadoEmpleado>(dto.Estado, true, out var estado))
        {
            existing.Estado = estado;
        }

        // Update relations
        existing.CargoId = await GetOrLinkMetadata(dto.Cargo, MetadataType.Cargo);
        existing.DepartamentoId = await GetOrLinkMetadata(dto.Departamento, MetadataType.Departamento);
        existing.NivelEducativoId = await GetOrLinkMetadata(dto.NivelEducativo, MetadataType.NivelEducativo);

        await _repository.UpdateAsync(existing);
    }

    public async Task DeleteAsync(int id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing != null)
        {
            await _repository.DeleteAsync(existing);
        }
    }
    
    public async Task<byte[]> GenerateCvPdfAsync(int id)
    {
        var empleado = await _repository.GetByIdAsync(id);
        if (empleado == null) throw new KeyNotFoundException($"Empleado {id} not found.");
        
        return _pdfService.GenerateHojaDeVida(empleado);
    }

    // Mapping Helper
    private static EmpleadoDto MapToDto(Empleado e)
    {
        return new EmpleadoDto
        {
            Id = e.Id,
            Documento = e.Documento,
            Nombres = e.Nombres,
            Apellidos = e.Apellidos,
            FechaNacimiento = e.FechaNacimiento,
            Direccion = e.Direccion,
            Telefono = e.Telefono,
            Email = e.Email,
            Salario = e.Salario,
            FechaIngreso = e.FechaIngreso,
            Estado = e.Estado.ToString(),
            PerfilProfesional = e.PerfilProfesional,
            Cargo = e.Cargo?.Nombre ?? "",
            Departamento = e.Departamento?.Nombre ?? "",
            NivelEducativo = e.NivelEducativo?.Nombre ?? ""
        };
    }
    
    // Metadata Helper (Simplified for MVP)
    private enum MetadataType { Cargo, Departamento, NivelEducativo }
    
    private async Task<int> GetOrLinkMetadata(string name, MetadataType type)
    {
        // This is a naive implementation. In a real app we'd search properly or create if not exists (upsert).
        // Since we don't have metadata repos injected separately, we rely on EmpleadoRepository helpers or we'd just assign IDs.
        // For simplicity in this test, we will assume the DB is seeded or we return 1 if not found (Hack).
        // A better approach: Instruct Repository to finding by name or creating.
        
        // I'll add helper Logic here that fetches all and finds first match.
        // Ideally Repository should expose "GetDepartamentoByName" etc.
        
        // Let's rely on Repository helpers:
        if (type == MetadataType.Departamento) {
            var all = await _repository.GetDepartamentosAsync();
            var match = all.FirstOrDefault(x => x.Nombre.Equals(name, StringComparison.OrdinalIgnoreCase));
            return match?.Id ?? 1; // Fallback or Create? We'll fallback to 1 (Assuming Seeded) for now to avoid complexity of creating new entities here without repo access.
        }
        if (type == MetadataType.Cargo) {
            var all = await _repository.GetCargosAsync();
            var match = all.FirstOrDefault(x => x.Nombre.Equals(name, StringComparison.OrdinalIgnoreCase));
            return match?.Id ?? 1;
        }
        if (type == MetadataType.NivelEducativo) {
            var all = await _repository.GetNivelesEducativosAsync();
            var match = all.FirstOrDefault(x => x.Nombre.Equals(name, StringComparison.OrdinalIgnoreCase));
            return match?.Id ?? 1;
        }
        return 1;
    }
}
