using AutoMapper;
using AutoMapper.QueryableExtensions;
using Moe.Core.Models.DTOs;
using Moe.Core.Data;
using Moe.Core.Extensions;
using Moe.Core.Helpers;
using Moe.Core.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Moe.Core.Null;

namespace Moe.Core.Services;

public interface IWarehouseItemTransactionsService
{
    Task<Response<PagedList<WarehouseItemTransactionDTO>>> GetAll(WarehouseItemTransactionFilter filter);
    Task<Response<WarehouseItemTransactionDTO>> GetById(Guid id);
    Task<Response<WarehouseItemTransactionDTO>> Create(WarehouseItemTransactionFormDTO form);
 
    Task Delete(Guid id);
}

public class WarehouseItemTransactionsService : BaseService, IWarehouseItemTransactionsService
{
    public WarehouseItemTransactionsService(MasterDbContext context, IMapper mapper) : base(context, mapper)
    { }

    public async Task<Response<PagedList<WarehouseItemTransactionDTO>>> GetAll(WarehouseItemTransactionFilter filter)
    {
        var transactions = await _context.WarehouseItemTransactions
            .WhereBaseFilter(filter)
            .OrderByCreationDate()
            .ProjectTo<WarehouseItemTransactionDTO>(_mapper.ConfigurationProvider)
            .Paginate(filter);

        return new Response<PagedList<WarehouseItemTransactionDTO>>(transactions, null, 200);
    }

    public async Task<Response<WarehouseItemTransactionDTO>> GetById(Guid id)
    {
        var dto = await _context.GetByIdOrException<WarehouseItemTransaction, WarehouseItemTransactionDTO>(id);
        return new Response<WarehouseItemTransactionDTO>(dto, null, 200);
    }

    public async Task<Response<WarehouseItemTransactionDTO>> Create(WarehouseItemTransactionFormDTO form)
    {
        await _context.EnsureEntityExists<Warehouse>(form.ToWarehouseId, "ToWarehouse is not Found");
        await _context.EnsureEntityExists<Item>(form.ItemId, "Item is not Found");
        var transaction = new WarehouseItemTransaction
        {
            FromId = form.FromWarehouseId,
            ToId = form.ToWarehouseId,
            ItemId = form.ItemId,
            Qty = form.Qtu,
          
        };
       
        await _context.WarehouseItemTransactions.AddAsync(transaction);

        if (form.FromWarehouseId != null)
        {
 
            await _context.EnsureEntityExists<Warehouse>(form.FromWarehouseId, "FromWarehouse is not Found");
            var fromWarehouseItem = await _context.WarehouseItems
                .Where(e => e.WarehouseId == form.FromWarehouseId && e.ItemId == form.ItemId)
                .FirstOrDefaultAsync();

            if (fromWarehouseItem == null || fromWarehouseItem.Qty < form.Qtu)
                return new Response<WarehouseItemTransactionDTO>(null, "Insufficient quantity in source warehouse", 400);

            fromWarehouseItem.Qty -= form.Qtu;
            _context.WarehouseItems.Update(fromWarehouseItem);
        }
        var toWarehouseItem = await _context.WarehouseItems
            .Where(e => e.WarehouseId == form.ToWarehouseId && e.ItemId == form.ItemId)
            .FirstOrDefaultAsync();

        if (toWarehouseItem == null)
        {
            toWarehouseItem = new WarehouseItem
            {
                WarehouseId = form.ToWarehouseId,
                ItemId = form.ItemId,
                Qty = form.Qtu,
            };
            await _context.WarehouseItems.AddAsync(toWarehouseItem);
        }
        else
        {
            toWarehouseItem.Qty += form.Qtu;
            _context.WarehouseItems.Update(toWarehouseItem);
        }
  
        await _context.SaveChangesAsync();

        var dto = _mapper.Map<WarehouseItemTransactionDTO>(transaction);
        return new Response<WarehouseItemTransactionDTO>(dto, null, 200);
    }



    public async Task Delete(Guid id) =>
        await _context.SoftDeleteOrException<WarehouseItemTransaction>(id);
}
