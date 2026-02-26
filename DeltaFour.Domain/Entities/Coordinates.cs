namespace DeltaFour.Domain.Entities
{
    public class Coordinates(double latitude, double longitude)
    {
        public double Latitude { get; set; } = latitude;
        
        public double Longitude { get; set; } = longitude;
    }
}