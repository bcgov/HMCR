namespace Hmcr.Chris.Models
{
    public class Property
    {
        public string NE_UNIQUE { get; set; }
        public float NE_LENGTH { get; set; }
        public string NE_DESCR { get; set; }
        public float MEASURE { get; set; }        
        public double POINT_VARIANCE { get; set; }
        // clipped length used by all inventory queries
        public double CLIPPED_LENGTH_KM { get; set; }
        // used only by surface type queries
        public string SURFACE_TYPE { get; set; }
        // used only by maintenace class queries
        public string SUMMER_CLASS_RATING { get; set; }
        public string WINTER_CLASS_RATING { get; set; }
        public string SCHOOL_BUS_ROUTE { get; set; }    //future use?
        // used only by highway profile queries
        public int NUMBER_OF_LANES { get; set; }
        public string DIVIDED_HIGHWAY_FLAG { get; set; }
        // used only by guardrail queries
        public string GUARDRAIL_TYPE { get; set; }

    }
}
