using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TanvirArjel.EFCore.GenericRepository;

namespace Application.Usecases.Admin.Users.GetUsersIds;

public class GetUsersIdsHandler
{
    IRepository _repository;

    public GetUsersIdsHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetUsersIdsResult> Handle(GetUsersIdsCommand command)
    {
        var users = await _repository.GetQueryable<User>().Select(u => u.Id ).ToListAsync();

        return new GetUsersIdsResult
        {
            Ids = users
        };
    }
}
