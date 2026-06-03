using DeltaFour.Application.Dtos.Requests;
using DeltaFour.Application.Dtos.Responses.Department;
using DeltaFour.Application.Services;
using DeltaFour.CrossCutting.Middleware;
using DeltaFour.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeltaFour.API.Controllers;

[Route("api/v1/department")]
[ApiController]
[Authorize(Policy = "RH_OR_ADMIN")]
public class DepartmentController : ControllerBase
{
    private readonly DepartmentService _departmentService;

    public DepartmentController(DepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    /// <summary>
    /// Cria um novo departamento no sistema.
    /// </summary>
    /// <remarks>
    /// Apenas usuários com perfil de ADMIN ou RH podem criar departamentos.
    /// </remarks>
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateDepartmentRequest request)
    {
        var user = HttpContext.GetUserAuthenticated<UserContext>();

        await _departmentService.Create(request, user.Id, user.CompanyId);

        return NoContent();
    }

    /// <summary>
    /// Lista todos os departamentos cadastrados da empresa do usuário autenticado.
    /// </summary>
    [HttpGet("list")]
    public async Task<ActionResult<ListDepartmentsResponse>> List()
    {
        var user = HttpContext.GetUserAuthenticated<UserContext>();

        var departments = await _departmentService.List(user.CompanyId);

        return Ok(departments);
    }

    /// <summary>
    /// Atualiza os dados de um departamento existente.
    /// </summary>
    /// <remarks>
    /// Apenas usuários com perfil de ADMIN ou RH podem atualizar departamentos.
    /// </remarks>
    [HttpPut("update/{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDepartmentRequest request)
    {
        var user = HttpContext.GetUserAuthenticated<UserContext>();

        await _departmentService.Update(id, request, user.Id, user.CompanyId);

        return NoContent();
    }

    /// <summary>
    /// Remove um departamento do sistema.
    /// </summary>
    /// <remarks>
    /// A remoção só é permitida se o departamento não estiver vinculado a nenhum usuário.
    /// Apenas usuários com perfil de ADMIN ou RH podem remover departamentos.
    /// </remarks>
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var user = HttpContext.GetUserAuthenticated<UserContext>();

        await _departmentService.Delete(id, user.CompanyId);

        return NoContent();
    }
}
