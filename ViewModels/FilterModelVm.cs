namespace Vision.ViewModels
{
    public class FilterModelVm
    {

        public int ClassifiedAdsCategoryId { get; set; }
        public double FromPrice { get; set; }
        public double ToPrice { get; set; }
        public int CityId { get; set; }
        public int AreaId { get; set; }
        public List<AdContentVm> adContentVMs { get; set; }
    }
  
}
