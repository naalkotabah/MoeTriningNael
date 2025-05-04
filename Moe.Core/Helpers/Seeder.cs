using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Moe.Core.Data;
using Moe.Core.Extensions;
using Moe.Core.Models.Entities;

namespace Moe.Core.Helpers;

public class Seeder
{
    private MasterDbContext _masterDbContext;
    private Random random = new Random();

    public Seeder(MasterDbContext masterDbContext)
    {
        _masterDbContext = masterDbContext;
    }

    public async Task SeedDefaultUsers()
    {
        var users = new List<(string Role, string Email, string Phone, string PhoneCode, string Name)>
    {
        ("SUPER_ADMIN", "superadmin@example.com", "700000001", "+964", "Super Admin"),
        ("ADMIN", "admin@example.com", "700000002", "+964", "System Admin"),
        ("NORMAL", "normal@example.com", "700000003", "+964", "Test User")
    };

        foreach (var (role, email, phone, phoneCode, name) in users)
        {
            if (!await _masterDbContext.Users.AnyAsync(u => u.Email == email))
            {
                var user = new User
                {
                    StaticRole = Enum.Parse<StaticRole>(role),
                    Email = email,
                    Phone = phone,
                    PhoneCountryCode = phoneCode,
                    Name = name
                };

                using var hmac = new HMACSHA512();
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("string"));
                user.PasswordSalt = hmac.Key;

                await _masterDbContext.Users.AddAsync(user);
            }
        }

        await _masterDbContext.SaveChangesAsync();
    }


    public async Task SeedCountriesCities(bool ignoreExistingRecords=false)
    {
        if (!ignoreExistingRecords)
        {
            var hasRecords = await _masterDbContext.Countries.AnyAsync();
            if (hasRecords)
                return;
        }
        
        if (!await _masterDbContext.Countries.AnyAsync())
        {
            var iraqCitiesData =
                new List<(string NameEn, string NameAr)>()
                {
                    ("Baghdad", "بغداد"),
                    ("Basra", "البصرة"),
                    ("Mosul", "الموصل"),
                    ("Erbil", "أربيل"),
                    ("Najaf", "النجف"), 
                    ("Kirkuk", "كركوك"), 
                    ("Sulaymaniyah", "السليمانية"), 
                    ("Dhi Qar", "ذي قار"), 
                    ("Anbar", "الأنبار"), 
                    ("Karbala", "كربلاء"), 
                    ("Duhok", "دهوك"), 
                    ("Muthanna", "المثنى"), 
                    ("Qadisiyyah", "القادسية"), 
                    ("Wasit", "واسط"), 
                    ("Babylon", "بابل"),
                };
            var chinaCitiesData = new List<(string NameEn, string NameAr)>()
                {
                    ("Beijing", "بكين"),
                    ("Shanghai", "شنغهاي"),
                    ("Guangzhou", "قوانغتشو"),
                    ("Shenzhen", "شنتشن"),
                    ("Chengdu", "تشنغدو"),
                    ("Wuhan", "ووهان"),
                    ("Xi'an", "شيان"),
                    ("Hangzhou", "هانغتشو"),
                    ("Nanjing", "نانجينغ"),
                    ("Tianjin", "تيانجين")
                };
            var germanyCitiesData = new List<(string NameEn, string NameAr)>
                {
                    ("Berlin", "برلين"),
                    ("Munich", "ميونيخ"),
                    ("Hamburg", "هامبورغ"),
                    ("Frankfurt", "فرانكفورت"),
                    ("Cologne", "كولونيا"),
                    ("Stuttgart", "شتوتغارت"),
                    ("Düsseldorf", "دوسلدورف"),
                    ("Dortmund", "دورتموند"),
                    ("Essen", "إيسن"),
                    ("Leipzig", "لايبزيغ")
                };

            var iraq = new Country()
            {
                Name = new LocalizedContent()
                {
                    En = "Iraq",
                    Ar = "العراق",
                },
                Cities = iraqCitiesData.Select(e =>
                    new City()
                    {
                        Name = new LocalizedContent(e.NameEn, e.NameAr),
                    }).ToList()
            };
            var germany = new Country()
            {
                Name = new LocalizedContent()
                {
                    En = "Germany",
                    Ar = "المانيا",
                },
                Cities = germanyCitiesData.Select(e =>
                    new City()
                    {
                        Name = new LocalizedContent(e.NameEn, e.NameAr),
                    }).ToList()
            };
            var china = new Country()
            {
                Name = new LocalizedContent()
                {
                    En = "China",
                    Ar = "الصين",
                },
                Cities = chinaCitiesData.Select(e =>
                    new City()
                    {
                        Name = new LocalizedContent(e.NameEn, e.NameAr),
                    }).ToList()
            };

            await _masterDbContext.Countries.AddAsync(iraq);
            await _masterDbContext.Countries.AddAsync(germany);
            await _masterDbContext.Countries.AddAsync(china);
            await _masterDbContext.SaveChangesAsync();
        }
    }
}
