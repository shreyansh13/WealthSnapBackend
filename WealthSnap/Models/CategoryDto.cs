namespace WealthSnap.Models
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public int? ParentCategoryId { get; set; }
        public List<CategoryDto> Subcategories { get; set; } = new List<CategoryDto>();
    }

    public class CategoryResponse
    {
        public List<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
    }
}
