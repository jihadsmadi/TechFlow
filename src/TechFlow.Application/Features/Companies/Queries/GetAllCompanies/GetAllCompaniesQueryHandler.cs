using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Features.Companies.Dtos;
using TechFlow.Application.Features.Companies.Mappers;
using TechFlow.Application.Features.Permissions.DTOs;
using TechFlow.Application.Features.Permissions.Queries.GetPermissionById;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Companies.Queries.GetAllCompanies
{
    public sealed class GetAllCompaniesQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetAllCompaniesQuery, Result<List<CompanyDto>>>
    {
        public async Task<Result<List<CompanyDto>>> Handle(GetAllCompaniesQuery request, CancellationToken ct)
        {
            var companies = await unitOfWork.Companies.GetAllAsync(ct);

            return companies.ToDtos();
        }
    }
}
