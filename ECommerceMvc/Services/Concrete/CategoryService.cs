﻿using ECommerceMvc.Areas.Identity.Data;
using ECommerceMvc.Services.Abstract;
using ECommerceShared;
using ECommerceShared.Dtos;
using ECommerceShared.Entites;
using Microsoft.EntityFrameworkCore;

namespace ECommerceMvc.Services.Concrete
{
    public class CategoryService : ICategoryService
    {
        private readonly ECommerceMvcContext _context;

        public CategoryService(ECommerceMvcContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<CategoryDto>> CreateCategory(CategoryDto request)
        {
            Category category = new Category()
            {
                CategoryName = request.Name,
            };
            var result = _context.Categories.Add(category);
            if(await _context.SaveChangesAsync() > 0)
            {
                return new ServiceResponse<CategoryDto>
                {
                    Message = "Operation success",
                    Success = true,
                };
            }
            else
            {
                return new ServiceResponse<CategoryDto>
                {
                    Message = "Operation is failed",
                    Success = false,
                };
            }

        }

        public async Task<ServiceResponse<bool>> DeleteCategory(int categoryId)
        {
            var result = await _context.Categories.FindAsync(categoryId);
            if(result == null)
            {
                return new ServiceResponse<bool>
                {
                    Message = "Category is not found",
                    Success = false,
                };
            }
            else
            {
                _context.Categories.Remove(result);
                if(await _context.SaveChangesAsync()> 0)
                {
                    return new ServiceResponse<bool>
                    {
                        Message = "Succes",
                        Success = true,
                    };
                }
                else
                {
                    return new ServiceResponse<bool>
                    {
                        Message = "While data is save to db,create event fail",
                        Success = false,
                    };
                }
            }
        }

        public List<Category> GetCategories()
        {
            var result = _context.Categories.ToList();
            if(result != null)
            {
                return result;
            }
            return null;
        }

        public async Task<ServiceResponse<CategoryDto>> GetCategory(int categoryId)
        {
            var result = await _context.Categories.Include(x => x.Products).FirstOrDefaultAsync(x => x.Id == categoryId);
            CategoryDto modelDto = new()
            {
                Name = result.CategoryName,
                Id = result.Id,
            };
            if(result == null)
            {
                return new ServiceResponse<CategoryDto>
                {
                    Success = false,
                    Message = "This category has not product",
                };
            }
            else
            {
                return new ServiceResponse<CategoryDto>
                {
                    Success = true,
                    Data = modelDto
                };
            }
        }

        public async Task<Category> GetCategoryByName(string categoryName)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.CategoryName == categoryName);
            if(category != null)
            {
                return category;
            }
            return null;
        }

        public async Task<ServiceResponse<List<Category>>> ListCategory()
        {
            ServiceResponse<List<Category>> serviceResponse = new ServiceResponse<List<Category>>();
            var result = await _context.Categories.ToListAsync();
            if(result.Count == 0)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Categories are not found";
                return serviceResponse;
            }
            else
            {
                serviceResponse.Success = true;
                serviceResponse.Message = "Categories listed";
                serviceResponse.Data = result;
                return serviceResponse;
            }
        }

        public async Task<ServiceResponse<CategoryDto>> UpdateCategory(int categoryId, CategoryDto categoryDto)
        {
            var result = await _context.Categories.FindAsync(categoryId);
            if(result == null)
            {
                return new ServiceResponse<CategoryDto>
                {
                    Message = "Oooops category is not found",
                    Success = false,
                };
            }
            else
            {
                result.CategoryName = categoryDto.Name;
                await _context.SaveChangesAsync();
                return new ServiceResponse<CategoryDto>
                {
                    Message = "your process is successfull",
                    Success = true,
                };
            }
        }
    }
}
