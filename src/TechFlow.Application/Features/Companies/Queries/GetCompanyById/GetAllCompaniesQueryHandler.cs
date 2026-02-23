using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using TechFlow.Application.Common.Errors;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Features.Companies.Dtos;
using TechFlow.Application.Features.Companies.Mappers;
using TechFlow.Application.Features.Permissions.DTOs;
using TechFlow.Application.Features.Permissions.Queries.GetPermissionById;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Companies;

namespace TechFlow.Application.Features.Companies.Queries.GetCompanyById
{
    public sealed class GetCompanyByIdQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetCompanyByIdQuery, Result<CompanyDto>>
    {
        public async Task<Result<CompanyDto>> Handle(GetCompanyByIdQuery request, CancellationToken ct)
        {
            var company = await unitOfWork.Companies.GetByIdAsync(request.Id,ct);

            if(company is null)
            {
                return CompanyErrors.NotFound;
            }

            return company.ToDto();
        }
    }
}
