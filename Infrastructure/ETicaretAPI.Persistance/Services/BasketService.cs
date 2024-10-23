using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Application.Services;
using ETicaretAPI.Application.ViewModels.Baskets;
using ETicaretAPI.Domain.Entities;
using ETicaretAPI.Domain.Entities.Identity;
using ETicaretAPI.Persistance.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistance.Services
{
    public class BasketService : IBasketService
    {
        readonly IHttpContextAccessor _contextAccessor;
        readonly UserManager<AppUser> _userManager;
        readonly IBasketItemWriteRepository _basketItemWriteRepository;
        readonly IBasketItemReadRepository _basketItemReadRepository;
        readonly IOrderReadRepository _orderReadRepository;
        readonly IBasketWriteRepository _basketWriteRepository;
        readonly IBasketReadRepository _basketReadRepository;
        public BasketService(
            IHttpContextAccessor contextAccessor,
            UserManager<AppUser> userManager,
            IBasketItemWriteRepository basketItemWriteRepository,
            IBasketItemReadRepository basketItemReadRepository,
            IOrderReadRepository orderReadRepository,
            IBasketWriteRepository basketWriteRepository,
            IBasketReadRepository  basketReadRepository
            )

        {
            _contextAccessor = contextAccessor;
            _userManager = userManager;
            _basketItemWriteRepository = basketItemWriteRepository;
            _basketItemReadRepository = basketItemReadRepository;
            _orderReadRepository = orderReadRepository;
            _basketWriteRepository = basketWriteRepository;
            _basketReadRepository = basketReadRepository;
        }

        private async Task<Basket> ContextUser() {

            var username = _contextAccessor?.HttpContext?.User?.Identity?.Name;
            if (!string.IsNullOrEmpty(username))
            {
                AppUser? user=  await _userManager.Users.Include(b=>b.Baskets).
                    FirstOrDefaultAsync(u=>u.UserName == username);

                var _basket = from basket in user.Baskets
                              join order in _orderReadRepository.Table
                              on basket.Id equals order.Id into BasketOrders
                              from order in BasketOrders.DefaultIfEmpty()
                              select new
                              {
                                 Basket= basket,
                                  Order=order
                              };
                Basket? targetBasket= null;
                if (_basket.Any(b => b.Order is null))
                {
                    targetBasket = _basket.FirstOrDefault(b => b.Order is null)?.Basket;
                }
                else
                {
                    targetBasket = new();
                    user.Baskets.Add(targetBasket);
                   
                }
                await _basketWriteRepository.SaveAsync();
                return targetBasket;

            }
            throw new Exception("Beklenmeyen Bir Hata İle Karşılaşıldı");
        }

        public async  Task AddItemToBasketAsync(VM_Create_BasketItem basketItem)
        {
            Basket? basket = await ContextUser();
            if (basket != null)
            {
              BasketItem _basketItem= await  _basketItemReadRepository.GetSingleAsync(bi => bi.BasketId == basket.Id &&
                bi.ProductId == Guid.Parse(basketItem.ProductId));
                if (_basketItem!=null)
                {
                    _basketItem.Quantity++;
                }
                else
                {
                   await _basketItemWriteRepository.AddAsync(new() {
                    BasketId= basket.Id,
                    ProductId= Guid.Parse(basketItem.ProductId),
                    Quantity= basketItem.Quantity,
                    
                    });
                  await  _basketItemWriteRepository.SaveAsync();
                }
            }
        }

        public async Task<List<BasketItem>> GetBasketItemsAsync()
        {
            Basket? basket = await ContextUser();
            Basket? result = await _basketReadRepository.Table
                .Include(b => b.BasketItems)
                .ThenInclude(bi => bi.Product).FirstOrDefaultAsync(b=>b.Id==basket.Id);
            return result.BasketItems.ToList();
        }

        public async Task RemoveBasketItemAsync(string basketItemId)
        {
          BasketItem basketItem= await _basketItemReadRepository.GetByIdAsync(basketItemId);
            if (basketItem != null )
            {
                _basketItemWriteRepository.Remove(basketItem);
               await _basketItemWriteRepository.SaveAsync();
            }
            
        }

        public async  Task UpdateQuantityAsync(VM_Update_BasketItem VMbasketItem)
        {
          BasketItem basketItem=  await _basketItemReadRepository.GetByIdAsync(VMbasketItem.BasketItemId);
            if (basketItem != null )
            {
                basketItem.Quantity= VMbasketItem.Quantity;
            }
           await _basketItemWriteRepository.SaveAsync();
        }
    }
}
