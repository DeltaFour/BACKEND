using DeltaFour.Domain.Entities;

namespace DeltaFour.Domain.IRepositories;

public interface ITimeSheetRepository : IBaseRepository<TimeSheet>
{
    /// <summary>
    /// Busca uma folha de ponto por usuário, mês e ano
    /// </summary>
    Task<TimeSheet?> FindByUserMonthYear(Guid userId, int month, int year);

    /// <summary>
    /// Busca todas as folhas de ponto que satisfazem o predicado
    /// </summary>
    Task<List<TimeSheet>> FindAll(System.Linq.Expressions.Expression<System.Func<TimeSheet, bool>> predicate);

    /// <summary>
    /// Cria uma nova folha de ponto
    /// </summary>
    void Create(TimeSheet timeSheet);

    /// <summary>
    /// Atualiza uma folha de ponto existente
    /// </summary>
    void Update(TimeSheet timeSheet);
}
