using AutoMapper;
using Moe.Core.Data;

namespace Moe.Core.Services;

public class BaseService
{
    protected readonly MasterDbContext _context;
    protected readonly IMapper _mapper;

    public BaseService(MasterDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
}