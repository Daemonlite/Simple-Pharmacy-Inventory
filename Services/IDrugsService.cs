using pharmacy_management.Models;

namespace pharmacy_management.Services
{
    public interface IDrugsService
    {
        public Task<List<DrugsListDto>> GetAllDrugs();
        public Task<List<DrugsListDto>> GetDrugsById(Guid id);
        public Task<DrugsListDto> AddDrugs(DrugsCreateDto drugsDto);
        public Task<DrugsListDto> UpdateDrugs(Guid id, DrugsCreateDto drugsDto);
        public Task<bool> DeleteDrugs(Guid id);
    }
}