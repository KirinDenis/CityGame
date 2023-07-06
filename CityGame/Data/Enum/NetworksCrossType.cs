namespace CityGame.Data.Enum
{
    /// <summary>
    /// Types of network object's crosses
    /// </summary>
    public enum NetworkCrossType
    {
        horisontal,  //cross with simple horisontal network object
        vartical,    //cross with simple vertical network object
        crossOnCross //cross with network object who cross self or ready crossed with other 
    }
}
