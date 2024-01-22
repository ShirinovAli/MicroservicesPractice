using AutoMapper;
using FreeCourse.Services.Catalog.Dtos;
using FreeCourse.Services.Catalog.Entities;
using FreeCourse.Services.Catalog.Settings;
using FreeCourse.Shared.Dtos;
using MongoDB.Driver;

namespace FreeCourse.Services.Catalog.Services
{
    internal class CategoryService : ICategoryService
    {
        private readonly IMongoCollection<Category> _categoryCollection;
        private readonly IMapper _mapper;

        public CategoryService(IMapper mapper, IDatabaseSettings databaseSettings)
        {
            var client = new MongoClient(databaseSettings.ConnectionString);
            var database = client.GetDatabase(databaseSettings.DatabaseName);

            _categoryCollection = database.GetCollection<Category>(databaseSettings.CategoryCollectionName);
            _mapper = mapper;
        }

        public async Task<Response<List<CategoryDto>>> GetAllAsync()
        {
            var data = await _categoryCollection.Find(category => true).ToListAsync();
            return Response<List<CategoryDto>>.Success(_mapper.Map<List<CategoryDto>>(data), 200);
        }

        public async Task<Response<CategoryDto>> CreateAsync(CategoryCreateDto categoryCreateDto)
        {
            await _categoryCollection.InsertOneAsync(_mapper.Map<Category>(categoryCreateDto));
            return Response<CategoryDto>.Success(_mapper.Map<CategoryDto>(categoryCreateDto), 200);
        }

        public async Task<Response<CategoryDto>> GetByIdAsync(string id)
        {
            var data = await _categoryCollection.Find<Category>(x => x.Id == id).FirstOrDefaultAsync();
            if (data == null)
                return Response<CategoryDto>.Fail("Data not found", 404);
            return Response<CategoryDto>.Success(_mapper.Map<CategoryDto>(data), 200);
        }
    }
}
