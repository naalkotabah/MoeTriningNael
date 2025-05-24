using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moe.Core.Data;
using Moe.Core.Helpers;
using Moe.Core.Models.DTOs.User;
using Moe.Core.Models.DTOs.Warehouse;
using Moe.Core.Models.Entities;

namespace Moe.Core.Services;

public interface IWarehouseService
{
    Task<Response<WarehouseDTO>> Create(WarehouseFormDTO form);
    Task<Response<UserDTO>> CreateAndAssignAdmin(Guid warehouseId, WarehouseAdminFormDTO form);
    Task<Response<List<WarehouseDTO>>> GetAll();
    Task<Response<WarehouseDTO>> GetById(Guid id);
    Task<Response<WarehouseDTO>> Update(Guid id, WarehouseUpdateFormDTO dto);
    Task<Response<string>> Delete(Guid id);
    Task<Response<string>> RemoveAdminFromWarehouse(Guid warehouseId, Guid userId);
}
public class WarehouseService : BaseService, IWarehouseService
{
    public WarehouseService(MasterDbContext context, IMapper mapper) : base(context, mapper) { }
    public async Task<Response<WarehouseDTO>> Create(WarehouseFormDTO form)
    {
        var entity = _mapper.Map<Warehouse>(form);


        if (form.AdminIds != null && form.AdminIds.Any())
        {
            var admins = await _context.Users
                .Where(u => form.AdminIds.Contains(u.Id))
                .ToListAsync();

    
            if (admins.Count != form.AdminIds.Count)
            {
                return new Response<WarehouseDTO>(null, "بعض المدراء غير موجودين في النظام", 400);
            }

           
            foreach (var user in admins)
            {
                if (user.StaticRole == StaticRole.NORMAL)
                {
                    user.StaticRole = StaticRole.ADMIN;
                    _context.Users.Update(user);
                }
            }

            entity.Admins = admins;
        }

        _context.warehouses.Add(entity);
        await _context.SaveChangesAsync();

        var dto = _mapper.Map<WarehouseDTO>(entity);
        return new Response<WarehouseDTO>(dto, "تم إنشاء المستودع", 201);
    }
    public async Task<Response<List<WarehouseDTO>>> GetAll()
    {
        var warehouses = await _context.warehouses
            .Include(w => w.Admins)
            .Include(w => w.WarehouseItems)
                .ThenInclude(wi => wi.Item)
            .ToListAsync();

        var result = warehouses.Select(w => new WarehouseDTO
        {
            Id = w.Id,
            Name = w.Name,
            Location = w.Location,
            AdminNames = w.Admins.Select(a => a.Name).ToList(),
            Items = w.WarehouseItems.Select(wi => new WarehouseItemDTO
            {
                ItemName = wi.Item.Name,
                Quantity = wi.Quantity
            }).ToList()
        }).ToList();

        return new Response<List<WarehouseDTO>>(result, null, 200);
    }
    public async Task<Response<UserDTO>> CreateAndAssignAdmin(Guid warehouseId, WarehouseAdminFormDTO form)
    {
  
        var warehouse = await _context.warehouses
            .Include(w => w.Admins)
            .FirstOrDefaultAsync(w => w.Id == warehouseId);

        if (warehouse == null)
            return new Response<UserDTO>(null, "المستودع غير موجود", 404);

 
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u =>
                (!string.IsNullOrEmpty(form.Email) && u.Email == form.Email) ||
                (!string.IsNullOrEmpty(form.Phone) && u.Phone == form.Phone) ||
                (!string.IsNullOrEmpty(form.Username) && u.Username == form.Username));

        if (existingUser != null)
            return new Response<UserDTO>(null, "المستخدم موجود مسبقًا (بريد أو هاتف أو اسم مستخدم)", 400);

    
        var user = new User
        {
            Name = form.Name,
            Email = form.Email,
            Phone = form.Phone,
            PhoneCountryCode = form.PhoneCountryCode,
            Username = form.Username,
            StaticRole = StaticRole.ADMIN
        };

        var (hash, salt) = PasswordHelper.CreatePasswordHash(form.Password);
        user.PasswordHash = hash;
        user.PasswordSalt = salt;

        _context.Users.Add(user);

       
        warehouse.Admins.Add(user);

        await _context.SaveChangesAsync();

        var dto = _mapper.Map<UserDTO>(user);
        return new Response<UserDTO>(dto, "تم إنشاء المدير وربطه بالمستودع بنجاح", 201);
    }
    public async Task<Response<WarehouseDTO>> Update(Guid id, WarehouseUpdateFormDTO dto)
    {
        var warehouse = await _context.warehouses
            .Include(w => w.Admins)
            .FirstOrDefaultAsync(w => w.Id == id);

        if (warehouse == null)
            return new Response<WarehouseDTO>(null, "المستودع غير موجود", 404);

        warehouse.Name = dto.Name;
        warehouse.Location = dto.Location;

        await _context.SaveChangesAsync();

        var result = _mapper.Map<WarehouseDTO>(warehouse);
        return new Response<WarehouseDTO>(result, "تم التحديث", 200);
    }
    public async Task<Response<string>> Delete(Guid id)
    {
        var warehouse = await _context.warehouses.FindAsync(id);
        if (warehouse == null)
            return new Response<string>(null, "المستودع غير موجود", 404);

        _context.warehouses.Remove(warehouse); 
        await _context.SaveChangesAsync();

        return new Response<string>("تم حذف المستودع", null, 200);
    }
    public async Task<Response<WarehouseDTO>> GetById(Guid id)
    {
        var warehouse = await _context.warehouses
            .Include(w => w.Admins)
            .Include(w => w.WarehouseItems)
                .ThenInclude(wi => wi.Item)
            .FirstOrDefaultAsync(w => w.Id == id);

        if (warehouse == null)
            return new Response<WarehouseDTO>(null, "المستودع غير موجود", 404);

        var dto = new WarehouseDTO
        {
            Id = warehouse.Id,
            Name = warehouse.Name,
            Location = warehouse.Location,
            AdminNames = warehouse.Admins.Select(a => a.Name).ToList(),
            Items = warehouse.WarehouseItems.Select(wi => new WarehouseItemDTO
            {
                ItemName = wi.Item.Name,
                Quantity = wi.Quantity
            }).ToList()
        };

        return new Response<WarehouseDTO>(dto, null, 200);
    }
    public async Task<Response<string>> RemoveAdminFromWarehouse(Guid warehouseId, Guid userId)
    {
        
        var warehouse = await _context.warehouses
            .Include(w => w.Admins)
            .FirstOrDefaultAsync(w => w.Id == warehouseId);

        if (warehouse == null)
            return new Response<string>(null, "المستودع غير موجود", 404);

     
        var adminToRemove = warehouse.Admins.FirstOrDefault(a => a.Id == userId);

        if (adminToRemove == null)
            return new Response<string>(null, "هذا المستخدم ليس مديرًا لهذا المستودع", 400);

      
        warehouse.Admins.Remove(adminToRemove);
        await _context.SaveChangesAsync();

        return new Response<string>("تم إزالة المدير من المستودع", null, 200);
    }


}



