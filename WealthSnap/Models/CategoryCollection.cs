namespace WealthSnap.Models
{
    public class CategoryCollection
    {
        public List<Category> Categories { get; set; }

        public CategoryCollection() 
        { 
            Categories = new List<Category>();
        }
    }
}
