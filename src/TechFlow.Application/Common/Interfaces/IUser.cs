namespace TechFlow.Application.Common.Interfaces;

public interface IUser
{
    Guid? Id { get; }
    Guid CompanyId { get; }          
    bool IsInRole(string role);
}